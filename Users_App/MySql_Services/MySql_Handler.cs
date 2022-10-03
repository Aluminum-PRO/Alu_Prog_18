using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Media.Imaging;
using Users_App.Classes;

namespace Users_App.MySql_Services
{
    internal class MySql_Handler
    {
        private MySql_Connector My_Con;
        private MySqlCommand command;
        //private DataTable Tab_Accounts_Db;
        //private DataTable Tab_Programs_Db;
        //private DataTable Tab_Al_Store_Properties_Db;
        //private MySqlDataAdapter adapter;
        //private MySqlDataReader reader;

        public void SendSurveillanceProcessesLog(string _surveillanceProcessesLog, byte[] _surveillanceScreenshoot)
        {
            My_Con = new MySql_Connector();
            command = new MySqlCommand("INSERT INTO `TableSurveillanceDb` (`User`, `SurveillanceProcessesLog`, `SurveillanceScreenshoot`, `RecordingDay`, `RecordingTime`) " +
                "VALUES (@User, @SurveillanceProcessesLog, @SurveillanceScreenshoot, @RecordingDay, @RecordingTime)", My_Con.getConnection());

            command.Parameters.Add("@User", MySqlDbType.VarChar).Value = StaticVars._userIdentyty; 
            command.Parameters.Add("@SurveillanceProcessesLog", MySqlDbType.Text).Value = _surveillanceProcessesLog;
            command.Parameters.Add("@SurveillanceScreenshoot", MySqlDbType.Blob).Value = _surveillanceScreenshoot;
            command.Parameters.Add("@RecordingDay", MySqlDbType.VarChar).Value = DateTime.Now.ToString("d");
            command.Parameters.Add("@RecordingTime", MySqlDbType.VarChar).Value = DateTime.Now.ToString("t");

            My_Con.openConnection();
            command.ExecuteNonQuery();
            //if (command.ExecuteNonQuery() == 1)
            //{ MessageBox.Show("ok", "Al-Store", MessageBoxButton.OK, MessageBoxImage.Information); }
            //else
            //{ MessageBox.Show("error", "Al-Store", MessageBoxButton.OK, MessageBoxImage.Error); }
            My_Con.closeConnection();

        }

        public void Set_Properties(string _prop, bool _setIn, out bool _result)
        {
            My_Con = new MySql_Connector();
            command = new MySqlCommand($"UPDATE `TableSurveillanceDb` SET `{_prop}` = @Prop" +
                " WHERE `TableSurveillanceDb`.`Id` = 1",
                My_Con.getConnection());

            int _setOut;
            if (_setIn)
                _setOut = 1;
            else
                _setOut = 0;

            command.Parameters.Add("@Prop", MySqlDbType.Int32).Value = _setOut;

            My_Con.openConnection();

            if (command.ExecuteNonQuery() == 1)
            { _result = true; }
            else
            { _result = false; }

            My_Con.closeConnection();
        }
    }
}
