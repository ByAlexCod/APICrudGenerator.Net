using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace ALS.API.CRUDGenerator
{
    public class ConfigurationHandler
    {
        private readonly string _path;
        private Configuration _conf;

        public Configuration GetConf()
        {
            return _conf;
        }

        public ConfigurationHandler(string path, Configuration conf = null)
        {
            _path = path;
            _conf = conf;
            _conf = conf;

            if (!File.Exists(_path) && GetConf() == null)
            {
                _conf = new Configuration();

            } else if (File.Exists(_path))
            {
                Load();
            }
        }

        public void Save(string path = null)
        {
            if (String.IsNullOrEmpty(path)) path = _path;
            var jsonString = JsonSerializer.Serialize(_conf);
            File.WriteAllText(path, jsonString);
        }

        public void Load(string path = null)
        {
            if (String.IsNullOrEmpty(path)) path = _path;
            _conf = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(path));
        }


    }
}
