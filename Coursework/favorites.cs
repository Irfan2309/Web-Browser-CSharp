using System;
using System.Collections.Generic;
using Gtk;
using Coursework;
using Gdk;

namespace Coursework
{
    public class favorites
    {
        private string name_given;
        private URL UrlObj;

        public favorites(string name, string url)
        {
            this.name_given = name;
            UrlObj = new URL(url);
        }

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