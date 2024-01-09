using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrackingVaksinApp.BPOMServiceReference;

namespace TrackingVaksinApp.Controllers.RS
{
    public class HomeRSController : Controller
    {
		// GET: HomeRS
		public ActionResult Index()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			BPOMServiceReference.RumahSakit curR = ob.GetRS(Session["username"].ToString());
			return View(ob.GetVaksinRS().Where(m => m.id_rumahsakit == curR.id));
		}

		public ActionResult CekVaksin()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			if (this.ControllerContext.RouteData.Values["id"] != null)
			{
				string no_reg = this.ControllerContext.RouteData.Values["id"].ToString();
				Models.ListViewModel viewmodel = new Models.ListViewModel
				{
					singlevaksin = ob.GetVaksinDetails(no_reg),
					produsens = ob.GetListProdusen()
				};
				return View(viewmodel);
			}
			Models.ListViewModel viewmodel2 = new Models.ListViewModel
			{
				singlevaksin = null,
				produsens = null
			};
			return View(viewmodel2);
		}

		[HttpPost]
		public ActionResult CekVaksin(string no_registration)
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			Models.ListViewModel viewmodel = new Models.ListViewModel
			{
				singlevaksin = ob.GetVaksinDetails(no_registration),
				produsens = ob.GetListProdusen()
			};
			return View(viewmodel);
		}

		public ActionResult CekReceiveVaksin()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			BPOMServiceReference.RumahSakit curR = ob.GetRS(Session["username"].ToString());
			return View(ob.GetVaksinRS().Where(m => m.id_rumahsakit == curR.id));
		}
		
		public ActionResult ReportArrivalVaccine()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			string noreg = this.ControllerContext.RouteData.Values["id"].ToString();
			BPOMServiceReference.RumahSakit curR = ob.GetRS(Session["username"].ToString());
			RS_Vaksin newData = ob.GetVaksinRSDetails(noreg);
			BPOM_Log_Kedatangan_Vaksin dataAdd = new BPOM_Log_Kedatangan_Vaksin
			{
				id_produsen = newData.id_produsen,
				id = curR.id,
				created_at = newData.created_at,
				no_registration = newData.no_registration,
			};

			newData.isReported = 1;
			if (ob.ReportArrivalVaccine(dataAdd).statusCode.Equals(HttpStatusCode.OK.ToString()))
			{
				if (ob.UpdateRSVaksin(newData) != null)
				{
					ViewBag.Err = ob.ReportArrivalVaccine(dataAdd).Message.ToString();
					return RedirectToRoute("RS", new { controller = "HomeRS", action = "CekReceiveVaksin" });
				}
			}
			return RedirectToRoute("RS", new { controller = "HomeRS", action = "CekReceiveVaksin", Err = ob.ReportArrivalVaccine(dataAdd).Description.ToString() });
		}

		public ActionResult ReportAllArrivalVaccine()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			BPOMServiceReference.RumahSakit curR = ob.GetRS(Session["username"].ToString());
			if (ob.ReportListArrivalVaccine(ob.GetLogArrivalVaccine()))
			{
				IEnumerable<RS_Vaksin> listAll = ob.GetVaksinRS().Where(m => m.id_rumahsakit == curR.id && m.isReported == 0).ToList();
				List<RS_Vaksin> newList = new List<RS_Vaksin>();
				List<BPOM_Log_Kedatangan_Vaksin> LogBaru = new List<BPOM_Log_Kedatangan_Vaksin>();
				foreach (var data in listAll)
				{
					BPOM_Log_Kedatangan_Vaksin singleData = new BPOM_Log_Kedatangan_Vaksin
					{
						id_produsen = data.id_produsen,
						id = curR.id,
						created_at = data.created_at,
						no_registration = data.no_registration
					};
					LogBaru.Add(singleData);
				}
				if (ob.ReportListArrivalVaccine(LogBaru.ToArray()))
				{
					foreach (var data in listAll)
					{
						data.isReported = 1;
						newList.Add(data);
					}
					if (ob.UpdateListRSVaksin(newList.ToArray()))
						return RedirectToRoute("RS", new { controller = "HomeRS", action = "CekReceiveVaksin" });
				}
			}
			return RedirectToAction("CekReceiveVaksin");
		}

		public ActionResult ReportByKodeReff(string ref_code)
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			BPOMServiceReference.RumahSakit curR = ob.GetRS(Session["username"].ToString());
			IEnumerable<RS_Vaksin> listVaksinbyRef = ob.GetVaksinRS().Where(m => m.ref_code.Equals(ref_code));
			List<BPOM_Log_Kedatangan_Vaksin> LogBaru = new List<BPOM_Log_Kedatangan_Vaksin>();
			foreach (var data in listVaksinbyRef)
			{
				BPOM_Log_Kedatangan_Vaksin singleData = new BPOM_Log_Kedatangan_Vaksin
				{
					id_produsen = data.id_produsen,
					id = curR.id,
					created_at = data.created_at,
					no_registration = data.no_registration
				};
				LogBaru.Add(singleData);
			}
			if (ob.ReportListArrivalVaccine(LogBaru.ToArray()))
			{
				List<RS_Vaksin> newList = new List<RS_Vaksin>();
				foreach (var data in listVaksinbyRef)
				{
					data.isReported = 1;
					newList.Add(data);
				}
				if (ob.UpdateListRSVaksin(newList.ToArray()))
					return RedirectToAction("CekReceiveVaksin");
			}
			return RedirectToAction("CekReceiveVaksin");

		}

		public ActionResult AlokasiPenggunaanVaksin()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			BPOMServiceReference.RumahSakit curR = ob.GetRS(Session["username"].ToString());
			Models.ListViewModel viewmodel = new Models.ListViewModel
			{
				pasiens = ob.GetListPasien().Where(m => m.id_rumahsakit == curR.id),
				rsVaksins = ob.GetVaksinRS().Where(m => m.id_rumahsakit == curR.id),
				rumahSakits = ob.GetListRumahSakit()
			};
			return View(viewmodel);
		}

		public ActionResult CheckNik()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			if (this.ControllerContext.RouteData.Values["id"] != null)
			{
				string nik = this.ControllerContext.RouteData.Values["id"].ToString();
				Models.ListViewModel viewmodel = new Models.ListViewModel
				{
					singlemasyarakat = ob.GetMasyarakatByNik(nik)
				};
				return View(viewmodel);
			}
			Models.ListViewModel viewmodel2 = new Models.ListViewModel
			{
				singlemasyarakat = null
			};
			return View(viewmodel2);
		}

		[HttpPost]
		public ActionResult CheckNik(string nik)
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			Models.ListViewModel viewmodel = new Models.ListViewModel
			{
				singlemasyarakat = ob.GetMasyarakatByNik(nik)
			};
			ViewBag.tempNik = nik;
			return View(viewmodel);
		}


		public ActionResult TambahAlokasiVaksin()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			BPOMServiceReference.RumahSakit curR = ob.GetRS(Session["username"].ToString());
			var query = (from M in ob.GetListMasyarakat()
						 join P in ob.GetListPasien() on M.nik equals P.nik into subset
						 from sc in subset.DefaultIfEmpty()
						 where sc == null
						 select M).Distinct().ToList();
			Models.ListViewModel viewmodel = new Models.ListViewModel
			{
				rsVaksins = ob.GetVaksinRS().Where(m => m.id_rumahsakit == curR.id),
				masyarakts = query
			};
			return View(viewmodel);
		}

		[HttpPost]
		public ActionResult TambahAlokasiVaksin(string no_rek_medis, string nik, string no_registration)
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			if (no_rek_medis == null)
			{
				ViewBag.EmptyNoReg = "No Rek is Empty";
			}
			if (nik == null)
			{
				ViewBag.NIK = "NIK is Empty";
			}
			if (no_registration == null)
			{
				ViewBag.idRS = "Select any RS";
			}
			RS_Vaksin vaksinData = ob.GetVaksinRSDetails(no_registration);
			Pasien data = new Pasien
			{
				no_rek_medis = no_rek_medis,
				nik = nik,
				no_registration = no_registration,
				id_rumahsakit = vaksinData.id_rumahsakit,
				isReported = 0,
				created_at = DateTime.Now
			};
			if (ob.AddPasien(data) != null)
			{
				vaksinData.total -= 1;
				ob.UpdateRSVaksin(vaksinData);
				return RedirectToAction("AlokasiPenggunaanVaksin");
			}
			return View();
		}

		public ActionResult LaporPenggunaanVaksin()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			string no_rekMedis = this.ControllerContext.RouteData.Values["id"].ToString();
			Pasien pasienByNoRek = ob.GetPasienByNoRek(no_rekMedis);
			BPOM_Log_Penggunaan_Vaksin data = new BPOM_Log_Penggunaan_Vaksin
			{
				no_rek_medis = pasienByNoRek.no_rek_medis,
				no_registration = pasienByNoRek.no_registration,
				created_at = DateTime.Now,
				id_rumahsakit = pasienByNoRek.id_rumahsakit,
				nik = pasienByNoRek.nik,
			};
			if (pasienByNoRek != null)
			{
				if (ob.ReportVaccineUse(data).statusCode.Equals(HttpStatusCode.Accepted.ToString()))
				{
					pasienByNoRek.isReported = 1;
					if (ob.EditPasien(pasienByNoRek))
					{
						return RedirectToAction("AlokasiPenggunaanVaksin");
					}
				}
			}
			Session["InValidNik"] = "Masyarakat dengan Nik " + pasienByNoRek.nik + " Tidak ditemukan";
			//return RedirectToRoute("RS", new { controller = "HomeRS", action = "CheckNik", id = pasienByNoRek.NIK });
			return RedirectToRoute("RS", new { controller = "HomeRS", action = "AlokasiPenggunaanVaksin" });
		}

		public ActionResult ReportVaccineUseByKodeReff(string ref_code)
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			IEnumerable<BPOM_Log_Penggunaan_Vaksin> listVaksinbyRef = ob.GetLogVaccineUse().Where(X => X.no_registration.Equals(ref_code));
			List<BPOM_Log_Penggunaan_Vaksin> LogBaru = new List<BPOM_Log_Penggunaan_Vaksin>();
			List<Pasien> newList = new List<Pasien>();
			foreach (var data in listVaksinbyRef)
			{
				BPOM_Log_Penggunaan_Vaksin singleData = new BPOM_Log_Penggunaan_Vaksin
				{
					no_rek_medis = data.no_rek_medis,
					no_registration = data.no_registration,
					created_at = DateTime.Now,
					id = data.id,
					nik = data.nik,
				};
				LogBaru.Add(singleData);
			}
			if (ob.ReportListVaccineUse(LogBaru.ToArray()))
			{
				foreach (var data in listVaksinbyRef)
				{
					Pasien temp = new Pasien();
					temp.isReported = 1;
					newList.Add(temp);
				}
				if (ob.EditListPasien(newList.ToArray()))
					return RedirectToAction("AlokasiPenggunaanVaksin");
			}
			return RedirectToAction("AlokasiPenggunaanVaksin");
		}

		public ActionResult ReportAllVaccineUse()
		{
			if (Session["username"] == null)
				return RedirectToRoute("Authentication", new { controller = "User" });
			BPOMServiceClient ob = new BPOMServiceClient();
			BPOMServiceReference.RumahSakit curR = ob.GetRS(Session["username"].ToString());
			if (ob.ReportListArrivalVaccine(ob.GetLogArrivalVaccine()))
			{
				IEnumerable<Pasien> listAll = ob.GetListPasien().Where(m => m.id_rumahsakit == curR.id && m.isReported == 0).ToList();
				List<Pasien> newList = new List<Pasien>();
				List<BPOM_Log_Penggunaan_Vaksin> LogBaru = new List<BPOM_Log_Penggunaan_Vaksin>();
				foreach (var data in listAll)
				{
					BPOM_Log_Penggunaan_Vaksin singleData = new BPOM_Log_Penggunaan_Vaksin
					{
						no_rek_medis = data.no_rek_medis,
						no_registration = data.no_registration,
						created_at = DateTime.Now,
						id_rumahsakit = data.id_rumahsakit,
						nik = data.nik,
					};
					LogBaru.Add(singleData);
				}
				if (ob.ReportListVaccineUse(LogBaru.ToArray()))
				{
					foreach (var data in listAll)
					{
						data.isReported = 1;
						newList.Add(data);
					}
					if (ob.EditListPasien(newList.ToArray()))
						return RedirectToRoute("RS", new { controller = "HomeRS", action = "AlokasiPenggunaanVaksin" });
				}
			}
			return RedirectToRoute("RS", new { controller = "HomeRS", action = "AlokasiPenggunaanVaksin" });
		}
	}
}