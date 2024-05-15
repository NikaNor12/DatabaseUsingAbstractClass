using DatabaseConnector;
using MySql.Data.MySqlClient;
using System.Data;

namespace DatabaseHelper.MySQL
{
    public sealed class DatabaseTSQL : DatabaseBase, IDisposable
    {
        private readonly string _connectionString;
        private MySqlConnection? _connection;
        private MySqlTransaction? _transaction;
        public DatabaseTSQL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public override int ConnectionTimeout => _connection!.ConnectionTimeout;
        public override string Database => _connection!.Database;
        public override string ConnectionString { get => _connectionString; set => throw new NotImplementedException(); }

        public MySqlConnection GetConnection() => _connection ??= new MySqlConnection(_connectionString);

        public MySqlConnection OpenConnection()
        {
            GetConnection().Open();
            return _connection!;
        }

        public void CloseConnection() => _connection?.Close();

        public MySqlCommand GetCommand(string commandText, CommandType commandType, params MySqlParameter[] parameters)
        {
            var command = GetConnection().CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;
            command.Parameters.AddRange(parameters);
            if (_transaction != null)
            {
                command.Transaction = _transaction;
            }

            return command;
        }

        public MySqlCommand GetCommand(string commandText, params MySqlParameter[] parameters) =>
            GetCommand(commandText, CommandType.Text, parameters);



        public void CommitTransaction()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            _transaction!.Commit();
            _transaction = null;
        }

        public void RollbackTransaction()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            _transaction!.Rollback();
            _transaction = null;
        }

        public override void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
            _transaction = null;
            _connection = null;
        }

        public override IDbTransaction BeginTransaction()
        {
            if (_transaction != null)
            {
                throw new ArgumentException("Transaction already started.");
            }

            _transaction = GetConnection().BeginTransaction();
            return _transaction;
        }

        public override IDbTransaction BeginTransaction(IsolationLevel il)
        {
            if (_transaction != null)
            {
                throw new ArgumentException("Transaction already started.");
            }

            _transaction = GetConnection().BeginTransaction(il);
            return _transaction;
        }

        public override void ChangeDatabase(string databaseName)
        {
            _connection!.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            _connection?.Close();
        }

        public override IDbCommand CreateCommand()
        {
            return _connection!.CreateCommand();
        }

        public override void Open()
        {
            _connection?.Open();
        }

        public override int ExecuteNonQuery(string commandText, CommandType commandType, params object[] parameters)
        {
            using var command = GetCommand(commandText, commandType, ConvertParameters(parameters));
            return command.ExecuteNonQuery();
        }

        public override int ExecuteNonQuery(string commandText, params object[] parameters) => ExecuteNonQuery(commandText, CommandType.Text, parameters);

        public override object ExecuteScalar(string commandText, CommandType commandType, params object[] parameters)
        {
            using var command = GetCommand(commandText, commandType, ConvertParameters(parameters));
            return command.ExecuteScalar();
        }

        public override object ExecuteScalar(string commandText, params object[] parameters) => ExecuteScalar(commandText, CommandType.Text, parameters);

        public override object ExecuteReader(string commandText, CommandType commandType, params object[] parameters)
        {
            using var command = GetCommand(commandText, commandType, ConvertParameters(parameters));
            return command.ExecuteReader();
        }

        public override object ExecuteReader(string commandText, params object[] parameters) => ExecuteReader(commandText, CommandType.Text, parameters);


        public override DataTable GetDataTable(string commandText, CommandType commandType, params object[] parameters)
        {
            using var command = GetCommand(commandText, commandType, ConvertParameters(parameters));
            using MySqlDataAdapter adapter = new(command);
            DataTable table = new();
            adapter.Fill(table);
            return table;
        }

        public override DataTable GetDataTable(string commandText, params object[] parameters) => GetDataTable(commandText, CommandType.Text, parameters);


        private MySqlParameter[] ConvertParameters(object[] parameters)
        {
            if (parameters != null)
            {
                return new MySqlParameter[0];
            }

            var sqlParameters = new MySqlParameter[parameters!.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                sqlParameters[i] = new MySqlParameter($"{i}", parameters[i]);
            }

            return sqlParameters;
        }
    }
}