using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WEBAPI.Domain.Entities;
using BCrypt.Net;

namespace WEBAPI.Infrastructure.DB
{
    public class ApplicationDbContext

    {
        public string ConnectionString { get; set; }

        public ApplicationDbContext(string connectionstr)
        {
            ConnectionString = connectionstr;

        }


        public List<User> GetUsers()
        {
            List<User> _userDetails = new List<User>();
            string connectionString = ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM Users"; // Replace YourTableName with your actual table name

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                Id = (int)reader["Id"],               
                                Username = reader["Username"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                Email = reader["Email"].ToString(),
                          
                            };

                            _userDetails.Add(user);
                        }
                    }
                }
            }
            return _userDetails;
        }
        //EOM GetUsers
        public string RegisterUser(User user)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Users (Email, Username, PasswordHash) " +
                             "VALUES (@Email, @Username, @PasswordHash)";

                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                    command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = user.Email });
                    command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = user.Username });
                    command.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar) { Value = hashedPassword });

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return "User registered successfully";
                    }
                    else
                    {
                        return "User registration failed";
                    }
                }
            }
        }
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name.ToLower() == column.ColumnName.ToLower())
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }


        //User Authentication and Authorization//


        //public static string sqlDataSource = "Server=.;Database=WEBAPIAJAXPROJ;TrustServerCertificate=True;Integrated Security=true; MultipleActiveResultSets=true";

        public DataTable GetData(string str, params IDataParameter[] parameters)
        {
            DataTable objresutl = new DataTable();

            try
            {
                using (SqlConnection myCon = new SqlConnection(ConnectionString))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(str, myCon))
                    {
                        if (parameters != null)
                        {
                            myCommand.Parameters.AddRange(parameters);
                        }

                        using (SqlDataReader myReader = myCommand.ExecuteReader())
                        {
                            objresutl.Load(myReader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
            }

            return objresutl;
        }

        public List<T> ConvertDataTableToList<T>(DataTable dt) where T : new()
        {
            List<T> data = new List<T>();

            foreach (DataRow row in dt.Rows)
            {
                T item = new T();

                foreach (DataColumn column in dt.Columns)
                {
                    PropertyInfo prop = typeof(T).GetProperty(column.ColumnName);
                    if (prop != null && row[column] != DBNull.Value)
                    {
                        prop.SetValue(item, row[column], null);
                    }
                }

                data.Add(item);
            }

            return data;
        }
        public int ExecuteData(string str, params IDataParameter[] sqlParams)
        {
            int rows = -1;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(str, conn))
                    {
                        if (sqlParams != null)
                        {
                            foreach (IDataParameter para in sqlParams)
                            {
                                cmd.Parameters.Add(para);
                            }
                            rows = cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }


            return rows;


        }

        public int ExecuteStoredProcedure(string procedureName, params IDataParameter[] sqlParams)
        {
            int rows = -1;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(procedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (sqlParams != null)
                        {
                            cmd.Parameters.AddRange(sqlParams);
                        }

                        rows = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }

            return rows;
        }
    }
}
