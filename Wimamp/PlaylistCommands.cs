using System.Windows.Input;

namespace Wimamp
{
    public class PlaylistCommands
    {
        private static RoutedUICommand newPlaylist;
        private static RoutedUICommand openPlaylist;
        private static RoutedUICommand savePlaylist;
        private static RoutedUICommand playPlaylist;

        static PlaylistCommands()
        {
            newPlaylist = new RoutedUICommand(
            "Utwórz nową playlistę", "Utwórz",
            typeof(PlaylistCommands));
            openPlaylist = new RoutedUICommand(
            "Otwórz playlistę", "Otwórz",
            typeof(PlaylistCommands));
            savePlaylist = new RoutedUICommand(
            "Zapisz playlistę", "Zapisz",
            typeof(PlaylistCommands));
            playPlaylist = new RoutedUICommand(
            "Odtwórz playlistę", "Odtwórz",
            typeof(PlaylistCommands));
        }
        public static RoutedUICommand NewPlaylist
        {
            get { return newPlaylist; }
        }
        public static RoutedUICommand OpenPlaylist
        {
            get { return openPlaylist; }
        }
        public static RoutedUICommand SavePlaylist
        {
            get { return savePlaylist; }
        }
        public static RoutedUICommand PlayPlaylist
        {
            get { return playPlaylist; }
        }
    }
}