using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALS.API.CRUDGenerator
{
    public class GenericTypeMapping
    {

        public Type GenericType { get; set; }
        public string SqlType { get; set; }
        public string UserEntryType { get; set; }

        public static List<GenericTypeMapping> GetTypesMapping()
        {
            var types = new List<GenericTypeMapping>()
            {
                new GenericTypeMapping()
                {
                    GenericType = typeof(int),
                    SqlType = "int",
                    UserEntryType = "int"
                },
                new GenericTypeMapping()
                {
                    GenericType = typeof(long),
                    SqlType = "bigint",
                    UserEntryType = "long"
                },
                new GenericTypeMapping()
                {
                    GenericType = typeof(string),
                    SqlType = "nvarchar({{size}})",
                    UserEntryType = "nvarchar"
                },
                new GenericTypeMapping()
                {
                    GenericType = typeof(string),
                    SqlType = "text",
                    UserEntryType = "text"
                },
                new GenericTypeMapping()
                {
                    GenericType = typeof(bool),
                    SqlType = "bit",
                    UserEntryType = "bool"
                },
                new GenericTypeMapping()
                {
                    GenericType = typeof(long),
                    SqlType = "float",
                    UserEntryType = "long"
                },
                new GenericTypeMapping()
                {
                    GenericType = typeof(DateTime),
                    SqlType = "datetime2",
                    UserEntryType = "datetime"
                }

            };

            return types;
        }

        public static GenericTypeMapping GetMappingFromUserEntry(string entry)
        {
            return GetTypesMapping().FirstOrDefault(e => e.UserEntryType == entry);
        }
    }
    
}
