using ProductsNew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Data.Entity;

namespace ProductsNew.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OrderController : ApiController
    {
        ProductsContext productsContext = new ProductsContext();
        Service service = new Service();

        public HttpResponseMessage GetOrders()
        {
            var orders = productsContext.Orders.ToList();
            return Request.CreateResponse(HttpStatusCode.OK, orders);
        }

        // [HttpGet]
        public HttpResponseMessage GetOrders(int id)
        {
            // var orders = productsContext.OrdernDetails.ToList(); //Include             //tabelka + poz zamówien
            // return Request.CreateResponse(HttpStatusCode.OK, orders);

            var result = productsContext.Orders.FirstOrDefault(e => e.Order_ID == id);


            if (result != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Product with ID " + id.ToString() + "not found.");
            }
        }

        public HttpResponseMessage GetOrderDetails()
        {
            var orders = productsContext.OrdernDetails.ToList();

            return Request.CreateResponse(HttpStatusCode.OK, orders);
        }

        //[HttpGet]
        public HttpResponseMessage GetOrderDetails(int id)
        {
            var result = productsContext.OrdernDetails.Where(e => e.Order_ID == id).ToList();//(e => e.Order_ID == id).ToString();

            if (result != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Product with ID " + id.ToString() + "not found.");
            }
        }


        [HttpPost]
        public HttpResponseMessage AddOrder([FromBody] Orders order)
        {

            try
            {

                productsContext.Orders.Add(order);
                productsContext.SaveChanges();
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, order);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { order.Order_ID }));

                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage EditOrder(int id, [FromBody] Orders order)
        {
            try
            {
                var result = productsContext.Orders.Where(p => p.Order_ID == id).Include(x => x.OrdernDetails).FirstOrDefault();
              
                var orders = productsContext.OrdernDetails.Where(e => e.Order_ID == id).ToList();
                var neworders = order.OrdernDetails.ToList();
                if (result == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Product with Id " + id.ToString() + " not found.");
                }
                else
                {
                    if (order.Address != null)
                    { result.Address = order.Address; }
                    else { }
                    result.OrderDate = DateTime.UtcNow;


                    //foreach(var item in result.OrdernDetails)
                    for(int i =0; i< orders.Count; i++)
                    { //foreach(var item2 in order.OrdernDetails)
                        var toBeChanged = result.OrdernDetails.ElementAtOrDefault(i);
                        var changing = order.OrdernDetails.ElementAtOrDefault(i);


                            if (toBeChanged.Product_ID == changing.Product_ID)
                                if (toBeChanged.Quantity != changing.Quantity)
                                {
                                    var findPrice = productsContext.Products.FirstOrDefault(e => e.Product_ID == toBeChanged.Product_ID);
                                  toBeChanged.Quantity = changing.Quantity;
                                    productsContext.SaveChanges();
                                    toBeChanged.Price = (findPrice.Price * toBeChanged.Quantity) ?? 0;
                                    productsContext.SaveChanges();
                                }

                        
                        // order.OrdernDetails[i] = 


                        //if (item.Order_ID == item2.Order_ID)
                        //{
                        //    if (item.Quantity != item2.Quantity)
                        //    {
                        //        item.Quantity = item2.Quantity;
                        //        var findPrice = productsContext.Products.FirstOrDefault(e => e.Product_ID == item.Product_ID);
                        //        item.Price = (findPrice.Price * item.Quantity) ?? 0;
                        //        productsContext.SaveChanges();
                        //    }
                        //}

                    }

                    //result.OrdernDetails.Clear(); // zamiast clear, porównaj quantity i podmienić record
                    // productsContext.SaveChanges();
                    //result.OrdernDetails = orders;

                    productsContext.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        }


        [HttpPost]
        public HttpResponseMessage EditOrderNew(int id, [FromBody] Orders order)
        {
            try
            {
                var result = productsContext.Orders.FirstOrDefault(e => e.Order_ID == id);

                if (result == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Product with Id " + id.ToString() + " not found.");
                }
                else
                {
                     if (order.Address != null)
                    {
                        result.Address = order.Address;
                    }
                    else { }
                    result.OrderDate = DateTime.UtcNow;
                    productsContext.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        }




        [HttpDelete]
        public HttpResponseMessage DeleteOrders(int id)
        {
            try
            {
                //var result = productsContext.Orders.FirstOrDefault(e => e.Order_ID == id);
                var result = productsContext.Orders.Where(p => p.Order_ID == id).Include(x => x.OrdernDetails).FirstOrDefault();


                if (result == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Product with ID " + id.ToString() + "not found.");
                }
                else
                {
                    result.OrdernDetails.Clear();
                    productsContext.SaveChanges();
                    productsContext.Orders.Remove(result);
                    productsContext.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, result);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        [HttpPost]
        public HttpResponseMessage AddOrderDetail(int id, [FromBody] OrdernDetails orderdet)
        {
            try
            {
                var result = productsContext.Orders.FirstOrDefault(e => e.Order_ID == id);
                var prod = productsContext.Products.FirstOrDefault(e => e.Product_ID == orderdet.Product_ID);
                orderdet.Order_ID = result.Order_ID;
                orderdet.Price = (orderdet.Quantity * prod.Price)??0;
                result.OrdernDetails.Add(orderdet);
                productsContext.SaveChanges();
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, orderdet);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { orderdet.Order_ID }));

                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        [HttpPost]
        public HttpResponseMessage EditOrderDetail(int id, [FromBody] OrdernDetails order)
        {
            try
            {
                var result = productsContext.OrdernDetails.FirstOrDefault(e => e.OrderDetail_ID == id);

                if (result == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Product with Id " + id.ToString() + " not found.");
                }
                else
                {
                    var findPrice = productsContext.Products.FirstOrDefault(e => e.Product_ID == result.Product_ID);
                   
                    result.Quantity = order.Quantity;;
                    result.Price = (findPrice.Price * order.Quantity)??0;
                    productsContext.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteOrderDetail(int id)
        {
            try
            {
                var result = productsContext.OrdernDetails.FirstOrDefault(e => e.OrderDetail_ID == id);

                if (result == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Partial order with ID " + id.ToString() + "not found.");
                }
                else
                {
                    productsContext.OrdernDetails.Remove(result);
                    productsContext.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
