/* Based on: 
 *   http://www.eggheadcafe.com/conversation.aspx?messageid=29988910&threadid=29988841
 * */

using System;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using HeuristicLab.Tracing;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  /// <summary>
  /// The VarBinaryStream class inherits from Stream. It uses a
  /// VarBinarySource class to execute the actual TSQL.
  /// </summary>
  public class VarBinaryStream : Stream, IDisposable {
    private long _position;
    private readonly VarBinarySource _source;

    public VarBinaryStream(VarBinarySource source) {
      _position = 0;
      _source = source;
    }

    public override bool CanRead {
      get { return true; }
    }

    public override bool CanSeek {
      get { return true; }
    }

    public override bool CanWrite {
      get { return true; }
    }

    public override long Length {
      get { return _source.Length; }
    }

    public override long Position {
      get { return _position; }
      set { this.Seek(value, SeekOrigin.Begin); }
    }

    public override void Flush() { }

    public override long Seek(long offset, SeekOrigin origin) {
      switch (origin) {
        case SeekOrigin.Begin: {
            if ((offset < 0) && (offset > this.Length))
              throw new ArgumentException("Invalid seek origin.");
            _position = offset;
            break;
          }
        case SeekOrigin.End: {
            if ((offset > 0) && (offset < -this.Length))
              throw new ArgumentException("Invalid seek origin.");
            _position = this.Length - offset;
            break;
          }
        case SeekOrigin.Current: {
            if ((_position + offset > this.Length))
              throw new ArgumentException("Invalid seek origin.");
            _position = _position + offset;
            break;
          }
        default: {
            throw new ArgumentOutOfRangeException("origin");
          }
      }
      return _position;
    }

    public override void SetLength(long value) {
      throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count) {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset < 0)
        throw new ArgumentOutOfRangeException("offset");
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      if (buffer.Length - offset < count)
        throw new ArgumentException("Offset and length were out of bounds for the array");

      byte[] data = _source.Read(Position, count);
      if (data == null)
        return 0;

      Buffer.BlockCopy(data, 0, buffer, offset, data.Length);
      _position += data.Length;
      return data.Length;
    }

    public override void Write(byte[] buffer, int offset, int count) {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset < 0)
        throw new ArgumentOutOfRangeException("offset");
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      if (buffer.Length - offset < count)
        throw new ArgumentException("Offset and length were out of bounds for the array");

      byte[] data = GetWriteBuffer(buffer, count, offset);
      _source.Write(data, _position, count);
      _position += count;
    }

    private static byte[] GetWriteBuffer(byte[] buffer, int count, int
    offset) {
      if (buffer.Length == count)
        return buffer;
      byte[] data = new byte[count];
      Buffer.BlockCopy(buffer, offset, data, 0, count);
      return data;
    }

    public override void Close() {
      _source.Close();
    }

    protected override void Dispose(bool disposing) {
      if (!disposing) {
        if (_source != null) {
          _source.Close();
          _source.Dispose();
        }
      }
      base.Dispose(disposing);
    }
  }


  public class VarBinarySource : IDisposable {
    private SqlCommand _readCommand;
    private SqlCommand _writeCommand;
    private SqlConnection _connection;
    private SqlTransaction _transaction;
    private bool _ownedTransaction = false;
    private bool _ownedConnection = false;
    private readonly long _length;

    public VarBinarySource(SqlConnection connection,
      SqlTransaction transaction,
      string table, string
    dataColumn, string keyColumn, Guid key) {
      _connection = connection;
      _transaction = (SqlTransaction) ContextFactory.Context.Transaction;
      /*
      if (_connection.State == ConnectionState.Closed) {
        _connection.Open();
        _ownedConnection = true;

        _transaction =
          _connection.BeginTransaction(IsolationLevel.RepeatableRead) as SqlTransaction;
        _ownedTransaction = true;
      } else {
        _ownedConnection = false;

        if (transaction != null) {
          _transaction = transaction;

          _ownedTransaction = false;
        } else {
          _transaction =
            _connection.BeginTransaction(IsolationLevel.ReadCommitted) as SqlTransaction;

          _ownedTransaction = true;
        }
      }        */

      _length = GetLength(connection, table, dataColumn, keyColumn, key);
      _readCommand = CreateReadCommand(connection, table, dataColumn,
      keyColumn, key);
      _writeCommand = CreateWriteCommand(connection, table, dataColumn,
      keyColumn, key);
    }

    public long Length {
      get { return _length; }
    }

    private static SqlCommand CreateReadCommand(SqlConnection connection,
    string table, string dataColumn, string keyColumn,
    Guid key) {
      SqlCommand readCommand = connection.CreateCommand();
      readCommand.CommandText = string.Format(@"
select substring({0}, @offset, @length)
from {1}
where {2} = @key", dataColumn, table, keyColumn);
      readCommand.Parameters.Add("@key", SqlDbType.UniqueIdentifier).Value = key;
      readCommand.Parameters.Add("@offset", SqlDbType.BigInt);
      readCommand.Parameters.Add("@length", SqlDbType.BigInt);
      return readCommand;
    }

    private static SqlCommand CreateWriteCommand(SqlConnection connection,
    string table, string dataColumn, string keyColumn,
    Guid key) {
      SqlCommand writecommand = connection.CreateCommand();
      writecommand.CommandText = string.Format(@"
update {0}
set {1}.write(@buffer, @offset, @length)
where {2} = @key", table, dataColumn, keyColumn);
      writecommand.Parameters.Add("@key", SqlDbType.UniqueIdentifier).Value = key;
      writecommand.Parameters.Add("@offset", SqlDbType.BigInt);
      writecommand.Parameters.Add("@length", SqlDbType.BigInt);
      writecommand.Parameters.Add("@buffer", SqlDbType.VarBinary);
      return writecommand;
    }

    private long GetLength(SqlConnection connection,
      string table,
    string dataColumn, string keyColumn,
    Guid key) {
      using (SqlCommand command = connection.CreateCommand()) {
        command.Transaction = _transaction;

        SqlParameter length = command.Parameters.Add("@length",
        SqlDbType.BigInt);
        length.Direction = ParameterDirection.Output;

        command.CommandText = string.Format(@"
select @length = cast(datalength({0}) as bigint)
from {1}
where {2} = @key", dataColumn, table, keyColumn);
        command.Parameters.Add("@key", SqlDbType.UniqueIdentifier).Value = key;
        command.ExecuteNonQuery();
        return length.Value == DBNull.Value ? 0 : (long)length.Value;
      }
    }

    public byte[] Read(long offset, long length) {
      _readCommand.Transaction = _transaction;

      // substring is 1-based.
      _readCommand.Parameters["@offset"].Value = offset + 1;
      _readCommand.Parameters["@length"].Value = length;
      return (byte[])_readCommand.ExecuteScalar();
    }

    public void Write(byte[] buffer, long offset, long length) {
      _writeCommand.Transaction = _transaction;

      _writeCommand.Parameters["@buffer"].Value = buffer;
      _writeCommand.Parameters["@offset"].Value = offset;
      _writeCommand.Parameters["@length"].Value = length;
      _writeCommand.ExecuteNonQuery();
    }

    public void Close() {
      Logger.Info("Closing Stream");
      if (_transaction != null) {
        _transaction.Commit();
        _transaction = null;
        Logger.Info("Transaction commited and closed");
      }

      if (_connection != null) {
        _connection.Close();
        ContextFactory.Context.Dispose();
        ContextFactory.Context = null;
        Logger.Info("Connection and Context disposed and set null");
      }
    }

    public void Dispose() {
      if (_readCommand != null) {
        _readCommand.Dispose();
      }
      if (_writeCommand != null) {
        _writeCommand.Dispose();
      }
    }
  }
}

