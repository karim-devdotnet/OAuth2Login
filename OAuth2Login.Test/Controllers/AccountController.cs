using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OAuth2Login.Test.Models;
using OAuth2Login.Service;

namespace OAuth2Login.Test.Controllers
{
    public class AccountController : Controller
    {

        public ActionResult Login(string id)
        {
            string absoluteUri = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
            var service = BaseOauth2Service.GetService(id, absoluteUri);

            if (service != null)
            {
                var url = service.BeginAuthentication();
                return Redirect(url);
            }
            else
            {
                return RedirectToAction("LoginFail");
            }
        }

        public ActionResult Callback(string id)
        {
            string absoluteUri = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
            var service = BaseOauth2Service.GetService(id, absoluteUri);

            if (service != null)
            {
                BaseUserData userData = null;
                var redirectUrl = service.ValidateLogin(Request);

                switch (id)
                {
                    case "Facebook":
                        userData = service.GetUserInfo<FacebookUserData>();
                        break;
                    case "Google":
                        userData = service.GetUserInfo<GoogleUserData>();
                        break;
                    case "Twitter":
                        userData = service.GetUserInfo<TwitterUserData>();
                        break;
                }

                if (redirectUrl != null)
                {
                    return Redirect(redirectUrl);
                }

                return RedirectToAction("LoginSuccess");

            }
            else
            {
                return RedirectToAction("LoginFail");
            }
        }

        public ActionResult LoginFail()
        {
            return View();
        }

        public ActionResult LoginSuccess()
        {
            return View();
        }
    }
}