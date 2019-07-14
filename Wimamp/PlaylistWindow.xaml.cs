using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace Wimamp
{
    /// <summary>
    /// Logika interakcji dla klasy PlaylistWindow.xaml
    /// </summary>
    public partial class PlaylistWindow : Window
    {
        public Playlist currentPlaylist;
        public PlaylistWindow()
        {
            InitializeComponent();
            currentPlaylist = new Playlist();
            LbPlaylist.ItemsSource = currentPlaylist.songs;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 1) //make sure an argument is passed
            {
                FileInfo file = new FileInfo(e.Args[0]);
                if (file.Exists) //make sure it's actually a file
                {
                    foreach (var uri in e.Args)
                    {
                        Song song1 = new Song();
                        song1.Uri = uri;
                        var files = TagLib.File.Create(song1.Uri);
                        song1.Duration = files.Properties.Duration.ToString(@"mm\:ss");
                        song1.Name = System.IO.Path.GetFileName(uri);
                        currentPlaylist.songs.Add(song1);
                    }
                }
            }
        }

        private void LbPlaylist_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null)
                {
                    foreach (var uri in files)
                    {
                        Song song1 = new Song();
                        song1.Uri = uri;
                        var file = TagLib.File.Create(song1.Uri);
                        song1.Duration = file.Properties.Duration.ToString(@"mm\:ss");
                        song1.Name = System.IO.Path.GetFileName(uri);
                        currentPlaylist.songs.Add(song1);
                    }
                }
            }
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                StackPanel sp = (StackPanel) sender;
                Song sg = (Song) sp.DataContext;
                Uri uri = new Uri(sg.Uri);
                Application.Current.Windows.OfType<MainWindow>().First().MePlayer.Source = uri;
                Application.Current.Windows.OfType<MainWindow>().First().TbSongName.Text = sg.Name;
                currentPlaylist.currentIndex = LbPlaylist.SelectedIndex;
            }
        }

        private void NewPlaylist_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (LbPlaylist.Items.Count != 0)
            {
                currentPlaylist = new Playlist();
                LbPlaylist.ItemsSource = currentPlaylist.songs;
            }
        }

        private void SavePlaylist_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (LbPlaylist.HasItems)
            {
                currentPlaylist.savePlaylist();
            }
        }

        private void OpenPlaylist_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Playlist newPlaylist = new Playlist();
            if (newPlaylist.loadPlaylist())
            {
                currentPlaylist = newPlaylist;
                LbPlaylist.ItemsSource = currentPlaylist.songs;
            }
            Application.Current.Windows.OfType<MainWindow>().First().MePlayer.Source = currentPlaylist.Play();
            LbPlaylist.SelectedIndex = 0;
            currentPlaylist.currentIndex = 0;
            Application.Current.Windows.OfType<MainWindow>().First().MePlayer.Play();
            MainWindow.mediaPlayerIsPlaying = true;


        }

        private void SaveCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = LbPlaylist.HasItems;
        }

        private void PlayPlaylist_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Windows.OfType<MainWindow>().First().MePlayer.Source = currentPlaylist.Play();
            LbPlaylist.SelectedIndex = 0;
            currentPlaylist.currentIndex = 0;
            Application.Current.Windows.OfType<MainWindow>().First().MePlayer.Play();
            MainWindow.mediaPlayerIsPlaying = true;
        }

        private void LbPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SavePDF(object sender, RoutedEventArgs e)
        {
            currentPlaylist.Print();
        }
    }
}