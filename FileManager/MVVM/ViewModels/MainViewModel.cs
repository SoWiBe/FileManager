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
using System.Diagnostics;

namespace FileManager.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<IModel> ElementsOfDirectory { get; set; }

        public IModel _selectedElement;
        public IModel Element { get { return _selectedElement; } 
            set { _selectedElement = value; OnPropertyChanged(); } }

        public RelayCommand OpenCommand { get; set; }

        public MainViewModel()
        {
            ElementsOfDirectory = new ObservableCollection<IModel>();
            OpenCommand = new RelayCommand(o => OpenFileOrFolder());
            SetBaseElements();
        }

        private void SetBaseElements()
        {

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
            string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory());

            for (int i = 0; i < dirs.Length; i++)
            {
                ElementsOfDirectory.Add(new FolderModel() { Name = dirs[i] });
            }

            for (int i = 0; i < files.Length; i++)
            {
                ElementsOfDirectory.Add(new FileModel() { Name = files[i] });
            }

            
        }
        
        private void OpenFileOrFolder()
        {
            Process.Start(Element.Name);
        }

        private void OpenFileInfo()
        {

        }
    }
}
