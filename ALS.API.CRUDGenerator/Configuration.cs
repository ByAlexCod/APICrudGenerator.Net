using System;
using System.Collections.Generic;
using System.Text;

namespace ALS.API.CRUDGenerator
{
    [Serializable]
    public class Configuration
    {
        public string DbMigrationPath { get; set; }
        public string ControllersPath { get; set; }
        public string ModelsPath { get; set; }
        public string DbSchemaName { get; set; }
        public string ModelNameSpace { get; set; }
    }
}
