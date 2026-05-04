using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace DropZone.Pages
{
    public class AllItemsModel : PageModel
    {
        public List<Item> Items { get; set; } = new();

        string conn = "server=localhost;port=1108;database=DropZoneDB;user=root;password=;";

        public void OnGet(int sellerId)
        {
            using var c = new MySqlConnection(conn);
            c.Open();

            // EXCLUDE InDA (opposite of LoadItems)
            var cmd = new MySqlCommand(
                "SELECT * FROM Items WHERE SellerID=@id AND itemStatus <> 'InDA'", c);

            cmd.Parameters.AddWithValue("@id", sellerId);

            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                Items.Add(new Item
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
                });
            }
        }
    }
}