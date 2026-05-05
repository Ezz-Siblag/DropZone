using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace DropZone.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; }

        string conn = "server=localhost;port=1108;database=DropZoneDB;user=root;password=;";

        public IActionResult OnGet()
        {
            // FIXED: consistent session key
            if (HttpContext.Session.GetString("User") != null)
            {
                return RedirectToPage("/SellerList");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            using var c = new MySqlConnection(conn);
            c.Open();

            var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM Users WHERE Username=@u AND Password=@p", c);

            cmd.Parameters.AddWithValue("@u", Username);
            cmd.Parameters.AddWithValue("@p", Password);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count == 1)
            {
                HttpContext.Session.SetString("User", Username);

                return RedirectToPage("/Index");
            }

            Message = "Invalid username or password";
            return Page();
        }
    }
}