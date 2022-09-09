using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.DB
{
    public partial class FilesDBEntities
    {
        private static FilesDBEntities _context;

        public static FilesDBEntities GetContext()
        {
            if (_context == null)
            {
                _context = new FilesDBEntities();
            }
            return _context;
        }
    }
}
