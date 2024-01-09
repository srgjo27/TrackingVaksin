using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackingVaksinApp.BPOMServiceReference;

namespace TrackingVaksinApp.Controllers.Produsen
{
    public class HomeProdusenController : Controller
    {
		// GET: Home	
		[Route("HomeProdusen", Name = "Produsen")]
		public ActionResult Index()
		{
			BPOMServiceClient ob = new BPOMServiceClient();
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			else
			{
				Account checkAccount = ob.GetAccount(Session["username"].ToString());
				if (checkAccount.role.Equals("Produsen"))
				{
					BPOMServiceReference.Produsen curP = ob.GetProdusen(Session["username"].ToString());
					ViewBag.TotalProduksiVaksin = ob.GetProdusenVaksin(curP.id_produsen.ToString()).Count();
					return View(ob.GetProdusenVaksin(curP.id_produsen.ToString()));
				}
				Session.RemoveAll();
				return RedirectToAction("Login");
			}
		}

		[HttpGet]
		[Route(Name = "ProdusenClient")]
		public ActionResult Create()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			BPOMServiceReference.Produsen curP = ob.GetProdusen(Session["username"].ToString());
			ViewBag.TotalProduksiVaksin = ob.GetProdusenVaksin(curP.id_produsen.ToString()).Count();
			return View();
		}

		[HttpPost]
		public ActionResult Create(string no_registration, string packaging, int total)
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			BPOMServiceReference.Produsen curP = ob.GetProdusen(Session["username"].ToString());
			if (ob.GetVaksinDetails(no_registration) != null)
			{
				return View();
			}
			else
			{
				Produsen_Vaksin data = new Produsen_Vaksin
				{
					created_at = DateTime.Now,
					id_produsen = curP.id_produsen,
					no_registration = no_registration,
					packaging = packaging,
					total = total,
					status = "Not Reported"
				};
				if (ob.AddProdusenVaksin(data) != null)
				{
					return RedirectToAction("Index");
				}
			}
			return View();
		}

		public ActionResult LaporVaksin()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			Produsen_Vaksin newProdusenVaksin = null;
			string no_registration = this.ControllerContext.RouteData.Values["id"].ToString();
			BPOMServiceReference.Produsen curP = ob.GetProdusen(Session["username"].ToString());
			Produsen_Vaksin detail = ob.GetProdusenVaksinDetails(curP.id_produsen.ToString(), no_registration);
			BPOM_Vaksin data = new BPOM_Vaksin
			{
				created_at = DateTime.Now,
				id_produsen = curP.id_produsen,
				no_registration = no_registration,
				packaging = detail.packaging,
				total = detail.total,
				status = "Invalid"
			};
			newProdusenVaksin = new Produsen_Vaksin
			{
				created_at = DateTime.Now,
				id_produsen = curP.id_produsen,
				no_registration = no_registration,
				total = detail.total,
				packaging = detail.packaging,
				status = "Invalid"
			};
			if (ob.Up(data) != null)
			{
				ob.UpdateProdusenVaksin(newProdusenVaksin);
				return RedirectToAction("Index", "HomeProdusen");
			}
			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Distributed()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			BPOMServiceReference.Produsen curP = ob.GetProdusen(Session["username"].ToString());
			ViewBag.TotalProduksiVaksin = ob.GetProdusenVaksin(curP.id_produsen.ToString()).Count();
			IEnumerable<RumahSakit> listRS = ob.GetListRumahSakit().ToList();
			IEnumerable<Produsen_Vaksin> listPVaksin = ob.GetProdusenVaksin(curP.id_produsen.ToString()).ToList();
			Models.ListViewModel viewModel = new Models.ListViewModel
			{
				rumahSakits = listRS,
				produsen_Vaksins = listPVaksin
			};
			return View(viewModel);
		}

		[HttpPost]
		public ActionResult Distributed(string ref_code, int id_rumahsakit, string[] no_registration)
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			try
			{
				BPOMServiceClient ob = new BPOMServiceClient();
				List<RS_Vaksin> listRS = new List<RS_Vaksin>();
				BPOMServiceReference.Produsen curP = ob.GetProdusen(Session["username"].ToString());
				foreach (string noreg in no_registration)
				{
					Produsen_Vaksin PDetail = ob.GetProdusenVaksinDetails(curP.id_produsen.ToString(), noreg);
					RS_Vaksin data = new RS_Vaksin
					{
						created_at = DateTime.Now,
						id_produsen = curP.id_produsen,
						id_rumahsakit = id_rumahsakit,
						ref_code = ref_code,
						packaging = PDetail.packaging,
						total = PDetail.total,
						no_registration = noreg,
						isReported = 0
					};
					Produsen_Vaksin data2 = new Produsen_Vaksin
					{
						created_at = DateTime.Now,
						id_produsen = curP.id_produsen,
						no_registration = noreg,
						total = 0,
						packaging = PDetail.packaging,
						status = "Was Distributed"
					};
					listRS.Add(data);
					ob.UpdateProdusenVaksin(data2);
				}
				ob.AddListRSVaksin(listRS.ToArray());
				//delete after send to RS
				return RedirectToRoute("Produsen", new { controller = "HomeProdusen", action = "Index" });
			}
			catch
			{
				throw new Exception();
			}
		}
	}
}