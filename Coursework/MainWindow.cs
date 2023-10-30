using System;
using System.Collections.Generic;
using Gtk;
using Coursework;
using Gdk;
using System.Runtime.CompilerServices;
using GLib;
using System.IO;

namespace Coursework
{
    class MainWindow : Gtk.Window
    {
        private Builder builder;

        //accelerator group for keyboard shortcuts
        private AccelGroup accelGroup;

        //making obejcts for the widgets
        private Entry url_input;
        private TextView display;
        //making URL objects
        private URL urlObj;
        private URL homeUrl;

        //objects for the download button
        private Entry fileInput;
        private Label downloadLabel;

        //list for history
        private List<URL> history = new List<URL>();

        //index for history
        int index = 0;

        //list for favorites with favorite objects
        private List<favorites> favoritesList = new List<favorites>();


        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            this.builder = builder;
            builder.Autoconnect(this);

            readwrite readw = new readwrite("homepage.txt");
            string homeUrlString = readw.read()[0];
            homeUrl = new URL(homeUrlString);

            readwrite readw2 = new readwrite("history.txt");
            string[] historyUrls = readw2.read();
            foreach (string url in historyUrls)
            {
                historyUpdate(new URL(url));
            }

            //adding the accelerator group for keyboard shortcuts
            //made for go home, refresh
            accelGroup = new AccelGroup();
            this.AddAccelGroup(accelGroup);

            //objects for the widgets
            url_input = (Entry)builder.GetObject("UrlBar");
            display = (TextView)builder.GetObject("DisplayScreen");
            pageUpdate(homeUrl.GetURL);

            //------------------------------------------------------------------------------------
            //------------------------------------------------------------------------------------
            //BUTTONS - BACK, FORWARD, GO, HOME, REFRESH, FAVORITES, DOWNLOAD

            //back button
            Button backButton = (Button)builder.GetObject("PrevButton");
            backButton.Clicked += (sender, e) =>
            {
                goBack();
            };

            //forward button    
            Button forwardButton = (Button)builder.GetObject("NextButton");
            forwardButton.Clicked += (sender, e) =>
            {
                goNext();
            };

            //go button
            Button goButton = (Button)builder.GetObject("GoButton");
            goButton.Clicked += goButton_Clicked;
            url_input.Activated += (sender, e) => goButton_Clicked(sender, e);


            //home button
            Button goHome = (Button)builder.GetObject("HomeButton");
            //keyboard shortcut for home button: ctrl + h
            goHome.AddAccelerator("clicked", accelGroup, new AccelKey(Gdk.Key.h, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
            goHome.Clicked += goHome_Clicked;
            //event handling for double clicking the home button
            goHome.ButtonPressEvent += (sender, e) =>
            {
                //if double clicked, open the dialog box to set a new homepage
                if (e.Event.Type == Gdk.EventType.TwoButtonPress)
                {
                    SetNewHomepage window = new SetNewHomepage(homeUrl);
                }
            };

            //refresh button
            Button refreshButton = (Button)builder.GetObject("RefreshButton");
            //keyboard shortcut for refresh button: ctrl + r
            refreshButton.AddAccelerator("clicked", accelGroup, new AccelKey(Gdk.Key.r, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
            refreshButton.Clicked += (sender, e) => refreshButton_Clicked(urlObj);

            //favorite button, opens the popover and event handling for clicking the add button there
            Button favoriteButton = (Button)builder.GetObject("AddFavoriteButton");
            favoriteButton.Clicked += (sender, e) =>
            {
                AddToFavorites window = new AddToFavorites(favoritesList, urlObj, index, "add");
                window.Added += addFavorites_Clicked;
            };

            //clear history button
            Button clearHistoryButton = (Button)builder.GetObject("ClearHistoryButton");
            clearHistoryButton.Clicked += (sender, e) =>
            {
                //clear the history list and update the UI
                history.Clear();
                readw2.write("history.txt", "");
                this.index = 0;
                updateHistoryUI(history, builder);
            };

            //file input for bulk download
            fileInput = (Entry)builder.GetObject("FileNameEntry");
            //event handling for enter key press
            fileInput.Activated += (sender, e) => downloadButton_Clicked(sender, e);

            //bulk download button
            downloadLabel = (Label)builder.GetObject("DownloadLabel");
            //download button
            Button downloadButton = (Button)builder.GetObject("DownloadButton");
            downloadButton.Clicked += downloadButton_Clicked;

            //updating the UI for history and favorites popovers
            updateHistoryUI(history, builder);
            updateFavoritesUI(favoritesList, builder);

            this.DeleteEvent += Window_DeleteEvent;
        }

        //------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------
        //MAIN PAGE UPDATE FUNCTION - UPDATES THE PAGE WITH THE INPUT URL

        //main function used to update the page with the input url
        private void pageUpdate(string urlText)
        {
            //create a new URL object with the input url, get the page content and title
            urlObj = new URL(urlText);
            url_input.Text = urlText;

            //set the page content to the textview
            string urlResult = urlObj.GetPageContent;
            display.Buffer.Text = urlResult;

            //set the title of the window to the page title
            string pageTitle = urlObj.GetPageTitle;
            this.Title = pageTitle;

            //update the history with the new URL object
            historyUpdate(urlObj);
            sensitivitySet();
        }


        //------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------
        //SENSITIVITY FUNCTIONS - SET SENSITIVITY FOR BACK AND FORWARD BUTTONS
        private void sensitivitySet()
        {
            if (index == 1)
            {
                Button backButton = (Button)builder.GetObject("PrevButton");
                backButton.Sensitive = false;
            }
            else
            {
                Button backButton = (Button)builder.GetObject("PrevButton");
                backButton.Sensitive = true;
            }

            if (index == history.Count)
            {
                Button forwardButton = (Button)builder.GetObject("NextButton");
                forwardButton.Sensitive = false;
            }
            else
            {
                Button forwardButton = (Button)builder.GetObject("NextButton");
                forwardButton.Sensitive = true;
            }
        }

        //------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------
        //HISTORY FUNCTIONS - UPDATE HISTORY, GO BACK, GO FORWARD, AND HISTORY UI

        //update the history list with the new URL object
        private void historyUpdate(URL urlObj)
        {

            //even if the page is not found, add it to the history
            if (urlObj.GetStatusCode != 0)
            {
                if (history.Count == 0)
                {
                    history.Add(urlObj);
                    index++;

                }
                else
            //if the page you're adding isnt already latest in the history, add it to the history
            if (history[index - 1].GetURL != urlObj.GetURL)
                {
                    history.Add(urlObj);
                    index++;
                }
            }

            //update the history UI
            updateHistoryUI(history, builder);
        }

        //function to go back in the history
        private void goBack()
        {
            //only possible if current page isn't the first page in history
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


        //function to go forward in the history
        private void goNext()
        {
            //only possible if current page isn't the last page in history
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

        //function to update the history popover


        private void updateHistoryUI(List<URL> history, Builder builder)
        {
            //get the history grid from the builder
            Gtk.Grid historyGrid = (Gtk.Grid)builder.GetObject("HistoryGrid");

            //clear the grid each time the function is called to avoid duplicates 
            //(put a sample website in the glade file to avoid empty grid error)
            foreach (Widget child in historyGrid.Children)
            {
                historyGrid.Remove(child);
                child.Destroy();
            }

            //create a row index
            int rowIndex = 0;

            //reverse the history list so that the latest page is at the top
            List<URL> reversedHistory = new List<URL>(history);
            reversedHistory.Reverse();

            //for each website in the history, create a button with the website title and a delete button
            foreach (URL website in reversedHistory)
            {
                // Create the website button
                Gtk.Button siteButton = new Gtk.Button(website.GetPageTitle);
                siteButton.Relief = Gtk.ReliefStyle.None;

                //if the website button is clicked, update the page
                siteButton.Clicked += (sender, e) =>
                {
                    pageUpdate(website.GetURL);
                };
                historyGrid.Attach(siteButton, 0, rowIndex, 1, 1);

                // Create the delete button
                Gtk.Button deleteButton = new Gtk.Button("Delete");

                //if the delete button is clicked, remove the website from the history and update the UI
                deleteButton.Clicked += (sender, e) =>
                {
                    history.Remove(website);
                    updateHistoryUI(history, builder);
                };
                historyGrid.Attach(deleteButton, 1, rowIndex, 1, 1);

                rowIndex++;
            };

            historyGrid.ShowAll();
        }

        //------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------
        //BUTTON CLICK FUNCTIONS - REFRESH, GO HOME, GO BUTTON
        //function to refresh the page
        private void refreshButton_Clicked(URL urlObj)
        {
            //get the url from the url bar and refresh the page
            string urlText = urlObj.GetURL;
            pageUpdate(urlText);
        }

        //function to go to the homepage
        private void goHome_Clicked(object sender, EventArgs e)
        {
            pageUpdate(homeUrl.GetURL);
        }

        //function to go to the page in the url bar
        private void goButton_Clicked(object sender, EventArgs e)
        {
            //get the url from the url bar and update the page
            string urlText = url_input.Text;
            pageUpdate(urlText);
        }

        //------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------
        //DOWNLOAD FUNCTIONS - BULK DOWNLOAD BUTTON
        //function to download the bulk urls
        private void downloadButton_Clicked(object sender, EventArgs e)
        {
            //get the filepath from the entry field
            string filepath = fileInput.Text;

            //default function if no filepath is entered
            if (filepath == "")
            {
                filepath = "bulk.txt";
            }

            //create a new readwrite object 
            readwrite rw = new readwrite(filepath);

            //create a string array with the urls from the file
            string[] file_urls;
            file_urls = rw.read();

            //create a string to write the download info to
            string write_file = "";

            try
            {
                //for each url in the array, create a new URL object and get the status code, bytes and url
                for (int i = 0; i < file_urls.Length; i++)
                {
                    URL url = new URL(file_urls[i]);
                    string urlStatus = url.GetStatusCode.ToString();
                    string urlBytes = url.GetBytes.ToString();
                    string urlResult = url.GetURL;

                    //add the info to the string and add it to the string that will be written to the file
                    string urlInfo = urlStatus + "  " + urlBytes + "  " + urlResult + "\n";
                    write_file = write_file + urlInfo;
                }

                //write the string to the file and display it in the textview
                rw.write("download.txt", write_file);
                display.Buffer.Text = write_file;

                //update the label and brower title to show the download was successful
                this.Title = "Downloaded File";
                downloadLabel.Text = "File Download Successful!";

            }
            catch (System.Exception)
            {
                //exception handling if the download fails
                downloadLabel.Text = "File Download Failed!";
            }
        }


        //------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------
        //FAVORITES FUNCTIONS - ADD, EDIT, DELETE FAVORITES AND UPDATE FAVORITES UI

        //function to update the favorites popover when a new favorite is added
        private void addFavorites_Clicked(object sender, EventArgs e)
        {
            updateFavoritesUI(favoritesList, builder);
        }

        //function to update the favorites popover
        private void updateFavoritesUI(List<favorites> favoritesList, Builder builder)
        {
            //get the favorites grid from the builder
            Gtk.Grid favoritesGrid = (Gtk.Grid)builder.GetObject("FavoritesGrid");

            //clear the grid each time the function is called to avoid duplicates
            //(put a sample website in the glade file to avoid empty grid error)
            foreach (Widget child in favoritesGrid.Children)
            {
                favoritesGrid.Remove(child);
                child.Destroy();
            }
            //for each favorite in the favorites list, create a button with the website title, an edit button and a delete button
            for (int i = 0; i < favoritesList.Count; i++)
            {
                //get the favorite object
                favorites favorite = favoritesList[i];

                // Create the favorite button
                Button favoriteButton = new Button();
                favoriteButton.Label = favorite.getName;
                favoriteButton.Relief = ReliefStyle.None;
                favoritesGrid.Attach(favoriteButton, 0, i, 1, 1);

                //If the favorite is clicked, update the page
                favoriteButton.Clicked += (sender, e) =>
                {
                    pageUpdate(favorite.getUrl.GetURL);
                };

                // Create the edit button
                Button editButton = new Button("Edit");
                favoritesGrid.Attach(editButton, 1, i, 1, 1);

                // if the edit button is clicked, open the edit window with the favorite object
                editButton.Clicked += (sender, e) =>
                {
                    AddToFavorites window = new AddToFavorites(favoritesList, null, i, "edit");

                    //if the favorite is edited, update the UI
                    window.Added += addFavorites_Clicked;
                };

                // Create the delete button
                Button deleteButton = new Button("Delete");
                favoritesGrid.Attach(deleteButton, 2, i, 1, 1);

                //if the delete button is clicked, remove the favorite from the list and update the UI
                deleteButton.Clicked += (sender, e) =>
                {
                    favoritesList.Remove(favorite);
                    updateFavoritesUI(favoritesList, builder);
                };
            }
            favoritesGrid.ShowAll();
        }


        //------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------


        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            File.AppendAllLines("history.txt", history.ConvertAll(x => x.GetURL));
            Gtk.Application.Quit();
        }
    }
}
