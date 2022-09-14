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

        public static async Task<long> DirSize(DirectoryInfo d, long limit = 100)
        {
            try
            {
                // Add file sizes.
                long Size = 0;
                FileInfo[] fis = d.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    Size += fi.Length;
                    if (limit > 0 && Size > limit)
                        return Size;
                }
                // Add subdirectory sizes.
                DirectoryInfo[] dis = d.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                    Size += await DirSize(di, limit);
                    if (limit > 0 && Size > limit)
                        return Size;
                }
                return (Size);
            }
            catch (UnauthorizedAccessException ex)
            {
                return 0;
            }

        }
    }
}
