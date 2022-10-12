using MySql.Data.MySqlClient;
using System;
using Users_App.Sevice;

namespace Users_App.MySql_Services
{
    public class MySql_Connector
    {
        private readonly MySqlConnection connection = new MySqlConnection("server=AlSurDb;port=3306;username=Aluminum;password=2ap_yywp92/i1NZn;database=AlDb");

        public bool openConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch(Exception ex)
                {
                    ErrorsSaves errorsSaves = new ErrorsSaves();
                    errorsSaves.Recording_Errors(ex, false);
                    return false;
                }
            }
            else return true;
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
                    errorsSaves.Recording_Errors(ex, false);
                }
            }
        }

        public MySqlConnection getConnection()
        {
            return connection;
        }
    }
}
