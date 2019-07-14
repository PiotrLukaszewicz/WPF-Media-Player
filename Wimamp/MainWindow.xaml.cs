using System;
using Microsoft.Win32;
using System.Linq;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.IO;
using TallComponents.PDF.Rasterizer;
using TallComponents.PDF.Rasterizer.Configuration;
using System.Xml;

namespace Wimamp
{
    // ciemniejszy #076CA6
    // jesniejszy #00AADD
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PlaylistWindow playlist;
        public static bool mediaPlayerIsPlaying = false;
        public static bool userIsDraggingSlider = false;
        public static bool loop = false;
        public static bool shuffle = false;
        //public static readonly string applicationPath = new System.Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)).LocalPath;

        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        //void Application_Startup_Loaded(object sender, StartupEventArgs e)
        //{
        //    if (e.Args.Length == 1) //make sure an argument is passed
        //    {
        //        FileInfo file = new FileInfo(e.Args[0]);
        //        if (Application.Current.Properties["ArbitraryArgName"] != null)
        //            {
        //            OpenFileDialog openFileDialog = new OpenFileDialog
        //            {
        //                Filter = "Media files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg|All files (*.*)|*.*"
        //            };
        //            if (openFileDialog.ShowDialog() == true)
        //            {
        //                MePlayer.Source = new Uri(openFileDialog.FileName);
        //                TbSongName.Text = openFileDialog.SafeFileName;
        //                playlist.currentPlaylist = new Playlist();
        //                playlist.LbPlaylist.ItemsSource = playlist.currentPlaylist.songs;
        //                Song song1 = new Song();
        //                song1.Uri = openFileDialog.FileName;
        //                var files = TagLib.File.Create(song1.Uri);
        //                song1.Duration = files.Properties.Duration.ToString(@"mm\:ss");
        //                song1.Name = System.IO.Path.GetFileName(openFileDialog.FileName);
        //                playlist.currentPlaylist.songs.Add(song1);
        //            }
        //        }
        //    }
        //}

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            playlist = new PlaylistWindow {Owner = this};
            playlist.WindowStartupLocation = WindowStartupLocation.Manual;
            PlaylistPositioning();
            playlist.Width = this.Width + 2;
            playlist.Show();
            BtNext.Command = MediaCommands.NextTrack;
            BtPrevious.Command = MediaCommands.PreviousTrack;
        }

        private void MainWindow_OnLocationChanged(object sender, EventArgs e)
        {
            PlaylistPositioning();
        }

        private void PlaylistPositioning()
        {
            playlist.Top = this.Top + this.Height - 7;
            playlist.Left = this.Left - 1;
        }

        //

        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((MePlayer.Source != null) && (MePlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                SlProgress.Minimum = 0;
                SlProgress.Maximum = MePlayer.NaturalDuration.TimeSpan.TotalSeconds;
                SlProgress.Value = MePlayer.Position.TotalSeconds;
            }
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Media files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                MePlayer.Source = new Uri(openFileDialog.FileName);
                TbSongName.Text = openFileDialog.SafeFileName;
                playlist.currentPlaylist = new Playlist();
                playlist.LbPlaylist.ItemsSource = playlist.currentPlaylist.songs;
                Song song1 = new Song();
                song1.Uri = openFileDialog.FileName;
                var file = TagLib.File.Create(song1.Uri);
                song1.Duration = file.Properties.Duration.ToString(@"mm\:ss");
                song1.Name = System.IO.Path.GetFileName(openFileDialog.FileName);
                playlist.currentPlaylist.songs.Add(song1);
            }
        }       

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (MePlayer != null) && (MePlayer.Source != null);
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MePlayer.Play();
            mediaPlayerIsPlaying = true;
        }

        public void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        public void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MePlayer.Pause();
        }

        public void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        public void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MePlayer.Stop();
            mediaPlayerIsPlaying = false;
        }

        private void SlProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void SlProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            MePlayer.Position = TimeSpan.FromSeconds(SlProgress.Value);
        }

        private void SlProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LbSongTime.Content = TimeSpan.FromSeconds(SlProgress.Value).ToString(@"mm\:ss");
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            MePlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        }

        private void MuteVolume_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MuteVolume_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MePlayer.IsMuted = !MePlayer.IsMuted;
            SlVolume.IsEnabled = (!MePlayer.IsMuted);
        }

        private void NextTrack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var next = playlist.currentPlaylist.Next();
            if (next != null)
            {
                MePlayer.Source = next;
                playlist.LbPlaylist.SelectedIndex = playlist.currentPlaylist.currentIndex;
            }
        }

        private void PreviousTrack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var prev = playlist.currentPlaylist.Previous();
            if (prev != null)
            {
                MePlayer.Source = prev;
                playlist.LbPlaylist.SelectedIndex = playlist.currentPlaylist.currentIndex;
            }
        }

        private void Stack_OnLoaded(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (Storyboard)this.stack.FindResource("slide"); stack.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => { sb.Begin(); }));
        }

        private void MePlayer_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            if (loop)
            {
                MePlayer.Position = TimeSpan.Zero;
                MePlayer.Play();
                return;
            }
            var next = playlist.currentPlaylist.Next();
            if (next != null)
            {
                MePlayer.Source = next;
                playlist.LbPlaylist.SelectedIndex = playlist.currentPlaylist.currentIndex;
            }
        }

        private void NextTrack_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = playlist.currentPlaylist.currentIndex != playlist.currentPlaylist.songs.Count - 1;
        }
        private void PreviousTrack_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = playlist.currentPlaylist.currentIndex != 0;
        }

        private void OpenPlaylist_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Playlist newPlaylist = new Playlist();
            if (newPlaylist.loadPlaylist())
            {
                playlist.currentPlaylist = newPlaylist;
                playlist.LbPlaylist.ItemsSource = playlist.currentPlaylist.songs;
            }
            Application.Current.Windows.OfType<MainWindow>().First().MePlayer.Source = playlist.currentPlaylist.Play();
            playlist.LbPlaylist.SelectedIndex = 0;
            playlist.currentPlaylist.currentIndex = 0;
            Application.Current.Windows.OfType<MainWindow>().First().MePlayer.Play();
            MainWindow.mediaPlayerIsPlaying = true;
        }

        private void BtLoop_OnClick(object sender, RoutedEventArgs e)
        {
            if (loop)
            {
                loop = false;
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"/Wimamp;component/media/loop.png", UriKind.Relative));
                BtLoop.Content = img;
            }
            else
            {
                loop = true;
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"/Wimamp;component/media/loopActive.png", UriKind.Relative));
                BtLoop.Content = img;
            }
        }

        private void BtShuffle_OnClick(object sender, RoutedEventArgs e)
        {
            if (shuffle)
            {
                shuffle = false;
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"/Wimamp;component/media/shuffle.png", UriKind.Relative));
                BtShuffle.Content = img;
            }
            else
            {
                shuffle = true;
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"/Wimamp;component/media/shuffleActive.png", UriKind.Relative));
                BtShuffle.Content = img;
            }
        }

        private void Style_Button(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            string tagValue = String.Empty;
            tagValue = button.Tag.ToString();
            Application.Current.Resources.MergedDictionaries.Clear();
            if (tagValue != null)
            {
                
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(tagValue, UriKind.Relative) });

                //ResourceDictionary newDictionary = new ResourceDictionary();
                //newDictionary.Source = new Uri(@"/YellowTheme.xaml", UriKind.Relative);
                //Application.Current.Resources.MergedDictionaries[0] = newDictionary;
            }
        }

        //drukowanie pliku
        private void ButtonPrintPressed(object sender, RoutedEventArgs e)
        {
            //możliwość wyboru odpowiedniego pliku - OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".pdf",
                Filter = "Playlist files (*.pdf) | *.pdf"
            };
            bool? fileOpenResult = openFileDialog.ShowDialog();
            if(fileOpenResult != true)
            {
                return;
            }

            //wyświetlanie PrintDialog dla użytkownika gdzie może zmienić wartość atrybutów
            PrintDialog printDialog = new PrintDialog();
            printDialog.PageRangeSelection = PageRangeSelection.AllPages;
            printDialog.UserPageRangeEnabled = true;
            bool? doPrint = printDialog.ShowDialog();
            if(doPrint != true)
            {
                return;
            }

            //otwieranie pliku .pdf renderowanie pliku PDF do WPFa(użycie PDFResterizer.NET)

            FixedDocument fixedDocument;
            using (FileStream pdfFile = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
            {
                Document document = new Document(pdfFile);
                RenderSettings renderSettings = new RenderSettings();
                ConvertToWpfOptions renderOptions = new ConvertToWpfOptions { ConvertToImages = false };
                renderSettings.RenderPurpose = RenderPurpose.Print;
                renderSettings.ColorSettings.TransformationMode = ColorTransformationMode.HighQuality;
                //convert the pdf with the rendersettings and options to a fixed-size document which can be printed
                fixedDocument = document.ConvertToWpf(renderSettings, renderOptions);
            }
            printDialog.PrintDocument(fixedDocument.DocumentPaginator, "Print");

        }
    }
    
}
