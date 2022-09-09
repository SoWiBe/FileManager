﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.MVVM.Models
{
    public class FileModel : IModel
    {
        public string Name { get; set; }
        public string Size { get; set ; }
        public string Path { get; set; }
    }
}
