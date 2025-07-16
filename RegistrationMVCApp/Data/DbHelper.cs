using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RegistrationApp.Models;
using System;
using System.Data;
using System.IO;

namespace RegistrationApp.Data
{
    public class DbHelper
    {
        private readonly string _conn;

        public DbHelper(IConfiguration config)
        {
            _conn = config.GetConnectionString("Dbconnection");
        }
        public bool SaveRegistration(RegistrationModel model, string imagePath)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Manage_Registration", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Action", "INSERT");
                    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
                    cmd.Parameters.AddWithValue("@FName", model.FName);
                    cmd.Parameters.AddWithValue("@LName", model.LName);
                    cmd.Parameters.AddWithValue("@Address", model.Address);
                    cmd.Parameters.AddWithValue("@DOB", model.DOB ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@MobileNo", model.MobileNo);
                    cmd.Parameters.AddWithValue("@ImagePath", (object)imagePath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContentType", (object)model.ContentType ?? DBNull.Value);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;  // is a boolean condition that checks whether at least one row was affected by the SQL command
                }
            }
        }

        public List<RegistrationModel> GetAllRegistrations()
        {
            List<RegistrationModel> list = new List<RegistrationModel>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Manage_Registration", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "SELECT");
                    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
                    cmd.Parameters.AddWithValue("@FName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@LName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", DBNull.Value);
                    cmd.Parameters.AddWithValue("@DOB", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContentType", DBNull.Value);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RegistrationModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                FName = reader["FName"].ToString(),
                                LName = reader["LName"].ToString(),
                                Address = reader["Address"].ToString(),
                                DOB = reader["DOB"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DOB"]),
                                Email = reader["Email"].ToString(),
                                MobileNo = reader["MobileNo"].ToString(),
                                ImagePath = reader["ImagePath"]?.ToString(),
                                ContentType = reader["ContentType"]?.ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }

        public bool UpdateRegistration(RegistrationModel model, string imagePath)
        {
            bool result = false;

            using (SqlConnection conn = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Manage_Registration", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Action", "UPDATE");
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@FName", model.FName);
                    cmd.Parameters.AddWithValue("@LName", model.LName);
                    cmd.Parameters.AddWithValue("@Address", model.Address);
                    cmd.Parameters.AddWithValue("@DOB", model.DOB ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@MobileNo", model.MobileNo);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(imagePath) ? (object)DBNull.Value : imagePath);
                    cmd.Parameters.AddWithValue("@ContentType", string.IsNullOrEmpty(model.ContentType) ? (object)DBNull.Value : model.ContentType);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    result = rowsAffected > 0;
                }
            }

            return result;
        }

        public List<RegistrationModel> SearchCandidatesByPrefix(string prefix)
        {
            List<RegistrationModel> results = new List<RegistrationModel>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Manage_Registration", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Action", "SEARCH_PREFIX");
                    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
                    cmd.Parameters.AddWithValue("@FName", prefix);
                    cmd.Parameters.AddWithValue("@LName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", DBNull.Value);
                    cmd.Parameters.AddWithValue("@DOB", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContentType", DBNull.Value);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new RegistrationModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                FName = reader["FName"].ToString(),
                                LName = reader["LName"].ToString()
                            });
                        }
                    }
                }
            }

            return results;
        }

        public CandidateFullDetailsModel GetFullCandidateById(int id)
        {
            var reg = GetAllRegistrations().FirstOrDefault(c => c.Id == id);
            if (reg == null) return null;

            CandidateFullDetailsModel full = new CandidateFullDetailsModel
            {
                Registration = reg,
                Details_ID = null, // Default to null — might remain unchanged
                Aadhar_No = "",
                Pan_No = "",
                Gender = "",
                Highest_Qualification = "",
                Company_Name = "",
                Dept = "",
                Post = "",
                Mode = ""
            };

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Manage_CandidateDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "SELECT");
                    cmd.Parameters.AddWithValue("@Candt_ID", id);
                    cmd.Parameters.AddWithValue("@Details_ID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Aadhar_No", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pan_No", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Highest_Qualification", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Name", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dept", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Post", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mode", DBNull.Value);

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            full.Details_ID = Convert.ToInt32(reader["Details_ID"]);
                            full.Aadhar_No = reader["Aadhar_No"].ToString();
                            full.Pan_No = reader["Pan_No"].ToString();
                            full.Gender = reader["Gender"].ToString();
                            full.Highest_Qualification = reader["Highest_Qualification"].ToString();
                            full.Company_Name = reader["Company_Name"].ToString();
                            full.Dept = reader["Dept"].ToString();
                            full.Post = reader["Post"].ToString();
                            full.Mode = reader["Mode"].ToString();
                        }
                    }
                }
            }

            return full;
        }

        public bool UpdateCandidateFullDetails(CandidateFullDetailsModel model)
        {
            string action = model.Details_ID.HasValue ? "UPDATE" : "INSERT";

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Manage_CandidateDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@Candt_ID", model.Registration.Id);
                    cmd.Parameters.AddWithValue("@Details_ID", model.Details_ID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Aadhar_No", model.Aadhar_No ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pan_No", model.Pan_No ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", model.Gender ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Highest_Qualification", model.Highest_Qualification ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Name", model.Company_Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dept", model.Dept ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Post", model.Post ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mode", model.Mode ?? (object)DBNull.Value);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}