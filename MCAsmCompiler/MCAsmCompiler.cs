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

namespace MCAsmCompiler
{
    public static class MCAsmCompiler
    {
        public static byte[] Compile(string input, out string errorLog)
        {
            errorLog = "";
            Dictionary<string, int> labelTable = new Dictionary<string, int>();

            string output = Parser.Parse(input, labelTable, out errorLog);

            if (errorLog.Length != 0)
                return null;

            byte[] bytecode = Linker.Link(output, labelTable, out errorLog);

            if (errorLog.Length != 0)
                return null;

            return bytecode;
        }
    }
}

