#region Licence
/*
   The MIT License (MIT)
   
   Copyright (c) 2015 usagirei
   
   Permission is hereby granted, free of charge, to any person obtaining a copy
   of this software and associated documentation files (the "Software"), to deal
   in the Software without restriction, including without limitation the rights
   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   copies of the Software, and to permit persons to whom the Software is
   furnished to do so, subject to the following conditions:
   
   The above copyright notice and this permission notice shall be included in all
   copies or substantial portions of the Software.
   
   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
   SOFTWARE.
   
 */
#endregion

// --------------------------------------------------
// ExIni - IniFile.cs
// --------------------------------------------------

#region Usings
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace ExIni
{

    /// <summary>
    ///     INI File Class
    /// </summary>
    public class IniFile
    {
        #region Fields
        private readonly IniComment _comments;
        private readonly List<IniSection> _sections;
        #endregion

        #region Properties
        /// <summary>
        ///     Creates or returns an existing <see cref="IniSection" /> of this <see cref="IniFile" />
        ///     <para />
        ///     Alias to <see cref="CreateSection" />
        /// </summary>
        /// <param name="sec">Section Name</param>
        public IniSection this[string sec]
        {
            get { return CreateSection(sec); }
        }

        /// <summary>
        ///     Unrooted Comments
        /// </summary>
        public IniComment Comments
        {
            get { return _comments; }
        }

        /// <summary>
        ///     Ini Sections
        /// </summary>
        public List<IniSection> Sections
        {
            get { return _sections; }
        }
        #endregion

        #region (De)Constructors
        /// <summary>
        ///     Creates an Empty Ini File
        /// </summary>
        public IniFile()
        {
            _comments = new IniComment();
            _sections = new List<IniSection>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Returns this <see cref="IniFile" /> Contents in Ini Format
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Sections.Count; i++)
            {
                IniSection s = Sections[i];

                if (s.Comments.Comments.Any())
                    sb.AppendLine(s.Comments.ToString());
                sb.AppendLine(s.ToString());

                foreach (IniKey k in s.Keys)
                {
                    if (k.Comments.Comments.Any())
                        sb.AppendLine(k.Comments.ToString());
                    sb.AppendLine(k.ToString());
                }

                if (i < Sections.Count - 1)
                    sb.AppendLine();
            }

            if (Comments.Comments.Any())
            {
                sb.AppendLine();
                sb.AppendLine(Comments.ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Creates or Returns an existing <see cref="IniSection" />
        /// </summary>
        /// <param name="section">Section Name</param>
        public IniSection CreateSection(string section)
        {
            IniSection get = GetSection(section);
            if (get != null)
                return get;

            IniSection gen = new IniSection(section);
            _sections.Add(gen);
            return gen;
        }

        /// <summary>
        ///     Deletes an <see cref="IniSection" />
        /// </summary>
        /// <param name="section">Key Name</param>
        /// <returns>True if Deleted</returns>
        public bool DeleteSection(string section)
        {
            if (!HasSection(section))
                return false;
            Sections.Remove(GetSection(section));
            return true;
        }

        /// <summary>
        ///     Gets an Existing <see cref="IniSection" /> or null
        /// </summary>
        /// <param name="section">Section Name</param>
        public IniSection GetSection(string section)
        {
            return HasSection(section)
                ? _sections.FirstOrDefault(iniSection => iniSection.Section == section)
                : null;
        }

        /// <summary>
        ///     Checks wether an <see cref="IniSection" /> exists
        /// </summary>
        /// <param name="section">Section Name</param>
        public bool HasSection(string section)
        {
            return _sections.Any(iniSection => iniSection.Section == section);
        }

        /// <summary>
        ///     Merges two <see cref="IniFile" />s into One
        ///     <para />
        ///     Conflicting values on this instance will be overriden by the second instance
        /// </summary>
        /// <param name="ini">Second Ini File</param>
        public void Merge(IniFile ini)
        {
            Comments.Append(ini.Comments.Comments.ToArray());
            foreach (IniSection sOther in ini.Sections)
            {
                IniSection sThis = this[sOther.Section];
                sThis.Comments.Append(sOther.Comments.Comments.ToArray());
                foreach (IniKey kOther in sOther.Keys)
                {
                    IniKey kThis = sThis[kOther.Key];
                    kThis.Comments.Append(kOther.Comments.Comments.ToArray());
                    kThis.Value = kOther.Value;
                }
            }
        }

        /// <summary>
        ///     Saves this <see cref="IniFile" /> to Disk
        /// </summary>
        /// <param name="filePath">File Path</param>
        public void Save(string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if(!string.IsNullOrEmpty(directoryName))
                Directory.CreateDirectory(directoryName);
            File.WriteAllText(filePath, ToString(), Encoding.UTF8);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        ///     Parses an Ini File from Disk
        /// </summary>
        /// <param name="iniString">File Path</param>
        public static IniFile FromFile(string iniString)
        {
            return IniParser.Parse(File.ReadAllText(iniString));
        }

        /// <summary>
        ///     Parses an Ini File from a String
        ///     <para />
        ///     To Save to a String use <see cref="ToString" />
        /// </summary>
        /// <param name="iniString">String</param>
        public static IniFile FromString(string iniString)
        {
            return IniParser.Parse(iniString);
        }
        #endregion
    }

}