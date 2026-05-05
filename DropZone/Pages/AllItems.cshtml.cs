using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace DropZone.Pages
{
    public class AllItemsModel : PageModel
    {
        public List<Item> Items { get; set; } = new();

        public int SellerID { get; set; }
        public string Filter { get; set; } = "All";

        string conn = "server=localhost;port=1108;database=DropZoneDB;user=root;password=;";

        public void OnGet(int sellerId, string? filter)
        {
            SellerID = sellerId;
            Filter = filter ?? "All";

            using var c = new MySqlConnection(conn);
            c.Open();

            string query = "SELECT * FROM Items WHERE SellerID=@id";

            if (Filter == "InDA")
                query += " AND itemStatus = 'InDA'";
            else if (Filter == "PickedUp")
                query += " AND itemStatus = 'PickedUp'";
            else if (Filter == "PulledOut")
                query += " AND itemStatus = 'PulledOut'";

            var cmd = new MySqlCommand(query, c);
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