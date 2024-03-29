using System;
using System.Net.Http;
using Gtk;

namespace Coursework
{
    public class URL
    {
        // Initializing the variables: page url, page title, page content, status code, file size
        private string pageUrl, pageTitle, pageContent;
        private int statusCode;
        private int bytes;

        // Setting these variables for the input url
        public URL(string url)
        {
            pageUrl = url;
            pageContent = GetContent(url);
            pageTitle = GetTitle(pageContent);
        }

        // Function to retrieve the page content and also set the status code and file size
        public string GetContent(string url)
        {
            HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                this.statusCode = (int)response.StatusCode;

                byte[] file_size = response.Content.ReadAsByteArrayAsync().Result;
                this.bytes = file_size.Length;

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
            catch (HttpRequestException e)
            {
                return "HTTP Request Error: " + e.Message;
            }
            catch (Exception e)
            {
                return "Error: " + e.Message;
            }

        }

        // Function to retrieve the page title
        public string GetTitle(string content)
        {
            try
            {
                // Getting the title of the page between the <title> tags
                string begin = "<title>";
                string end = "</title>";

                //trimming the title due to potential whitespace like in the case of https://www.hw.ac.uk
                int start = content.IndexOf(begin, StringComparison.OrdinalIgnoreCase);
                int stop = content.IndexOf(end, start, StringComparison.OrdinalIgnoreCase);

                if (start != -1 && stop != -1)
                {
                    string title = content.Substring(start + begin.Length, stop - start - begin.Length);
                    return statusCode + " " + title.Trim();
                }
                else
                {
                    return "Untitled Page";
                }
            }
            catch (Exception)
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

        //getter function for bytes
        public int GetBytes
        {
            get { return bytes; }
        }

        //getter function for page title
        public string GetPageTitle
        {
            get { return pageTitle; }
        }
    }
}
