namespace Core.Veeam.DBManager
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using SharpRaven;
    using SharpRaven.Data;

    public class LocalDbAccessor
    {
        private readonly string _connectionString;
        protected RavenClient RavenClient;

        public LocalDbAccessor(string connectionString)
        {
            RavenClient = Sentry.RavenClient.Instance;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public DataTable GetDataTable(string spName, CommandBehavior commandBehavior, int timoutSeconds, bool retryable, params SqlParameter[] spParams)
        {
            try
            {
                var connection = new SqlConnection(_connectionString);
                try
                {
                    var selectCommand = new SqlCommand(spName, connection);
                    var dataTable = new DataTable();
                    using (selectCommand)
                    {
                        using (var sqlDataAdapter = new SqlDataAdapter(selectCommand))
                        {
                            try
                            {
                                selectCommand.Parameters.AddRange(spParams);
                                selectCommand.CommandType = CommandType.StoredProcedure;
                                selectCommand.CommandTimeout = timoutSeconds;
                                dataTable.BeginLoadData();
                                sqlDataAdapter.Fill(dataTable);
                                dataTable.EndLoadData();
                            }
                            finally
                            {
                                selectCommand.Parameters.Clear();
                            }
                        }
                    }

                    return dataTable;
                }
                finally
                {
                    connection.Dispose();
                }
            }            
            catch (Exception ex)
            {
                if (ex is SqlException sqlException)
                {

                }
                else
                {
                    RavenClient.Capture(new SentryEvent(ex));
                }
                
                throw;
            }
        }

        public DataTable GetDataTable(string spName, params SqlParameter[] spParams)
        {
            return GetDataTable(spName, true, spParams);
        }

        public DataTable GetDataTable(string spName, bool retryable, params SqlParameter[] spParams)
        {
            return GetDataTable(spName, CommandBehavior.SingleResult | CommandBehavior.CloseConnection, retryable, spParams);
        }

        public DataTable GetDataTable(string spName, CommandBehavior commandBehavior, bool retryable, params SqlParameter[] spParams)
        {
            return GetDataTable(spName, commandBehavior, 30, false, spParams);
        }
    }
}