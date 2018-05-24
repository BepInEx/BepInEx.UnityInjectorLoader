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