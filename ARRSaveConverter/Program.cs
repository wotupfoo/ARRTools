using GvasFormat;
using GvasFormat.Serialization;
using ARRSaveFormat;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace ARRSaveConverter
{
    internal class Program
    {
        private static bool silent;
        private static bool noWait;
        private static bool genDirty;

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  arr-sav-converter path_to_save_file");
            Console.WriteLine("  or just simply drag&drop save on this app");
            Console.WriteLine();
            Console.WriteLine("Parameters:");
            Console.WriteLine("  --silent - disables output to console");
            Console.WriteLine("  --no-wait - disables keyboard input at the end of run");
            Console.WriteLine("  --gen-dirty - generate \"dirty\" JSON in original format");

            Console.ReadKey();
        }

        private static string ParseParams(string[] args)
        {
            string fileName = null;

            if (args.Length == 0)
            {
                Console.WriteLine("Missing <path_to_save> parameter!");
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Length > 2 && args[i][0] == '-')
                {
                    string lowerVar = args[i].ToLower();
                    if (lowerVar == "--silent")
                        silent = true;
                    else if (lowerVar == "--no-wait")
                        noWait = true;
                    else if (lowerVar == "--gen-dirty")
                        genDirty = true;
                    else
                    {
                        Console.WriteLine($"Unknown parameter \"{args[i]}\"!");
                        return null;
                    }
                } 
                else if (fileName == null)
                {
                    fileName = args[i];
                }
                else
                {
                    Console.WriteLine("Only one file at once is supported!");
                    return null;
                }
            }

            return fileName;

        }
        private static void Main(string[] args)
        {
            string filePath = ParseParams(args);
            if (filePath == null)
            {
                PrintUsage();
                return;
            }

            string ext = Path.GetExtension(args[0]).ToLower();
            if (ext == ".json")
            {
                if (!silent)
                {
                    Console.WriteLine("Not implemented atm");
                    if (!noWait)
                        Console.ReadKey();
                }
            }
            else
            {
                if (!silent)
                    Console.WriteLine("Parsing UE4 save file structure...");
                Gvas save;
                using (FileStream stream = File.Open(args[0], FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    save = UESerializer.Read(stream);
                }

                if (genDirty)
                {
                    if (!silent)
                        Console.WriteLine("Converting to dirty json...");
                    string json = JsonConvert.SerializeObject(save, new JsonSerializerSettings{Formatting = Formatting.Indented});

                    if (silent)
                        Console.WriteLine("Saving drity json...");
                    using (FileStream stream = File.Open(args[0] + ".orig.json", FileMode.Create, FileAccess.Write, FileShare.Read))
                    using (StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)))
                    {
                        writer.Write(json);
                    }
                }

                Properties properties = new Properties(save);

                if (!silent)
                    Console.WriteLine("Converting to better json...");
                string betterJson = JsonConvert.SerializeObject(properties, new JsonSerializerSettings{Formatting = Formatting.Indented});

                if (!silent)
                    Console.WriteLine("Saving better json...");
                using (FileStream stream = File.Open(args[0] + ".better.json", FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)))
                    {
                        writer.Write(betterJson);
                    }
                }
            }
            if (!silent)
                Console.WriteLine("Done.");

            if (Environment.UserInteractive && !Console.IsInputRedirected && !silent && !noWait)
                Console.ReadKey(true);
        }
    }
}
