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
    public static class Linker
    {
        private static readonly Regex regex = new Regex("<([A-Za-z][A-Za-z0-9]*)>", RegexOptions.Compiled);

        public static byte[] Link(string hexCode, Dictionary<string, int> labelTable, out string errorLog)
        {
            string output = "";

            bool res = resolveLabels(hexCode, labelTable, out output);

            if (!res)
            {
                errorLog = output;
                return null;
            }

            errorLog = "";
            return Utils.StringToByteArrayFastest(output);
        }

        private static bool resolveLabels(string mixedCode, Dictionary<string, int> labelTable, out string plainHex)
        {
            MatchCollection matches = regex.Matches(mixedCode);

            foreach (Match item in matches)
            {
                if (!item.Success)
                {
                    if (!mixedCode.Contains("<"))
                    {
                        plainHex = mixedCode;
                        return true;
                    }
                    else
                    {
                        plainHex = "Error in hexCode";
                        return false;
                    }
                }

                string debug = ((byte)labelTable[item.Groups[1].Value]).ToString("x2");
                mixedCode = mixedCode.Replace(item.Groups[0].Value, debug);
            }

            plainHex = mixedCode;
            return true;
        }
    }
}

