using MySql.Data.MySqlClient;
using System;

namespace Users_App.MySql_Services
{
    public class MySql_Connector
    {
        private readonly MySqlConnection connection = new MySqlConnection("server=******;port=******;username=******;password=******;database=******");

        public void openConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                try
                {
                    connection.Open();
                }
                catch
                { Environment.Exit(0); }
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
                catch
                { Environment.Exit(0); }
            }
        }

        public MySqlConnection getConnection()
        {
            return connection;
        }
    }
}
