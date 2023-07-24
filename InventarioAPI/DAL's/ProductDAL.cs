using InventarioAPI.Models;
using Microsoft.AspNetCore.Components.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System.Text.Json;
using System.Xml.Linq;

namespace InventarioAPI.DAL_s
{
    public class ProductDAL
    {
        private readonly IConfiguration _Config;
        private static IMongoCollection<Product> _product_collection;

        public ProductDAL(IConfiguration iconfig)
        {
            _Config = iconfig;
            this.Setup();
        }

        private void Setup()
        {
            string connection_strings = _Config["ConnectionStrings:DBDev"];
            string databaseName = _Config["DBInventory:DbName"];
            string collectionName = _Config["DBInventory:ProductsCollection"];

            var client = new MongoClient(connection_strings);
            var database = client.GetDatabase(databaseName);
            _product_collection = database.GetCollection<Product>(collectionName);
        }

        public List<ProductSearch> getProductsDAL()
        {
            try
            {
                List<ProductSearch> list_product = new List<ProductSearch>();
                var bson_products = _product_collection.Aggregate()
                                .Lookup("categories", "category_id", "_id", "Category")
                                .Project(p => new
                                {
                                    id = p["_id"],
                                    name = p["name"],
                                    category_name = p["Category"]["name"],
                                    stock = p["stock"],
                                    price_per_unit = p["price_per_unit"],
                                    price = p["price"],
                                    description = p["description"],
                                    created_at = p["created_at"],
                                    created_by = p["created_by"],
                                    estado = p["estado"]
                                }).ToList();

                foreach (var product in bson_products)
                {
                    if(product.estado.AsInt32 == 1)
                    {
                        ProductSearch new_product = new ProductSearch();
                        new_product.id = product.id.AsObjectId.ToString();
                        new_product.name = product.name.AsString;
                        new_product.stock = product.stock.AsInt32;
                        new_product.category_name = product.category_name.AsBsonArray[0].AsString;
                        new_product.price_per_unit = product.price_per_unit.AsDouble;
                        new_product.price = product.price.AsDouble;
                        new_product.description = product.description.AsString;
                        new_product.created_at = product.created_at.ToUniversalTime();
                        new_product.created_by = product.created_by.AsInt32;

                        list_product.Add(new_product);
                    }
                    
                }

                return list_product;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Product getProductIdDAL(ObjectId id)
        {
            try
            {
                Product product = new Product();
                product = _product_collection.AsQueryable()
                                              .Where(r => r.id == id)
                                              .Where(r => r.estado == 1)
                                               .FirstOrDefault();
                return product;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Product existProductDAL(string name_product, ObjectId category_id)
        {
            try
            {
                Product product = new Product();
                product = _product_collection.AsQueryable()
                                             .Where( r => 
                                                    r.name.Trim().ToUpper() == name_product.Trim().ToUpper())
                                             .Where( r => r.category_id == category_id)
                                             .Where(r => r.estado == 1)
                                             .FirstOrDefault();

                return product;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public Product createProductDAL(Product product)
        {
            try
            {
                //var document = new BsonDocument
                //{
                //    { "name", producto.name },
                //    { "mark", producto.category_id },
                //    { "stock", producto.stock },
                //    { "price_per_unit", producto.price_per_unit},
                //    { "price", producto.price }
                //};

                CategoryDAL category = new CategoryDAL(_Config);
                Category my_category = category.getCategoryIdDAL(product.category_id);
                Product exist_product = this.existProductDAL(product.name, product.category_id);
                if (exist_product != null) return null;
                if (my_category == null) return null;

                product.name = product.name.Trim().ToUpper();
                product.created_at = DateTime.Now;

                _product_collection.InsertOne(product);
                var insertedDocumentId = product.ToBsonDocument()["_id"];
                // Puedes obtener el objeto insertado mediante una consulta
                var insertedDocument = _product_collection.Find(Builders<Product>.Filter.Eq("_id", insertedDocumentId)).FirstOrDefault();
                return insertedDocument;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Response updateProduct(string id, Product product)
        {
            try
            {
                DateTime time = DateTime.UtcNow;
                DateTime fechaHoraLocal = time.ToLocalTime();
                Response response = new Response();
                CategoryDAL category = new CategoryDAL(_Config);
                Product my_product_update = new Product();
                Category my_category = category.getCategoryIdDAL(product.category_id);
                ObjectId product_id = new ObjectId(id);

                if (my_category == null)
                {
                    response.statusCode = 400;
                    response.message = "La categoria no existe";

                    return response;
                }


                my_product_update = this.getProductIdDAL(product_id);
                if (my_product_update == null)
                {
                    response.statusCode = 404;
                    response.message = "El producto que quiere actualizar no existe";
                }
                else
                {
                    var filter = Builders<Product>.Filter.Eq("_id", product_id);
                    var actualizar = Builders<Product>.Update
                                                        .Set("category_id", product.category_id)
                                                        .Set("name", product.name)
                                                        .Set("stock", product.stock)
                                                        .Set("price_per_unit", product.price_per_unit)
                                                        .Set("price", product.price)
                                                        .Set("description", product.description)
                                                        .Set("modified_at", fechaHoraLocal);
                    var resultado = _product_collection.UpdateOne(filter, actualizar);
                    if (resultado.ModifiedCount > 0)
                    {
                        response.statusCode = 200;
                        response.message = "Se actualizo el producto correctamente";
                        var productoActualizado = _product_collection.Find(filter).FirstOrDefault();
                        response.responseText = JsonSerializer.Serialize(productoActualizado);
                    }
                    else
                    {
                        response.statusCode = 400;
                        response.message = "No existieron cambios en el producto";
                    }
                }
                
                return response;
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        public Response DeleteProduct(string id)
        {
            try
            {
                ObjectId product_id = new ObjectId(id);
                Product my_product_delete = new Product();
                Response response = new Response();
                var filter = Builders<Product>.Filter.Eq("_id", product_id);
                var eliminar = Builders<Product>.Update
                                                    .Set("estado", 0)
                                                    .Set("deleted_at", DateTime.Now); ;
                my_product_delete = this.getProductIdDAL(product_id);

                if(my_product_delete == null)
                {
                    response.statusCode = 404;
                    response.message = "El producto que quiere actualizar no existe";
                }
                else
                {
                    var resultado = _product_collection.UpdateOne(filter, eliminar);
                    if (resultado.ModifiedCount > 0)
                    {
                        response.statusCode = 200;
                        response.message = "Se elimino el producto correctamente";
                    }
                    else
                    {
                        response.statusCode = 400;
                        response.message = "No se pudo eliminar el producto";
                    }
                }
                
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
