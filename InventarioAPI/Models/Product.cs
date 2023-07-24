using InventarioAPI.Helpers;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace InventarioAPI.Models
{
    public class Product
    {
        [JsonConverter(typeof(ObjectIdJsonConverter))]
        public ObjectId id { get; set; }
        [JsonConverter(typeof(ObjectIdJsonConverter))]
        public ObjectId category_id {get; set; }
        public string   name {get; set; }
        public string   description { get; set; }
        public int      stock {get; set; }
        public double   price_per_unit {get; set; }
        public double   price {get; set; }
        public DateTime created_at {get; set; }
        public int      created_by {get; set; }
        public DateTime modified_at { get; set; }
        public int      modified_by { get; set; }
        public DateTime deleted_at { get; set; }
        public int      deleted_by { get; set; }
        public int      estado { get; set; }
    }
}
