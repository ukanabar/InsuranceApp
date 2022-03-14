using InsuranceApp.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace InsuranceApp.Controllers
{
    public class HomeController : Controller
    {
        private InterviewEntities interviewEntities;
        public HomeController()
        {
            interviewEntities = new InterviewEntities();
        }        
        public ActionResult Index(LoginModel model)
        {
            string error = string.Empty;
            if (model.UserName!=null && model.Password!=null)
            {
                var user = this.interviewEntities.Users.Where(u => u.UserName.Equals(model.UserName) && u.Password.Equals(model.Password)).FirstOrDefault();

                error = ValidateUser(user);
                if (string.IsNullOrWhiteSpace(error))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    return RedirectToAction("LossTypes");
                }
                ViewBag.Message = error;
            }         
            return View("Index");
        }

        public ActionResult LossTypes(int? page)
        {
            var lossTypes = this.interviewEntities.LossTypes;
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(lossTypes.OrderBy(l=>l.LossTypeId).ToPagedList(pageNumber, pageSize));           
        }

        [HttpPost]
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        private String ValidateUser(User user)
        {
            string error = string.Empty;
            if (user == null)
            {
                error = "Username and/or password is incorrect.";
            }
            else
            {
                if (!user.Active)
                {
                    error = "Account has not been activated.";
                }                
            }
            return error;
        }
    }
}