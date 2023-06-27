using eCommerce.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace eCommerce.Services
{
    public class OrdersService
    {
        #region Define as Singleton
        private static OrdersService _Instance;

        public static OrdersService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new OrdersService();
                }

                return (_Instance);
            }
        }

        private OrdersService()
        {
        }
        #endregion
        
        public bool SaveOrder(Order order)
        {
            try
            {
                var context = DataContextHelper.GetNewContext();

                context.Orders.Add(order);

                return context.SaveChanges() > 0;
            }
            catch (System.Exception x)
            {

                throw x;
            }
           
        }

        public bool ReturnItem(ReturnRequest ReturnItem)
        {
            try
            {
                var context = DataContextHelper.GetNewContext();

                context.ReturnRequest.Add(ReturnItem);

                return context.SaveChanges() > 0;
            }
            catch (System.Exception x)
            {

                throw x;
            }

        }
        public bool CancelItem(CancelRequest CancelItem)
        {
            try
            {
                var context = DataContextHelper.GetNewContext();
                context.CancelRequest.Add(CancelItem);
                return context.SaveChanges() > 0;
            }
            catch (System.Exception x)
            {
               throw x;
            }

        }

        public Order GetOrderByID(int ID)
        {
            var context = DataContextHelper.GetNewContext();

            return context.Orders.Include("OrderItems.Product.ProductRecords").FirstOrDefault(x => x.ID == ID);
        }
        //get order by user id
        public Order GetOrderByUserID(string ID)
        {
            var context = DataContextHelper.GetNewContext();

            return context.Orders.Include("OrderItems.Product.ProductRecords").Where(x => x.CustomerID == ID).OrderByDescending(y=>y.ID).FirstOrDefault();
        }
        //updating order table after payment
        public bool paymentupdate(int ID,string paymentid,bool status)
        {
            var context = DataContextHelper.GetNewContext();
            var orders = context.Orders.Single(x => x.ID == ID);

            orders.TransactionID = paymentid;
            orders.IsPaid = status;
            context.Entry(orders).State = EntityState.Modified;

            return context.SaveChanges() > 0;
        }
        public bool SavePaymentDetails(OrderPaymentDetails PaymentDetails)
        {
            try
            {
                var context = DataContextHelper.GetNewContext();

                context.OrderPayment_Dtls.Add(PaymentDetails);

                return context.SaveChanges() > 0;
            }
            catch (System.Exception x)
            {

                throw x;
            }

        }
        public List<Order> SearchOrders(string userID, int? orderID, int? orderStatus, int? pageNo, int recordSize, out int count)
        {
            var context = DataContextHelper.GetNewContext();
            //var orders = context.Orders.AsQueryable();

            //i have set here is paid=true,this will show only record that is paid.
            var orders = context.Orders.Where(x => x.IsPaid == true).AsQueryable();

            if (!string.IsNullOrEmpty(userID))
            {
                

                orders = orders.Where(x => x.CustomerID.Equals(userID));
            }

            if (orderID.HasValue && orderID.Value > 0)
            {
                orders = orders.Where(x => x.ID == orderID.Value);
            }

            if (orderStatus.HasValue && orderStatus.Value > 0)
            {
                orders = orders.Where(x => x.OrderHistory.OrderByDescending(y => y.ModifiedOn).FirstOrDefault().OrderStatus == orderStatus);
            }

            count = orders.Count();

            pageNo = pageNo ?? 1;
            var skipCount = (pageNo.Value - 1) * recordSize;

            return orders.OrderByDescending(x => x.PlacedOn).Skip(skipCount).Take(recordSize).ToList();
        }

        public bool AddOrderHistory(OrderHistory orderHistory)
        {
            var context = DataContextHelper.GetNewContext();

            context.OrderHistories.Add(orderHistory);

            return context.SaveChanges() > 0;
        }
    }
}
