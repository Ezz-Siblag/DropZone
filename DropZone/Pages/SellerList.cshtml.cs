using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace DropZone.Pages
{
    public class SellerListModel : PageModel
    {
        public List<Item> Items { get; set; } = new();
        public List<Seller> Sellers { get; set; } = new();

        [BindProperty] public int SellerID { get; set; }
        [BindProperty] public int ItemID { get; set; }
        [BindProperty] public string ActionType { get; set; }

        public string? Message { get; set; }

        string conn = "server=localhost;port=1108;database=DropZoneDB;user=root;password=;";

        public void OnGet()
        {
            LoadSellers();
        }

        public IActionResult OnPostLoadItems()
        {
            LoadSellers();

            // IMPORTANT: ensure SellerID is preserved from form
            if (SellerID > 0)
            {
                LoadItems();
            }

            return Page();
        }

        public IActionResult OnPostProcessAction()
        {
            if (string.Equals(ActionType, "Update", StringComparison.OrdinalIgnoreCase))
                return RedirectToPage("UpdateItem", new { id = ItemID });

            if (SellerID <= 0)
            {
                Message = "Please select a seller first.";
                LoadSellers();
                return Page();
            }

            if (string.Equals(ActionType, "PickedUp", StringComparison.OrdinalIgnoreCase))
                UpdateStatus("Picked Up");

            if (string.Equals(ActionType, "PulledOut", StringComparison.OrdinalIgnoreCase))
                UpdateStatus("Pulled Out");

            LoadSellers();
            LoadItems();

            return Page();
        }

        void UpdateStatus(string status)
        {
            using var c = new MySqlConnection(conn);
            c.Open();

            var cmd = new MySqlCommand("UPDATE Items SET itemStatus=@s WHERE ItemID=@id", c);
            cmd.Parameters.AddWithValue("@s", status);
            cmd.Parameters.AddWithValue("@id", ItemID);
            cmd.ExecuteNonQuery();
        }

        void LoadSellers()
        {
            Sellers.Clear();

            using var c = new MySqlConnection(conn);
            c.Open();

            var cmd = new MySqlCommand("SELECT * FROM Sellers", c);
            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                Sellers.Add(new Seller
                {
                    SellerID = r.GetInt32("SellerID"),
                    SellerName = r.GetString("SellerName"),

                    // ✅ FIX: DateTime conversion (IMPORTANT)
                    DateAdded = r.GetDateTime("DateAdded").ToString("yyyy-MM-dd"),
                    ContactNumber = r["ContactNumber"]?.ToString()
                });
            }
        }

        void LoadItems()
        {
            Items.Clear();

            using var c = new MySqlConnection(conn);
            c.Open();

            var cmd = new MySqlCommand("SELECT * FROM Items WHERE SellerID=@id AND itemStatus='InDA'", c);
            cmd.Parameters.AddWithValue("@id", SellerID);

            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                Items.Add(new Item
                {
                    ItemID = r.GetInt32("ItemID"),
             
                    BuyerName = r["BuyerName"]?.ToString(),

                    DateDropped = r["DateDropped"] == DBNull.Value
                        ? null
                        : r.GetDateTime("DateDropped").ToString("yyyy-MM-dd"),

                    Price = r.GetDecimal("price"),
                    HoldingFee = r.GetInt32("holdingFee"),
                    ItemStatus = r["itemStatus"]?.ToString(),
                    Placed = r["placed"]?.ToString(),

                    DatePickedUp = r["datePickedUp"] == DBNull.Value
                        ? null
                        : r.GetDateTime("datePickedUp").ToString("yyyy-MM-dd")
                });
            }
        }
    }

    public class Item
    {
        public int ItemID { get; set; }
        public int SellerID { get; set; }
        public string? BuyerName { get; set; }
        public string? DateDropped { get; set; }
        public decimal Price { get; set; }
        public int HoldingFee { get; set; }
        public string? ItemStatus { get; set; }
        public string? Placed { get; set; }
        public string? DatePickedUp { get; set; }
    }

    public class Seller
    {
        public int SellerID { get; set; }
        public string SellerName { get; set; }
        public string DateAdded { get; set; }
        public string ContactNumber { get; set; }
    }
}