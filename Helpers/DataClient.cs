using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BCMCHOVR.Helpers
{
    public interface IDataClient
    {
    }
    public partial class DataClient : IDataClient, IDisposable
    {
        private static string _ConnectionString = "";
        public string SetConnectionString
        {
            set
            {
                if(string.IsNullOrEmpty(_ConnectionString))
                {
                    _ConnectionString = value;
                }
            }
        }
        public SqlConnection GetConnection()
        {
            if (SqlConnection == null)
            {
                var connectionString = _ConnectionString;   // Configurations.DefaultConnectionString;

                SqlConnection = new SqlConnection(connectionString);
            }
            return SqlConnection;
        }

        public SqlCommand GetCommand()
        {
            return GetCommand(CommandText);
        }
        public SqlCommand GetCommand(string commandText)
        {
            if (SqlCommand == null)
            {
                SqlCommand = new SqlCommand(commandText, GetConnection())
                {
                    CommandType = CommandType.StoredProcedure,
                };
            }
            return SqlCommand;
        }
        public SqlCommand GetCommand(string commandText, bool inline)
        {
            if (SqlCommand == null)
            {
                SqlCommand = new SqlCommand(commandText, GetConnection())
                {
                    CommandType = inline ?  CommandType.Text : CommandType.StoredProcedure
                };
            }
            return SqlCommand;
        }
        private void OpenConnection()
        {
            if (SqlConnection.State == ConnectionState.Closed)
            {
                SqlConnection.Open();
            }
        }
        private void CloseConnection()
        {
            if (SqlConnection.State == ConnectionState.Open)
            {
                SqlConnection.Close();
            }
        }
        private void CreateParameter(SqlDbType dataType, string parameter, object value)
        {
            if (SqlParameters == null)
            {
                SqlParameters = new System.Collections.Generic.List<SqlParameter>();
            }
            SqlParameters.Add(new SqlParameter(parameter, dataType)
            {
                Value = value
            });
        }

        private void PrepareCommand()
        {
            SqlCommand.Parameters.Clear();

            if (SqlParameters != null && SqlParameters.Count > 0)
            {
                SqlCommand.Parameters.AddRange(SqlParameters.ToArray());
            }
            OpenConnection();
        }
        public DataClient AddParameter(string dataType, string parameter, object value)
        {
            if (!sqlDataTypes.TryGetValue(dataType, out SqlDbType type))
            {
                throw new Exception("Invalid cast");
            };
            CreateParameter(type, parameter, value);
            return this;
        }
        /// <summary>
        /// This method will return a single result
        /// </summary>
        /// <returns>String : Output of query</returns>
        public string ExecuteQuery()
        {
            using var command = GetCommand();
            PrepareCommand();
            try
            {
                var result = command.ExecuteScalar();
                if(result==null)
                {
                    throw new Exception("Null");
                }
                return result.ToString();
            }
            catch (Exception exception)
            {
                Exception = exception;
                return null;
            }
        }
        public int ExecuteInlinNonQuery(string sql) 
        {
            using (var command = GetCommand(sql, true))
            {
                PrepareCommand();
                try
                {
                    return command.ExecuteNonQuery();
                }
                catch (SqlException exception)
                {
                    Exception = exception;
                }
            }
            return 0;
        }

        public int ExecuteNonQuery()
        {
            using (var command = GetCommand())
            {
                PrepareCommand();
                try
                {
                    return command.ExecuteNonQuery();
                }
                catch (SqlException exception)
                {
                    Exception = exception;
                }
            }
            return 0;
        }
        public SqlDataReader ExecuteAsReader()
        {
            using (var command = GetCommand(CommandText))
            {
                PrepareCommand();
                try
                {
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        return reader;
                    }
                }
                catch (SqlException exception)
                {
                    Exception = exception;
                }
            }
            return null;
        }
        public IDataGrid ExecuteAsDataGrid()
        {
            using var reader = ExecuteAsReader();
            return reader?.ToDataGrid();
        }
        public JsonDataGrid ExecuteAsJsonDataGrid()
        {
            using var reader = ExecuteAsReader();
            var dataGrid = reader?.ToDataGrid();
            if (dataGrid != null)
            {
                JsonDataGrid jsonDataGrid = new()
                {
                    Header = dataGrid.GetHeader(),
                    Rows = dataGrid.GetRows()
                };
                return jsonDataGrid;
            }
            return null;
        }

        public JsonList ExecuteAsJsonData()
        {
            using var reader = ExecuteAsReader();
            return reader?.ToJsonList();
        }
        public DataTable ExecuteAsTable()
        {
            using (var command = GetCommand(CommandText))
            {
                PrepareCommand();
                try
                {
                    using var adapter = new SqlDataAdapter(command);
                    var dataTable = new DataTable();
                    var results = adapter.Fill(dataTable);
                    CloseConnection();
                    return dataTable;
                }
                catch (SqlException exception)
                {
                    Exception = exception;
                }
            }
            return null;
        }
        public DataSet ExecuteAsTableSet() 
        {
            using (var command = GetCommand(CommandText))
            {
                PrepareCommand();
                try
                {
                    using var adapter = new SqlDataAdapter(command);
                    var dataSet = new DataSet();
                    var results = adapter.Fill(dataSet);
                    CloseConnection();
                    return dataSet;
                }
                catch (SqlException exception)
                {
                    Exception = exception;
                }
            }
            return null;
        }

    }
    public partial class DataClient
    {
        #region "Contructors"
        public DataClient()
        {

        }
        public DataClient(string commandText)
        {
            CommandText = commandText;
        }

        #endregion "Contructors"

        #region "Static Memebers"


        //public static void SetConnectionString(string connectionString)
        //{
        //    SqlConnectionString = connectionString;
        //}

        #endregion "Static Memebers"

        #region "Private Memebers"

        private string CommandText { get; set; }
        private SqlConnection SqlConnection { get; set; }
        private SqlCommand SqlCommand { get; set; }

        public Exception Exception { get; set; }

        private string ConnectionString { get; set; }
        private List<SqlParameter> SqlParameters { get; set; }

        #endregion "Private Memebers"

        ~DataClient()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseConnection();
            }
        }

        private static readonly Dictionary<string, SqlDbType> sqlDataTypes = new Dictionary<string, SqlDbType>()
        {
            {"Int",SqlDbType.Int},
            {"BigInt",SqlDbType.BigInt},
            {"Bit",SqlDbType.Bit },
            {"Date",SqlDbType.Date },
            {"DateTime",SqlDbType.DateTime },
            {"NVarChar",SqlDbType.NVarChar },
            {"Text",SqlDbType.Text },
            {"VarChar",SqlDbType.VarChar},
            {"Time",SqlDbType.Time },
            {"Char",SqlDbType.Char},
            {"Float",SqlDbType.Float}
        };

    }

}
