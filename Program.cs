using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace RenameChallenge
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Are you changing the name of a FILE or FOLDER?");
            Console.WriteLine("1. File");
            Console.WriteLine("2. Folder");
            var Input = Console.ReadLine();
            if(Input.ToLower() == "2" || Input.ToLower() == "folder")
            {
                using(FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.ShowNewFolderButton = true;
                    var Info = new DirectoryStruct { };
                    if(fbd.ShowDialog() == DialogResult.OK)
                    {
                        Console.WriteLine("Searching...");
                        Info.Name = fbd.SelectedPath;
                        Info.Parent = Path.GetDirectoryName(Info.Name);
                    }

                    var DirectoryName = Info.Name;
                    var Parent = Info.Parent;

                    if (Directory.Exists(DirectoryName))
                    {
                        goto DirectoryChoice;
                    }

                    DirectoryChoice:
                        if (Directory.Exists(DirectoryName))
                        {
                            Console.WriteLine(string.Format("New directory name for {0}", DirectoryName));
                            Console.Write("Input: ");
                            Input = Console.ReadLine();
                            Helper.TransferAllFilesInDirectory(DirectoryName, Parent + "/" + Input);
                            Console.WriteLine("DONE!");
                        }
                }
            }
            else
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.InitialDirectory = "./";
                    ofd.Filter = "All files (*.*)|*.*";
                    Console.WriteLine("What's the file you want to rename?");
                    //foreach (var Direc in Directory.EnumerateFileSystemEntries("."))
                    //{
                    //    var Temp = Direc;
                    //    if (File.Exists(Direc)) Temp = Temp + " -- File -- ";
                    //    if (Directory.Exists(Direc)) Temp = Temp + " -- Directory -- ";
                    //    Console.WriteLine(Temp.Substring(2));
                    //}
                    //Console.Write("Input: ");
                    //var Input = Console.ReadLine();
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        Input = ofd.FileName;
                        FileStruct Info = new FileStruct { };
                        Info.Name = Input;
                        Info.Extension = Path.GetExtension(Info.Name);
                        Info.Parent = Path.GetDirectoryName(Info.Name);
                        Console.WriteLine("Searching...");
                        /*foreach (var Direc in Directory.EnumerateFileSystemEntries("."))
                        {
                            var NewDirec = Direc.Substring(2);
                            var NewExtension = NewDirec.GetAfterOrEmpty();
                            if (Input.GetAfterOrEmpty() != string.Empty && Input == NewDirec)
                            {
                                if (File.Exists(NewDirec))
                                {
                                    var Temp2 = new FileInfo(NewDirec);
                                    Info.Name = Temp2.Name;
                                    Info.Extension = Temp2.Extension;
                                }
                                else
                                {
                                    var Temp2 = new DirectoryInfo(NewDirec);
                                    Info.Name = Temp2.Name;
                                    Info.Extension = Temp2.Extension;
                                }
                                break;
                            }
                            var Temp = NewDirec.Substring(0, NewDirec.Length - NewExtension.Length);
                            if (Input == Temp)
                            {
                                if (File.Exists(NewDirec))
                                {
                                    var Temp2 = new FileInfo(NewDirec);
                                    Info.Name = Temp2.Name;
                                    Info.Extension = Temp2.Extension;
                                }
                                else
                                {
                                    var Temp2 = new DirectoryInfo(NewDirec);
                                    Info.Name = Temp2.Name;
                                    Info.Extension = Temp2.Extension;
                                }
                                break;
                            }
                        }*/
                        //var NameAndExtension = Input.SeperateFileAndExtesion();
                        var FileName = Info.Name;
                        var Extension = Info.Extension;

                        if (File.Exists(FileName))
                        {
                            goto FileChoice;
                        }
                        else
                        {
                            //Console.WriteLine("Couldn't find file! Creating...");
                            ///File.Create(Input);
                            //Console.WriteLine("Done! Restart program.");
                            Console.WriteLine("It seems there was a problem with the file.");
                            Console.ReadKey();
                        }

                    FileChoice:
                        if (File.Exists(FileName))
                        {
                            Console.WriteLine(string.Format("New file name for {0}", FileName));
                            Console.Write("Input: ");
                            Input = Console.ReadLine();
                            var NewFileName = Input;
                            if (Input.Length > Extension.Length)
                            {
                                if (Input.Substring(Input.Length - Extension.Length) == Extension)
                                {
                                    NewFileName = NewFileName.Substring(0, Input.Length - Extension.Length);
                                }
                            }
                            NewFileName = NewFileName + Extension;
                            //Console.WriteLine("new: " + NewFileName);
                            var FileBytes = File.ReadAllBytes(FileName);
                            File.Delete(FileName);
                            File.WriteAllBytes($"{Info.Parent}/{NewFileName}", FileBytes);
                            Console.WriteLine("DONE!");
                        }
                    }
                }
            }
        }
    }

    static class Helper
    {
        public static string GetAfterOrEmpty(this string text, string stopAt = ".")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(charLocation);
                }
            }

            return String.Empty;
        }

        public static string GetAfterLastOrEmpty(this string text, char stopAt = '\\')
        {
            var CharArray = text.ToCharArray();
            var Index = 0;
            var LastIndex = -1;

            foreach(char character in CharArray)
            {
                if(CharArray[Index] == stopAt)
                {
                    LastIndex = Index;
                }
                Index = Index + 1;
            }

            if(LastIndex != -1)
            {
                var Last = text.Substring(LastIndex);
                return Last;
            }

            return String.Empty;
        }

        public static Dictionary<string, string> SeperateFileAndExtesion(this string text)
        {
            var Extension = text.GetAfterOrEmpty(".");
            var Name = text.Substring(0, text.Length - Extension.Length);
            var Dictionary = new Dictionary<string, string>
            {
                ["Name"] = Name,
                ["Extension"] = Extension
            };
            return Dictionary;
        }

        public static void TransferAllFilesInDirectory(string path, string newpath)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(newpath);
            if (!Directory.Exists(newpath)) Directory.CreateDirectory(newpath);
            foreach (var i in Directory.EnumerateFiles(path))
            {
                var Name = i.GetAfterLastOrEmpty(stopAt: '\\');
                var Bytes = File.ReadAllBytes(i);
                File.Delete(i);
                File.WriteAllBytes($"{newpath}/{Name}", Bytes);
            }
            Directory.Delete(path);
        }
    }

    public struct FileStruct
    {
        public string Name;
        public string Extension;
        public string Parent;
    }

    public struct DirectoryStruct
    {
        public string Name;
        public string Parent;
    }
}
