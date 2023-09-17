using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Chola.Models
{

    public class QueryUserGridModel
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string ContactNo { get; set; }

        public string IsApproved { get; set; }

        public string Roles { get; set; }

    }

    public class UserRole
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }

    public class CreateUserModel
    {
        public int? UserID { get; set; }

        [Required, Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string Email { get; set; }

        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Phone Number")]
        public string ContactNo { get; set; }

        public List<UserRole> RoleMaster { get; set; }

    }

    public class EditUserModel
    {
        public int? UserID { get; set; }

        public string UserName { get; set; }       

        public string Email { get; set; }

        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Phone Number")]
        public string ContactNo { get; set; }

        public List<UserRole> RoleMaster { get; set; }

        public int? Status { get; set; }

        public bool IsAdmin { get; set; }
    }
    
}