using System;

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

            if (String.IsNullOrEmpty(userInput))
            {
                if (yesDefault) return true;
                return false;
            }

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
                if (String.IsNullOrEmpty(userInput)) return defaultAnswer;
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

                if (String.IsNullOrEmpty(userInput)) return defaultAnswer;

                int ret = 0;
                if (!Int32.TryParse(userInput, out ret))
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
    }
}
