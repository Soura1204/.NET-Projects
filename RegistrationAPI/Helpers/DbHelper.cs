using Microsoft.Data.SqlClient;
using System.Data;
using RegistrationAPI.Models;

namespace RegistrationAPI.Repository
{
    public class DbHelper
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("Dbconnection");
        }

        // ✅ Method to get user by email
        public RegistrationModel GetUserByEmail(string email)
        {
            RegistrationModel user = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_RegistrationAPI", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETBYEMAIL");
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    user = new RegistrationModel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        FName = reader["FName"].ToString(),
                        LName = reader["LName"].ToString(),
                        Address = reader["Address"].ToString(),
                        DOB = Convert.ToDateTime(reader["DOB"]),
                        Email = reader["Email"].ToString(),
                        MobileNo = reader["MobileNo"].ToString(),
                        ImagePath = reader["ImagePath"].ToString(),
                        Password = reader["Password"].ToString()
                    };
                }

                conn.Close();
            }

            return user;
        }
    }
}
