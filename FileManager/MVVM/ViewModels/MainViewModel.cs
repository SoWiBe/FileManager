using FileManager.Core;
using FileManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace FileManager.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<IModel> ElementsOfDirectory { get; set; }

        public IModel _selectedElement;
        public IModel Element { get { return _selectedElement; } set { _selectedElement = value; OnPropertyChanged(); } }

        public MainViewModel()
        {
            ElementsOfDirectory = new ObservableCollection<IModel>();

            SetBaseElements();
        }

        private void SetBaseElements()
        {

            string[] dirs = Directory.GetFiles(Directory.GetCurrentDirectory());
            MessageBox.Show(Directory.GetCurrentDirectory() + "");
            foreach (string item in dirs)
            {
                ElementsOfDirectory.Add(new FolderModel() { Name = item });
            }
        }
    }
}
