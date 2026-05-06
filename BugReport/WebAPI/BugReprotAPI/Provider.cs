using Npgsql;
using NpgsqlTypes;
using Newtonsoft.Json;

namespace BugReprotAPI
{
    public class Provider
    {
        private const string connString
            = "Host=s0.v100.vip;Port=32649;Database=postgres;Username=postgres;Password=123456";

        public static async Task InsertIntoSchemaTable(string schemaName, string tableName, BugTrace data)
        {
            var sql = $"INSERT INTO data.bug (error_message, stack_trace, timestamp) VALUES (@p1, @p2, @p3)"; //table大写会找不到，报错！

            await using var dataSource = NpgsqlDataSource.Create(connString);
            await using var cmd = dataSource.CreateCommand(sql);

            cmd.Parameters.AddWithValue("p1", data.Error_message);
            cmd.Parameters.AddWithValue("p2", data.Stack_trace);
            cmd.Parameters.AddWithValue("p3", data.Timestamp);

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task InsertJsonB(string schemaName, string tableName, string json)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
            var dataSource = dataSourceBuilder.Build();
            var conn = await dataSource.OpenConnectionAsync();

            string sql = $"INSERT INTO {schemaName}.{tableName} (json) VALUES (@param)"; //table大写会找不到，报错！
            await using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("param", NpgsqlDbType.Jsonb, json);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task<string> Query()
        {
            List<string> results = new List<string>();

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
            var dataSource = dataSourceBuilder.Build();
            var conn = await dataSource.OpenConnectionAsync();
            //await using (var cmd = new NpgsqlCommand("SELECT json,owner,correct_time FROM log_bugtrace.bug", conn))
            await using (var cmd = new NpgsqlCommand("SELECT * FROM log_bugtrace.bug", conn))
            await using (var reader = await cmd.ExecuteReaderAsync()) //读取
            {
                // 组装成数组，返回。
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }
                    rows.Add(row);
                }

                // Serialize the list of rows to JSON
                string jsonOutput = JsonConvert.SerializeObject(rows, Formatting.Indented);
                Console.WriteLine("\nTable data as JSON:");
                Console.WriteLine(jsonOutput);

                return jsonOutput;
            }
        }

        // 程序员修改 BUG 后，记录操作人和修改时间
        public static async Task Update()
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
            var dataSource = dataSourceBuilder.Build();
            var conn = await dataSource.OpenConnectionAsync();
            string sql = "UPDATE log_bugtrace.bug SET owner = @owner, correct_time = @correct_time WHERE id = @id";
            await using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("owner", "Alice");
                cmd.Parameters.AddWithValue("correct_time", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("id", 1); // 假设我们更新 ID 为 1 的记录
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"Rows affected: {rowsAffected}");
            }
        }

        public static async Task DeleteAll(string schemaName, string tableName)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
            var dataSource = dataSourceBuilder.Build();
            var conn = await dataSource.OpenConnectionAsync();
            string sql = $"DELETE FROM {schemaName}.{tableName}";
            await using (var cmd = new NpgsqlCommand(sql, conn))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}

public class Bugs
{
    //public BugTrace Trace { get; set; }
    public string? Json { get; set; }
    public string? Owner { get; set; }
    public string? Correct_time { get; set; }
}