using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static System.Net.Mime.MediaTypeNames;
using System.Configuration;


namespace Registration_Form
{
    public partial class Candidate_Registration : System.Web.UI.Page
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRecord();
                btnsubmit.Visible = true;
                btnsubmit1.Visible = false;

            }
        }
        void clear()
        {
                txtFirstName.Text = txtLastName.Text = txtAddress.Text = txtDOB.Text = txtEmail.Text = txtMobile.Text = "";
                hdnID.Value = "";
                txtEmail.Enabled = true;
                btnsubmit.Visible = true;
                btnsubmit1.Visible = false;              
            }

        

        protected void btnsubmit_Click(object sender, EventArgs e)
        {
            DateTime dob;
            if (!DateTime.TryParse(txtDOB.Text, out dob))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert('Invalid DOB format');", true);
                return;
            }

            SqlCommand cmd = new SqlCommand("SP_candidate_registration", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", DBNull.Value);
            cmd.Parameters.AddWithValue("@fname", txtFirstName.Text.Trim());
            cmd.Parameters.AddWithValue("@lname", txtLastName.Text.Trim());
            cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
            cmd.Parameters.AddWithValue("@dob", dob);
            cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
            cmd.Parameters.AddWithValue("@mobnum", txtMobile.Text.Trim());
            cmd.Parameters.AddWithValue("@mode", "Insert");

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert('Successfully Inserted');", true);
            LoadRecord();
            clear();
        }
        void LoadRecord()
        {
            SqlCommand cd = new SqlCommand("Select * from candidate_registration", con);
            SqlDataAdapter d = new SqlDataAdapter(cd);
            DataTable dt = new DataTable();
            d.Fill(dt);
            dt.Columns["id"].SetOrdinal(0);
            grdCanDtls.DataSource = dt;
            grdCanDtls.DataBind();          
        }

        protected void btnsubmit1_Click(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(hdnID.Value, out id))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert('Invalid ID');", true);
                return;
            }

            DateTime dob;
            if (!DateTime.TryParse(txtDOB.Text, out dob))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert('Invalid DOB format');", true);
                return;
            }

            SqlCommand cmd = new SqlCommand("SP_candidate_registration", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fname", txtFirstName.Text.Trim());
            cmd.Parameters.AddWithValue("@lname", txtLastName.Text.Trim());
            cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
            cmd.Parameters.AddWithValue("@dob", dob);
            cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
            cmd.Parameters.AddWithValue("@mobnum", txtMobile.Text.Trim());
            cmd.Parameters.AddWithValue("@mode", "Update");

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert('Successfully Updated');", true);
            LoadRecord();
            clear();

        }

        //protected void btnsubmit3_Click(object sender, EventArgs e)
        //{
        //    SqlCommand cd = new SqlCommand("Select * from candidate_registration where email = '" + txtEmail.Text + "'", con);
        //    SqlDataAdapter d = new SqlDataAdapter(cd);
        //    DataTable dt = new DataTable();
        //    d.Fill(dt);
        //    grdCanDtls.DataSource = dt;
        //    grdCanDtls.DataBind();
        //    clear();
        //}


        protected void grdCanDtls_RowEditing(object sender, GridViewEditEventArgs e)
        {
            grdCanDtls.EditIndex = e.NewEditIndex;
            LoadRecord();
        }


        protected void grdCanDtls_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            grdCanDtls.EditIndex = -1;
            LoadRecord();
        }
        protected void Edit_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            int rowIndex = row.RowIndex;
            int id = Convert.ToInt32(grdCanDtls.DataKeys[rowIndex].Value);

            SqlCommand cmd = new SqlCommand("SELECT * FROM candidate_registration WHERE id = @id", con);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            SqlDataReader r = cmd.ExecuteReader();
            if (r.Read())
            {
                hdnID.Value = id.ToString();
                txtFirstName.Text = r["fname"].ToString();
                txtLastName.Text = r["lname"].ToString();
                txtAddress.Text = r["address"].ToString();
                txtDOB.Text = Convert.ToDateTime(r["dob"]).ToString("yyyy-MM-dd");
                txtEmail.Text = r["email"].ToString();
                txtMobile.Text = r["mobnum"].ToString();
            }
            con.Close();
            btnsubmit.Visible = false;
            btnsubmit1.Visible = true;
        }

        //protected void Cancel_Click(object sender, EventArgs e)
        //{
        //    clear(); // Clear the top form
        //}


        protected void grdCanDtls_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // Get the ID of the row being edited
            int id = Convert.ToInt32(grdCanDtls.DataKeys[e.RowIndex].Value);
            GridViewRow row = grdCanDtls.Rows[e.RowIndex];

            // Get updated values from the TextBoxes inside the row
            string fname = ((TextBox)row.FindControl("txtEditFName")).Text.Trim();
            string lname = ((TextBox)row.FindControl("txtEditLName")).Text.Trim();
            string address = ((TextBox)row.FindControl("txtEditAddress")).Text.Trim();
            string dobInput = ((TextBox)row.FindControl("txtEditDOB")).Text.Trim();
            string email = ((TextBox)row.FindControl("txtEditEmail")).Text.Trim();
            string mobnum = ((TextBox)row.FindControl("txtEditMobile")).Text.Trim();

            // Convert DOB string to DateTime
            DateTime dob;
            if (!DateTime.TryParseExact(dobInput, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out dob))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert('Invalid DOB format.');", true);
                return;
            }

            // Create and configure the SQL command
            SqlCommand cmd = new SqlCommand("SP_candidate_registration", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // Pass parameters to the stored procedure
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fname", fname);
            cmd.Parameters.AddWithValue("@lname", lname);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@dob", dob);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@mobnum", mobnum);
            cmd.Parameters.AddWithValue("@mode", "Update"); // Set the operation mode

            // Execute the command
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            // Reset edit mode and reload the updated data
            grdCanDtls.EditIndex = -1;
            LoadRecord();

            // Notify the user
            ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert('Updated Successfully');", true);

        }

        protected void grdCanDtls_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(grdCanDtls.DataKeys[e.RowIndex].Value);

            SqlCommand cmd = new SqlCommand("SP_candidate_registration", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fname", DBNull.Value);
            cmd.Parameters.AddWithValue("@lname", DBNull.Value);
            cmd.Parameters.AddWithValue("@address", DBNull.Value);
            cmd.Parameters.AddWithValue("@dob", DBNull.Value);
            cmd.Parameters.AddWithValue("@email", DBNull.Value);
            cmd.Parameters.AddWithValue("@mobnum", DBNull.Value);
            cmd.Parameters.AddWithValue("@mode", "Delete");

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert('Deleted Successfully');", true);
            LoadRecord();
        }
    }
}