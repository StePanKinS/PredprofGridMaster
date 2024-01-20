using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Diagnostics;

namespace PredprofInterpreter
{
    internal class DB
    {
        readonly MySqlConnection connection;

        readonly MySqlCommand onOpen;
        readonly string onOpenStr =
            "CREATE DATABASE IF NOT EXISTS GridMaster;" +
            "USE GridMaster;" +
            "CREATE TABLE IF NOT EXISTS Programs (" +
            "Name VARCHAR(512) UNIQUE NOT NULL," +
            "Code MEDIUMTEXT NOT NULL," +
            "Result MEDIUMTEXT NULL );";

        readonly MySqlCommand getNames;
        readonly string getNamesStr =
            "SELECT Name FROM programs;";

        readonly MySqlCommand getCode;
        readonly string getCodeStr =
            "SELECT Code FROM programs WHERE Name = @name;";

        readonly MySqlCommand setNameCode;
        readonly string setNameCodeStr =
            "INSERT programs(Name, Code) VALUES (@name, @code);";

        readonly MySqlCommand setNameCodeRes;
        readonly string setNameCodeResStr =
            "INSERT programs(Name, Code, Result) VALUES (@name, @code, @res);";

        readonly MySqlCommand updateNameCode;
        readonly string updateNameCodeStr =
            "UPDATE programs SET Code = @code WHERE Name = @name;";

        readonly MySqlCommand updateNameCodeRes;
        readonly string updateNameCodeResStr =
            "UPDATE programs SET Code = @code, Result = @res WHERE Name = @name;";

        readonly MySqlCommand deleteProgram;
        readonly string deleteProgradStr =
            "DELETE FROM programs WHERE Name = @name;";

        Process? dbproc;

        public DB() : this("localhost", 3306, "root", "root") { }

        public DB(string host, int port, string username, string password)
        {

            connection = new MySqlConnection($"server={host};port={port};username={username};password={password}");
            onOpen = new MySqlCommand(onOpenStr, connection);
            getCode = new MySqlCommand(getCodeStr, connection);
            getNames = new MySqlCommand(getNamesStr, connection);
            setNameCode = new MySqlCommand(setNameCodeStr, connection);
            deleteProgram = new MySqlCommand(deleteProgradStr, connection);
            setNameCodeRes = new MySqlCommand(setNameCodeResStr, connection);
            updateNameCode = new MySqlCommand(updateNameCodeStr, connection);
            updateNameCodeRes = new MySqlCommand(updateNameCodeResStr, connection);

            OpenConnection();
        }

        private void OpenConnection()
        {
            try
            {
                connection.Open();
                onOpen.ExecuteReader().Close();
            }
            catch (MySqlException e)
            {
                switch (e.Number)
                {
                    case 1042:
                    case 0:
                        {
                            StartServer();

                            for (int i = 0; i < 5; i++)
                            {
                                try
                                {
                                    connection.Open();
                                    onOpen.ExecuteReader().Close();

                                    return;
                                }
                                catch (MySqlException)
                                {
                                    //Thread.Sleep(1000);
                                    continue;
                                }
                            }

                            connection.Close();

#pragma warning disable CA2200
                            throw e;
                        }
                    case 1045:
                        {
                            Console.WriteLine("Login/password incorrect");

                            if (dbproc != null)
                            {
                                foreach (Process proc in Process.GetProcessesByName(dbproc.ProcessName))
                                {
                                    proc.Kill();
                                }
                            }

                            dbproc = null;

                            throw e;
                        }
                    default:
                        {
                            throw e;
                        }
#pragma warning restore CA2200
                }
            }
        }

        public string[] GetNames()
        {
            List<string> names = [];

            MySqlDataReader reader = getNames.ExecuteReader();
            while (reader.Read())
            {
                names.Add(reader.GetString(0));
            }

            reader.Close();

            return [.. names];
        }

        public string? GetCode(string name)
        {
            getCode.Parameters.Clear();
            getCode.Parameters.Add(new MySqlParameter("@name", name));

            MySqlDataReader? reader = null;
            try
            {
                reader = getCode.ExecuteReader();
            }
            catch (MySqlException)
            {
                reader?.Close();
                return null;
            }

            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }

            reader.Read();

            string result = reader.GetString("Code");

            reader.Close();

            return result;
        }

        public bool SetProgram(string name, string code, string? result)
        {
            try
            {
                if (result != null)
                {
                    setNameCodeRes.Parameters.Clear();
                    setNameCodeRes.Parameters.Add(new MySqlParameter("@name", name));
                    setNameCodeRes.Parameters.Add(new MySqlParameter("@code", code));
                    setNameCodeRes.Parameters.Add(new MySqlParameter("@res", result));

                    setNameCodeRes.ExecuteReader().Close();
                }
                else
                {
                    setNameCode.Parameters.Clear();
                    setNameCode.Parameters.Add(new MySqlParameter("@name", name));
                    setNameCode.Parameters.Add(new MySqlParameter("@code", code));

                    setNameCode.ExecuteReader().Close();
                }

                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }

        public bool UpdateProgram(string name, string code, string? result)
        {
            try
            {
                if (result != null)
                {
                    updateNameCodeRes.Parameters.Clear();
                    updateNameCodeRes.Parameters.Add(new MySqlParameter("@name", name));
                    updateNameCodeRes.Parameters.Add(new MySqlParameter("@code", code));
                    updateNameCodeRes.Parameters.Add(new MySqlParameter("@res", result));

                    updateNameCodeRes.ExecuteReader().Close();
                }
                else
                {
                    updateNameCode.Parameters.Clear();
                    updateNameCode.Parameters.Add(new MySqlParameter("@name", name));
                    updateNameCode.Parameters.Add(new MySqlParameter("@code", code));

                    updateNameCode.ExecuteReader().Close();
                }

                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }

        public bool RemoveProgram(string name)
        {
            try
            {
                deleteProgram.Parameters.Clear();
                deleteProgram.Parameters.Add(new MySqlParameter("@name", name));
                deleteProgram.ExecuteReader().Close();

                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }

        public void StartServer()
        {
            try
            {
                dbproc = new Process();
                ProcessStartInfo info = new()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "mysql/bin/mysqld.exe"
                };
                dbproc.StartInfo = info;

                dbproc.Start();


                Thread.Sleep(500);
            }
            catch (Win32Exception)
            {

            }
        }

        public void SDServer()
        {
            connection.Close();

            if (dbproc == null)
                return;


            Process adminproc = new();
            ProcessStartInfo info = new()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "mysql/bin/mysqladmin.exe",
                Arguments = "-u root -proot shutdown"
            };
            adminproc.StartInfo = info;

            adminproc.Start();
        }
    }
}
