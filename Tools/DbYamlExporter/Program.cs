using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DbYamlExporter
{
    class Program
    {
        static string connectionString = @"Data Source=(localdb)\mssqllocaldb;Initial Catalog=test;Integrated Security=True";
        static string tableName = "TestTable";
        static void Main(string[] args)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select * from " + tableName;
                    command.CommandType = CommandType.Text;

                    var buffer = new List<Dictionary<string, object>>();

                    using (var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            var item = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var name = reader.GetName(i);
                                var value = reader.GetValue(i);
                                item[name] = value;
                            }

                            buffer.Add(item);
                        }
                    }


                    var serializer = new YamlDotNet.Serialization.Serializer();
                    serializer.Serialize(Console.Out, buffer);
                }
            }

        }
    }
}
