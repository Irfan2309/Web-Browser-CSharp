using Gtk;
using System;

namespace Coursework
{
    public class SetNewHomepage : Gtk.Window
    {
        private Entry urlInput;

        public SetNewHomepage(URL homeurl) : base(Gtk.WindowType.Toplevel)
        {
            SetDefaultSize(300, 100);

            // Create label
            Label label = new Label("Edit Home Page");

            // Create input field
            urlInput = new Entry();
            urlInput.Text = homeurl.GetURL; // Assuming homeURL is a variable with the URL

            // Create buttons
            Button submitButton = new Button("Submit");
            Button cancelButton = new Button("Cancel");

            submitButton.Clicked += (sender, e) =>
            {
                homeurl.GetURL = urlInput.Text;
                this.Close();
            };

            //event handler for enter button
            urlInput.Activated += (sender, e) =>
            {
                homeurl.GetURL = urlInput.Text;
                this.Close();
            };

            cancelButton.Clicked += (sender, e) =>
            {
                this.Close(); // Close the window without saving
            };

            // Create a layout container (e.g., a VBox)
            Box layout = new Box(Orientation.Vertical, 0);
            layout.PackStart(label, false, false, 0);
            layout.PackStart(urlInput, false, false, 0);

            // Create a horizontal box for buttons
            Box buttonBox = new Box(Orientation.Horizontal, 0);
            buttonBox.PackStart(submitButton, false, false, 0);
            buttonBox.PackStart(cancelButton, false, false, 0);

            layout.PackStart(buttonBox, false, false, 0);

            // Add the layout to the window
            Add(layout);

            ShowAll();
        }
    }
}
