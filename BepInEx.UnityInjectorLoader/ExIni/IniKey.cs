// --------------------------------------------------
// ExIni - IniKey.cs
// --------------------------------------------------

#region Usings
using System;
using System.IO;
using System.Text.RegularExpressions;

using Microsoft.Win32;
#endregion

namespace ExIni
{

    /// <summary>
    ///     INI Key Node
    /// </summary>
    public class IniKey
    {
        #region Fields
        private readonly IniComment _comments;
        #endregion

        #region Properties
        /// <summary>
        ///     Comments of this Node
        /// </summary>
        public IniComment Comments
        {
            get { return _comments; }
        }

        /// <summary>
        ///     Key Name
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        ///     Unresolved Value
        /// </summary>
        public string RawValue { get; set; }

        /// <summary>
        ///     Resolved Value
        /// </summary>
        public string Value
        {
            get { return Resolve(RawValue); }
            set { RawValue = value; }
        }
        #endregion

        #region (De)Constructors
        /// <summary>
        ///     Creates a new <see cref="IniKey" />
        /// </summary>
        /// <param name="key">Key Name</param>
        /// <param name="value">Key Value</param>
        public IniKey(string key, string value = null)
        {
            Key = key;
            Value = value;

            _comments = new IniComment();
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Returns this <see cref="IniKey" /> in INI Format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}={1}", Key, RawValue);
        }
        #endregion

        #region Static Methods
        private static string GetEnvironment(string env)
        {
            return Environment.ExpandEnvironmentVariables(env);
        }

        private static string GetRegistry(string path)
        {
            var keyName = Path.GetDirectoryName(path);
            var valueName = Path.GetFileName(path);
            if (string.IsNullOrEmpty(keyName))
                return null;
            var regVal = Registry.GetValue(keyName, valueName, string.Empty);

            return regVal != null
                ? regVal.ToString()
                : null;
        }

        private static string Resolve(string value)
        {
            if (value == null)
                return null;

            Regex reg = new Regex(@"\$\((?<reg>.*)\)");
            Regex env = new Regex(@"%.*%");

            while (reg.IsMatch(value) || env.IsMatch(value))
            {
                value = reg.Replace(value, match => GetRegistry(match.Groups["reg"].Value));
                value = GetEnvironment(value);
            }
            return value;
        }
        #endregion
    }

}