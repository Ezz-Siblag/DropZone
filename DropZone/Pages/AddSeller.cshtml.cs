using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace DropZone.Pages
{
    public class AddSellerModel : PageModel
    {
        [BindProperty]
        public string SellerName { get; set; }

        [BindProperty]
        public string DateAdded { get; set; }

        public string Message { get; set; }

        string connString = "server=localhost;port=1108;database=DropZoneDB;user=root;password=;";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            using var conn = new MySqlConnection(connString);
            conn.Open();

            string query = "INSERT INTO Sellers (SellerName, DateAdded) VALUES (@name, @date)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", SellerName);
            cmd.Parameters.AddWithValue("@date", DateAdded);

            int result = cmd.ExecuteNonQuery();

            if (result > 0)
            {
                Message = "Seller added successfully!";
            }
            else
            {
                Message = "Error adding seller.";
            }

            return Page();
        }
    }
}