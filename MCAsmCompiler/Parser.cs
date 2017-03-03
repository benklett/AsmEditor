// //  (
// //  )\ )                                (         (          (
// // (()/(      (  (      (   (           )\ )      )\         )\ )   (
// //  /(_)) (   )\))(    ))\  )(      (  (()/(    (((_)   (   (()/(  ))\
// // (_))   )\ ((_)()\  /((_)(()\     )\  /(_))   )\___   )\   ((_))/((_)
// // | _ \ ((_)_(()((_)(_))   ((_)   ((_)(_) _|  ((/ __| ((_)  _| |(_))
// // |  _// _ \\ V  V // -_) | '_|  / _ \ |  _|   | (__ / _ \/ _` |/ -_)
// // |_|  \___/ \_/\_/ \___| |_|    \___/ |_|      \___|\___/\__,_|\___|
// //
// //
// // Copyright (c) 2015 Power of Code
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MCAsmCompiler
{
    public static class Parser
    {
        private static readonly Regex opCodeRegex =
            new Regex("\\s*([a-z]+)(?:\\s+([a-z0-9]+)(?:\\s*,\\s*([a-z0-9]+))?)?", RegexOptions.Compiled);
        private static readonly Regex labelRegex =
            new Regex("([A-Za-z][A-Za-z0-9]*):", RegexOptions.Compiled);

        public static string Parse(string document, Dictionary<string, int> labelTable, out string errorLog)
        {
            var opProvider = new MCAsmOpCodeProvider();

            document.Replace('\r', ' ');
            string[] lines = document.Split(new[] { '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb = new StringBuilder();
            bool insideMultilineComment = false;

            int programCounter = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (String.IsNullOrWhiteSpace(line))
                    continue;
               
                line = line.TrimStart();

                if (line.StartsWith(";"))
                    continue;
                if (line.StartsWith("/*"))
                {
                    insideMultilineComment = true;
                    continue;
                }
                if (line.StartsWith("*/"))
                {
                    insideMultilineComment = false;
                    continue;
                }
                if (insideMultilineComment)
                    continue;

                //Labels
                var labelMatch = labelRegex.Match(line);
                if (labelMatch.Success)
                {
                    labelTable.Add(labelMatch.Groups[1].Value, programCounter);
                    continue;
                }
                    
                var opCodeMatch = opCodeRegex.Match(line.ToLowerInvariant());

                if (!opCodeMatch.Success)
                    continue;

                var res = opProvider.GetCode(opCodeMatch.Groups[1].Value, opCodeMatch.Groups[2].Value, opCodeMatch.Groups[3].Value);
                if (res.Length == 2)
                    programCounter += 2;
                if (res.Length >= 4)
                    programCounter += 4;

                sb.Append(res);
            }

            errorLog = opProvider.ErrorLog;

            return sb.ToString();
        }


    }
}

