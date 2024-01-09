using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackingVaksinApp.BPOMServiceReference;

namespace TrackingVaksinApp.Controllers.BPOM
{
    public class BPOMVaksinController : Controller
    {
		// GET: BPOMVaksin
		
		[HttpGet]
		public ActionResult Index()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			Models.ListViewModel viewmodel = new Models.ListViewModel
			{
				bpom_vaksins = ob.GetVaksin(),
				produsens = ob.GetListProdusen()
			};
			return View(viewmodel);
		}

		[HttpPost]
		public ActionResult Index(string no_registration, DateTime created_at, string status)
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			Produsen_Vaksin newDataProdusen = null;
			Produsen_Vaksin cekStatus = ob.GetProdusenVaksinDetailsByNoRegistration(no_registration);
			BPOM_Vaksin newData = new BPOM_Vaksin
			{
				created_at = created_at,
				no_registration = no_registration,
				packaging = cekStatus.packaging,
				total = cekStatus.total,
				status = status
			};
			if (cekStatus != null)
			{
				if (!cekStatus.status.Equals("Not Reported"))
				{
					newDataProdusen = new Produsen_Vaksin
					{
						created_at = created_at,
						no_registration = no_registration,
						packaging = cekStatus.packaging,
						total = cekStatus.total,
						status = status
					};
					ob.UpdateProdusenVaksin(newDataProdusen);
				}
			}
			ob.UpdateVaksin(newData);
			Models.ListViewModel viewmodel = new Models.ListViewModel
			{
				bpom_vaksins = ob.GetVaksin(),
				produsens = ob.GetListProdusen()
			};
			return View(viewmodel);
		}

		public ActionResult CheckLogArrivalVaccine()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			Models.ListViewModel viewmodel = new Models.ListViewModel
			{
				LogsArrivals = ob.GetLogArrivalVaccine(),
				produsens = ob.GetListProdusen(),
				rumahSakits = ob.GetListRumahSakit(),
				rsVaksins = ob.GetVaksinRS()
			};
			return View(viewmodel);
		}

		public ActionResult CheckLogVaccineUse()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			Models.ListViewModel viewmodel = new Models.ListViewModel
			{
				logUses = ob.GetLogVaccineUse(),
				masyarakts = ob.GetListMasyarakat(),
				pasiens = ob.GetListPasien(),
				produsens = ob.GetListProdusen(),
				rumahSakits = ob.GetListRumahSakit(),
				rsVaksins = ob.GetVaksinRS()
			};
			return View(viewmodel);
		}
	}
}