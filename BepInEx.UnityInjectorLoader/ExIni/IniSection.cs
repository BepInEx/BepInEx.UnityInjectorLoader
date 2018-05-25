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
// ExIni - IniSection.cs
// --------------------------------------------------

#region Usings
using System.Collections.Generic;
using System.Linq;
#endregion

namespace ExIni
{

    /// <summary>
    ///     INI Section Node
    /// </summary>
    public class IniSection
    {
        #region Fields
        private readonly IniComment _comments;
        private readonly List<IniKey> _keys;
        #endregion

        #region Properties
        /// <summary>
        ///     Creates or returns an existing <see cref="IniKey"/> of this <see cref="IniSection"/>
        ///     <para />
        ///     Alias to <see cref="CreateKey"/>
        /// </summary>
        /// <param name="key">Key Name</param>
        public IniKey this[string key]
        {
            get { return CreateKey(key); }
        }

        /// <summary>
        ///     Comments of this Node
        /// </summary>
        public IniComment Comments
        {
            get { return _comments; }
        }

        /// <summary>
        ///     Section Keys
        /// </summary>
        public List<IniKey> Keys
        {
            get { return _keys; }
        }

        /// <summary>
        ///     Section Name
        /// </summary>
        public string Section { get; set; }
        #endregion

        #region (De)Constructors
        /// <summary>
        ///     Creates a new <see cref="IniSection"/>
        /// </summary>
        /// <param name="section">Section Name</param>
        public IniSection(string section)
        {
            Section = section;

            _comments = new IniComment();
            _keys = new List<IniKey>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Returns this <see cref="IniSection"/> in INI Format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}]", Section);
        }

        /// <summary>
        ///     Creates or Returns an existing <see cref="IniKey"/>
        /// </summary>
        /// <param name="key">Key Name</param>
        public IniKey CreateKey(string key)
        {
            IniKey get = GetKey(key);
            if (get != null)
                return get;

            IniKey gen = new IniKey(key);
            _keys.Add(gen);
            return gen;
        }

        /// <summary>
        ///     Gets an Existing <see cref="IniKey"/> or null
        /// </summary>
        /// <param name="key">Key Name</param>
        public IniKey GetKey(string key)
        {
            return HasKey(key)
                ? _keys.FirstOrDefault(iniKey => iniKey.Key == key)
                : null;
        }

        /// <summary>
        ///     Checks wether an <see cref="IniKey"/> exists
        /// </summary>
        /// <param name="key">Key Name</param>
        public bool HasKey(string key)
        {
            return _keys.Any(iniKey => iniKey.Key == key);
        }

        /// <summary>
        ///     Deletes an <see cref="IniKey"/>
        /// </summary>
        /// <param name="key">Key Name</param>
        /// <returns>True if Deleted</returns>
        public bool DeleteKey(string key)
        {
            if (!HasKey(key))
                return false;
            Keys.Remove(GetKey(key));
            return true;
        }
        #endregion
    }

}