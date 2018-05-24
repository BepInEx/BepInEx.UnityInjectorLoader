// --------------------------------------------------
// ExIni - IniParser.cs
// --------------------------------------------------

#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
#endregion

namespace ExIni
{

    internal class IniParser
    {
        #region Static Fields
        private static readonly Regex CommentRegex = new Regex(@"^;(?<com>.*)");
        private static readonly Regex KeyRegex = new Regex(@"^(?<key>[\w\s]+)=(?<val>.*)$");
        private static readonly Regex SectionRegex = new Regex(@"^\[(?<sec>[\w\s]+)\]$");
        private static readonly Regex VarRegex = new Regex(@"^\@(?<key>[\w\s]+)=(?<val>.*)$");
        #endregion

        #region Public Static Methods
        public static IniFile Parse(string iniString)
        {
            IniFile ini = new IniFile();

            var lines = (from line in iniString.Split('\n')
                         //
                         let trimmed = line.Trim()
                         //
                         select trimmed.TrimEnd('\r')).ToArray();
            var comments = new List<string>();
            IniSection section = null;
            bool hasComments = false;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (String.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (IsComment(line))
                {
                    string comment = GetComment(line);
                    comments.Add(comment);
                    if (IsVariable(comment))
                    {
                        var cVar = GetVariable(comment);
                        string name = cVar[0];
                        string value = cVar[1];
                        Environment.SetEnvironmentVariable(name, value);
                    }
                    hasComments = true;
                    continue;
                }

                if (IsSection(line))
                {
                    section = ini[GetSection(line)];

                    if (hasComments)
                    {
                        section.Comments.Append(comments.ToArray());
                        comments.Clear();
                        hasComments = false;
                    }
                    continue;
                }

                if (IsKey(line))
                {
                    if (section == null)
                        throw new Exception(string.Format("{0}: Sectionless Key Value Pair", i));

                    var keyArr = GetKey(line);
                    string k = keyArr[0];
                    string v = keyArr[1];

                    if (hasComments)
                    {
                        section[k].Comments.Append(comments.ToArray());
                        comments.Clear();
                        hasComments = false;
                    }

                    section[k].Value = v;
                }
            }
            if (hasComments)
                ini.Comments.Append(comments.ToArray());
            return ini;
        }
        #endregion

        #region Static Methods
        private static string GetComment(string line)
        {
            return CommentRegex.Match(line)
                               .Groups["com"].Value;
        }

        private static string[] GetKey(string line)
        {
            Match match = KeyRegex.Match(line);
            return new[] {match.Groups["key"].Value, match.Groups["val"].Value};
        }

        private static string GetSection(string line)
        {
            return SectionRegex.Match(line)
                               .Groups["sec"].Value;
        }

        private static string[] GetVariable(string line)
        {
            Match match = VarRegex.Match(line);
            return new[] {match.Groups["key"].Value, match.Groups["val"].Value};
        }

        private static bool IsComment(string line)
        {
            return CommentRegex.IsMatch(line);
        }

        private static bool IsKey(string line)
        {
            return KeyRegex.IsMatch(line);
        }

        private static bool IsSection(string line)
        {
            return SectionRegex.IsMatch(line);
        }

        private static bool IsVariable(string line)
        {
            return VarRegex.IsMatch(line);
        }
        #endregion
    }

}