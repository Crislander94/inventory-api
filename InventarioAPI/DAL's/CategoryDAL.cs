using CsharpExamples.UsageExamples.InsertOne;
using InventarioAPI.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Xml.Linq;

namespace InventarioAPI.DAL_s
{
    public class CategoryDAL
    {
        private readonly IConfiguration _Config;
        private static IMongoCollection<Category> _category_collection;

        public CategoryDAL(IConfiguration iconfig)
        {
            _Config = iconfig;
            this.Setup();
        }

        private void Setup()
        {
            string connection_strings = _Config["ConnectionStrings:DBDev"];
            string databaseName = _Config["DBInventory:DbName"];
            string collectionName = _Config["DBInventory:CategoriesCollection"];

            var client = new MongoClient(connection_strings);
            var database = client.GetDatabase(databaseName);
            _category_collection = database.GetCollection<Category>(collectionName);
        }
        public Category createCategoryDAL(Category category)
        {
            try
            {
                //string connection_strings = _Config["ConnectionStrings:DBDev"];
                //string databaseName = _Config["DBInventory:DbName"];
                //string collectionName = _Config["DBInventory:CategoriesCollection"];
                //
                //var client = new MongoClient(connection_strings);
                //var database = client.GetDatabase(databaseName);
                //var collection = database.GetCollection<BsonDocument>(collectionName);
                //var newCategory = category.ToBsonDocument();
                category.created_at = DateTime.Now;
                _category_collection.InsertOne(category);

                var insertedDocumentId = category.ToBsonDocument()["_id"];
                // Puedes obtener el objeto insertado mediante una consulta
                var insertedDocument = _category_collection.Find(Builders<Category>.Filter.Eq("_id", insertedDocumentId)).FirstOrDefault();

                return insertedDocument;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Category> getCategoryDAL()
        {
            try
            {
                var filter = Builders<Category>.Filter.Eq( r => r.estado , 1);
                List<Category> list_categories = _category_collection.Find(filter).ToList();
                return list_categories;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public Category getCategoryIdDAL(ObjectId id)
        {
            try
            {
                //var filter = Builders<Category>.Filter;
                //Category category = _category_collection.Find(filter).FirstOrDefault();
                Category category = _category_collection.AsQueryable()
                                                        .Where(r => r.id == id)
                                                        .Where(r => r.estado == 1)
                                                        .FirstOrDefault();
                return category;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
