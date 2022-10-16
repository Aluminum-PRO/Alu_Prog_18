using Admin_App.Services;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static Admin_App.Classes.StaticVars;

namespace Admin_App.MySql_Services
{
    internal class MySql_Handler
    {
        private Handler handler = new Handler();
        private MySql_Connector MyConnector;
        private MySqlDataAdapter MyAdapter;
        private MySqlCommand MyCommand;
        private MySqlDataReader MyReader;
        private DataTable TabSurveillanceDb;
        private BitmapImage _surveillanceScreenshootIn;

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
                    _startCreatingShortcut = Convert.ToBoolean(MyReader["StartCreatingShortcut"]);
                    _needPassword = Convert.ToBoolean(MyReader["NeedPassword"]);
                }
                MyConnector.closeConnection();
            }
            else
            {
                return false;
            }

            MyCommand = new MySqlCommand("SELECT * FROM `TabUpdateDb` WHERE `id` = 2", MyConnector.getConnection());
            if (MyConnector.openConnection())
            {
                MyReader = null;
                MyReader = MyCommand.ExecuteReader();
                while (MyReader.Read())
                {
                    _newVersionApp = MyReader["AppVersion"].ToString();
                    _referenceApp = MyReader["AppReference"].ToString(); ;
                }
                MyConnector.closeConnection();
                handler.GettingInformationAboutNeedUpdate();
            }
            else
            {
                return false;
            }

            return true;
        }

        public async void GetSurveillanceLogs()
        {
            bool _isExistsUser;
            int _recordingDay = 0, _recordingMonth = 0, _recordingYear = 0, _recordingHour = 0, _recordingMinets = 0;

            MyConnector = new MySql_Connector();
            MyCommand = new MySqlCommand("SELECT * FROM `TabSurveillanceDb` ORDER BY `RecordingDay` asc", MyConnector.getConnection());

            await Task.Run(() =>
            {
                if (MyConnector.openConnection())
                {
                    if (_generalUsersLogList != null)
                        _generalUsersLogList.Clear();
                    if (_generalUsersList != null)
                        _generalUsersList.Clear();
                    MyReader = null;
                    MyReader = MyCommand.ExecuteReader();
                    while (MyReader.Read())
                    {
                        _numberOfRecords++;
                        _recordingDay = Convert.ToInt32(MyReader["RecordingDay"].ToString().Split('.')[0]);
                        _recordingMonth = Convert.ToInt32(MyReader["RecordingDay"].ToString().Split('.')[1]);
                        _recordingYear = Convert.ToInt32(MyReader["RecordingDay"].ToString().Split('.')[2]);
                        _recordingHour = Convert.ToInt32(MyReader["RecordingTime"].ToString().Split(':')[0]);
                        _recordingMinets = Convert.ToInt32(MyReader["RecordingTime"].ToString().Split(':')[1]);
                        _generalUsersLogList.Add(new CurrentUsersClass()
                        {
                            _surveillanceProcessesLogId = Convert.ToInt32(MyReader["Id"]),
                            _surveillanceProcessesLog = MyReader["SurveillanceProcessesLog"].ToString(),
                            _userIdentyty = MyReader["User"].ToString(),
                            _recordingDateTime = new DateTime(_recordingYear, _recordingMonth, _recordingDay, _recordingHour, _recordingMinets, 0),
                            _surveillanceScreenshoot = _surveillanceScreenshootIn,
                        });

                        Action AddNewUsers = () =>
                        {
                            _generalUsersList.Add(new GeneralUsersClass()
                            {
                                _userIdentyty = MyReader["User"].ToString()
                            });
                        };

                        if (_generalUsersList == null || _generalUsersList.Count == 0)
                        {
                            AddNewUsers();
                        }
                        else
                        {
                            _isExistsUser = false;
                            foreach (GeneralUsersClass _generalUsersClass in _generalUsersList)
                            {
                                if (_generalUsersClass._userIdentyty == MyReader["User"].ToString())
                                    _isExistsUser = true;
                            }
                            if (!_isExistsUser)
                                AddNewUsers();
                        }
                    }
                    MyConnector.closeConnection();
                }
                _isLoadingData = true;
            });
        }

        public void Set_Properties(string _prop, bool _set, out bool _result)
        {
            MyConnector = new MySql_Connector();
            MyCommand = new MySqlCommand($"UPDATE `TabSettingsDb` SET `{_prop}` = @Prop" +
                " WHERE `TabSettingsDb`.`Id` = 1",
                MyConnector.getConnection());

            MyConnector.openConnection();

            if (MyCommand.ExecuteNonQuery() == 1)
                _result = true;
            else
                _result = false;

            MyConnector.closeConnection();
        }
    }
}
