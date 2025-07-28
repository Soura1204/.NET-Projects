using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RegistrationApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace RegistrationApp.Data
{
    public class DbHelper
    {
        private readonly string _conn;

        public DbHelper(IConfiguration config)
        {
            _conn = config.GetConnectionString("Dbconnection");
        }

        // Insert new registration
        public bool SaveRegistration(RegistrationModel model, string imagePath)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            using (SqlCommand cmd = new SqlCommand("SP_Manage_Registration", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

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
                cmd.Parameters.AddWithValue("@Password", model.Password ?? (object)DBNull.Value);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Get all registrations
        public List<RegistrationModel> GetAllRegistrations()
        {
            List<RegistrationModel> list = new List<RegistrationModel>();

            using (SqlConnection con = new SqlConnection(_conn))
            using (SqlCommand cmd = new SqlCommand("SP_Manage_Registration", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
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
                            ContentType = reader["ContentType"]?.ToString(),
                            Password = reader["Password"]?.ToString()
                        });
                    }
                }
            }

            return list;
        }

        // Update registration
        public bool UpdateRegistration(RegistrationModel model, string imagePath)
        {
            using (SqlConnection conn = new SqlConnection(_conn))
            using (SqlCommand cmd = new SqlCommand("SP_Manage_Registration", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

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
                cmd.Parameters.AddWithValue("@Password", model.Password ?? (object)DBNull.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Search by prefix
        public List<RegistrationModel> SearchCandidatesByPrefix(string prefix)
        {
            List<RegistrationModel> results = new List<RegistrationModel>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Manage_Registration", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Action", "SEARCH_PREFIX");
                    cmd.Parameters.AddWithValue("@FName", prefix);
                    cmd.Parameters.AddWithValue("@LName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", DBNull.Value);
                    cmd.Parameters.AddWithValue("@DOB", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath", DBNull.Value);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new RegistrationModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                FName = reader["FName"].ToString(),
                                LName = reader["LName"].ToString(),
                                Address = reader["Address"] == DBNull.Value ? "" : reader["Address"].ToString(),
                                DOB = reader["DOB"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DOB"]),
                                Email = reader["Email"] == DBNull.Value ? "" : reader["Email"].ToString(),
                                MobileNo = reader["MobileNo"] == DBNull.Value ? "" : reader["MobileNo"].ToString(),
                                ImagePath = reader["ImagePath"] == DBNull.Value ? "" : reader["ImagePath"].ToString()
                            });
                        }
                    }
                }
            }
            return results;
        }

        // Get full details by ID
        public CandidateFullDetailsModel GetFullCandidateById(int id)
        {
            var reg = GetAllRegistrations().FirstOrDefault(c => c.Id == id);
            if (reg == null) return null;

            CandidateFullDetailsModel full = new CandidateFullDetailsModel
            {
                Registration = reg,
                CandidateDetails = new CandidateDetailsModel
                {
                    Details_ID = null,
                    Aadhar_No = "",
                    Pan_No = "",
                    Gender = "",
                    Highest_Qualification = "",
                    Company_Name = "",
                    Dept = "",
                    Post = "",
                    Mode = ""
                }
            };

            using (SqlConnection con = new SqlConnection(_conn))
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
                        full.CandidateDetails.Details_ID = Convert.ToInt32(reader["Details_ID"]);
                        full.CandidateDetails.Aadhar_No = reader["Aadhar_No"].ToString();
                        full.CandidateDetails.Pan_No = reader["Pan_No"].ToString();
                        full.CandidateDetails.Gender = reader["Gender"].ToString();
                        full.CandidateDetails.Highest_Qualification = reader["Highest_Qualification"].ToString();
                        full.CandidateDetails.Company_Name = reader["Company_Name"].ToString();
                        full.CandidateDetails.Dept = reader["Dept"].ToString();
                        full.CandidateDetails.Post = reader["Post"].ToString();
                        full.CandidateDetails.Mode = reader["Mode"].ToString();
                    }
                }
            }

            return full;
        }

        // Update or insert candidate details
        public bool UpdateCandidateFullDetails(CandidateFullDetailsModel model)
        {
            string action = model.CandidateDetails?.Details_ID.HasValue == true ? "UPDATE" : "INSERT";

            using (SqlConnection con = new SqlConnection(_conn))
            using (SqlCommand cmd = new SqlCommand("SP_Manage_CandidateDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Candt_ID", model.Registration.Id);
                cmd.Parameters.AddWithValue("@Details_ID", (object?)model.CandidateDetails?.Details_ID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Aadhar_No", (object?)model.CandidateDetails?.Aadhar_No ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Pan_No", (object?)model.CandidateDetails?.Pan_No ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Gender", (object?)model.CandidateDetails?.Gender ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Highest_Qualification", (object?)model.CandidateDetails?.Highest_Qualification ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Company_Name", (object?)model.CandidateDetails?.Company_Name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Dept", (object?)model.CandidateDetails?.Dept ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Post", (object?)model.CandidateDetails?.Post ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Mode", (object?)model.CandidateDetails?.Mode ?? DBNull.Value);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public RegistrationModel Authenticate(string email, string hashedPassword)
        {
            RegistrationModel user = null;
            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Manage_Registration", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "AUTHENTICATE");
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        user = new RegistrationModel
                        {
                            Id = Convert.ToInt32(rdr["Id"]),
                            FName = rdr["FName"].ToString(),
                            Email = rdr["Email"].ToString(),
                            // Add more fields if needed
                        };
                    }
                }
            }
            return user;
        }
    }
}
