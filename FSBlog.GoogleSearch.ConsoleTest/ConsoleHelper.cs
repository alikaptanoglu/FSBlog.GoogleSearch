using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSBlog.GoogleSearch.GoogleClient;

namespace FSBlog.GoogleSearch.ConsoleTest
{
    internal static class ConsoleHelper
    {
        // CONSTS
        private const ConsoleColor PROMPT_CONSOLE_COLOR = ConsoleColor.Magenta ;

        // PUBLIC METHODS
        public static void PrintWelcome()
        {
            const string welcomeText = "Welcome to GoogleSearch Command Prompt";
            var welcomeMsg = String.Format("{0}{1}{2}{1}{0}{1}", new String('=', welcomeText.Length), Environment.NewLine, welcomeText);
            WriteLineToConsole(welcomeMsg, ConsoleColor.Cyan);
        }
        public static void PrintInitiatingSearchQuery(string query) {
            var msg = String.Format("Querying Google: {0}", query);
            WriteLineToConsole(msg, ConsoleColor.Green);
        }
        public static void PrintSearchResultHit(SearchResultHit hit) {
            var hitLink = String.Format("[{0}]{1}", hit.CleanUri, Environment.NewLine);

            WriteLineToConsole(hit.Text, ConsoleColor.Yellow);
            WriteLineToConsole(hitLink);
        }
        public static void PrintSearchResultHit(SearchResultHit hit, int hitCounter) {
            var hitCounterTitle = String.Format("{0}Result #{1}: ", Environment.NewLine, hitCounter);
            WriteToConsole(hitCounterTitle);

            PrintSearchResultHit(hit);
        }
        public static string PromptForSearchQuery() {
            WriteToConsole("Enter search query: ", PROMPT_CONSOLE_COLOR);
            return Console.ReadLine();
        }
        public static bool PromptForMore() {
            // print message
            var promptMessage = String.Format("{0}{0}Show more hits? Press any key to show more, <Esc> to quit.", Environment.NewLine);
            WriteLineToConsole(promptMessage, PROMPT_CONSOLE_COLOR);

            // read and check input
            var keyPressed = Console.ReadKey(true);
            return keyPressed.Key != ConsoleKey.Escape;
        }
        public static void PromptForExit()
        {
            var exitPrompt = String.Format("{0}Press any key to exit.", Environment.NewLine);
            WriteLineToConsole(exitPrompt, PROMPT_CONSOLE_COLOR);
            Console.ReadKey();
        }

        // PRIVATE METHODS
        private static void WriteLineToConsole(string message, ConsoleColor color = ConsoleColor.White) {
            WriteToConsole(message, color, true);
        }
        private static void WriteToConsole(string message, ConsoleColor color = ConsoleColor.White, bool writeLine = false) {
            Console.ForegroundColor = color;

            if (writeLine) {
                Console.WriteLine(message);
            } else {
                Console.Write(message);
            }

            Console.ResetColor();
        }
    }
}