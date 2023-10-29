using System;
using System.IO;
using System.Threading.Tasks.Dataflow;

namespace Coursework
{
    // Class to read and write to a file
    public class readwrite
    {
        // Initializing the variable for the file path
        private string filePath;

        public readwrite(string filePath)
        {
            this.filePath = filePath;
        }

        // Function to read the file
        public string[] read()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string[] file = File.ReadAllLines(filePath);
                    return file;
                }
                else
                {
                    throw new FileNotFoundException();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading file: " + e.Message);
                return null;
            }
        }

        // Function to write to the file
        public void write(string writePath, string file)
        {
            try
            {
                File.WriteAllText(writePath, file);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing file: " + e.Message);
            }
        }
    }
}

