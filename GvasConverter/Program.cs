using GvasFormat;
using GvasFormat.Serialization;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace RailroadsOnlineSaveViewer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  arr-sav-converter path_to_save_file");
                Console.WriteLine("  or just simply drag&drop save on this app");
                Console.ReadKey();
                return;
            }

            string ext = Path.GetExtension(args[0]).ToLower();
            if (ext == ".json")
            {
                Console.WriteLine("Not implemented atm");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Parsing UE4 save file structure...");
                Gvas save;
                using (FileStream stream = File.Open(args[0], FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    save = UESerializer.Read(stream);
                }

                Console.WriteLine("Converting to json...");
                string json = JsonConvert.SerializeObject(save, new JsonSerializerSettings{Formatting = Formatting.Indented});

                Console.WriteLine("Saving json...");
                using (FileStream stream = File.Open(args[0] + ".orig.json", FileMode.Create, FileAccess.Write, FileShare.Read))
                using (StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)))
                {
                    writer.Write(json);
                }

                Properties properties = new Properties(save);

                Console.WriteLine("Converting to better json...");
                string betterJson = JsonConvert.SerializeObject(properties, new JsonSerializerSettings{Formatting = Formatting.Indented});

                Console.WriteLine("Saving better json...");
                using (FileStream stream = File.Open(args[0] + ".better.json", FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)))
                    {
                        writer.Write(betterJson);
                    }
                }
            }
            Console.WriteLine("Done.");
            Console.ReadKey(true);
        }
    }
}
