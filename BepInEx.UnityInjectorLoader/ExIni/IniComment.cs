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
// ExIni - IniComment.cs
// --------------------------------------------------

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace ExIni
{

    /// <summary>
    ///     INI Comment Node
    /// </summary>
    public class IniComment
    {
        #region Properties
        /// <summary>
        ///     Comments
        /// </summary>
        public List<string> Comments { get; set; }
        #endregion

        #region (De)Constructors
        /// <summary>
        ///     Creates a new <see cref="IniComment"/> Node
        /// </summary>
        public IniComment()
        {
            Comments = new List<string>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Returns this <see cref="IniComment"/> in Ini Format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Comments.Count; i++)
            {
                string comment = Comments[i];

                string value = i < Comments.Count - 1
                    ? ";" + comment + Environment.NewLine
                    : ";" + comment;

                sb.Append(value);
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Append one or more comments to this node
        /// </summary>
        /// <param name="comments">Comments</param>
        public void Append(params string[] comments)
        {
            Comments.AddRange(comments);
        }
        #endregion
    }

}