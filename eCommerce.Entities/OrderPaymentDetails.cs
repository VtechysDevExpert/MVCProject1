using System;

namespace eCommerce.Entities
{
  public class OrderPaymentDetails
    {

        public int ID { get; set; }
        public int OrderID { get; set; }
        public int OrderFncd { get; set; }
        public string TransactionID { get; set; }
        public string PaymentID { get; set; }
        public string PaymentMethod { get; set; }
        public decimal FinalAmmount { get; set; }
        public string CardName { get; set; }
        public string CardNo { get; set; }
        
        public int? MoneyReceiptNo { get; set; }
 
        public int MoneyReceiptFncd { get; set; }
        public DateTime? EntryOn { get; set; }



    }
}
