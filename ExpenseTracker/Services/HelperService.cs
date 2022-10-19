using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace ExpenseTracker.Services
{
    public interface IHelperService
    {
        string DbQuery(string query);
    }

    public class HelperService : IHelperService
    {
        public IConfiguration config { get; set; } = null!;
        public HelperService(IConfiguration config)
        {
            this.config = config;
        }

        public string DbQuery(string query)
        {
            var connstring = config.GetConnectionString("DefaultConnection");
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connstring))
            {
                SqlCommand objSqlCommand = new SqlCommand(query, con);
                objSqlCommand.CommandType = CommandType.Text;
                SqlDataAdapter objSqlDataAdapter = new SqlDataAdapter(objSqlCommand);
                try
                {
                    objSqlDataAdapter.Fill(dt);
                    string jsonresult = JsonConvert.SerializeObject(dt);
                    return jsonresult;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
