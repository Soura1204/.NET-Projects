using RegistrationAPI.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace RegistrationAPI.Repository
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly string _conn;

        public RegistrationRepository(IConfiguration config)
        {
            _conn = config.GetConnectionString("Dbconnection");
        }

        // --- Get all records ---
        public IEnumerable<RegistrationModel> GetAll()
        {
            var list = new List<RegistrationModel>();
            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("SP_RegistrationAPI", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GETALL");

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(MapData(dr));
            }
            return list;
        }

        // --- Get record by Id ---
        public RegistrationModel GetById(int id)
        {
            RegistrationModel model = null;
            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("SP_RegistrationAPI", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GETBYID");
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                model = MapData(dr);
            }
            return model;
        }

        // --- Insert ---
        public bool Insert(RegistrationModel model)
        {
            return ExecuteNonQuery(model, "INSERT");
        }

        // --- Update ---
        public bool Update(RegistrationModel model)
        {
            return ExecuteNonQuery(model, "UPDATE");
        }

        // --- Delete ---
        public bool Delete(int id)
        {
            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("SP_RegistrationAPI", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "DELETE");
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // --- Search by prefix ---
        public IEnumerable<RegistrationModel> SearchByPrefix(string term)
        {
            List<RegistrationModel> results = new List<RegistrationModel>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Manage_Registration", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "SEARCH_PREFIX");
                    cmd.Parameters.AddWithValue("@FName", term ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@LName", DBNull.Value);
                    //cmd.Parameters.AddWithValue("@Address", DBNull.Value);
                    //cmd.Parameters.AddWithValue("@DOB", DBNull.Value);
                    //cmd.Parameters.AddWithValue("@Email", DBNull.Value);
                    //cmd.Parameters.AddWithValue("@MobileNo", DBNull.Value);
                    //cmd.Parameters.AddWithValue("@ImagePath", DBNull.Value);

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
                                Address = reader["Address"] == DBNull.Value ? null : reader["Address"].ToString(),
                                DOB = reader["DOB"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DOB"]),
                                Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                                MobileNo = reader["MobileNo"] == DBNull.Value ? null : reader["MobileNo"].ToString(),
                                ImagePath = reader["ImagePath"] == DBNull.Value ? null : reader["ImagePath"].ToString()
                            });
                        }
                    }
                }
            }

            return results;
        }


        // --- Get full details ---
        public CandidateFullDetailsModel GetFullDetails(int id)
        {
            var reg = GetById(id);
            if (reg == null) return null;

            CandidateFullDetailsModel full = new CandidateFullDetailsModel
            {
                Registration = reg,
                CandidateDetails = null
            };

            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("SP_Manage_CandidateDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SELECT");
            cmd.Parameters.AddWithValue("@Candt_ID", id);

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                full.CandidateDetails = new CandidateDetailsModel
                {
                    Details_ID = dr["Details_ID"] == DBNull.Value ? null : Convert.ToInt32(dr["Details_ID"]),
                    Candt_ID = Convert.ToInt32(dr["Candt_ID"]),
                    Aadhar_No = dr["Aadhar_No"].ToString(),
                    Pan_No = dr["Pan_No"].ToString(),
                    Gender = dr["Gender"].ToString(),
                    Highest_Qualification = dr["Highest_Qualification"].ToString(),
                    Company_Name = dr["Company_Name"].ToString(),
                    Dept = dr["Dept"].ToString(),
                    Post = dr["Post"].ToString(),
                    Mode = dr["Mode"].ToString()
                };
            }
            return full;
        }

        // --- Update full details ---
        public bool UpdateFullDetails(CandidateFullDetailsModel model)
        {
            string action = model.CandidateDetails?.Details_ID.HasValue == true ? "UPDATE" : "INSERT";

            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("SP_Manage_CandidateDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@Details_ID", (object?)model.CandidateDetails?.Details_ID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Candt_ID", model.Registration.Id);
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

        // --- Helper: Map data ---
        private RegistrationModel MapData(SqlDataReader dr)
        {
            return new RegistrationModel
            {
                Id = Convert.ToInt32(dr["Id"]),
                FName = dr["FName"].ToString(),
                LName = dr["LName"].ToString(),
                Address = dr["Address"].ToString(),
                DOB = dr["DOB"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["DOB"]),
                Email = dr["Email"].ToString(),
                MobileNo = dr["MobileNo"].ToString(),
                ImagePath = dr["ImagePath"]?.ToString()
            };
        }

        private bool ExecuteNonQuery(RegistrationModel model, string action)
        {
            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("SP_RegistrationAPI", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@FName", model.FName);
            cmd.Parameters.AddWithValue("@LName", model.LName);
            cmd.Parameters.AddWithValue("@Address", model.Address);
            cmd.Parameters.AddWithValue("@DOB", model.DOB ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@MobileNo", model.MobileNo);
            cmd.Parameters.AddWithValue("@ImagePath", model.ImagePath ?? "");

            // ✅ Add this line:
            cmd.Parameters.AddWithValue("@Password", model.Password ?? "");

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
        public RegistrationModel GetUserByEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                SqlCommand cmd = new SqlCommand("SP_RegistrationAPI", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "GETBYEMAIL");
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new RegistrationModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            FName = reader["FName"].ToString(),
                            LName = reader["LName"].ToString(),
                            Email = reader["Email"].ToString(),
                            Password = reader["Password"].ToString()
                        };
                    }
                }
            }
            return null;
        }
    }
}
