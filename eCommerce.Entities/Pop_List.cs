using System.Collections.Generic;

namespace eCommerce.Entities
{
    public class Pop_List : BaseEntity
    {
        public int? Pol_code { get; set; }       
        public string Pol_Name { get; set; }
        public string Pol_Shnm { get; set; }
        public string Pol_Scrl { get; set; }



        public virtual List<Pop_List> PopList { get; set; }
         
    }

    
}
