using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using static Users_App.Classes.StaticVars;

namespace Users_App.Sevice
{
    internal class Handler
    {
        public void GettingInformationAboutNeedUpdate()
        {
            if (Convert.ToDouble($"{_currentVersionApp.Split('.')[0]}.{_currentVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture) < Convert.ToDouble($"{_newVersionApp.Split('.')[0]}.{_newVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture))
            {
                _isNeedUpdateApp = true;
            }
            else if (Convert.ToDouble($"{_currentVersionApp.Split('.')[0]}.{_currentVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture) > Convert.ToDouble($"{_newVersionApp.Split('.')[0]}.{_newVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture))
            {
                _isCurrentVersionAppNewer = true;
            }
            else if (Convert.ToDouble($"{_currentVersionApp.Split('.')[0]}.{_currentVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture) == Convert.ToDouble($"{_newVersionApp.Split('.')[0]}.{_newVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture))
            {
                if (_currentVersionApp.Count(f => f == '.') == 2 && _newVersionApp.Count(f => f == '.') == 3)
                {
                    _isNeedUpdateApp = true;
                }
                else if (_currentVersionApp.Count(f => f == '.') == 3 && _newVersionApp.Count(f => f == '.') == 2)
                {
                    _isCurrentVersionAppNewer = true;
                }
                else if (_currentVersionApp.Count(f => f == '.') == 3 && _newVersionApp.Count(f => f == '.') == 3)
                {
                    if (Convert.ToInt32(_currentVersionApp.Split('.')[2], CultureInfo.InvariantCulture) < Convert.ToInt32(_newVersionApp.Split('.')[2], CultureInfo.InvariantCulture))
                        _isNeedUpdateApp = true;
                    if (Convert.ToInt32(_currentVersionApp.Split('.')[2], CultureInfo.InvariantCulture) > Convert.ToInt32(_newVersionApp.Split('.')[2], CultureInfo.InvariantCulture))
                        _isCurrentVersionAppNewer = true;
                }
            }
        }
    }
}
