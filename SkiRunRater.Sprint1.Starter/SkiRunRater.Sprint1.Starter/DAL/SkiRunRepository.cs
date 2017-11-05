using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SkiRunRater
{
    /// <summary>
    /// method to write all ski run information to the date file
    /// </summary>
    public class SkiRunRepository : IDisposable
    {
        private List<SkiRun> _skiRuns;

        public SkiRunRepository()
        {
            _skiRuns = ReadSkiRunsData(DataSettings.dataFilePath);
        }

        /// <summary>
        /// method to read all ski run information from the data file and return it as a list of SkiRun objects
        /// </summary>
        /// <param name="dataFilePath">path the data file</param>
        /// <returns>list of SkiRun objects</returns>
        public static List<SkiRun> ReadSkiRunsData(string dataFilePath)
        {
            const char delineator = ',';

            // create lists to hold the ski run strings and objects
            List<string> skiRunStringList = new List<string>();
            List<SkiRun> skiRunClassList = new List<SkiRun>();

            // initialize a StreamReader object for reading
            StreamReader sReader = new StreamReader(DataSettings.dataFilePath);

            using (sReader)
            {
                // keep reading lines of text until the end of the file is reached
                while (!sReader.EndOfStream)
                {
                    skiRunStringList.Add(sReader.ReadLine());
                }
            }
            
            foreach (string skiRun in skiRunStringList)
            {
                // use the Split method and the delineator on the array to separate each property into an array of properties
                string[] properties = skiRun.Split(delineator);

                // populate the ski run list with SkiRun objects
                skiRunClassList.Add(new SkiRun() { ID = Convert.ToInt32(properties[0]), Name = properties[1], Vertical = Convert.ToInt32(properties[2]) });
            }

            return skiRunClassList;
        }

        /// <summary>
        /// method to write all of the list of ski runs to the text file
        /// </summary>
        public void WriteSkiRunsData()
        {
            string skiRunString;

            // wrap the FileStream object in a StreamWriter object to simplify writing strings
            StreamWriter sWriter = new StreamWriter(DataSettings.dataFilePath, false);

            using (sWriter)
            {
                foreach (SkiRun skiRun in _skiRuns)
                {
                    skiRunString = skiRun.ID + "," + skiRun.Name + "," + skiRun.Vertical;
                    sWriter.WriteLine(skiRunString);
                }
            }
        }

        /// <summary>
        /// method to add a new ski run
        /// </summary>
        /// <param name="skiRun"></param>
        public void InsertSkiRun(SkiRun skiRun)
        {
            _skiRuns.Add(skiRun); 
            WriteSkiRunsData();
        }

        /// <summary>
        /// method to delete a ski run by ski run ID
        /// </summary>
        /// <param name="ID"></param>
        public void DeleteSkiRun(int ID)
        {
            for (int index = 0; index < _skiRuns.Count(); index++)
            {
                if (_skiRuns[index].ID == ID)
                {
                    _skiRuns.RemoveAt(index);
                }
            }

            WriteSkiRunsData();
        }

        /// <summary>
        /// method to update an existing ski run
        /// </summary>
        /// <param name="skiRun">ski run object</param>
        public void UpdateSkiRun(SkiRun skiRun)
        {
            string tabLeft = ConsoleUtil.FillStringWithSpaces(ViewSettings.DISPLAY_HORIZONTAL_MARGIN);

            //Finds the ID of the skirun you wish to change
            for (int index = 0; index < _skiRuns.Count(); index++)
            {
                if (_skiRuns[index].ID == skiRun.ID)
                {
                    skiRun.Name = _skiRuns[index].Name;
                    skiRun.Vertical = _skiRuns[index].Vertical;

                    //The remove is here because it's going to add the skirun back at the 
                    //end of the update.
                    _skiRuns.RemoveAt(index);
                }
            }

            //Gets a new name. If the user does not input anything, it keeps the old one.
            Console.WriteLine(tabLeft + "Changing Name. If you wish to keep the same name, just hit enter.");
            Console.Write(tabLeft);
            string nameChange = Console.ReadLine();
            if (nameChange != "")
            {
                skiRun.Name = nameChange;
            }

            //Gets a new vertical. If the user does not input anything, it keeps the old one.
            Console.WriteLine(tabLeft + "Changeing Vertical. If you wish to keep the same vertical, just hit enter.");
            Console.Write(tabLeft);
            string verticalChangeCheck = Console.ReadLine();
            int verticalChange;
            if (verticalChangeCheck != "")
            {
                while (!int.TryParse(verticalChangeCheck, out verticalChange))
                {
                    ConsoleView.DisplayPromptMessage("Sorry, but the ID needs to be a number. Please try again.");
                };
                skiRun.Vertical = verticalChange;
            }

            //Adds the skirun to the list and writes the list.
            _skiRuns.Add(skiRun);
            WriteSkiRunsData();

        }

        /// <summary>
        /// method to return a ski run object given the ID
        /// </summary>
        /// <param name="ID">int ID</param>
        /// <returns>ski run object</returns>
        public List<SkiRun> GetSkiRunByID(int ID)
        {
            SkiRun skiRun;
            List<SkiRun> ski2 = new List<SkiRun>();
            bool valid = true;
            foreach (SkiRun skiRuns in _skiRuns )
            {
                if(skiRuns.ID == ID)
                {
                    ski2.Add(skiRuns);
                   
                }
                else
                {
                    valid = false;
                }
            }

            return ski2;
        }

        /// <summary>
        /// method to return a list of ski run objects
        /// </summary>
        /// <returns>list of ski run objects</returns>
        public List<SkiRun> GetSkiAllRuns()
        {
            return _skiRuns;
        }

        /// <summary>
        /// method to query the data by the vertical of each ski run in feet
        /// </summary>
        /// <param name="minimumVertical">int minimum vertical</param>
        /// <param name="maximumVertical">int maximum vertical</param>
        /// <returns></returns>
        public List<SkiRun> QueryByVertical(int minimumVertical, int maximumVertical)
        {
            List<SkiRun> matchingSkiRuns = new List<SkiRun>();
            for (int index = 0; index < _skiRuns.Count(); index++)
            {
                if (minimumVertical <= _skiRuns[index].Vertical && _skiRuns[index].Vertical <= maximumVertical)
                {
                    matchingSkiRuns.Add(_skiRuns[index]);
                }
            }
            return matchingSkiRuns;
        }

        public int GetMinimumVertical()
        {
            int minimumVertical = 0;

            //gets the minimum vertical from the user and checks to see if 
            //it is a vaild responce. The default is zero.

            Console.WriteLine(Environment.NewLine + "Please enter a minimum vertical you wish to query by:");
            while (!int.TryParse(Console.ReadLine(), out minimumVertical))
            {
                ConsoleView.DisplayPromptMessage("Sorry, but the Vertical needs to be a valid number. Please try again.");
                Console.WriteLine();
            };

            return minimumVertical;
        }

        public int GetMaximumVertical()
        {
            int maximumVertical = 0;

            //gets the maximum vertical from the user and checks to see if 
            //it is a vaild responce. The default is zero.
            Console.WriteLine("Please enter a maximum vertical you wish to query by:");
            while (!int.TryParse(Console.ReadLine(), out maximumVertical))
            {
                ConsoleView.DisplayPromptMessage("Sorry, but the Vertical needs to be a valid number. Please try again.");
                Console.WriteLine();
            };

            return maximumVertical;
        }

        public void DisplayQueriedVertical(List<SkiRun> matchingSkiRuns)
        {
            if (matchingSkiRuns.Count > 0)
            {
                foreach (SkiRun queriedSkiRun in matchingSkiRuns)
                {
                    StringBuilder skiRunInfo = new StringBuilder();

                    skiRunInfo.Append(queriedSkiRun.ID.ToString().PadRight(8));
                    skiRunInfo.Append(queriedSkiRun.Name.PadRight(25));
                    skiRunInfo.Append(queriedSkiRun.Vertical.ToString().PadRight(5));
                    ConsoleView.DisplayMessage(skiRunInfo.ToString());

                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("There were no ski runs that matched your query.");
            }
        }

        /// <summary>
        /// method to handle the IDisposable interface contract
        /// </summary>
        public void Dispose()
        {
            _skiRuns = null;
        }
    }
}
