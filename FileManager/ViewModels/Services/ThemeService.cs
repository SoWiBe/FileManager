using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileManager.ViewModels.Services
{
    public static class ThemeService
    {
        public static void ThemeChange(bool themeStatus)
        {
            try
            {
                var uri = new Uri(@"View/Themes/LightTheme.xaml", UriKind.Relative);

                if (themeStatus)
                {
                    uri = new Uri(@"View/Themes/RedTheme.xaml", UriKind.Relative);
                }
                MessageBox.Show(uri.ToString());
                ResourceDictionary resourceDictionary = Application.LoadComponent(uri) as ResourceDictionary;
                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            } 
            catch( Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
