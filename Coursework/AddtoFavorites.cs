using Gtk;
using System;
using System.Collections.Generic;

namespace Coursework
{
    public class AddToFavorites : Gtk.Window
    {
        private Entry urlInput;
        private Entry nameInput;
        public new event EventHandler Added;

        public AddToFavorites(List<favorites> favoritesList, URL UrlObj, int index, string addedit) : base(Gtk.WindowType.Toplevel)
        {
            SetDefaultSize(300, 250);

            Label label = new Label();

            if (addedit == "add")
            {
                label = new Label("Add Favorite");
            }
            else
            if (addedit == "edit")
            {
                label = new Label("Edit Favorite");
            }

            Label urlLabel = new Label("Enter URL:");
            Label nameLabel = new Label("Enter Name:");

            // Create input fields
            urlInput = new Entry();
            nameInput = new Entry();

            // Create buttons

            Button addButton = new Button("Add");
            Button cancelButton = new Button("Cancel");

            if (addedit == "add")
            {
                urlInput.Text = UrlObj.GetURL;
                nameInput.Text = UrlObj.GetPageTitle;
            }
            else
            if (addedit == "edit")
            {
                favorites favorite = favoritesList[index - 1];
                urlInput.Text = favorite.getUrl.GetURL;
                nameInput.Text = favorite.getUrl.GetPageTitle;
            }



            addButton.Clicked += (sender, e) =>
                {
                    if (addedit == "add")
                    {
                        favorites favorite = new favorites(nameInput.Text, urlInput.Text);
                        favoritesList.Add(favorite);
                    }
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

            cancelButton.Clicked += (sender, e) =>
                    {
                        this.Close();
                    };

            Box layout = new Box(Orientation.Vertical, 0);
            layout.PackStart(label, false, false, 0);

            layout.PackStart(urlLabel, false, false, 0);
            layout.PackStart(urlInput, false, false, 0);

            layout.PackStart(nameLabel, false, false, 0);
            layout.PackStart(nameInput, false, false, 0);

            Box buttonBox = new Box(Orientation.Horizontal, 0);
            buttonBox.PackStart(addButton, false, false, 0);
            buttonBox.PackStart(cancelButton, false, false, 0);

            layout.PackStart(buttonBox, false, false, 0);

            Add(layout);

            ShowAll();
        }
    }
}
