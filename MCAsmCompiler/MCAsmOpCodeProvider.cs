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

namespace MCAsmCompiler
{
    public class MCAsmOpCodeProvider : IOpCodeProvider
    {
        public string ErrorLog { get { return errorLogger.ToString(); } }

        private static readonly Dictionary<string, int> registerLUT;
        private readonly StringBuilder errorLogger = new StringBuilder();

        static MCAsmOpCodeProvider()
        {
            registerLUT = new Dictionary<string, int>();
            registerLUT.Add("a", 0);
            registerLUT.Add("b", 1);
            registerLUT.Add("c", 2);
            registerLUT.Add("d", 3);
            registerLUT.Add("e", 4);
            registerLUT.Add("sp", 5);
            registerLUT.Add("bp", 6);
        }

        #region IOpCodeProvider implementation

        public string GetCode(string opCode, string arg1, string arg2)
        {
            switch (opCode)
            {
                case "nop":
                    return "00";

                // ALU
                case "add":
                    return "01" + doubleRegisters(arg1, arg2);
                case "sub":
                    return "02" + doubleRegisters(arg1, arg2);
                case "and":
                    return "03" + doubleRegisters(arg1, arg2);
                case "or":
                    return "04" + doubleRegisters(arg1, arg2);
                case "xor":
                    return "05" + doubleRegisters(arg1, arg2);
                case "not":
                    return "06" + singleRegister(arg1);
                case "neg":
                    return "07" + singleRegister(arg1);
                case "mul":
                    return "08" + doubleRegisters(arg1, arg2);
                case "muls":
                    return "08" + doubleRegistersFlag(arg1, arg2);
                case "div":
                    return "09" + doubleRegisters(arg1, arg2);
                case "lsh":
                    return "0a" + doubleRegisters(arg1, arg2);
                case "rsh":
                    return "0b" + doubleRegisters(arg1, arg2);
                case "rsha":
                    return "0b" + doubleRegistersFlag(arg1, arg2);
                case "inc":
                    return "0c" + singleRegister(arg1);
                case "dec":
                    return "0d" + singleRegister(arg1);
                case "adc":
                    return "0e" + doubleRegisters(arg1, arg2);
                case "sbb":
                    return "0f" + doubleRegisters(arg1, arg2);
                case "cmp":
                    return "10" + doubleRegisters(arg1, arg2);
                
                // Memory
                case "mov":
                    return "14" + doubleRegisters(arg1, arg2);
                case "ldc":
                    return "15" + registerAddressOrConst(arg1, arg2);
                case "ldrs":
                    return "16" + registerAddressOrConst(arg1, arg2);
                case "ldr":
                    return "17" + registerIndirectAddress(arg1, arg2);
                case "strs":
                    return "18" + registerAddressOrConst(arg1, arg2);
                case "str":
                    return "19" + registerIndirectAddress(arg1, arg2);

                // Stack
                case "push":
                    return "1a" + singleRegister(arg1);
                case "pusha":
                    return "1b";
                case "pushc":
                    return "1c" + constant(arg1);
                case "pop":
                    return "1d" + singleRegister(arg1);
                case "popa":
                    return "1e";

                // Jumps
                case "jmp":
                    return "20" + label(arg1);
                case "je":
                case "jz":
                    return "21" + label(arg1);
                case "ja":
                    return "22" + label(arg1);
                case "jae":
                case "jnc":
                    return "23" + label(arg1);
                case "jb":
                case "jc":
                    return "24" + label(arg1);
                case "jbe":
                    return "25" + label(arg1);
                case "jg":
                    return "26" + label(arg1);
                case "jge":
                    return "27" + label(arg1);
                case "jl":
                    return "28" + label(arg1);
                case "jle":
                    return "29" + label(arg1);
                case "jne":
                case "jnz":
                    return "2a" + label(arg1);
                case "jns":
                    return "2b" + label(arg1);
                case "jo":
                    return "2c" + label(arg1);
                case "js":
                    return "2d" + label(arg1);
                case "jnp":
                case "jpo":
                    return "2e" + label(arg1);
                case "jp":
                case "jpe":
                    return "2f" + label(arg1);

                // Functions
                case "call":
                    return "30" + label(arg1);
                case "ret":
                    return "31";

                // Other
                case "hlt":
                    return "ff";

                default:
                    errorLogger.AppendLine(
                        "Unknown OpCode: "
                        + opCode);
                    return "";
            }
        }

        #endregion

        string singleRegister(string arg1)
        {
            int reg;
            if (!registerLUT.TryGetValue(arg1, out reg))
            {
                errorLogger.AppendLine("Expected register but got: " + arg1);
                return "";
            }
            return reg.ToString("x2");
        }

        string doubleRegisters(string arg1, string arg2)
        {
            int reg1;
            if (!registerLUT.TryGetValue(arg1, out reg1))
            {
                errorLogger.AppendLine("Expected register but got: " + arg1);
                return "";
            }
            int reg2;
            if (!registerLUT.TryGetValue(arg2, out reg2))
            {
                errorLogger.AppendLine("Expected register but got: " + arg2);
                return "";
            }

            return (reg1 | reg2 << 3).ToString("x2");
        }

        string doubleRegistersFlag(string arg1, string arg2)
        {
            int reg1;
            if (!registerLUT.TryGetValue(arg1, out reg1))
            {
                errorLogger.AppendLine("Expected register but got: " + arg1);
                return "";
            }
            int reg2;
            if (!registerLUT.TryGetValue(arg2, out reg2))
            {
                errorLogger.AppendLine("Expected register but got: " + arg2);
                return "";
            }

            return "1" + (reg1 | reg2 << 3).ToString("X");
        }

        string constant(string arg1)
        {
            int con;
            if (!int.TryParse(arg1, out con))
            {
                errorLogger.AppendLine("Expected number but got: " + arg1);
                return "";
            }

            return con.ToString("x2");
        }

        string label(string arg1)
        {
            if (String.IsNullOrWhiteSpace(arg1))
            {
                errorLogger.AppendLine("Label may not be empty.");
                return "";
            }

            return "<" + arg1 + ">";
        }

        string registerAddressOrConst(string arg1, string arg2)
        {
            int reg;
            if (!registerLUT.TryGetValue(arg1, out reg))
            {
                errorLogger.AppendLine("Expected register but got: " + arg1);
                return "";
            }
            int num;
            if (!int.TryParse(arg2, out num))
            {
                errorLogger.AppendLine("Expected number but got: " + arg2);
                return "";
            }

            return (reg | num << 3).ToString("x2");
        }

        string registerIndirectAddress(string arg1, string arg2)
        {
            return doubleRegisters(arg1, arg2.Replace("[", "").Replace("]", ""));
        }
    }
}

