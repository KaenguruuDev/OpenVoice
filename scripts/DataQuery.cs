using MySql.Data.MySqlClient;

namespace OpenVoice
{
    public class DataQuery
    {
        string DataBase;
        string Id;

        MySqlConnection CurrentConnection;

        public DataQuery(string Server, string DataBase, string Id, string Password)
        {
            this.DataBase = DataBase;
            this.Id = Id;
            string ConnectionString = string.Format("server={0};database={1};uid={2};pwd={3}", Server, DataBase, Id, Password);
            CurrentConnection = new MySqlConnection(ConnectionString);
            try
            { CurrentConnection.Open(); }
            catch (MySqlException e) { throw new TimeoutException("Failed to open connection to: " + Server, e); }
        }

        private MySqlCommand GenerateCommand(string Command, string[][] Parameters = null)
        {
            MySqlCommand GeneratedCommand = CurrentConnection.CreateCommand();
            GeneratedCommand.CommandText = Command;

            // If no parameters are given, directly return command
            if (Parameters == null) return GeneratedCommand;
            for (int i = 0; i < Parameters.Length; i++)
            {
                GeneratedCommand.Parameters.AddWithValue(Parameters[i][0], Parameters[i][1]);
            }
            return GeneratedCommand;
        }

        public string[][] QueryTable(string Table, string[] Values = null)
        {
            string CommandText = "SELECT ";
            if (Values != null)
            {
                for (int i = 0; i < Values.Length; i++)
                {
                    CommandText += Values[i];
                }
            }

            CommandText += "* FROM " + Table;

            MySqlCommand NewCommand = GenerateCommand(CommandText);
            MySqlDataReader SQLResult = NewCommand.ExecuteReader();

            string[][] Output = new string[SQLResult.FieldCount][];

            int idx = 0;
            while (SQLResult.Read())
            {
                string[] Result = new string[SQLResult.FieldCount];
                for (int i = 0; i < SQLResult.FieldCount; i++)
                {
                    Result[i] = ((Int32) SQLResult.GetValue(i)).ToString();
                }
                Output[idx] = (string[]) Result.Clone();
                idx++;
            }
            return (string[][]) Output.Clone();
        }
    }
}
