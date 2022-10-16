using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Users_App.Classes;
using static Users_App.Classes.StaticVars;

namespace Users_App.Sevice
{
    internal class Update
    {
        string _pathApp = Assembly.GetExecutingAssembly().Location;
        string _nameApp = AppDomain.CurrentDomain.FriendlyName;

        public void StartUpdate()
        { Update_Process(); }

        private async void Update_Process()
        {
            using (HttpClient client = new HttpClient())
            {
                if (Internet.OK())
                {
                    bool _isFilesReceived = false;
                    if (Directory.Exists(_localAppPath))
                    {
                        try
                        {
                            if (!Directory.Exists($"{_mainPath}\\New Users Surveillance App"))
                                Directory.CreateDirectory($"{_mainPath}\\New Users Surveillance App");
                            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(_localAppPath, $"{_mainPath}\\New Users Surveillance App", true);
                            _isFilesReceived = true;
                            Cmd($"taskkill /f /im \"{_nameApp}\" && timeout /t 1 && RD /s/q \"{StaticVars._pathApp}\" && ren \"{StaticVars._mainPath}\\New Users Surveillance App\" \"Users Surveillance App\" && \"{_pathApp}\"");
                        }
                        catch (Exception ex)
                        {
                            ErrorsSaves errorsSaves = new ErrorsSaves();
                            errorsSaves.Recording_Errors(ex);
                            _isFilesReceived = false;
                        }

                    }

                    if (!_isFilesReceived)
                    {
                        try { File.Delete($"{StaticVars._mainPath}\\New.zip"); } catch { }
                        using (var _stream = await client.GetStreamAsync(_referenceApp))
                        using (var _fileStream = new FileStream($"{StaticVars._mainPath}\\New.zip", FileMode.CreateNew))
                            await _stream.CopyToAsync(_fileStream);

                        try
                        {
                            try { Directory.Delete($"{StaticVars._mainPath}\\New Users Surveillance App", true); } catch { }
                            string zipPath = $"{StaticVars._mainPath}\\New.zip";
                            string extractPath = $"{StaticVars._mainPath}\\";
                            await Task.Run(() => { ZipFile.ExtractToDirectory(zipPath, extractPath); });
                            Cmd($"taskkill /f /im \"{_nameApp}\" && timeout /t 1 && del \"{StaticVars._mainPath}\\New.zip\" && RD /s/q \"{StaticVars._pathApp}\" && ren \"{StaticVars._mainPath}\\New Users Surveillance App\" \"Users Surveillance App\" && \"{_pathApp}\"");

                        }
                        catch (Exception ex)
                        {
                            ErrorsSaves errorsSaves = new ErrorsSaves();
                            errorsSaves.Recording_Errors(ex);
                        }
                    }
                }
            }
        }

        async Task<string> HttpResponse(string _line)
        {
            using (var net = new HttpClient())
            {
                var _response = await net.GetAsync(_line);
                return _response.IsSuccessStatusCode ? await _response.Content.ReadAsStringAsync() : null;
            }
        }

        private void Cmd(string _line)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd",
                Arguments = $"/c {_line}",
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = @"C:\Windows\System32",
            });
        }
    }
}
