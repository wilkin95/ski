using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiRunRater
{
    public class Controller
    {
        #region FIELDS

        bool active = true;

        #endregion

        #region PROPERTIES


        #endregion

        #region CONSTRUCTORS

        public Controller()
        {
            ApplicationControl();
        }

        #endregion

        #region METHODS

        private void ApplicationControl()
        {
            SkiRunRepository skiRunRepository = new SkiRunRepository();

            ConsoleView.DisplayWelcomeScreen();

            using (skiRunRepository)
            {
                List<SkiRun> skiRuns = skiRunRepository.GetSkiAllRuns();

                while (active)
                {
                    AppEnum.ManagerAction userActionChoice;
                    int vertical;
                    int skiRunID;
                    SkiRun skiRun;
                    string skiRunString;
                    List<SkiRun> ski2 = new List<SkiRun>();

                    userActionChoice = ConsoleView.GetUserActionChoice();

                    switch (userActionChoice)
                    {
                        case AppEnum.ManagerAction.None:
                            break;
                        case AppEnum.ManagerAction.ListAllSkiRuns:
                            ConsoleView.DisplayAllSkiRuns(skiRuns);
                            ConsoleView.DisplayContinuePrompt();
                            break;
                        case AppEnum.ManagerAction.DisplaySkiRunDetail:
                            ConsoleView.DisplayAllSkiRuns(skiRuns);
                            ski2 = skiRunRepository.GetSkiRunByID(ConsoleView.DisplayGetSkiRunIDChoice(skiRuns));
                            ConsoleView.DisplaySkiRunDetails(ski2);
                            ConsoleView.DisplayContinuePrompt();

                            break;
                        case AppEnum.ManagerAction.DeleteSkiRun:                            
                            skiRunRepository.DeleteSkiRun(ConsoleView.DisplayGetSkiRunIDChoice(skiRuns));
                            ConsoleView.DisplayReset();
                            ConsoleView.DisplayMessage("Ski Run has been deleted.");
                            ConsoleView.DisplayContinuePrompt();
                            break;
                        case AppEnum.ManagerAction.AddSkiRun:
                            
                            skiRun = new SkiRun();
                            skiRun.ID = ConsoleView.DisplayGetSkiRunID();
                            skiRun.Name = ConsoleView.DisplayGetSkiRunName();
                            skiRun.Vertical = ConsoleView.DisplayGetSkiRunVertical();
                            skiRunRepository.InsertSkiRun(skiRun);
                            ConsoleView.DisplayAllSkiRuns(skiRuns);
                            ConsoleView.DisplayNewSkiRunMessage();
                            
                            break;
                        case AppEnum.ManagerAction.UpdateSkiRun:

                            skiRun = new SkiRun();
                            skiRun.ID = ConsoleView.DisplayGetSkiRunIDChoice(skiRuns);
                            skiRunRepository.UpdateSkiRun(skiRun);

                            Console.WriteLine();

                            break;
                        case AppEnum.ManagerAction.QuerySkiRunsByVertical:
                            int minimumVertical = skiRunRepository.GetMinimumVertical();
                            int maximumVertical = skiRunRepository.GetMaximumVertical();
                            
                            //Sorts all the available entries and pushes out those that match into a new list.
                            List<SkiRun> matchingSkiRuns = skiRunRepository.QueryByVertical(minimumVertical, maximumVertical);
                            ConsoleView.DisplayReset();

                            //Displays the new list.
                            Console.WriteLine("Ski Runs with a vertical between " + minimumVertical + " and " + maximumVertical + ".");
                            skiRunRepository.DisplayQueriedVertical(matchingSkiRuns);
                            ConsoleView.DisplayContinuePrompt();
                            break;
                        case AppEnum.ManagerAction.Quit:
                            active = false;
                            break;
                        default:
                            break;
                    }
                }
            }

            ConsoleView.DisplayExitPrompt();
        }

        #endregion

    }
}
