using System.Data;

namespace DatabaseConnector
{
    public abstract class DatabaseBase : IDbConnection
    {
        protected string _connectionString;


        public DatabaseBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public abstract string ConnectionString { get; set; }

        public abstract int ConnectionTimeout { get; }

        public abstract string Database { get; }

        public ConnectionState State => throw new NotImplementedException();

        public abstract IDbTransaction BeginTransaction();


        public abstract IDbTransaction BeginTransaction(IsolationLevel il);

        public abstract void ChangeDatabase(string databaseName);

        public abstract void Close();

        public abstract IDbCommand CreateCommand();

        public abstract void Dispose();

        public abstract void Open();


        public abstract int ExecuteNonQuery(string commandText, CommandType commandType, params object[] parameters);
        public abstract int ExecuteNonQuery(string commandText, params object[] parameters);
        public abstract object ExecuteScalar(string commandText, CommandType commandType, params object[] parameters);
        public abstract object ExecuteScalar(string commandText, params object[] parameters);
        public abstract object ExecuteReader(string commandText, CommandType commandType, params object[] parameters);
        public abstract object ExecuteReader(string commandText, params object[] parameters);
        public abstract DataTable GetDataTable(string commandText, CommandType commandType, params object[] parameters);
        public abstract DataTable GetDataTable(string commandText, params object[] parameters);
    }
}