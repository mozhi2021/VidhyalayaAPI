using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json;
using VidhyalayaAPI.Common;
using VidhyalayaAPI.Models;


namespace VidhyalayaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly string _connectionString;
        private readonly SqlConnection sqluserName;

        public LoginController(IOptions<ConnectionConfigDTO> connectionOptions)
        {
            _connectionString = connectionOptions.Value.DefaultConnection;
        }

        [Route("UpdatePassword")]
        [HttpPost]
        public IActionResult UpdatePassword(string UserName, string EmailAddress)
        {
            string status = string.Empty;
            string newPassword = string.Empty;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                //SqlCommand cmd = new SqlCommand();
                //var connection = new SqlConnection(_connectionString);

                //cmd.Connection = connection;

                con.Open();
                using (SqlCommand cmd = new SqlCommand("VDA_Login_Update_SetUserPassword", con))
                {
                    //command.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;

                    cmd.Parameters.Add("@EmailAddress", SqlDbType.VarChar).Value = EmailAddress;

                    SqlParameter parameter = new SqlParameter("@NewPassword", SqlDbType.VarChar, 25);
                    parameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(parameter);

                    cmd.ExecuteNonQuery();
                    newPassword = Convert.ToString(parameter.Value);
                    //Util.SendMail();

                }
                con.Close();
            }
           
            return Ok(newPassword);
        }


        [Route ("ValidateUser")]
        [HttpPost]
        public IActionResult ValidateUser(UserRequestDTO user)
        {

            var response = new UserResponseDTO();            

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter("VDA_Login_SELECT_UserAuthentication", con);

                da.SelectCommand.CommandType = CommandType.StoredProcedure;                

                //Parameters
                da.SelectCommand.Parameters.AddWithValue("@UserName", SqlDbType.VarChar).Value = user.UserName;
                da.SelectCommand.Parameters.AddWithValue("@Password", SqlDbType.VarChar).Value = user.Password;
                da.SelectCommand.Parameters.AddWithValue("@IPAddress", SqlDbType.VarChar).Value = user.IPAddress;
                da.SelectCommand.Parameters.AddWithValue("@Country", SqlDbType.VarChar).Value = user.Country;
                SqlParameter parameter = new SqlParameter("@Msg", SqlDbType.VarChar,50);
                parameter.Direction = ParameterDirection.Output;
                da.SelectCommand.Parameters.Add(parameter);

                // Retrieving Record from datasource  
                DataSet ds = new DataSet();
                da.Fill(ds);
                response.Msg = Convert.ToString(parameter.Value);                

                if (response.Msg == "Success")
                {
                    response.PersonID = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"]);
                    response.PersonFullName = Convert.ToString(ds.Tables[0].Rows[0]["PersonFullName"]);
                    response.LogID = Convert.ToInt32(ds.Tables[1].Rows[0]["LogID"]);                     

                }
                con.Close();
            }
            var opt = new JsonSerializerOptions() { WriteIndented = true };
            var status = JsonSerializer.Serialize<UserResponseDTO>(response, opt);

            return Ok(status);
        }


        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LoginController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}



