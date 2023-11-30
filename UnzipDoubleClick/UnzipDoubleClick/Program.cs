using Aspose.Zip;
using Aspose.Zip.SevenZip;
using System;
using System.IO;
using System.Text;
using System.Linq;

namespace UnzipDoubleClick
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      try
      {
        string sourceFile = Environment.GetCommandLineArgs()[1];
        string outputFolder = sourceFile.Substring(0, sourceFile.LastIndexOf('\\'));

        //Open file from disk using a file stream
        FileStream ZipFileToBeExtracted = File.Open(sourceFile, FileMode.Open);

        //Load Zip file stream to Archive object
        if (sourceFile.Contains(".7z"))
        {
          SevenZipArchive ZipArchiveToExtract = new SevenZipArchive(ZipFileToBeExtracted);
          //Get number of files
          int NumberOfFileInArchive = ZipArchiveToExtract.Entries.Count;
          //Loop through the archive for each file
          for (int FileCounter = 0; FileCounter < NumberOfFileInArchive; FileCounter++)
          {
            //Get each zip archive entry and extract the file
            SevenZipArchiveEntry ArchiveFileEntry = ZipArchiveToExtract.Entries[FileCounter];
            string NameOfFileInZipEntry = ArchiveFileEntry.Name;
            ArchiveFileEntry.Extract(outputFolder + "\\" + NameOfFileInZipEntry);
          }
        }
        else
        {
          using (var archive = new Archive(ZipFileToBeExtracted))
          {
            if (archive.Entries.Count == 1) //il n'y a qu'un fichier ou un dossier vide
            {
              if (archive.Entries[0].IsDirectory) //c'est un dossier
              {
                Directory.CreateDirectory(Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(sourceFile)));
              }
              else //c'est un fichier
              {
                archive.Entries[0].Extract(Path.Combine(outputFolder, Path.ChangeExtension(Path.GetFileName(sourceFile), Path.GetExtension(archive.Entries[0].Name))));
              }
            }
            else
            {
              if (archive.Entries.Any(x => GetMostParent(x.Name) != Path.GetFileNameWithoutExtension(sourceFile))) 
              {
                //on crée un dossier dans lequel on extrait
                archive.ExtractToDirectory(Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(sourceFile)));
              }
              else
              {
                //tous les fichiers sont dans un dossier
                archive.ExtractToDirectory(outputFolder); //on extrait simplement
              }
              
            }
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message.ToString());
      }
    }

    private static string GetMostParent(string objectName)
    {
      string parent = objectName;
      string tmp = "";
      do
      {
        tmp = parent;
        parent = Path.GetDirectoryName(parent);
      }
      while (!String.IsNullOrEmpty(parent));
      return tmp;
    }
  }
}