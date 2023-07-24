using InventarioAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace InventarioAPI.DAL_s
{
    public class CustomerDAL
    {

        private readonly IConfiguration _Config;
        private static IMongoCollection<Customer> _customer_collection;

        public CustomerDAL(IConfiguration iconfig)
        {
            _Config = iconfig;
            this.Setup();
        }

        private void Setup()
        {
            string connection_strings = _Config["ConnectionStrings:DBDev"];
            string databaseName = _Config["DBInventory:DbName"];
            string collectionName = _Config["DBInventory:CustomersCollection"];

            var client = new MongoClient(connection_strings);
            var database = client.GetDatabase(databaseName);
            _customer_collection = database.GetCollection<Customer>(collectionName);
        }

        public Response createCustomerDAL(Customer customer)
        {
            try
            {
                Response response = new Response();
                Customer exist_customer = existCustomerDAL(customer.identification);

                if (exist_customer != null)
                {
                    response.statusCode = 400;
                    response.message = "El customer ya existe en la DB";

                    return response;
                }
                customer.created_at = DateTime.Now;
                customer.estado = 1;
                customer.created_by = 1;
                _customer_collection.InsertOne(customer);
                var insertedDocumentId = customer.ToBsonDocument()["_id"];
                var insertedDocument = _customer_collection.Find(Builders<Customer>.Filter.Eq("_id", insertedDocumentId)).FirstOrDefault();
                response.statusCode = 200;
                response.message = "Registro exitosos.";
                response.responseText = JsonSerializer.Serialize(insertedDocument);

                return response;

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<Customer> getCustomersDAL()
        {
            try
            {
                var filter = Builders<Customer>.Filter.Eq(r => r.estado, 1);
                List<Customer> list_customer = _customer_collection.Find(filter).ToList();
                return list_customer;
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        public Customer getCustomerIdDAL(ObjectId id)
        {
            try
            {
                Customer customer = _customer_collection.AsQueryable()
                                                                    .Where(r => r.id == id)
                                                                    .Where(r => r.estado == 1 )
                                                                    .FirstOrDefault();
                return customer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Customer existCustomerDAL(string identificacion)
        {
            try
            {
                Customer customer = new Customer();
                var filter = Builders<Customer>.Filter.Eq(r => r.identification, identificacion.Trim());
                customer = _customer_collection.Find(filter).FirstOrDefault();
                return customer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Response updateCustomerDAL(string id, Customer customer)
        {
            try
            {
                Response response = new Response();
                Customer exist_customer = new Customer();
                ObjectId customer_id = new ObjectId(id);
                exist_customer = this.getCustomerIdDAL(customer_id);
                if(exist_customer == null)
                {
                    response.statusCode = 404;
                    response.message = "No existe el customer";

                    return response;
                }
                var filter = Builders<Customer>.Filter.Eq(r => r.id, customer_id);
                var actualizar = Builders<Customer>.Update
                                                .Set("celular", customer.celular)
                                                .Set("name",    customer.name)
                                                .Set("identification", customer.identification)
                                                .Set("modified_at", DateTime.Now)
                                                .Set("modified_by", 1);
                var resultado = _customer_collection.UpdateOne(filter, actualizar);
                if(resultado.ModifiedCount > 0)
                {
                    response.statusCode = 200;
                    response.message = "Se actualizo el customer correctamente";
                    var customer_actualizado = _customer_collection.Find(filter).FirstOrDefault();
                    response.responseText = JsonSerializer.Serialize(customer_actualizado);
                }
                else
                {
                    response.statusCode = 201;
                    response.message = "No hubo cambios en el customer";
                }
                return response;
            }catch(Exception ex)
            {
                throw ex;
            }
        }


        public Response deleteCustomerDAL(string id)
        {
            try
            {
                Response response = new Response();
                Customer exist_customer = new Customer();
                ObjectId customer_id = new ObjectId(id);
                exist_customer = this.getCustomerIdDAL(customer_id);
                if (exist_customer == null)
                {
                    response.statusCode = 404;
                    response.message = "No existe el customer";

                    return response;
                }
                var filter = Builders<Customer>.Filter.Eq(r => r.id, customer_id);
                var eliminar = Builders<Customer>.Update
                                                .Set("estado", 0)
                                                .Set("deleted_at", DateTime.Now)
                                                .Set("deleted_by", 1);
                var resultado = _customer_collection.UpdateOne(filter, eliminar);
                if (resultado.ModifiedCount > 0)
                {
                    response.statusCode = 200;
                    response.message = "Se elimino el customer correctamente";
                }
                else
                {
                    response.statusCode = 201;
                    response.message = "Existió un error en el proceso";
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
