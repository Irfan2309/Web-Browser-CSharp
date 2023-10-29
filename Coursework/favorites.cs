using System;
using System.Collections.Generic;
using Gtk;
using Coursework;
using Gdk;

namespace Coursework
{
    public class favorites
    {
        //favorites class will contain the name of the favorite and the URL object
        private string name_given;
        private URL UrlObj;

        public favorites(string name, string url)
        {
            this.name_given = name;
            UrlObj = new URL(url);
        }

        //getters and setters for the name and URL object
        public string getName
        {
            get { return name_given; }
            set { name_given = value; }
        }

        public URL getUrl
        {
            get { return UrlObj; }
            set { UrlObj = value; }
        }
    }
}