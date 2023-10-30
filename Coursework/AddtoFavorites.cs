using Gtk;
using System;
using System.Collections.Generic;

namespace Coursework
{
    // Class to add/edit a favorite to the favorites list with a dialog box
    public class AddToFavorites : Gtk.Window
    {
        // Initializing the variables for the input url and name
        private Entry urlInput;
        private Entry nameInput;
        public new event EventHandler Added;

        //the constructor will take the favorites list, the url object of the current webpage (if adding, else null)
        //the index of the favorite to be edited and whether it is an add or edit
        public AddToFavorites(List<favorites> favoritesList, URL UrlObj, int index, string addedit) : base(Gtk.WindowType.Toplevel)
        {
            SetDefaultSize(300, 250);

            Label label = new Label();

            // Create label based on whether it is an add or edit
            if (addedit == "add")
            {
                label = new Label("Add Favorite");
            }
            else
            if (addedit == "edit")
            {
                label = new Label("Edit Favorite");
            }

            // Create labels
            Label urlLabel = new Label("Enter URL:");
            Label nameLabel = new Label("Enter Name:");

            // Create input fields
            urlInput = new Entry();
            nameInput = new Entry();

            // Create buttons
            Button addButton = new Button("Add");
            Button cancelButton = new Button("Cancel");

            //if adding, set the input fields to the current url object
            if (addedit == "add")
            {
                urlInput.Text = UrlObj.GetURL;
                nameInput.Text = UrlObj.GetPageTitle;
            }
            //if editing, set the input fields to the current favorite that the user wants to edit
            else
            if (addedit == "edit")
            {
                favorites favorite = favoritesList[index - 1];
                urlInput.Text = favorite.getUrl.GetURL;
                nameInput.Text = favorite.getUrl.GetPageTitle;
            }

            //event handler for clicking the add button
            addButton.Clicked += (sender, e) =>
                {
                    //if adding, create a new favorite object and add it to the favorites list
                    if (addedit == "add")
                    {
                        favorites favorite = new favorites(nameInput.Text, urlInput.Text);
                        favoritesList.Add(favorite);
                    }
                    //if editing, set the url and name of the current favorite to the input fields
                    else
                    if (addedit == "edit")
                    {
                        favorites favorite = favoritesList[index - 1];
                        favorite.getUrl.GetURL = urlInput.Text;
                        favorite.getName = nameInput.Text;
                    }
                    Added?.Invoke(this, EventArgs.Empty);
                    this.Close();
                };

            //event handler for cancel button
            cancelButton.Clicked += (sender, e) =>
                    {
                        this.Close();
                    };

            //creating the UI for the dialog box
            Box layout = new Box(Orientation.Vertical, 0);
            layout.PackStart(label, false, false, 0);

            //url
            layout.PackStart(urlLabel, false, false, 0);
            layout.PackStart(urlInput, false, false, 0);

            //favorite name
            layout.PackStart(nameLabel, false, false, 0);
            layout.PackStart(nameInput, false, false, 0);

            // add/edit and cancel buttons
            Box buttonBox = new Box(Orientation.Horizontal, 0);
            buttonBox.PackStart(addButton, false, false, 0);
            buttonBox.PackStart(cancelButton, false, false, 0);

            layout.PackStart(buttonBox, false, false, 0);

            Add(layout);

            ShowAll();
        }
    }
}
