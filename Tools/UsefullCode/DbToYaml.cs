using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Xunit;
using Xunit.Abstractions;

namespace UsefullCode
{
    public class DbToYaml
    {
        private readonly ITestOutputHelper output;

        public DbToYaml(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test1()
        {
            string connectionString = @"Data Source=(localdb)\mssqllocaldb;Initial Catalog=test;Integrated Security=True";
            string tableName = "TestTable";

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
                        while (reader.Read())
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
                    var result = serializer.Serialize(buffer);

                    output.WriteLine(result);
                }
            }
        }
    }
}
