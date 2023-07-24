using InventarioAPI.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace InventarioAPI.Models
{
    public class Category
    {
        [JsonConverter(typeof(ObjectIdJsonConverter))]
        public ObjectId id { get; set; }
        public string   name { get; set; }
        public DateTime created_at { get; set; }
        public int      created_by { get; set; }
        public DateTime modified_at { get; set; }
        public int      modified_by { get; set; }
        public DateTime deleted_at { get; set; }
        public int      deleted_by { get; set; }
        public int      estado { get; set; }
    }
}
