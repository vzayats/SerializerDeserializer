using System;
using System.Collections.Generic;
using System.IO;

namespace SerializerDeserializer
{
    [Serializable]
    public class Folder
    {
        private List<string> listFolders;
        private List<string> listFiles;

        public Folder(string[] folds, string[] fls)
        {
            listFolders = new List<string>();
            listFiles = new List<string>();

            foreach (string fold in folds)
            {
                listFolders.Add(fold);
            }

            foreach (string file in fls)
            {
                listFiles.Add(file);
            }
        }

        //Extract folders and files into the selected folder
        public void Extract(string folderPath)
        {
            var path = Path.GetDirectoryName(listFolders[0]);

            foreach (var fold in listFolders)
            {
                if (path != null)
                {
                    var p = fold.Replace(path, folderPath);
                    Directory.CreateDirectory(p);
                }
            }

            foreach (var file in listFiles)
            {
                if (path != null)
                {
                    var p = file.Replace(path, folderPath);
                    File.Create(p);
                }
            }
        }
    }
}