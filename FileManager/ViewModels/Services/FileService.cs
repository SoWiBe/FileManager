using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileManager.ViewModels.Services
{
    public static class FileService
    {

        public static int GetFilesCount(DirectoryInfo d)
        {
            try
            {
                return d.GetFiles().Count() + d.GetDirectories().Count();
            }
            catch (UnauthorizedAccessException ex)
            {
                return 0;
            }
        }

        public static bool CheckFileOrFolder(string path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    return true;
                }
                return false;
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
