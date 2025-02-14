//this is what im using to parse pattern script into c# code for the corruptor

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RTCV.CorruptCore;
using RTCV.NetCore;
using RTCV.Common;

using PatternEngine;

//example code
//DEF SomeFunction// this is a function definition
//{ 
//      // code here
//
//
//}
//SET 0x00 0x05 // this sets the byte at 0x00 to 0x05
//GET 0x00 // this gets the byte at 0x00
//IF 0x00 == 0x05 // if statement if the byte at 0x00 is equal to 0x05
//SET 0x01 0x10 // it sets the byte at 0x01 to 0x10
//ELSE// if its not equal to 0x05
//SET 0x01 0x20 // it sets the byte at 0x01 to 0x20
//ENDIF // ends the if statement
//WHILE 0x00 < 0x10 // while the byte at 0x00 is less than 0x10
//ADD 0x00 0x00 0x01 // it adds 1 to the byte at 0x00
//ENDWHILE // ends the while loop
//FOR i = 0; i < 10; i++ // for loop
//SUB 0x00 0x00 0x01 // subtracts 1 from the byte at 0x00
//BVALUE 0x00 0x05 // if the byte at 0x00 is equal to 0x05
//BREAK // it breaks the loop
//EVERY 2 1 // for every 2 bytes it adds 1
//ADD 0x00 0x00 0x01 // it adds 1 to the byte at 0x00
//CALL SomeFunction // calls a function
//RETURN // returns from a function
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
                string[] parts = line.Split(' ');
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
                        sb.AppendLine(ParseEndIf(parts));
                        break;
                    case "WHILE":
                        sb.AppendLine(ParseWhile(parts));
                        break;
                    case "ENDWHILE":
                        sb.AppendLine(ParseEndWhile(parts));
                        break;
                    case "CALL": // this is for calling functions this wont be used that much but its here just in case
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
                        sb.AppendLine(ParseComment(parts));
                        break;
                    case "DEF":
                        sb.AppendLine(ParseDef(parts)); // this is for defining functions very important if you want to make a complex script
                        break;
                    case "ADD":
                        sb.AppendLine(ParseAdd(parts));
                        break;
                    case "FOR": // the main loopy guy lmao
                        sb.AppendLine(ParseFor(parts));
                        break;
                    case "BVALUE": // byte value basically how it works is like this "if bvalue =0x05 then break" and stuff along those lines
                        sb.AppendLine(ParseBValue(parts));
                        break;
                    case "EVERY": // this is basical a type of loop where you can set paramaters so for example "for every 2b add 1" so it would add 1 every two bytes to the byte it stops on
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
                    default:
                        sb.AppendLine(ParseDefault(parts));
                        break;
                }
            }
            return sb.ToString();
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
            return $"if ({parts[1]})";
        }

        private static string ParseElse(string[] parts)
        {
            return "else";
        }

        private static string ParseEndIf(string[] parts)
        {
            return "endif";
        }

        private static string ParseWhile(string[] parts)
        {
            return $"while ({parts[1]})";
        }

        private static string ParseEndWhile(string[] parts)
        {
            return "endwhile";
        }

        private static string ParseCall(string[] parts)
        {
            return $"{parts[1]}();";
        }

        private static string ParseReturn(string[] parts)
        {
            return "return;";
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

        private static string ParseFor(string[] parts)
        {
           
            return $"for ({parts[1]}; {parts[2]}; {parts[3]})";
        }

        private static string ParseBValue(string[] parts)
        {
           
            return $"if ({parts[1]} == {parts[2]})";
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
            return $"private void {parts[1]}()"; // i dont know if i should be using private or public so i just used private
        }
    }
}
