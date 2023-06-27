using eCommerce.Entities;
using eCommerce.Entities.CustomEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Services
{
   public class UserProfileService
    {
        #region Define as Singleton
        private static UserProfileService _Instance;

        public static UserProfileService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new UserProfileService();
                }

                return (_Instance);
            }
        }

        private UserProfileService()
        {
        }
        #endregion


        public bool SaveAddress(UserAddress Address)
        {
            try
            {
                var context = DataContextHelper.GetNewContext();

                context.Users_Address.Add(Address);

                return context.SaveChanges() > 0;
            }
            catch (Exception x)
            {

                throw x;
            }
            
        }
        public bool Update(UserAddress address)
        {
            var context = DataContextHelper.GetNewContext();

            var existingaddress = context.Users_Address.Find(address.Id);

            context.Entry(existingaddress).CurrentValues.SetValues(address);

            return context.SaveChanges() > 0;
        }
        public  int  GetUserId()
        {
            var context = DataContextHelper.GetNewContext();

            var userid = context.Users.Max(x=>x.UserId);
            
            return userid+1;
        }
        public UserAddress GetAddressByID(int ID)
        {
            var context = DataContextHelper.GetNewContext();

            var Address = context.Users_Address.Find(ID);

            return Address != null && !Address.IsRemoved ? Address : null;
        }
        public UserAddress GetAddressByID(int? ID)
        {
            var context = DataContextHelper.GetNewContext();

            var Address = context.Users_Address.Find(ID);

            return Address != null && !Address.IsRemoved ? Address : null;
        }
        public List<State> GetStateList()
        {
            try
            {
                var context = DataContextHelper.GetNewContext();
                var Statelist = context.States.ToList();

                return Statelist;
            }
            catch (Exception X)
            {

                throw X;
            }
        }

        public List<UserAddress> GetAddressList(string ID)
        {
            try
            {
                var context = DataContextHelper.GetNewContext();
                var Addresslist = context.Users_Address.Where(x=>x.UserId==ID && !x.IsRemoved).ToList();

                return Addresslist;
            }
            catch (Exception X)
            {

                throw X;
            }
        }
        public eCommerceUser GetUser(string ID)
        {
            try
            {
                var context = DataContextHelper.GetNewContext();
                var UserDetails = context.Users.Where(x=>x.Id==ID).FirstOrDefault();

                return UserDetails;
            }
            catch (Exception X)
            {

                throw X;
            }
        }

        public bool Remove(int ID)
        {
            var context = DataContextHelper.GetNewContext();

           

            var address = context.Users_Address.Single(x => x.Id == ID && x.IsRemoved == false);

            address.IsRemoved = true;

            context.Entry(address).State = EntityState.Modified;

            return context.SaveChanges() > 0;
        }
        public UserAddress GetAddress(int ID)
        {
            var context = DataContextHelper.GetNewContext();

            var Address = context.Users_Address.Find(ID);

            return Address;
        }
    }



    
}
