using System;
using System.Net.Http;
using Gtk;

namespace WebBrowser
{
    public class URL
    {
        // Initializing the variables: page url, page title, page content, status code
        private string pageUrl, pageTitle, pageContent;
        private int statusCode;

        // Setting these variables for the input url
        public URL(string url)
        {
            pageUrl = url;
            pageContent = GetContent(url);
            pageTitle = GetTitle(pageContent);
        }

        // Function to retrieve the page content and also set the status code
        public string GetContent(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(url).Result;
            this.statusCode = (int)response.StatusCode;

            if (statusCode == 200)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else if (statusCode == 400)
            {
                return "400 Error! Bad Request";
            }
            else if (statusCode == 403)
            {
                return "403 Error! Forbidden";
            }
            else if (statusCode == 404)
            {
                return "404 Error! Not Found";
            }
            else
            {
                return "HTTP Response Status Error Code " + statusCode;
            }
        }

        // Function to retrieve the page title
        public string GetTitle(string content)
        {
            string begin = "<title>";
            string end = "</title>";

            int start = content.IndexOf(begin, StringComparison.OrdinalIgnoreCase);
            int stop = content.IndexOf(end, start, StringComparison.OrdinalIgnoreCase);

            if (start != -1 && stop != -1)
            {
                string title = content.Substring(start + begin.Length, stop - start - begin.Length);
                return title.Trim();
            }
            else
            {
                return "Untitled Page";
            }
        }

        //getter setter for page url
        public string GetURL
        {
            get { return pageUrl; }
            set { pageUrl = value; }
        }

        //getter for page content
        public string GetPageContent
        {
            get { return pageContent; }
        }

        //getter function for status code
        public int GetStatusCode
        {
            get { return statusCode; }
        }

        //getter function for page title
        public string GetPageTitle
        {
            get { return pageTitle; }
        }
    }
}
