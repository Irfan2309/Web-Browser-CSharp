using System;
using System.Security.AccessControl;
using Gtk;
using WebBrowser;
using UI = Gtk.Builder.ObjectAttribute;

namespace Coursework
{
    class MainWindow : Window
    {

        private Entry url_input;
        private TextView display;
        private URL urlObj;
        private string homepage = "https://www.hw.ac.uk";

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);

            //getting the url entry box and the text view from the glade file
            urlObj = new URL(homepage);

            url_input = (Entry)builder.GetObject("UrlBar");
            display = (TextView)builder.GetObject("DisplayScreen");

            pageUpdate(homepage);

            //getting the go button from the glade file and setting the event handler for the go button
            Button button = (Button)builder.GetObject("GoButton");
            button.Clicked += button_Clicked;

            // Add event listener for Enter key on url Entry
            url_input.Activated += (sender, e) => button_Clicked(sender, e);

            //getting the home button from the glade file and setting the event handler for the home button
            Button home = (Button)builder.GetObject("HomeButton");
            home.Clicked += home_Clicked;

            // Add event listener for double-clicking the home button
            home.ButtonPressEvent += home_DoubleClicked;
        }

        private void home_Clicked(object sender, EventArgs e)
        {
            pageUpdate(homepage);
        }
        private void pageUpdate(string urlText)
        {
            //displaying the result in the text view
            urlObj = new URL(urlText);

            string urlResult = urlObj.GetPageContent;
            display.Buffer.Text = urlResult;

            //setting the title
            string pageTitle = urlObj.GetPageTitle;

            this.Title = pageTitle;
        }
        private void button_Clicked(object sender, EventArgs e)
        {

            //getting the url from the entry box
            string urlText = url_input.Text;
            pageUpdate(urlText);
        }

        // Add this new event handler for double-clicking the home button
        private void home_DoubleClicked(object sender, ButtonPressEventArgs e)
        {
            //TODO: Add code to open a new window here
        }



        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }
    }
}
