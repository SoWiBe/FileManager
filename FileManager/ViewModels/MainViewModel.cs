﻿using FileManager.Core;
using FileManager.Models;
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
using FileManager.ViewModels.Commands;

namespace FileManager.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        //main update collection folders and files
        public ObservableCollection<IModel> ElementsOfDirectory { get; set; }
        //main collection for search, filtered, sorting and other
        public List<IModel> sourceItems = new List<IModel>();


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
                SearchFolderAndFiles();
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

        private string _currentPath;

        private Visibility _backButtonState;
        public Visibility BackButtonState
        {
            get
            {
                return _backButtonState;
            }
            set
            {
                _backButtonState = value;
                OnPropertyChanged();
            }
        }

        private List<DriveInfo> drives;

        //create commands for communication with buttons, doubleclicks and etc
        private RelayCommand _openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                if(_openCommand == null)
                {
                    _openCommand = new RelayCommand(o => OpenFileOrFolder());
                }
                return _openCommand;
            }
        }

        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                if(_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(o => SearchFolderAndFiles());
                }
                return _searchCommand;
            }
        }

        private RelayCommand _comeBackCommand;
        public RelayCommand ComeBackCommand
        {
            get
            {
                if(_comeBackCommand == null)
                {
                    _comeBackCommand = new RelayCommand(o => ComeBackToThePastDirectory());
                }
                return _comeBackCommand;
            }
        }

        private RelayCommand _closeCommand;
        public RelayCommand CloseCommand
        {
            get
            {
                if(_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(o => CloseWindow());
                }
                return _closeCommand;
            }
        }

        private RelayCommand _minimizeCommand;
        public RelayCommand MinimizeCommand
        {
            get
            {
                if(_minimizeCommand == null)
                {
                    _minimizeCommand = new RelayCommand(o => MinimizeWindow());
                }
                return _minimizeCommand;
            }
        }

        private RelayCommand _maximizeCommand;
        public RelayCommand MaximizeCommand
        {
            get
            {
                if (_maximizeCommand == null)
                {
                    _maximizeCommand = new RelayCommand(o => MaximizeWindow());
                }
                return _maximizeCommand;
            }
        }

        public RelayCommand DragMoveCommand { get; set; }


        public MainViewModel()
        {
            drives = DriveInfo.GetDrives().ToList();

            StartupConfiguration();

            ElementsOfDirectory = new ObservableCollection<IModel>(sourceItems);
            
            Info = "Just some click to file or folder)";

            BackButtonState = Visibility.Hidden;
        }

        private void StartupConfiguration()
        {
            sourceItems.Clear();

            foreach (var item in drives)
            {
                sourceItems.Add(new FolderModel() { Name = new DirectoryInfo(item.Name).Name, Path = item.Name, Icon = "/Images/folder.png" });
            }

            if (ElementsOfDirectory != null)
            {
                ElementsOfDirectory.Clear();
                foreach (var value in sourceItems)
                {
                    ElementsOfDirectory.Add(value);
                }
            }
        }

        //search realization
        private void SearchFolderAndFiles()
        {
            var searchItem = SearchText;

            if (string.IsNullOrWhiteSpace(searchItem))
            {
                searchItem = string.Empty;
            }

            searchItem = searchItem.ToLowerInvariant();

            var filteredItems = sourceItems.Where(value => value.Name.ToLowerInvariant().Contains(searchItem)).ToList();

            foreach (var value in sourceItems)
            {
                if (!filteredItems.Contains(value))
                {
                    ElementsOfDirectory.Remove(value);
                }
                else if (!ElementsOfDirectory.Contains(value))
                {
                    ElementsOfDirectory.Add(value);
                }
            }
        }

        //show all folders and files
        private async Task<string> SetFoldersAndFiles(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            sourceItems.Clear();

            for (int i = 0; i < dirs.Length; i++)
            {
                sourceItems.Add(new FolderModel() { Name = new DirectoryInfo(dirs[i]).Name, Path = dirs[i], Icon = "/Images/folder.png" });
            }

            for (int i = 0; i < files.Length; i++)
            {
                sourceItems.Add(new FileModel() { Name = new FileInfo(files[i]).Name, Path = files[i], Icon = "/Images/files.png" });
            }

            if (ElementsOfDirectory != null)
            {
                ElementsOfDirectory.Clear();
                foreach (var value in sourceItems)
                {
                    ElementsOfDirectory.Add(value);
                }
            }
            return "Success!";
        }

        private async void ComeBackToThePastDirectory()
        {
            if (drives.Any(item => item.Name.Equals(_currentPath)))
            {
                BackButtonState = Visibility.Hidden;
                StartupConfiguration();
                return;
            }

            var pastPath = Directory.GetParent(_currentPath);

            await SetFoldersAndFiles(pastPath.FullName);

            _currentPath = pastPath.FullName;
        }

        public async void OpenFileOrFolder()
        {
            if (CheckFileOrFolder(Element.Path))
            {
                BackButtonState = Visibility.Visible;
                _currentPath = Element.Path;
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
            if (Element == null)
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
        private async Task<long> DirSize(DirectoryInfo d, long limit = 100)
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
                return d.GetFiles().Count() + d.GetDirectories().Count();
            }
            catch(UnauthorizedAccessException ex)
            {
                return 0;
            }
        }

        private bool CheckFileOrFolder(string path)
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
            catch(IOException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        //methods for upper panel
        private void MinimizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow()
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void CloseWindow()
        {
            Application.Current.Shutdown();
        }
    }
}
