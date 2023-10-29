using System;
using System.Collections.Generic;
using Gtk;
using Coursework;
using Gdk;
using System.Runtime.CompilerServices;
using GLib;

namespace Coursework
{
    class MainWindow : Gtk.Window
    {
        private Builder builder;
        //making obejcts for the widgets
        private Entry url_input;
        private TextView display;
        //making URL objects
        private URL urlObj;
        private URL homeUrl = new URL("https://www.hw.ac.uk");

        //objects for the download button
        private Entry fileInput;
        private Label downloadLabel;

        //list for history
        private List<URL> history = new List<URL>();
        int index = 0;

        private List<favorites> favoritesList = new List<favorites>();

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            this.builder = builder;
            builder.Autoconnect(this);
            //InitializeSetHomePageDialog(builder);

            url_input = (Entry)builder.GetObject("UrlBar");
            display = (TextView)builder.GetObject("DisplayScreen");
            pageUpdate(homeUrl.GetURL);

            //back button
            Button backButton = (Button)builder.GetObject("PrevButton");
            backButton.Clicked += (sender, e) => { goBack(); };

            //forward button    
            Button forwardButton = (Button)builder.GetObject("NextButton");
            forwardButton.Clicked += (sender, e) => { goNext(); };

            //go button
            Button goButton = (Button)builder.GetObject("GoButton");
            goButton.Clicked += goButton_Clicked;
            url_input.Activated += (sender, e) => goButton_Clicked(sender, e);


            //home button
            Button goHome = (Button)builder.GetObject("HomeButton");
            goHome.Clicked += goHome_Clicked;
            goHome.ButtonPressEvent += (sender, e) =>
            {
                if (e.Event.Type == Gdk.EventType.TwoButtonPress)
                {
                    SetNewHomepage window = new SetNewHomepage(homeUrl);
                }
            };

            //refresh button
            Button refreshButton = (Button)builder.GetObject("RefreshButton");
            refreshButton.Clicked += (sender, e) => refreshButton_Clicked(urlObj);

            //favorite button 
            Button favoriteButton = (Button)builder.GetObject("AddFavoriteButton");
            favoriteButton.Clicked += (sender, e) =>
            {
                AddToFavorites window = new AddToFavorites(favoritesList, urlObj, index, "add");
                window.Added += addFavorites_Clicked;
            };

            //download button

            fileInput = (Entry)builder.GetObject("FileNameEntry");
            fileInput.Activated += (sender, e) => downloadButton_Clicked(sender, e);
            downloadLabel = (Label)builder.GetObject("DownloadLabel");

            Button downloadButton = (Button)builder.GetObject("DownloadButton");
            downloadButton.Clicked += downloadButton_Clicked;


            updateHistoryUI(history, builder);
            updateFavoritesUI(favoritesList, builder);

        }

        private void pageUpdate(string urlText)
        {
            urlObj = new URL(urlText);
            url_input.Text = urlText;
            string urlResult = urlObj.GetPageContent;
            display.Buffer.Text = urlResult;
            string pageTitle = urlObj.GetPageTitle;
            this.Title = pageTitle;
            historyUpdate(urlObj);
        }

        private void historyUpdate(URL urlObj)
        {

            if (urlObj.GetStatusCode != 0)
            {
                if (history.Count == 0)
                {
                    history.Add(urlObj);
                    index++;
                }
                else
            if (history[index - 1].GetURL != urlObj.GetURL)
                {
                    history.Add(urlObj);
                    index++;
                }
            }
            updateHistoryUI(history, builder);
        }

        private void contentUpdate(URL homeUrl)
        {
            string urlResult = homeUrl.GetPageContent;
            display.Buffer.Text = urlResult;
            string pageTitle = urlObj.GetPageTitle;
            this.Title = pageTitle;
        }

        private void goBack()
        {
            if (index > 1)
            {
                index--;
                pageUpdate(history[index - 1].GetURL);
            }
            else
            {
                pageUpdate(homeUrl.GetURL);
            }
        }

        private void goNext()
        {
            if (index < history.Count)
            {
                index++;
                pageUpdate(history[index - 1].GetURL);
            }
            else
            {
                pageUpdate(homeUrl.GetURL);
            }
        }
        private void goHome_Clicked(object sender, EventArgs e)
        {

            Console.WriteLine(homeUrl.GetURL);
            pageUpdate(homeUrl.GetURL);
        }

        private void goButton_Clicked(object sender, EventArgs e)
        {
            string urlText = url_input.Text;
            pageUpdate(urlText);
        }

        private void refreshButton_Clicked(URL urlObj)
        {
            string urlText = urlObj.GetURL;
            pageUpdate(urlText);
        }

        private void downloadButton_Clicked(object sender, EventArgs e)
        {
            string filepath = fileInput.Text;
            if (filepath == "")
            {
                filepath = "bulk.txt";
            }
            readwrite rw = new readwrite(filepath);
            string[] file_urls;
            file_urls = rw.read();
            string write_file = "";

            try
            {
                for (int i = 0; i < file_urls.Length; i++)
                {
                    URL url = new URL(file_urls[i]);
                    string urlStatus = url.GetStatusCode.ToString();
                    string urlBytes = url.GetBytes.ToString();
                    string urlResult = url.GetURL;

                    string urlInfo = urlStatus + "  " + urlBytes + "  " + urlResult + "\n";
                    write_file = write_file + urlInfo;
                }
                rw.write("download.txt", write_file);
                display.Buffer.Text = write_file;
                this.Title = "Downloaded File";
                downloadLabel.Text = "File Download Successful!";

            }
            catch (System.Exception)
            {
                downloadLabel.Text = "File Download Failed!";
            }
        }

        private void updateHistoryUI(List<URL> history, Builder builder)
        {
            Gtk.Grid historyGrid = (Gtk.Grid)builder.GetObject("HistoryGrid");

            foreach (Widget child in historyGrid.Children)
            {
                historyGrid.Remove(child);
                child.Destroy();
            }
            int rowIndex = 0;

            List<URL> reversedHistory = new List<URL>(history);
            reversedHistory.Reverse();

            foreach (URL website in reversedHistory)
            {
                // Create the site button
                Gtk.Button siteButton = new Gtk.Button(website.GetPageTitle);
                siteButton.Relief = Gtk.ReliefStyle.None;
                siteButton.Clicked += (sender, e) =>
                {
                    pageUpdate(website.GetURL);
                };

                // Add siteButton to the grid at rowIndex and column 0
                historyGrid.Attach(siteButton, 0, rowIndex, 1, 1);

                // Create the delete button
                Gtk.Button deleteButton = new Gtk.Button("Delete");
                deleteButton.Clicked += (sender, e) =>
                {
                    history.Remove(website);
                    updateHistoryUI(history, builder);
                };

                // Add deleteButton to the grid at rowIndex and column 1
                historyGrid.Attach(deleteButton, 1, rowIndex, 1, 1);

                rowIndex++; // Move to the next row for the next website
            };

            historyGrid.ShowAll();
        }


        private void addFavorites_Clicked(object sender, EventArgs e)
        {
            updateFavoritesUI(favoritesList, builder);
        }

        private void updateFavoritesUI(List<favorites> favoritesList, Builder builder)
        {
            Gtk.Grid favoritesGrid = (Gtk.Grid)builder.GetObject("FavoritesGrid");
            foreach (Widget child in favoritesGrid.Children)
            {
                favoritesGrid.Remove(child);
                child.Destroy();
            }
            for (int i = 0; i < favoritesList.Count; i++)
            {
                favorites favorite = favoritesList[i];

                // Create the favorite button
                Button favoriteButton = new Button();
                favoriteButton.Label = favorite.getName;
                favoriteButton.Relief = ReliefStyle.None;
                favoritesGrid.Attach(favoriteButton, 0, i, 1, 1);

                favoriteButton.Clicked += (sender, e) =>
                {
                    pageUpdate(favorite.getUrl.GetURL);
                };

                // Create the edit button
                Button editButton = new Button("Edit");
                favoritesGrid.Attach(editButton, 1, i, 1, 1);

                // Assign the edit button event
                editButton.Clicked += (sender, e) =>
                {
                    AddToFavorites window = new AddToFavorites(favoritesList, null, i, "edit");
                    window.Added += addFavorites_Clicked;
                };

                // Create the delete button
                Button deleteButton = new Button("Delete");
                favoritesGrid.Attach(deleteButton, 2, i, 1, 1);

                // Assign the delete button event
                deleteButton.Clicked += (sender, e) =>
                {
                    favoritesList.Remove(favorite);
                    updateFavoritesUI(favoritesList, builder);
                };
            }
            favoritesGrid.ShowAll();
        }




        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Gtk.Application.Quit();
        }
    }
}
