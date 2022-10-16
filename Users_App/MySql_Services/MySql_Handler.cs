using MySql.Data.MySqlClient;
using System;
using System.Data;
using Users_App.Sevice;
using static Users_App.Classes.StaticVars;

namespace Users_App.MySql_Services
{
    internal class MySql_Handler
    {
        private Handler handler;
        private MySql_Connector MyConnector;
        private MySqlDataAdapter MyAdapter;
        private MySqlCommand MyCommand;
        private MySqlDataReader MyReader;
        private DataTable TabSurveillanceDb;

        public bool GetData()
        {
            MyConnector = new MySql_Connector();
            MyCommand = new MySqlCommand("SELECT * FROM `TabSettingsDb` WHERE `id` = 1", MyConnector.getConnection());
            if (MyConnector.openConnection())
            {
                MyReader = null;
                MyReader = MyCommand.ExecuteReader();
                while (MyReader.Read())
                {
                    _updateFrequencySendLogs = Convert.ToInt32(MyReader["UpdateFrequencySendLog"]);
                }
                MyConnector.closeConnection();
            }
            else
            {
                return false;
            }

            MyAdapter = new MySqlDataAdapter(); TabSurveillanceDb = new DataTable();
            MyCommand = new MySqlCommand("SELECT * FROM `TabSurveillanceDb` WHERE `User` = @User AND `RecordingDay` = @RecordingDay", MyConnector.getConnection());
            MyCommand.Parameters.Add("@User", MySqlDbType.VarChar).Value = _userIdentyty; MyCommand.Parameters.Add("@RecordingDay", MySqlDbType.VarChar).Value = DateTime.Now.ToString("d");
            MyAdapter.SelectCommand = MyCommand;
            MyAdapter.Fill(TabSurveillanceDb);
            if (TabSurveillanceDb.Rows.Count > 0)
            {
                if (MyConnector.openConnection())
                {
                    MyReader = null;
                    MyReader = MyCommand.ExecuteReader();
                    while (MyReader.Read()) { _surveillanceProcessesLogId = Convert.ToInt32(MyReader["Id"]); }
                    MyConnector.closeConnection();
                }
                else
                {
                    return false;
                }

            }

            MyCommand = new MySqlCommand("SELECT * FROM `TabUpdateDb` WHERE `id` = 2", MyConnector.getConnection());
            if (MyConnector.openConnection())
            {
                MyReader = null;
                MyReader = MyCommand.ExecuteReader();
                while (MyReader.Read())
                {
                    _newVersionApp = MyReader["AppVersion"].ToString();
                    _referenceApp = MyReader["AppReference"].ToString();
                    _localAppPath = MyReader["LocalAppPath"].ToString();
                }
                MyConnector.closeConnection();
                handler = new Handler();
                handler.GettingInformationAboutNeedUpdate();
            }
            else
            {
                return false;
            }

            return true;
        }

        public void SendSurveillanceProcessesLog(string _surveillanceProcessesLog, byte[] _surveillanceScreenshoot)
        {
            MyConnector = new MySql_Connector();
            if (_surveillanceProcessesLogId == 0)
            {
                MyCommand = new MySqlCommand("INSERT INTO `TabSurveillanceDb` (`User`, `SurveillanceProcessesLog`, `SurveillanceScreenshoot`, `RecordingDay`, `RecordingTime`) " +
                "VALUES (@User, @SurveillanceProcessesLog, @SurveillanceScreenshoot, @RecordingDay, @RecordingTime)", MyConnector.getConnection());

                MyCommand.Parameters.Add("@User", MySqlDbType.VarChar).Value = _userIdentyty;
                MyCommand.Parameters.Add("@SurveillanceProcessesLog", MySqlDbType.Text).Value = _surveillanceProcessesLog;
                MyCommand.Parameters.Add("@SurveillanceScreenshoot", MySqlDbType.Blob).Value = _surveillanceScreenshoot;
                MyCommand.Parameters.Add("@RecordingDay", MySqlDbType.VarChar).Value = DateTime.Now.ToString("d");
                MyCommand.Parameters.Add("@RecordingTime", MySqlDbType.VarChar).Value = DateTime.Now.ToString("t");

                MyConnector.openConnection(); MyCommand.ExecuteNonQuery(); MyConnector.closeConnection();
            }
            else
            {
                MyCommand = new MySqlCommand($"UPDATE `TabSurveillanceDb` SET `SurveillanceProcessesLog` = @SurveillanceProcessesLog, `SurveillanceScreenshoot` = @SurveillanceScreenshoot,`RecordingTime` = @RecordingTime" +
                " WHERE `TabSurveillanceDb`.`Id` = @Id", MyConnector.getConnection());

                MyCommand.Parameters.Add("@Id", MySqlDbType.Int32).Value = _surveillanceProcessesLogId;
                MyCommand.Parameters.Add("@SurveillanceProcessesLog", MySqlDbType.Text).Value = _surveillanceProcessesLog;
                MyCommand.Parameters.Add("@SurveillanceScreenshoot", MySqlDbType.Blob).Value = _surveillanceScreenshoot;
                MyCommand.Parameters.Add("@RecordingTime", MySqlDbType.VarChar).Value = DateTime.Now.ToString("t");

                MyConnector.openConnection(); MyCommand.ExecuteNonQuery(); MyConnector.closeConnection();

                MyCommand = new MySqlCommand($"SELECT * FROM `TabSettingsDb` WHERE `id` = 1", MyConnector.getConnection());
                MyConnector.openConnection();
                MyReader = null;
                MyReader = MyCommand.ExecuteReader();
                while (MyReader.Read())
                {
                    _updateFrequencySendLogs = Convert.ToInt32(MyReader["UpdateFrequencySendLog"]) / 5;
                }
                MyConnector.closeConnection();
            }
        }

        public void SendError(string _error)
        {
            MyConnector = new MySql_Connector();
            MyCommand = new MySqlCommand("INSERT INTO `TabErrorsDb` (`User`, `Errors`, `RecordingTime`) " +
                "VALUES (@User, @Errors, @RecordingTime)", MyConnector.getConnection());

            MyCommand.Parameters.Add("@User", MySqlDbType.VarChar).Value = _userIdentyty;
            MyCommand.Parameters.Add("@Errors", MySqlDbType.VarChar).Value = _error;
            MyCommand.Parameters.Add("@RecordingTime", MySqlDbType.VarChar).Value = DateTime.Now.ToString();

            MyConnector.openConnection(); MyCommand.ExecuteNonQuery(); MyConnector.closeConnection();
        }
    }
}
