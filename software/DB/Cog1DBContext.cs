using cog1;
using cog1.Literals;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;

namespace Cog1.DB
{

    public class Cog1DbContext : IDisposable
    {
        //private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private SqliteConnection conn = null;
        private SqliteTransaction tran = null;
        //private bool inSemaphore = true;
        public IDbTransaction Transaction { get => GetTransaction(); }

        public static Cog1DbContext CreateInstance()
        {
            //semaphore.Wait();
            try
            {
                return new Cog1DbContext();
            }
            catch
            {
                //semaphore.Release();
                throw;
            }
            
        }

        #region Private

        private Cog1DbContext()
        {

        }

        private void EnsureConnection()
        {
            if (conn == null)
            {
                try
                {
                    var sb = new SqliteConnectionStringBuilder()
                    {
                        DataSource = Path.Combine(Global.DataDirectory, "cog1.sqlite"),
                        Pooling = true,
                        
                    };
                    conn = new SqliteConnection(sb.ConnectionString);
                    conn.Open();

                    // Use "EXTRA" synchronous mode (https://www.sqlite.org/pragma.html)
                    using (var cmd = conn.CreateCommand())
                    {

                        cmd.CommandText = "PRAGMA synchronous = 3";
                        cmd.ExecuteNonQuery();
                    }

                    // Open a transaction
                    tran = conn.BeginTransaction();
                }
                catch
                {
                    if (tran != null)
                        tran.Dispose();
                    if (conn != null)
                        conn.Dispose();
                    tran = null;
                    conn = null;
                    throw;
                }
            }
        }

        private SqliteTransaction GetTransaction()
        {
            EnsureConnection();
            if (tran == null)
                throw new Exception("This database context has already been commited or rolled back. Re-use is not permitted.");
            return tran;
        }

        private SqliteConnection GetConnection()
        {
            EnsureConnection();
            return conn;
        }

        private SqliteCommand CreateCommand(string sql, Dictionary<string, object> parameters)
        {
            var result = new SqliteCommand(sql, GetConnection(), GetTransaction());
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var p in parameters)
                    result.Parameters.AddWithValue(p.Key, p.Value);
            }
            return result;
        }

        void IDisposable.Dispose()
        {
            if (tran != null)
            {
                tran.Rollback();
                tran.Dispose();
                tran = null;
            }
            if (conn != null)
            {
                conn.Close();
                conn.Dispose();
                conn = null;
            }
            //if (inSemaphore)
            //{
            //    semaphore.Release();
            //    inSemaphore = false;
            //}
        }

        #endregion

        #region Transaction management

        public void Commit()
        {
            if (tran == null)
                return;
            tran.Commit();
            tran = null;       // This context cannot be used again after committing.
        }

        #endregion

        #region Data retrieval

        public DataTable GetDataTable(string sql)
        {
            return GetDataTable(sql, null);
        }

        public DataTable GetDataTable(string sql, Dictionary<string, object> parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    var result = new DataTable();
                    result.BeginLoadData();
                    result.Load(reader);
                    result.EndLoadData();
                    return result;
                }
            }
        }

        public string GetString(string sql)
        {
            return GetString(sql, null);
        }

        public string GetString(string sql, Dictionary<string, object> parameters)
        {
            var t = GetDataTable(sql, parameters);
            if (t.Rows.Count > 0)
                return t.Rows[0].Field<string>(0);
            return null;
        }

        public int? GetInt(string sql)
        {
            return GetInt(sql, null);
        }

        public int? GetInt(string sql, Dictionary<string, object> parameters)
        {
            var t = GetDataTable(sql, parameters);
            if (t.Rows.Count > 0)
                return t.Rows[0].Field<int>(0);
            return null;
        }

        public long? GetLong(string sql)
        {
            return GetLong(sql, null);
        }

        public long? GetLong(string sql, Dictionary<string, object> parameters)
        {
            var t = GetDataTable(sql, parameters);
            if (t.Rows.Count > 0)
                return t.Rows[0].Field<long>(0);
            return null;
        }

        #endregion

        #region Statement execution

        public int Execute(string sql)
        {
            return Execute(sql, null);
        }

        public int Execute(string sql, Dictionary<string, object> parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Database initialization

        public static void InitializeDatabase()
        {
            using (var ctx = CreateInstance())
            {
                // Users table
                try
                {
                    ctx.GetLong("select 1 from users");
                }
                catch
                {
                    Console.Write("Missing users table. Creating table... ");
                    ctx.Execute(
                        @"create table users 
                        (
                            user_id integer not null primary key,
                            user_name text not null,
                            password text not null,
                            locale_code text not null,
                            is_admin int not null
                        );
                    ");
                    Console.WriteLine("Done");

                    Console.Write("Adding default users... ");
                    ctx.Execute(
                        @"insert into 
                        users 
                            (user_id, user_name, password, locale_code, is_admin) 
                        values 
                            ('1', 'admin', @password, @locale_code, 1)
                        ",
                            new()
                            {
                            { "@password", Utils.HashPassword(1, "cog1pass") },
                            { "@locale_code", Locales.English.LocaleCode },
                    });
                    Console.WriteLine("Done");
                }

                // Variables table
                try
                {
                    ctx.GetLong("select 1 from variables");
                }
                catch
                {
                    Console.Write("Missing variables table. Creating table... ");
                    ctx.Execute(
                        @"create table variables
                        (
                            variable_id integer not null primary key,
                            description text not null,
                            variable_code text null,
                            variable_type integer not null,
                            variable_direction integer not null,
                            units text null,
                            value real null,
                            utc_last_updated text null
                        );
                    ");
                    Console.WriteLine("Done");
                }

                // Done
                ctx.Commit();
            }
        }

        #endregion

    }
}
