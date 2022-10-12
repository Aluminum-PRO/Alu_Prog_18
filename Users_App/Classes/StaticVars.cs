using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using Microsoft.VisualBasic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Users_App.Classes
{
    public static class StaticVars
    {
        public static int _surveillanceProcessesLogId = 0, _updateFrequencySendLogs, _currentDay;
        public static string _mainPath, _pathApp, _pathErrorsLog, _userIdentyty, _referenceApp, _currentVersionApp, _newVersionApp;
        public static bool _isNeedUpdateApp = false, _isCurrentVersionAppNewer = false, _isAdminLaunch = false;
        //public static bool ;
        public static List<Process> _processesList = null;
        public static List<CompletedProcessesClass> _completedProcessesList = null;
        public static List<OpenProcessesClass> _openProcessesList = null;
        public static List<String> _processSystemList = new List<String>() { "svchost", "PhoneExperienceHost", "ShellExperienceHost", "smartscreen", "dllhost", "msedgewebview2", "lsass", "services",
            "RtkAudUService64", "SearchIndexer", "conhost", "Users Surveillance", "NisSrv", "ApplicationFrameHost", "SearchFilterHost", "SgrmBroker", "RuntimeBroker", "Microsoft.Msn.Weather",
            "SystemSettings", "ps64ldr", "csrss", "MsMpEng", "wininit", "smss", "dasHost", "Microsoft.Photos", "Cortana", "dwm", "Video.UI", "WUDFHost", "SecurityHealthService", "jusched",
            "UserOOBEBroker", "tv_x64", "ServiceHub.SettingsHost", "service_update", "DTShellHlp", "sqlwriter", "SecurityHealthSystray", "TabTip", "TextInputHost", "fontdrvhost", "WmiPrvSE",
            "Memory Compression", "remoting_host", "DiscSoftBusServiceLite", "MpCopyAccelerator", "atieclxx", "ctfmon", "OriginWebHelperService", "WebViewHost", "Registry", "StartMenuExperienceHost",
            "spoolsv", "TvUpdateInfo", "SearchProtocolHost", "sihost", "wlanext", "winlogon", "atiesrxx", "StandardCollector.Service", "jucheck", "audiodg", "MoUsoCoreWorker", "System", "explorer",
            "backgroundTaskHost", "FileCoAuth", "HxTsr", "MicrosoftEdgeUpdate", "GoogleUpdate", "sppsvc", "TrustedInstaller", "TiWorker", "OneDriveStandaloneUpdater", "Microsoft.SharePoint", "PilotshubApp",
            "wermgr", "MusNotification", "MusNotificationUx", "rundll32", "consent", "msfeedssync", "ielowutil", "LaunchTM", "ScreenClippingHost", "upfc", "SIHClient", "MSBuild", "SearchApp", "taskhostw",
            "SystemSettingsBroker", "Idle", "NETSTAT", "findstr"};
        public static List<String> _processSystemVirusList = new List<String>() { "BackgroundDownload" };

        public class CompletedProcessesClass
        {
            public string _processName = "", _processWindowName = "", _processType = "App";
            public double _processTime = 0;
            public bool _processBackground = false;

        }

        public class OpenProcessesClass
        {
            public string _processName = "", _processWindowName = "";
            public DateTime _processStartedTime;
            public bool _processBackground = false;
        }
    }
}
