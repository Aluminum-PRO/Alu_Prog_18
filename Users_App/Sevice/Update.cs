using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Users_App.Classes;

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
            //My_Hand.Getting_Information_of_Update_Al(out double size, out app_reference);
            using (HttpClient client = new HttpClient())
            {
                if (Internet.OK())
                {
                    try
                    {
                        File.Delete($"{StaticVars._mainPath}\\New.zip");
                    }
                    catch { }
                    using (var _stream = await client.GetStreamAsync("https://getfile.dokpub.com/yandex/get/https://disk.yandex.ru/d/KgLd6tN2Xa0kdQ"))
                    using (var _fileStream = new FileStream($"{StaticVars._mainPath}\\New.zip", FileMode.CreateNew))
                        await _stream.CopyToAsync(_fileStream);
                    string zipPath = $"{StaticVars._mainPath}\\New.zip";
                    string extractPath = $"{StaticVars._mainPath}\\";
                    try
                    {
                        try
                        {
                            try
                            {
                                Directory.Delete($"{StaticVars._mainPath}\\New Users Surveillance", true);
                            }
                            catch { }
                        }
                        catch { }
                        await Task.Run(() =>
                        {
                            ZipFile.ExtractToDirectory(zipPath, extractPath);
                        });
                        Cmd($"taskkill /f /im \"{_nameApp}\" && timeout /t 1 && del \"{StaticVars._mainPath}\\New.zip\" && RD /s/q \"{StaticVars._pathApp}\" && ren \"{StaticVars._mainPath}\\New Users Surveillance\" \"Users Surveillance\" && \"{_pathApp}\"");
                    }
                    catch (Exception ex)
                    {
                        ErrorsSaves errorsSaves = new ErrorsSaves();
                        errorsSaves.Recording_Errors(ex);
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
