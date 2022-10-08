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
        private DataTable TabSurveillanceDb;
        //private DataTable Tab_Programs_Db;
        //private DataTable Tab_Al_Store_Properties_Db;
        private MySqlDataAdapter adapter;
        private MySqlDataReader reader;

        public void SendSurveillanceProcessesLog(string _surveillanceProcessesLog, byte[] _surveillanceScreenshoot)
        {
            My_Con = new MySql_Connector();
            adapter = new MySqlDataAdapter();

            TabSurveillanceDb = new DataTable();
            if (StaticVars._surveillanceProcessesLogId == 0)
            {
                command = new MySqlCommand("SELECT * FROM `TabSurveillanceDb` WHERE `User` = @User AND `RecordingDay` = @RecordingDay", My_Con.getConnection());
                command.Parameters.Add("@User", MySqlDbType.VarChar).Value = StaticVars._userIdentyty; command.Parameters.Add("@RecordingDay", MySqlDbType.VarChar).Value = DateTime.Now.ToString("d");

                adapter.SelectCommand = command;
                adapter.Fill(TabSurveillanceDb);

                if (TabSurveillanceDb.Rows.Count > 0)
                {
                    My_Con.openConnection();
                    reader = null;
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        StaticVars._surveillanceProcessesLogId = Convert.ToInt32(reader["Id"]);
                    }
                    My_Con.closeConnection();
                }
                else
                {
                    command = new MySqlCommand("INSERT INTO `TabSurveillanceDb` (`User`, `SurveillanceProcessesLog`, `SurveillanceScreenshoot`, `RecordingDay`, `RecordingTime`) " +
                "VALUES (@User, @SurveillanceProcessesLog, @SurveillanceScreenshoot, @RecordingDay, @RecordingTime)", My_Con.getConnection());

                    command.Parameters.Add("@User", MySqlDbType.VarChar).Value = StaticVars._userIdentyty;
                    command.Parameters.Add("@SurveillanceProcessesLog", MySqlDbType.Text).Value = _surveillanceProcessesLog;
                    command.Parameters.Add("@SurveillanceScreenshoot", MySqlDbType.Blob).Value = _surveillanceScreenshoot;
                    command.Parameters.Add("@RecordingDay", MySqlDbType.VarChar).Value = DateTime.Now.ToString("d");
                    command.Parameters.Add("@RecordingTime", MySqlDbType.VarChar).Value = DateTime.Now.ToString("t");

                    My_Con.openConnection();
                    command.ExecuteNonQuery();
                    My_Con.closeConnection();
                }
            }
            else
            {
                //System.Windows.Forms.MessageBox.Show("Test 3");
                command = new MySqlCommand($"UPDATE `TabSurveillanceDb` SET `SurveillanceProcessesLog` = @SurveillanceProcessesLog, `SurveillanceScreenshoot` = @SurveillanceScreenshoot,`RecordingTime` = @RecordingTime" +
                " WHERE `TabSurveillanceDb`.`Id` = @Id",
                My_Con.getConnection());

                command.Parameters.Add("@Id", MySqlDbType.Int32).Value = StaticVars._surveillanceProcessesLogId;
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
