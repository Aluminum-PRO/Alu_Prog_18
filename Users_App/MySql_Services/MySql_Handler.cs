using MySql.Data.MySqlClient;
using System;
using System.Data;
using Users_App.Classes;

namespace Users_App.MySql_Services
{
    internal class MySql_Handler
    {
        private MySql_Connector My_Con;
        private MySqlCommand command;
        private DataTable TableSurveillanceDb;
        //private DataTable Tab_Programs_Db;
        //private DataTable Tab_Al_Store_Properties_Db;
        private MySqlDataAdapter adapter;
        private MySqlDataReader reader;

        public void SendSurveillanceProcessesLog(string _surveillanceProcessesLog, byte[] _surveillanceScreenshoot)
        {
            My_Con = new MySql_Connector();
            if (StaticVars._surveillanceProcessesLogId == 0)
            {
                command = new MySqlCommand("SELECT * FROM `TableSurveillanceDb` WHERE `User` = @User AND `RecordingDay` = @RecordingDay", My_Con.getConnection());
                command.Parameters.Add("@User", MySqlDbType.VarChar).Value = StaticVars._userIdentyty; command.Parameters.Add("@RecordingDay", MySqlDbType.VarChar).Value = DateTime.Now.ToString("d");

                adapter.SelectCommand = command;
                adapter.Fill(TableSurveillanceDb);

                if (TableSurveillanceDb.Rows.Count > 0)
                {
                    reader = null;
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        StaticVars._surveillanceProcessesLogId = Convert.ToInt32(reader["SurveillanceProcessesLogId"]);
                    }
                }
                else
                {
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
            }
            else
            {
                command = new MySqlCommand($"UPDATE `TableSurveillanceDb` SET `SurveillanceProcessesLog` = @SurveillanceProcessesLog, `SurveillanceScreenshoot` = @SurveillanceScreenshoot,`RecordingTime` = @RecordingTime" +
                " WHERE `TableSurveillanceDb`.`SurveillanceProcessesLogId` = @SurveillanceProcessesLogId",
                My_Con.getConnection());

                command.Parameters.Add("@SurveillanceProcessesLogId", MySqlDbType.Int32).Value = StaticVars._surveillanceProcessesLogId;
                command.Parameters.Add("@SurveillanceProcessesLog", MySqlDbType.Text).Value = _surveillanceProcessesLog;
                command.Parameters.Add("@SurveillanceScreenshoot", MySqlDbType.Blob).Value = _surveillanceScreenshoot;
                command.Parameters.Add("@RecordingTime", MySqlDbType.VarChar).Value = DateTime.Now.ToString("t");

                My_Con.openConnection();

                command.ExecuteNonQuery();

                My_Con.closeConnection();
            }


        }

        
    }
}
