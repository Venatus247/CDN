using System;
using System.IO;
using System.Linq;

namespace Commons.Utils
{
    public static class FileUtils
    {

        public const string DefaultExtension = ".file";
        
        public static bool TryGetExtension(string fileName, out string extension)
        {
            try
            {
                extension = Path.GetExtension(fileName);
                return !string.IsNullOrEmpty(fileName);
            }
            catch (Exception e)
            {
                extension = null;
                return false;
            }
            
        }

        public static string RemoveExtension(string fileName)
        {

            if (string.IsNullOrEmpty(fileName))
                return "";
            
            var parts = fileName.Split('.');
            
            if (parts.Length == 1) return fileName;
            return fileName.Substring(0, (fileName.Length - parts[^1].Length -1));

        }

        public static string RemoveIllegalCharsFromFileName(string fileName)
        {
            var chars = Path.GetInvalidFileNameChars();
            return chars.Aggregate(fileName, (current, c) => current.Replace(c.ToString(), ""));
        }
        
    }
}