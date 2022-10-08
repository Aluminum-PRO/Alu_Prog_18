using MySql.Data.MySqlClient;
using System;
using Users_App.Sevice;

namespace Users_App.MySql_Services
{
    public class MySql_Connector
    {
        private readonly MySqlConnection connection = new MySqlConnection("server=AlSurDb;port=3306;username=Aluminum;password=2ap_yywp92/i1NZn;database=AlDb");

        public void openConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                try
                {
                    connection.Open();
                }
                catch(Exception ex)
                {
                    ErrorsSaves errorsSaves = new ErrorsSaves();
                    errorsSaves.Recording_Errors(ex);
                }
            }
        }

        public void closeConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    connection.Close();
                }
                catch (Exception ex)
                {
                    ErrorsSaves errorsSaves = new ErrorsSaves();
                    errorsSaves.Recording_Errors(ex);
                }
            }
        }

        public MySqlConnection getConnection()
        {
            return connection;
        }
    }
}
