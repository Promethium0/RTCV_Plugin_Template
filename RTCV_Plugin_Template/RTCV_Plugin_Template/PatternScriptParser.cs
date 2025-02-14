//this is what im using to parse pattern script into c# code for the corruptor

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RTCV.CorruptCore;
using RTCV.NetCore;
using RTCV.Common;

using PatternEngine;





namespace PatternEngine.Parser
{
    public class PatternScriptParser
    {
        public static string ParsePatternScript(string script)
        {
            StringBuilder sb = new StringBuilder();
            string[] lines = script.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                string[] parts = SplitLine(line);
                if (parts.Length == 0)
                    continue;
                string command = parts[0];
                switch (command)
                {
                    case "SET": // this is so you can set bytes to a specific value like "SET 0x00 0x05" would set the byte at 0x00 to 0x05 
                        sb.AppendLine(ParseSet(parts));
                        break;
                    case "GET":
                        sb.AppendLine(ParseGet(parts));
                        break;
                    case "IF":
                        sb.AppendLine(ParseIf(parts));
                        break;
                    case "ELSE":
                        sb.AppendLine(ParseElse(parts));
                        break;
                    case "ENDIF":
                        sb.AppendLine(ParseEndIf(parts)); // ends an if statement
                        break;
                    case "WHILE":
                        sb.AppendLine(ParseWhile(parts));
                        break;
                    case "ENDWHILE":
                        sb.AppendLine(ParseEndWhile(parts)); // ends a while loop
                        break;
                    case "CALL": // this is for calling functions 
                        sb.AppendLine(ParseCall(parts));
                        break;
                    case "RETURN":
                        sb.AppendLine(ParseReturn(parts));
                        break;
                    case "BREAK":
                        sb.AppendLine(ParseBreak(parts));
                        break;
                    case "CONTINUE": // this is for loops to continue to the next iteration
                        sb.AppendLine(ParseContinue(parts));
                        break;
                    case "COMMENT":
                        sb.AppendLine(ParseComment(parts)); // comments are just // so it just gets ignored
                        break;
                    case "DEF":
                        sb.AppendLine(ParseDef(parts)); // this is for defining functions very important if you want to make a complex script
                        break;
                    case "ADD":
                        sb.AppendLine(ParseAdd(parts)); // adds (you know what this crap dose i dont need to comment this)
                        break;
                    case "FOR": // the main loopy guy lmao
                        sb.AppendLine(ParseFor(parts));
                        break;
                    case "BVALUE": // byte value basically how it works is like this "if bvalue =0x05 then break" and stuff along those lines
                        sb.AppendLine(ParseBValue(parts));
                        break;
                    case "EVERY": // this is basically a type of loop where you can set paramaters so for example "for every 2b add 1" so it would add 1 every two bytes to the byte it stops on
                        sb.AppendLine(ParseEvery(parts));
                        break;

                    case "SUB": //stinky math booooo i totally dont suck at math (trust)
                        sb.AppendLine(ParseSub(parts));
                        break;
                    case "MUL":
                        sb.AppendLine(ParseMul(parts));
                        break;
                    case "DIV":
                        sb.AppendLine(ParseDiv(parts));
                        break;
                    case "THEN": // this is for the if statement so you can do "if 0x00 == 0x05 then" and then do stuff
                        sb.AppendLine(ParseThen(parts));
                        break;
                    case "IN RANGE": // this for defining an adress range so you can do stuff like "for every 2b in range 0x00 0x05 add 1" so it would add 1 to every two bytes in the range of 0x00 to 0x05
                        sb.AppendLine(ParseInRange(parts));
                        break;
                    default:
                        sb.AppendLine(ParseDefault(parts));
                        break;
                }
            }
            return sb.ToString();
        }

        private static string[] SplitLine(string line) // promethiums sad attempt at getting this to work (it dosent)
        {
            List<string> parts = new List<string>();
            StringBuilder currentPart = new StringBuilder(); 
            bool insideSpecial = false;

            foreach (char c in line)
            {
                if (c == ' ' && !insideSpecial)
                {
                    if (currentPart.Length > 0)
                    {
                        parts.Add(currentPart.ToString());
                        currentPart.Clear();
                    }
                }
                else
                {
                    if (c == '(' || c == '{' || c == '[')
                    {
                        insideSpecial = true; 
                    }
                    else if (c == ')' || c == '}' || c == ']')
                    {
                        insideSpecial = false;
                    }
                    currentPart.Append(c);
                }
            }

            if (currentPart.Length > 0)
            {
                parts.Add(currentPart.ToString());
            }

            return parts.ToArray();
        }

        private static string ParseSet(string[] parts)
        {
            return $"S.SET<{parts[1]}>({parts[2]});";
        }

        private static string ParseGet(string[] parts)
        {
            return $"S.GET<{parts[1]}>();";
        }

        private static string ParseIf(string[] parts)
        {
            return $"if ({parts[1]}) {{";
        }

        private static string ParseElse(string[] parts)
        {
            return "} else {";
        }

        private static string ParseEndIf(string[] parts)
        {
            return "}";
        }

        private static string ParseWhile(string[] parts)
        {
            return $"while ({parts[1]}) {{";
        }

        private static string ParseEndWhile(string[] parts)
        {
            return "}";
        }

        private static string ParseCall(string[] parts)
        {
            return $"{parts[1]}();";
        }

        private static string ParseReturn(string[] parts)
        {
            return "return;";
        }

        private static string ParseInRange(string[] parts)
        {
            return $"for (int i = {parts[1]}; i < {parts[2]}; i += {parts[3]}) {{";
        }

        private static string ParseBreak(string[] parts)
        {
            return "break;";
        }

        private static string ParseContinue(string[] parts)
        {
            return "continue;";
        }

        private static string ParseComment(string[] parts)
        {
            return $"//{parts[1]}";
        }

        private static string ParseAdd(string[] parts)
        {
            return $"{parts[1]} = {parts[2]} + {parts[3]};";
        }

        private static string ParseSub(string[] parts)
        {
            return $"{parts[1]} = {parts[2]} - {parts[3]};";
        }

        private static string ParseMul(string[] parts)
        {
            return $"{parts[1]} = {parts[2]} * {parts[3]};";
        }

        private static string ParseDiv(string[] parts)
        {
            return $"{parts[1]} = {parts[2]} / {parts[3]};";
        }

        private static string ParseThen(string[] parts)
        {
            if (parts.Length > 1)
            {
                return $"then {string.Join(" ", parts.Skip(1))}";
            }
            return "then";
        }

        private static string ParseFor(string[] parts)
        {
            return $"for ({parts[1]}; {parts[2]}; {parts[3]}) {{";
        }

        private static string ParseBValue(string[] parts)
        {
            return $"if ({parts[1]} == {parts[2]}) {{";
        }

        private static string ParseEvery(string[] parts)
        {
            return $"for (int i = 0; i < {parts[1]}; i += {parts[2]}) {{ {parts[3]} }}";
        }

        private static string ParseDefault(string[] parts)
        {
            return string.Join(" ", parts);
        }

        private static string ParseDef(string[] parts)
        {
            return $"private void {parts[1]}() {{";
        }
    }
}
