using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FileManager.Models
{
    public class FolderModel : IModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
    }
}
