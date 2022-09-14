using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FileManager.Models
{
    public class DriveModel : IModel
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
