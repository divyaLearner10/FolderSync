using System;
using System.IO;

namespace FolderSync
{
    class Program
    {
        static System.Threading.Timer _timer;
        static string sourcePath = "";
        static string destinationPath = "";
        static string log_path="";
        static int mins=0;
        static void Main(string[] args)
        {
            try
            {
                //geting values from console arguments
                sourcePath = args[0];
                destinationPath = args[1];
                log_path = args[2];
                mins = int.Parse(args[3]);
                mins = mins * 60000;//to convert milli second to minute
                _timer = new System.Threading.Timer(TimerCallback, null, 0, mins);

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void TimerCallback(object state)
        {
            var source = new DirectoryInfo(sourcePath);
            var destination = new DirectoryInfo(destinationPath);
            //this method will pass the source and destination folder address for the creation or updation of the files
            CopyFolderContents(sourcePath, destinationPath);
            DeleteAll(source, destination);
        }

        public static void CopyFolderContents(string sourceFolder, string destinationFolder)
        {
            try
            {
                
                var exDir = sourceFolder;
                var dir = new DirectoryInfo(exDir);
                var destDir = new DirectoryInfo(destinationFolder);
                String[] arr = Directory.GetFiles(dir.ToString());
                foreach (string sourceFile in Directory.GetFiles(dir.ToString()))
                {
                    FileInfo srcFile = new FileInfo(sourceFile);
                    string srcFileName = srcFile.Name;

                    // Create a destination that matches the source structure
                    FileInfo destFile = new FileInfo(destinationFolder + srcFile.FullName.Replace(sourceFolder, ""));
                    if (!destFile.Exists)
                    {
                        Console.WriteLine("A File Name " + destFile.Name + " has been Created");
                        File.Copy(srcFile.FullName, destFile.FullName, true);
                        //Writting the event in the log file
                        string toWrite = "[" + DateTime.Now.ToString() + "] " + "A File Name " + destFile.Name + " has been Created";
                        StreamWriter wtr = File.AppendText(log_path);
                        wtr.WriteLine(toWrite);
                        wtr.Close();
                    }

                    //Check if src file was modified and modify the destination file
                    else if (srcFile.LastWriteTime > destFile.LastWriteTime || !destFile.Exists)
                    {
                        Console.WriteLine("A File Name " + destFile.Name + " has been Updated");
                        File.Copy(srcFile.FullName, destFile.FullName, true);
                        //Writting the event in the log file
                        string toWrite = "[" + DateTime.Now.ToString() + "] " + "A File Name " + destFile.Name + " has been Updated";
                        StreamWriter wtr = File.AppendText(log_path);
                        wtr.WriteLine(toWrite);
                        wtr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
            }
        }

        private static void DeleteAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (!source.Exists)
            {
                target.Delete(true);
                return;
            }

            // Delete each existing file in target directory not existing in the source directory.
            foreach (FileInfo fi in target.GetFiles())
            {
                var sourceFile = Path.Combine(source.FullName, fi.Name);
                if (!File.Exists(sourceFile)) //Source file doesn't exist, delete target file
                {
                    Console.WriteLine("A File Name " + fi.Name + " has been Deleted");
                    fi.Delete();
                    //Writting the event in the log file
                    string toWrite = "[" + DateTime.Now.ToString() + "] " + "A File Name " + fi.Name + " has been Deleted";
                    StreamWriter wtr = File.AppendText(log_path);
                    wtr.WriteLine(toWrite);
                    wtr.Close();
                }
            }
        }
    }
}
