using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileManager.DB
{
    public static class DbWriter
    {
        public static async Task<string> AddRecord(Files file)
        {
            if(file == null)
            {
                MessageBox.Show("File is undefined");
                return "Error";
            }
            FilesDBEntities.GetContext().Files.Add(file);
            await FilesDBEntities.GetContext().SaveChangesAsync();

            return "Success!";
        }
    }
}
