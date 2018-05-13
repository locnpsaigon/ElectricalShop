using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Principal;

/// <summary>
/// ElectricalShopPrinciple class
/// Prepresent user authorization info
/// Author: LocNP
/// Date: March 22, 2017
/// </summary>
namespace ElectricalShop.Models.Principle
{
    public class ElectricalShopPrinciple : IPrincipal
    {
        public IIdentity Identity { get; private set; }

        public int UserId { get; set; }

        public string FullName { get; set; }

        public String[] Roles { get; set; }

        public ElectricalShopPrinciple(string username)
        {
            this.Identity = new GenericIdentity(username);
        }

        public Boolean IsInRole(string userRoles)
        {
            foreach(var role in Roles)
            {
                if (userRoles.Contains(role))
                {
                    return true;
                }
            }
            return false;
        }
    }
}