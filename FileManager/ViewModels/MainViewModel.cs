using FileManager.Core;
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
using System.Windows.Input;
using FileManager.ViewModels.Services;
using Prism.Mvvm;

namespace FileManager.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private string _title = "File Manager";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        //search text for writing in textbox and use this for check available element in collection
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                SearchFolderAndFiles();
            }
        }

        //information about selected file or folder
        private string _info;
        public string Info
        {
            get => _info;
            set => SetProperty(ref _info, value);
        }

        //selected model for click on the listbox
        public IModel _selectedElement;
        public IModel Element
        {
            get => _selectedElement;
            set
            {
                SetProperty(ref _selectedElement, value);
                OpenFileInfo();
            }
        }

        private string _currentPath;

        private Visibility _backButtonState;
        public Visibility BackButtonState
        {
            get => _backButtonState;
            set => SetProperty(ref _backButtonState, value);
        }

        private bool _themeStatus;
        public bool ThemeStatus
        {
            get => _themeStatus;
            set
            {
                SetProperty(ref _themeStatus, value);
                ThemeService.ThemeChange(_themeStatus);
            }
        }

        //create commands for communication with buttons, doubleclicks and etc
        private RelayCommand _openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand ?? (_openCommand = new RelayCommand(o => OpenFileOrFolder()));
            }
        }

        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new RelayCommand(o => SearchFolderAndFiles()));
            }
        }

        private RelayCommand _comeBackCommand;
        public RelayCommand ComeBackCommand
        {
            get
            {
                return _comeBackCommand ?? (_comeBackCommand = new RelayCommand(o => ComeBackToThePastDirectory()));
            }
        }

        private RelayCommand _closeCommand;
        public RelayCommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new RelayCommand(o => WindowService.CloseWindow()));
            }
        }

        private RelayCommand _minimizeCommand;
        public RelayCommand MinimizeCommand
        {
            get
            {
                return _minimizeCommand ?? (_minimizeCommand = new RelayCommand(o => WindowService.MinimizeWindow()));
            }
        }

        private RelayCommand _maximizeCommand;
        public RelayCommand MaximizeCommand
        {
            get
            {
                return _maximizeCommand ?? (_maximizeCommand = new RelayCommand(o => WindowService.MaximizeWindow()));
            }
        }

        //main update collection folders and files
        public ObservableCollection<IModel> ElementsOfDirectory { get; set; }
        //main collection for search, filtered, sorting and other
        public List<IModel> sourceItems = new List<IModel>();
        //collection for start
        private List<DriveInfo> drives;

        public MainViewModel()
        {
            drives = DriveInfo.GetDrives().ToList();

            StartupConfiguration();

            ElementsOfDirectory = new ObservableCollection<IModel>(sourceItems);
            
            Info = "Just some click to file or folder)";

            BackButtonState = Visibility.Hidden;

            ThemeService.ThemeChange(false);
        }

        //setup start array with drives
        private void StartupConfiguration()
        {
            sourceItems.Clear();

            foreach (var item in drives)
            {
                sourceItems.Add(new DriveModel() { Name = new DirectoryInfo(item.Name).Name, Path = item.Name, Icon = "/Images/drive.png" });
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

        //back to the past directory
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

        //open file or folder with many files and folders
        public async void OpenFileOrFolder()
        {
            try
            {
                if (FileService.CheckFileOrFolder(Element.Path))
                {
                    BackButtonState = Visibility.Visible;
                    _currentPath = Element.Path;
                    await SetFoldersAndFiles(Element.Path);
                    return;
                }

                //write info about file in db
                await DbWriter.AddRecord(new Files() { Filename = Element.Name, DataVisited = DateTime.Now.ToString() });

                await Task.Run(() => System.Diagnostics.Process.Start(Element.Path));
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //activating after one click on the listbox
        private async void OpenFileInfo()
        {
            if (Element == null || drives.Any(drive => drive.Name.Equals(Element.Name)))
            {
                return;
            }
            //general info
            Info = "Name: " + Element.Name + "\n";
            
            //check our selected element folder or file
            if (!FileService.CheckFileOrFolder(Element.Path))
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
            Info += "Size: " + await FileService.DirSize(new DirectoryInfo(Element.Path)) + " байт.\n";
            Info += "Count Files: " + FileService.GetFilesCount(new DirectoryInfo(Element.Path)) + "\n";
        }
    }
}
