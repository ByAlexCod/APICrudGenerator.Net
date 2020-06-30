using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Utility.CommandLine;
using static System.String;

namespace ALS.API.CRUDGenerator
{
    class Program
    {
        [Argument('c', "conf", "Configuration path")]
        private static string ConfigurationPath { get; set; }

        [Argument('i', "init", "Init")]
        private static bool Init { get; set; }

        private static string _confPath = "";

        private static string _sqlTableTemplatePath = "SQLRequestTemplate.template";
        private static string _csharpModelTemplatePath = "CSharpDalModel.template";

        static void Main(string[] args)
        {
            Arguments.Populate();

            _confPath = ConfigurationPath;
            if (Init)
            {
                InitConfig();
            }
            else
            {

                Console.WriteLine("Model name:");
                var modelName = Console.ReadLine();

                Dictionary<string, GenericTypeMapping> fields = new Dictionary<string, GenericTypeMapping>();
                while (true)
                {
                    Console.WriteLine("Field name:");
                    string entry = Console.ReadLine();
                    if (IsNullOrEmpty(entry)) break;
                    Console.WriteLine("Field type:");

                    var idx = 0;
                    foreach (var t in GenericTypeMapping.GetTypesMapping())
                    {
                        Console.WriteLine($"{idx}) {t.UserEntryType}");
                        idx++;
                    }

                    var userEntryType = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());

                    fields.Add(entry, GenericTypeMapping.GetTypesMapping()[userEntryType]);
                    Console.WriteLine("Field added");
                    Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                }

                CreateTable(modelName, fields);
                CreateModel(modelName, fields);
            }
        }

        static void InitConfig()
        {
            Configuration conf = new Configuration();
            foreach (var field in typeof(Configuration).GetProperties())
            {
                Console.WriteLine($"Set {field.Name}");
                var stringResult = Console.ReadLine();
                field.SetValue(conf, Convert.ChangeType(stringResult, field.PropertyType), null);
            }
            var configurationHandler = new ConfigurationHandler(_confPath, conf);
            configurationHandler.Save();
            Console.WriteLine($"Configuration saved, type dotnet ALS.ApiCRUDGenerator -c {ConfigurationPath} to start generating models.");
        }

        static void CreateTable(string modelName, Dictionary<string, GenericTypeMapping> fields)
        {
            int idx = 0;
            foreach (var field in fields)
            {
                Console.WriteLine($"{idx}) {field.Key}");
                idx++;
            }
            Console.WriteLine("Primary keys (ex: UserId,UserName):");

            var selectedPrimaries = Console.ReadLine();
            var conf = (new ConfigurationHandler(ConfigurationPath)).GetConf();
            var schema = conf.DbSchemaName;


            string template = File.ReadAllText(_sqlTableTemplatePath);
            template = template.Replace("{{table_name}}", schema + "." + modelName);

            List<string> fieldLines = new List<string>();
            foreach (var (key, value) in fields)
            {
                // Ask size if needed
                if (value.SqlType.Contains("{{size}}"))
                {
                    Console.WriteLine($"What max size should {value.UserEntryType} {key} be?");
                    value.SqlType = value.SqlType.Replace("{{size}}", Console.ReadLine());
                }
                string line = $"{key} {value.SqlType}";
                fieldLines.Add(line);
            }

            var constraints = new List<string>();
            constraints.Add($"PRIMARY KEY({selectedPrimaries})");

            template = template.Replace("{{columns}}", Join(",\n", fieldLines));
            template = template.Replace("{{constraints}}", Join(",\n", constraints));
            
            File.WriteAllText(Path.Join(conf.DbMigrationPath, 
                GenerateNextFileName(Directory.GetFiles(conf.DbMigrationPath).Select(Path.GetFileName).ToList(), modelName))+".sql", 
                template);
            Console.WriteLine(template);

        }

        private static void CreateModel(string modelName, Dictionary<string, GenericTypeMapping> fields)
        {
            var conf = (new ConfigurationHandler(ConfigurationPath)).GetConf();
            var modelPath = Path.Join(conf.ModelsPath, modelName + "Model.cs");

            var lines = new List<string>();
            foreach (var (key, value) in fields)
            {
                lines.Add($"public {value.GenericType.FullName} {key} " + "{get; set;}");
            }

            var template = File.ReadAllText(_csharpModelTemplatePath);
            template = template.Replace("{{namespace}}", conf.ModelNameSpace);
            template = template.Replace("{{model_name}}", modelName);

            template = template.Replace("{{properties}}", Join("\n", lines));

            File.WriteAllText(modelPath, template);
        }

        private static string GenerateNextFileName(List<string> files, string baseName)
        {
            files.Sort();
            var last = files[^1];
            var name = baseName;
            var lastId = 0;
            while(Compare(last, name, StringComparison.InvariantCulture) > 0)
            {
                name = lastId + "-" + baseName;
                lastId++;
            }

            return name;
        }
    }
}
