using System;
using System.Collections.Generic;
using System.Linq;
using API.Controllers;
using Commons;
using Core.Data.File;
using MongoDB.Driver;

namespace Core.Utils
{
    [Obsolete]
    public class FileIdGenerator : Singleton<FileIdGenerator>
    {
        private const int DefaultLength = 8;
        private const string AvailableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private readonly Dictionary<char, int> _charIndexDirectory = new Dictionary<char, int>();
        private int AvailableCharAmount { get; }
        
        protected FileIdGenerator()
        {
            var i = 1;
            foreach (var c in AvailableChars)
            {
                _charIndexDirectory.Add(c, i++);
            }

            AvailableCharAmount = _charIndexDirectory.Count;
        }
        
        public string GenerateId()
        {
            var last = FileController.Instance.Collection.FindSync(FilterDefinition<SavedFile>.Empty).ToList()
                .LastOrDefault();
            
            return last == null ? GetDefault() : GetNext(last.FileId);
        }

        public bool IsEmpty(object id)
        {
            if (id != null)
                return (string) id == "";
            return true;
        }

        private int[] StringIdToIndexArray(string id)
        {
            var indexArray = new int[id.Length];
            
            for (var i = 0; i < id.Length; i++)
            {
                if (_charIndexDirectory.TryGetValue(id[i], out var value))
                    indexArray[i] = value;
                else
                    Logger.Error($"Could not get char for '{id[i]}'!");
            }

            return indexArray;
        }

        private IEnumerable<int> IncrementIdIndexArray(int[] idIndexArray)
        {
            for (var i = idIndexArray.Length - 1; i >= 0; i--)
            {
                if (idIndexArray[i] < AvailableCharAmount)
                {
                    idIndexArray[i] = idIndexArray[i] + 1;
                    return idIndexArray;
                }

                idIndexArray[i] = 1;

                if (i != 0) continue;
		
                var newList = idIndexArray.ToList();
                newList.Insert(0, 1);
                idIndexArray = newList.ToArray();
            }
	
            return idIndexArray;
        }

        /*
        private IEnumerable<int> IncrementIdIndexArray(int[] idIndexArray)
        {
	        var i = idIndexArray.Length;
	        while (true)
	        {
		        i -= 1;

		        if (idIndexArray[i] < AvailableCharAmount)
		        {
			        idIndexArray[i] = idIndexArray[i] + 1;
			        return idIndexArray;
		        }

		        idIndexArray[i] = 1;

		        if (i != 0) continue;
		        
		        var newList = idIndexArray.ToList();
		        newList.Insert(0, 1);
		        idIndexArray = newList.ToArray();

		        return idIndexArray;
	        }
        }
        */
        
        private static string IdIndexArrayToString(IEnumerable<int> idIndexArray)
        {
            return idIndexArray.Aggregate("", (current, i) => current + AvailableChars[i-1]);
        }
        
        private string GetNext(string current)
        {
            return IdIndexArrayToString(IncrementIdIndexArray(StringIdToIndexArray(current)));
        }

        private static string GetDefault()
        {
            var str = "";
            for (var i = 0; i < DefaultLength; i++)
            {
                str += AvailableChars[0];
            }

            return str;
        }
        
    }
}