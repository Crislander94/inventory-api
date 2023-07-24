using MongoDB.Bson;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace InventarioAPI.Helpers
{
    public class ObjectIdJsonConverter : JsonConverter<ObjectId>
    {
        public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //throw new NotImplementedException();
            bool isObjectId = Helpers.isObjectId(reader.GetString());
            if (reader.TokenType == JsonTokenType.String && isObjectId)
            {
                
                string value = reader.GetString();
                return ObjectId.Parse(value);
            }
            
            // Si el valor no se puede convertir a ObjectId, lanza una excepción o devuelve un valor predeterminado
            throw new JsonException($"No se pudo convertir el valor {reader.GetString()} a ObjectId.");
        }

        public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
