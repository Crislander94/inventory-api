using InventarioAPI.Helpers;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace InventarioAPI.Models
{
    public class ProductSearch
    {
        public string       id { get; set; }
        public string       category_name { get; set; }
        public string       name { get; set; }
        public int          stock { get; set; }
        public double       price_per_unit { get; set; }
        public double       price { get; set; }
        public string       description { get; set; }
        public int          estado { get; set; }
        public DateTime     created_at { get; set; }
        public int          created_by { get; set; }
    }
}
