using Admin_App.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin_App.MySql_Services
{
    internal class MySql_Handler
    {
        private MySql_Connector MyConnector;
        private MySqlDataAdapter MyAdapter;
        private MySqlCommand MyCommand;
        private MySqlDataReader MyReader;
        private DataTable TabSurveillanceDb;

        public void Getting_Data()
        {
            MyConnector = new MySql_Connector(); MyAdapter = new MySqlDataAdapter();
            MyCommand = new MySqlCommand("SELECT * FROM `TabUpdateDb` WHERE `id` = 1; SELECT * FROM `Tab_Al_Store_Db` WHERE `id` = 2; SELECT * FROM `Tab_Al_Store_Db` WHERE `id` = 3", My_Con.getConnection());

            My_Con.openConnection();

            reader = null;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                StaticVars._newVersionApp = reader["VersionApp"].ToString();
                StaticVars._whatNewsUpdate = reader["WhatNewsUpdate"].ToString();
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    StaticVars._startCreatingShortcut = Convert.ToBoolean(reader["StartCreatingShortcut"]); 
                }
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    
                }
            }

            My_Con.closeConnection();
        }

        public void Set_Properties(string _prop, bool _setIn, out bool _result)
        {
            My_Con = new MySql_Connector();
            command = new MySqlCommand($"UPDATE `TabSettingsDb` SET `{_prop}` = @Prop" +
                " WHERE `TabSettingsDb`.`Id` = 1",
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
