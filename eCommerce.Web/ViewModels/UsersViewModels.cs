using eCommerce.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Web.ViewModels
{
    public class LoginViewModel : PageViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class RegisterViewModel : PageViewModel
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordViewModel : PageViewModel
    {
        public string Username { get; set; }
    }

    public class ResetPasswordViewModel : PageViewModel
    {
        public string UserId { get; set; }
        public string Code { get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ProfileDetailsViewModel : PageViewModel
    {
        public eCommerceUser User { get; set; }
        public List<UserAddress> userAddresses { get; set; }
        public List<State> States { get; set; }
        public Pop_List PopList { get; set; }
        public string ActiveTab { get; internal set; }
    }
    public class AddressEditViewModel:PageViewModel
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int ZipCode { get; set; }
        public int StateCode { get; set; }
        public List<State> States { get; set; }

    }


    public class ProfileAddressViewModel : PageViewModel
    {
        public eCommerceUser User { get; set; }


        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int ZipCode { get; set; }
        public int StateCode { get; set; }
       // public UserAddress Address { get; set; }

        public List<UserAddress> userAddresses { get; set; }
        public List<State> States { get; set; }
        public Pop_List PopList { get; set; }
        public string ActiveTab { get; internal set; }
    }
    public class UpdateProfileDetailsViewModel : PageViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        //public string Country { get; set; }
        //public string City { get; set; }
        //public string Address { get; set; }
        //public string ZipCode { get; set; }
        public string Gstin { get; set; }
        public string Gender { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Birthday { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Anniversary { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? SpouseBirthday { get; set; }

        public int StateCode { get; set; }
        //public string Country_ofc { get; set; }
        //public string City_ofc { get; set; }
        //public string Address_ofc { get; set; }
        //public string ZipCode_ofc { get; set; }

        //public string Address_other { get; set; }
        //public string City_other { get; set; }
        //public string Country_other { get; set; }
        //public string ZipCode_other { get; set; }

    }

    public class UpdatePasswordViewModel : PageViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLoginConfirmationViewModel
    {
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }
     
}