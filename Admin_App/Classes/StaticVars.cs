using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Admin_App.Classes
{
    public static class StaticVars
    {
        public static bool _startCreatingShortcut, _needPassword;

        public static int _updateFrequencySendLogs, _numberOfRecords = 0;
        public static string _messageInfoContacts = ", обратитесь к системному администратору за помощью."/*\n https://vk.com/aluminum343 Aluminum-Company163@Yandex.ru"*/, _messageInfoClose = "\n\n Программа бдет закрыта...";
        public static string _mainPath, _pathApp, _pathShortcut, _pathErrorsLog, _userIdentyty, _currentVersionApp, _newVersionApp, _whatNewsUpdate, _referenceApp;
        public static bool _isLoadingData = true, _logoAnimation = true, _isNeedUpdateApp = false, _isCurrentVersionAppNewer = false, _isAdminLaunch = false;

        public static List<CurrentUsersClass> _generalUsersLogList = null;
        public static List<GeneralUsersClass> _generalUsersList = null;
        public class GeneralUsersClass
        {
            public static List<CurrentUsersClass> _currentUsersList = null;
            public string _userIdentyty;
            public bool _haveBannedProcess = false;
        }

        public class CurrentUsersClass
        {
            public int _surveillanceProcessesLogId;
            public string _userIdentyty, _surveillanceProcessesLog;
            public DateTime _recordingDateTime;
            public BitmapImage _surveillanceScreenshoot;
        }

        public class CompletedProcessesClass
        {
            public string _processName = "", _processWindowName = "", _processType = "App";
            public double _processTime = 0;
            public bool _processBackground = false;

        }
    }
}
