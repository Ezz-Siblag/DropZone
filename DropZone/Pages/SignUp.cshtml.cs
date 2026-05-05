using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace DropZone.Pages
{
    public class SignUpModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; }
        public string Error { get; set; }

        string conn = "server=localhost;port=1108;database=DropZoneDB;user=root;password=;";

        public IActionResult OnPost()
        {
            using var c = new MySqlConnection(conn);
            c.Open();

            // check if username exists
            var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@u", c);
            checkCmd.Parameters.AddWithValue("@u", Username);

            var exists = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (exists > 0)
            {
                Error = "Username already exists.";
                return Page();
            }

            // insert new user
            var cmd = new MySqlCommand(
                "INSERT INTO Users (Username, Password) VALUES (@u, @p)", c);

            cmd.Parameters.AddWithValue("@u", Username);
            cmd.Parameters.AddWithValue("@p", Password);

            cmd.ExecuteNonQuery();

            Message = "Account created successfully! You can now login.";

            Username = "";
            Password = "";

            return Page();
        }
    }
}