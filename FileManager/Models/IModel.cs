using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Models
{
    public interface IModel
    {
        string Path { get; set; }
        string Name { get; set; }
        string Icon { get; set; }

    }
}
