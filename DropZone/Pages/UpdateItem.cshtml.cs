using DropZone.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace DropZone.Pages
{
    public class UpdateItemModel : PageModel
    {
        [BindProperty]
        public Item Item { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public string? Message { get; set; }

        string conn = "server=localhost;port=1108;database=DropZoneDB;user=root;password=;";

        public void OnGet()
        {
            using var c = new MySqlConnection(conn);
            c.Open();

            var cmd = new MySqlCommand("SELECT * FROM Items WHERE ItemID=@id", c);
            cmd.Parameters.AddWithValue("@id", Id);

            using var r = cmd.ExecuteReader();

            if (r.Read())
            {
                Item = new Item
                {
                    ItemID = r.GetInt32("ItemID"),
                    SellerID = r.GetInt32("SellerID"),
                    BuyerName = r["BuyerName"]?.ToString(),
                    DateDropped = r["DateDropped"]?.ToString(),
                    Price = r.GetDecimal("price"),
                    HoldingFee = r.GetInt32("holdingFee"),
                    ItemStatus = r["itemStatus"]?.ToString(),
                    Placed = r["placed"]?.ToString(),
                    DatePickedUp = r["datePickedUp"]?.ToString()
                };
            }
        }

        public IActionResult OnPost()
        {
            using var c = new MySqlConnection(conn);
            c.Open();

            var cmd = new MySqlCommand(@"
                UPDATE Items SET
                BuyerName=@b,
                DateDropped=@d,
                price=@p,
                holdingFee=@h,
                itemStatus=@s,
                placed=@pl,
                datePickedUp=@dp
                WHERE ItemID=@id", c);

            cmd.Parameters.AddWithValue("@b", Item.BuyerName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@d", Item.DateDropped ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p", Item.Price);
            cmd.Parameters.AddWithValue("@h", Item.HoldingFee);
            cmd.Parameters.AddWithValue("@s", Item.ItemStatus ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@pl", Item.Placed ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@dp", Item.DatePickedUp ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@id", Item.ItemID);

            cmd.ExecuteNonQuery();

            Message = "Updated successfully";
            Id = Item.ItemID;
            OnGet();

            return Page();
        }
    }
}