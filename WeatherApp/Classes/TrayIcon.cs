using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.ObjectModel;

namespace WeatherApp.Classes
{

    public class TrayIcon
    {
        private MainWindow mainWindow;
        private ContextMenu m_menu;
        private NotifyIcon ni;

        public TrayIcon(MainWindow mainWindow, List<string> locationsList)
        {
            this.mainWindow = mainWindow;
            ni = new NotifyIcon();
            var iconURI = new Uri("/Icons/Sun.ico", UriKind.Relative);
            Stream iconStream = System.Windows.Application.GetResourceStream(iconURI).Stream;
            ni.Icon = new System.Drawing.Icon(iconStream);
            ni.Text = "Weather";
            ni.Visible = true;
            ni.ContextMenu = CreateContextMenu(locationsList);
        }

        public ContextMenu CreateContextMenu(List<string> locationsList)
        {
            ContextMenu tempCMenu = new ContextMenu();
            tempCMenu.MenuItems.Add(0, new MenuItem("Open", new System.EventHandler(TrayOpen_Click)));
            MenuItem separatorMenuItem = new MenuItem("-");
            tempCMenu.MenuItems.Add(1, separatorMenuItem);
            tempCMenu.MenuItems.Add(2, CreateLocationsSMenu(locationsList));
            tempCMenu.MenuItems.Add(3, new MenuItem("Close", new System.EventHandler(TrayExit_Click)));

            return tempCMenu;
        }

        public void UpdateContextMenu(List<string> locationsList)
        {
            ni.ContextMenu = null;
            ni.ContextMenu = CreateContextMenu(locationsList);
        }

        public MenuItem CreateLocationsSMenu(List<string> locationsList)
        {
            MenuItem mnuSubMenuItem = new MenuItem("Locations");

            foreach(string location in locationsList)
            {
                mnuSubMenuItem.MenuItems.Add(location, new EventHandler(TrayLocation_Click));
            }

            return mnuSubMenuItem;

        }

        public void ShowTrayInformation(string Title, string Content)
        {
            ni.BalloonTipTitle = Title;
            ni.BalloonTipText = Content;
            ni.BalloonTipIcon = ToolTipIcon.None;
            ni.Visible = true;
            ni.ShowBalloonTip(30000);

            ni.BalloonTipClicked += delegate (object sender, EventArgs args)
            {
                mainWindow.Show();
                mainWindow.Activate();
            };
        }

        private void TrayExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void TrayOpen_Click(object sender, EventArgs e)
        {
            mainWindow.Show();
            mainWindow.Activate();
            
        }

        private void TrayLocation_Click(object sender, EventArgs e)
        {
            mainWindow.Show();
            mainWindow.Activate();
            MenuItem tempMenuItem = (MenuItem)sender;
            mainWindow.LocationsLB.SelectedIndex = tempMenuItem.Index;
            //maximize window
            mainWindow.WindowState = System.Windows.WindowState.Normal;

        }

    }
}
