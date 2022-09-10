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
using FileManager.DB;

namespace FileManager.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        //main collection folders and files
        public ObservableCollection<IModel> ElementsOfDirectory { get; set; }
        public ObservableCollection<IModel> SearchCollection { get; set; }

        //search text for writing in textbox and use this for check available element in collection
        private string _searchText;
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                OnPropertyChanged();
                //SearchFolderAndFile();
            }
        }

        //information about selected file or folder
        private string _info;
        public string Info
        {
            get { return _info; }
            set { _info = value; OnPropertyChanged(); }
        }

        //selected model for click on the listbox
        public IModel _selectedElement;
        public IModel Element { get { return _selectedElement; } 
            set { _selectedElement = value; OnPropertyChanged(); OpenFileInfo(); } }


        //create commands for communication with buttons, doubleclicks and etc
        public RelayCommand OpenCommand { get; set; }
        public RelayCommand OpenMoreInfoCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }



        public MainViewModel()
        {
            ElementsOfDirectory = new ObservableCollection<IModel>();

            OpenCommand = new RelayCommand(o => OpenFileOrFolder());
            OpenMoreInfoCommand = new RelayCommand(o => OpenFileInfo());
            Info = "Just some click to file or folder)";

            SetFoldersAndFiles("C:\\");
            
        }

        //show all folders and files
        private async Task<string> SetFoldersAndFiles(string path)
        {
            ClearFoldersAndFiles();
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            for (int i = 0; i < dirs.Length; i++)
            {
                ElementsOfDirectory.Add(new FolderModel() { Name = new DirectoryInfo(dirs[i]).Name, Path = dirs[i], Icon = "/Images/folder.png" });
            }

            for (int i = 0; i < files.Length; i++)
            {
                ElementsOfDirectory.Add(new FileModel() { Name = new FileInfo(files[i]).Name, Path = files[i], Icon = "/Images/files.png" });
            }
            return "Success!";
        }

        private void ClearFoldersAndFiles()
        {
            ElementsOfDirectory.Clear();
        }

        private async void OpenFileOrFolder()
        {
            if (CheckFileOrFolder(Element.Path))
            {
                await SetFoldersAndFiles(Element.Path);
                return;
            }
            //write info about file in db
            await DbWriter.AddRecord(new Files() { Filename = Element.Name, DataVisited = DateTime.Now.ToString() });


            System.Diagnostics.Process.Start(Element.Path);


        }

        //activating after one click on the listbox
        private async void OpenFileInfo()
        {
            if(Element == null)
            {
                return;
            }
            //general info
            Info = "Name: " + Element.Name + "\n";
            
            //check our selected element folder or file
            if (!CheckFileOrFolder(Element.Path))
            {
                //write file info
                FileInfo fileInfo = new FileInfo(Element.Path);
                Info += "Directory Name: " + fileInfo.DirectoryName + "\n";
                Info += "Creation Time: " + fileInfo.CreationTime.ToString() + "\n";
                Info += "Size: " + fileInfo.Length + " byte.\n";
                return;
            }
            //write folder info
            Info += "Type: Folder\n";
            Info += "Size: " + await DirSize(new DirectoryInfo(Element.Path)) + " байт.\n";
            Info += "Count Files: " + GetFilesCount(new DirectoryInfo(Element.Path)) + "\n";
        }

        //caculating folder size
        private async Task<long> DirSize(DirectoryInfo d, long limit = 0)
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

        private int GetFilesCount(DirectoryInfo d)
        {
            try
            {
                return d.GetFiles().Count();
            }
            catch(UnauthorizedAccessException ex)
            {
                return 0;
            }
        }

        private bool CheckFileOrFolder(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return true;
            }
            return false;
        }
    }
}
