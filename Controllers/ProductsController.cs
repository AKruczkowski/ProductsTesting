using ProductsNew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProductsNew.Controllers
{
    public class ProductsController : ApiController
    {
        // QUESTIONS
        //  IhttpActionResult instead of HttpResponseMessage? - suggested by microsoft

        ProductsContext productsContext = new ProductsContext();


        // GET: api/Products
        public HttpResponseMessage Get()
        {
            var products = productsContext.Products.ToList();
            return Request.CreateResponse(HttpStatusCode.OK, products);
        }


        // GET: api/Products/5
        public HttpResponseMessage Get(int id)
        {
            var result = productsContext.Products.FirstOrDefault(e => e.Product_ID == id);
            if(result != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Product with ID " + id.ToString() + "not found.");
            }
        }

        // POST: api/Products

        public HttpResponseMessage Post([FromBody] Products product)
        {
            try
            {
                //
                productsContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Products ON");
                productsContext.Products.Add(product);
                productsContext.SaveChanges();
                productsContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Products OFF");


                var message = Request.CreateResponse(HttpStatusCode.Created, product);
                message.Headers.Location = new Uri(Request.RequestUri + "/"+ product.Product_ID.ToString());


                return message;

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // PUT: api/Products/5
        public HttpResponseMessage Put(int id, [FromBody] Products value)
        {
            try
            {
                var result = productsContext.Products.FirstOrDefault(e => e.Product_ID == id);
                if (result == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Product with Id " + id.ToString() + " not found.");
                }
                else
                    {
                    result.Name = value.Name;
                    result.Description = value.Description;
                    result.Price = value.Price;
                    result.Height = value.Height;
                    result.Width = value.Width;
                    result.Date = value.Date;
                    result.Length = value.Length;
                    result.Date = DateTime.Now;
                    result.Category = value.Category;

                    productsContext.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, result);
                    }
                }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

        }

        // DELETE: api/Products/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var result = productsContext.Products.FirstOrDefault(e => e.Product_ID == id);
                if(result == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Product with ID " + id.ToString() + "not found.");
                }
                else
                {
                    productsContext.Products.Remove(result);
                    productsContext.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);  
            }
           // productsContext.Products.Remove(productsContext.Products.FirstOrDefault(e => e.Product_ID == id));


        }
        //public void Delete(int id)
        //{
        //    productsContext.Products.Remove(productsContext.Products.FirstOrDefault(e => e.Product_ID == id));

        //    productsContext.SaveChanges();
        //}
    }
}
