using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackingVaksinApp.BPOMServiceReference;

namespace TrackingVaksinApp.Controllers.Authentication
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View("Login");
        }

		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Register(string name, string address, string no_license, string username, string password)
		{
			BPOMServiceClient ob = new BPOMServiceClient();
			if (ob.Register(new Account
			{
				username = username,
				password = password,
				role = "Produsen",
				created_at = DateTime.Now
			}) != null)
			{
				if (ob.AddProdusen(new BPOMServiceReference.Produsen
				{
					name = name,
					address = address,
					no_license = no_license,
					username = username
				}) != null)
				{
					Session["SuccessRegis"] = "True";
					return RedirectToAction("Login");
				}
			}
			ViewBag.usernameErr = "Username has already exits";
			return View();
		}

		public ActionResult RSRegister()
		{
			return View();
		}

		[HttpPost]
		public ActionResult RSRegister(string name, string address, string no_license, string username, string password)
		{
			BPOMServiceClient ob = new BPOMServiceClient();
			if (ob.Register(new Account
			{
				username = username,
				password = password,
				role = "RS",
				created_at = DateTime.Now
			}) != null)
			{
				if (ob.AddRS(new BPOMServiceReference.RumahSakit
				{
					name = name,
					address = address,
					no_license = no_license,
					username = username
				}) != null)
				{
					Session["SuccessRegis"] = "True";
					return RedirectToAction("Login");
				}
			}
			ViewBag.usernameErr = "Username has already exits";
			return View();
		}


		public ActionResult Login()
		{
			if (Session["username"] != null)
			{
				return RedirectToAction("Index");
			}
			return View();
		}

		[HttpPost]
		public ActionResult Login(string username, string password)
		{
			BPOMServiceClient ob = new BPOMServiceClient();
			if (ob.Login(username, password))
			{
				Account getAccount = ob.GetAccount(username);
				if (getAccount.role.Equals("Produsen"))
				{
					BPOMServiceReference.Produsen produsen = ob.GetProdusen(username);
					Session["username"] = username;
					Session["name"] = produsen.name;
					Session["Role"] = "Produsen";
					return RedirectToRoute("Produsen", new { action = "Index", controller = "HomeProdusen" });
				}
				else if (getAccount.role.Equals("BPOM"))
				{
					BPOMServiceReference.BPOM bpom = ob.GetBPOM(username);
					Session["username"] = username;
					Session["name"] = bpom.name;
					Session["Role"] = "BPOM";
					return RedirectToRoute("Default", new { action = "Index", controller = "BPOMVaksin" });
				}
				else if (getAccount.role.Equals("RS"))
				{
					BPOMServiceReference.RumahSakit rs = ob.GetRS(username);
					Session["username"] = username;
					Session["name"] = rs.name;
					Session["Role"] = "RS";
					return RedirectToRoute("RS", new { action = "Index", controller = "HomeRS" });
				}
			}
			Session["Error"] = "Username or Password incorrect";
			return RedirectToAction("Login");
		}

		public ActionResult Logout()
		{
			Session.RemoveAll();
			Session["isLogout"] = "True";
			return RedirectToAction("Login");
		}
	}
}