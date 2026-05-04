using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace DropZone.Pages
{
    public class AddItemModel : PageModel
    {
        [BindProperty]
        public Item Item { get; set; } = new();

        public string? Message { get; set; }

        string conn = "server=localhost;port=1108;database=DropZoneDB;user=root;password=;";

        public void OnGet(int sellerId)
        {
            Item.SellerID = sellerId;
        }

        public IActionResult OnPost()
        {
            using var c = new MySqlConnection(conn);
            c.Open();

            var cmd = new MySqlCommand(@"
                INSERT INTO Items
                (SellerID, BuyerName, DateDropped, price, holdingFee, itemStatus, placed, datePickedUp)
                VALUES
                (@sid, @b, @d, @p, @h, 'InDA', @pl, @dp)", c);

            cmd.Parameters.AddWithValue("@sid", Item.SellerID);
            cmd.Parameters.AddWithValue("@b", Item.BuyerName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@d", Item.DateDropped ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p", Item.Price);
            cmd.Parameters.AddWithValue("@h", Item.HoldingFee);
            cmd.Parameters.AddWithValue("@pl", Item.Placed ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@dp", Item.DatePickedUp ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();

            Message = "Item added successfully (status = InDA)";

            return Page();
        }
    }
}