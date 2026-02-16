/*
* FILE : ConnectionProtocol.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
* handles each request made by the client to the server and the respective action
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace ClientUI.Protocols
{
    class ClientProtocol
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkDictionary"></param>
        /// <returns></returns>
       public static string BuildMessage(Dictionary<string, string> checkDictionary)
       {
            StringBuilder sb = new StringBuilder();
            
            foreach (KeyValuePair<string, string> checkKey in checkDictionary)
            {
                sb.AppendLine(checkKey.Key + ":"+ checkKey.Value);
            }
            sb.AppendLine("END");

            return sb.ToString();
       }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawText"></param>
        /// <returns></returns>
        public static Dictionary<string, string> RawMessage(string rawText)
        {
            Dictionary<string, string> checkFields = new Dictionary<string, string>();

            string[] lookLines = rawText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (string lookLine in lookLines)
            {
                if (lookLine.Trim() == "END")
                {
                    break;
                }
                string[] checkParts = lookLine.Split(':', 2);

                if (checkParts.Length == 2)
                {
                    string lookKey = checkParts[0].Trim();
                    string lookValue = checkParts[1].Trim();

                    checkFields[checkParts[0].Trim()] = checkParts[1].Trim();
                }
            }
            return checkFields;
        }
    }
}
