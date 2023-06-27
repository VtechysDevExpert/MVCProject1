namespace eCommerce.Entities
{
    public class ProductPicture : BaseEntity
    {
        public int ProductID { get; set; }

        public int PictureID { get; set; }
        public virtual Picture Picture { get; set; }
    }
}
