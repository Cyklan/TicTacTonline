using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Server.General
{
    public static class UserInput
    {
        public static bool InputBoolean(string question, bool yesDefault)
        {
            string output = question;

            if (yesDefault) output += " [Y/n]: ";
            else output += " [y/N]: ";
            Log.Add(output, MessageType.Question);

            string userInput = Console.ReadLine();
            if (string.IsNullOrEmpty(userInput)) return yesDefault;

            userInput = userInput.ToLower();
            if (yesDefault)
            {
                if (userInput == "n") return false;
                return true;
            }

            if (userInput == "y") return true;
            return false;
        }

        public static string InputString(string question, string defaultAnswer)
        {
            while (true)
            {
                string output = $"{question} [{defaultAnswer}]: ";
                Log.Add(output, MessageType.Question);

                string userInput = Console.ReadLine();
                if (string.IsNullOrEmpty(userInput)) return defaultAnswer;
                return userInput;
            }
        }

        public static int InputInteger(string question, int defaultAnswer, bool positiveOnly, bool allowZero)
        {
            while (true)
            {
                string output = $"{question} [{defaultAnswer}]: ";
                Log.Add(output, MessageType.Question);

                string userInput = Console.ReadLine();

                if (string.IsNullOrEmpty(userInput)) return defaultAnswer;

                int ret;
                if (!int.TryParse(userInput, out ret))
                {
                    Log.Add("Please enter a valid integer.", MessageType.Error);
                    continue;
                }

                if (ret < 0 && positiveOnly)
                {
                    Log.Add("Please enter a value greater than zero.", MessageType.Error);
                    continue;
                }

                if (ret == 0 && !allowZero)
                {
                    Log.Add("Please enter a value that is not zero.", MessageType.Error);
                    continue;
                }

                return ret;
            }
        }

        public static string InputIp()
        {
            bool invalidIp = true;
            string ip;
            do
            {
                ip = InputString("Ip Address", "127.0.0.1");
                Regex regex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
                Match ipMatch = regex.Match(ip);

                if (ipMatch.Success) invalidIp = false;
                else Log.Add("Please enter a valid IP address", MessageType.Error);
            } while (invalidIp);

            return ip;
        }
    }
}
