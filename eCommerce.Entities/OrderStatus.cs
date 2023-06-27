namespace eCommerce.Entities
{
    public enum OrderStatus
    {
       Placed = 1, //will be update from online order
        ReadyForDispatch = 2, //will be update from offline sale
        Shipped = 3, //will be update from offline DESP_HDR
        Delivered = 4,  //will be update from offline DESP_HDR 
        Cancelled = 5,  //will be update from online cancellation by customer
        Refunded = 6, // will be update from offline Return Request table of chbh 
        Failed = 7,
        OnHold = 8,
        WaitingForPayment = 9,
        Return=10,
        //Placed = 1, //will be update from online order
        //ReadyForDispatch = 2, //will be update from offline sale
        //Dispatched = 9, //will be update from offline DESP_HDR
        //Delivered = 3,  //will be update from offline DESP_HDR 
        //Cancelled = 5,  //will be update from online cancellation by customer
        //Refunded = 8, // will be update from offline Return Request table of chbh 
        //Failed = 4,
        //OnHold = 6,
        //WaitingForPayment = 7,


    }
}
