using Gtk;
using System;
using System.IO;

namespace Coursework
{
    // Class to set a new homepage for the browser with a dialog box
    public class SetNewHomepage : Gtk.Window
    {
        private Entry urlInput;
        public SetNewHomepage(URL homeurl) : base(Gtk.WindowType.Toplevel)
        {
            //set the size of the window
            SetDefaultSize(300, 150);

            // Create label
            Label label = new Label("Edit Home Page");

            // Create input field
            urlInput = new Entry();
            urlInput.Text = homeurl.GetURL;

            // Create buttons
            Button submitButton = new Button("Submit");
            Button cancelButton = new Button("Cancel");

            //event handler for clicking the submit button
            submitButton.Clicked += (sender, e) =>
            {
                //write the new url to the homepage.txt file
                readwrite rw = new readwrite("homepage.txt");
                rw.write("homepage.txt", urlInput.Text);

                //set the new url to the homeurl variable
                string newURL = File.ReadAllText("homepage.txt");

                //set the new url to the homeurl variable
                homeurl.GetURL = newURL;

                this.Close(); ;
            };

            //event handler for enter button
            urlInput.Activated += (sender, e) =>
            {
                //write the new url to the homepage.txt file
                readwrite rw = new readwrite("homepage.txt");
                rw.write("homepage.txt", urlInput.Text);

                //set the new url to the homeurl variable
                string newURL = File.ReadAllText("homepage.txt");

                //set the new url to the homeurl variable
                homeurl.GetURL = newURL;

                this.Close(); ;
            };

            //event handler for clicking the cancel button
            cancelButton.Clicked += (sender, e) =>
            {
                this.Close();
            };

            //creating the UI for the dialog box
            Box layout = new Box(Orientation.Vertical, 0);
            layout.PackStart(label, false, false, 0);
            layout.PackStart(urlInput, false, false, 0);

            // submit and cancel buttons
            Box buttonBox = new Box(Orientation.Horizontal, 0);
            buttonBox.PackStart(submitButton, false, false, 0);
            buttonBox.PackStart(cancelButton, false, false, 0);

            layout.PackStart(buttonBox, false, false, 0);

            Add(layout);

            ShowAll();
        }
    }
}
