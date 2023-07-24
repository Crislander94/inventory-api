using InventarioAPI.Models;
using MongoDB.Bson;

namespace InventarioAPI.Helpers
{
    public class Helpers
    {
        public static List<Object> generateResponseCategories(List<Category> categories)
        {
            List<Object> new_category_obj = new List<Object>();

            foreach(Category category in categories)
            {
                new_category_obj.Add(new
                {
                    _id = category.id.ToString(),
                    category.name,
                    category.created_at,
                    category.created_by
                });
            }
            return new_category_obj;
        }

        public static bool isObjectId(string id)
        {
            ObjectId objectId;
            return ObjectId.TryParse(id, out objectId);
        }
    }
}
