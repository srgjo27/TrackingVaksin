using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackingVaksinApp.BPOMServiceReference;

namespace TrackingVaksinApp.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			BPOMServiceClient ob = new BPOMServiceClient();
			Models.ListViewModel viewmodel = new Models.ListViewModel
			{
				bpom_vaksins = ob.GetVaksin(),
				produsens = ob.GetListProdusen()
			};

			ViewBag.CNT = ResultArrival().Count();
			return View(viewmodel);
		}

		[HttpPost]
		public ActionResult Index(string kat_input, string value_input)
		{
			BPOMServiceClient ob = new BPOMServiceClient();
			Session["InSearch"] = "True";
			IEnumerable<BPOM_Vaksin> list = new List<BPOM_Vaksin>();
			if (kat_input.Length < 1 || value_input.Length < 1)
			{
				list = ob.GetVaksin();
			}
			else
			{
				if (kat_input.Equals("0"))
				{
					list = ob.GetVaksin().Where(m => m.no_registration.Equals(value_input)).ToList();
				}
				else if (kat_input.Equals("1"))
				{
					BPOMServiceReference.Produsen produsen = ob.GetListProdusen().FirstOrDefault(m => m.name.Equals(value_input));
					if (produsen != null)
						list = ob.GetVaksin().Where(m => m.id_produsen == produsen.id_produsen).ToList();
					else
						list = null;
				}
			}
			Models.ListViewModel viewmodel = new Models.ListViewModel
			{
				bpom_vaksins = list,
				produsens = ob.GetListProdusen()
			};
			return View(viewmodel);
		}

		public ActionResult VisualizeProdusenVaksinResult()
		{
			return Json(Result(), JsonRequestBehavior.AllowGet);
		}

		public ActionResult VisualizeArrivalVaccine()
		{
			return Json(ResultArrival(), JsonRequestBehavior.AllowGet);
		}


		public List<Models.ProdusenVisualization> Result()
		{
			BPOMServiceClient ob = new BPOMServiceClient();
			List<Models.ProdusenVisualization> list = new List<Models.ProdusenVisualization>();
			foreach (var data in ob.GetListProdusen())
			{
				list.Add(new Models.ProdusenVisualization
				{
					Name = data.name,
					ProdukCount = ob.GetVaksin().Where(m => m.id_produsen == data.id_produsen).Count()
				});
			}
			return list;
		}

		public List<Models.ArrivalVaccineVisualization> ResultArrival()
		{
			BPOMServiceClient ob = new BPOMServiceClient();
			List<Models.ArrivalVaccineVisualization> list = new List<Models.ArrivalVaccineVisualization>();
			var data = (
					from LGA in ob.GetLogArrivalVaccine()
					join R in ob.GetListRumahSakit()
					on LGA.id_rumahsakit equals R.id
					group new { R } by new
					{
						R.name
					}
					into LGAR
					select new
					{
						RSName = LGAR.Key.name,
						VaccineCount = LGAR.Count()
					}
				).AsEnumerable();
			foreach (var newdata in data)
			{
				list.Add(new Models.ArrivalVaccineVisualization
				{
					RS_Name = newdata.RSName,
					Vaccine_count = newdata.VaccineCount
				});
			}
			return list;
		}
	}
}