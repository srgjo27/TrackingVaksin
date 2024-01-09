using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TrackingVaksinService.Helper;

namespace TrackingVaksinService
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
	public class BPOMService : IBPOMService
	{
		// TODO : Add your service operations here

		// Start : Authentication
		public Account Register(Account data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			try
			{
				if (data == null)
					return null;
				if (GetAccount(data.username) != null)
					return null;
				db.Account.Add(data);
				db.SaveChanges();
				db.Dispose();
				return data;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public bool Login(string username, string password)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			Account getAccount = db.Account.FirstOrDefault(m => m.username == username && m.password == password);
			if (getAccount != null)
			{
				db.Dispose();
				return true;
			}
			return false;
		}

		public Account GetAccount(string username)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			Account getAccount = db.Account.FirstOrDefault(m => m.username == username);
			return getAccount;
		}
		// End : Authentication

		// BPOM Vaksin
		// Start
		public BPOM GetBPOM(string username)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			BPOM getData = db.BPOM.FirstOrDefault(m => m.username == username);
			if (getData != null)
				return getData;
			return null;
		}

		public IEnumerable<BPOM_Vaksin> GetVaksin()
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			IEnumerable<BPOM_Vaksin> listVaksin = db.BPOM_Vaksin.ToList();
			return listVaksin;
		}

		public BPOM_Vaksin GetVaksinDetails(string no_registration)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			BPOM_Vaksin getVaksinByNoReg = db.BPOM_Vaksin.FirstOrDefault(m => m.no_registration == no_registration);
			return getVaksinByNoReg;
		}

		public BPOM_Vaksin Up(BPOM_Vaksin data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			data.created_at = DateTime.Now;

			if (data != null)
			{
				if (db.BPOM_Vaksin.FirstOrDefault(m => m.no_registration == data.no_registration) != null)
				{
					return null;
				}
				db.BPOM_Vaksin.Add(data);
				db.SaveChanges();
				db.Dispose();
				return data;
			}
			else
			{
				db.Dispose();
				return null;
			}
		}

		public BPOM_Vaksin UpdateVaksin(BPOM_Vaksin data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			if (data != null)
			{
				BPOM_Vaksin dtVaksin = db.BPOM_Vaksin.FirstOrDefault(m => m.no_registration == data.no_registration);
				data.created_at = DateTime.Now;
				if (dtVaksin != null)
				{
					dtVaksin.status = data.status;
					db.SaveChanges();
					db.Dispose();
					return dtVaksin;
				}
				else
					throw new Exception("Data number registration not found");
			}
			return null;
		}

		public Feedback DeleteVaksin(string no_registration)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			BPOM_Vaksin dtVaksin = db.BPOM_Vaksin.FirstOrDefault(m => m.no_registration == no_registration);
			if (dtVaksin != null)
			{
				db.BPOM_Vaksin.Remove(dtVaksin);
				db.SaveChanges();
				return new Feedback { statusCode = HttpStatusCode.OK.ToString(), Description = "Data has been deleted" };
			}
			else
			{
				return new Feedback
				{
					statusCode = HttpStatusCode.NotFound.ToString(),
					Description = "Failed to delete data",
					Message = "No Registration avaible on record"
				};
			}
		}
		// End

		// Produsen Vaksin
		public IEnumerable<Produsen_Vaksin> GetProdusenVaksin(string id_produsen)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			int id = int.Parse(id_produsen);
			IEnumerable<Produsen_Vaksin> list = db.Produsen_Vaksin.Where(m => m.id_produsen == id).ToList();
			return list;
		}

		public Produsen_Vaksin GetProdusenVaksinDetails(string id_produsen, string no_registration)
		{
			IEnumerable<Produsen_Vaksin> list = GetProdusenVaksin(id_produsen);
			Produsen_Vaksin dataByNoReg = list.FirstOrDefault(m => m.no_registration == no_registration);
			return dataByNoReg;
		}

		public Produsen_Vaksin GetProdusenVaksinDetailsByNoRegistration(string no_registration)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			Produsen_Vaksin dataByNoReg = db.Produsen_Vaksin.FirstOrDefault(m => m.no_registration == no_registration);
			return dataByNoReg;
		}

		public Produsen GetProdusen(string username)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			Produsen getData = db.Produsen.FirstOrDefault(m => m.username == username);
			if (getData != null)
				return getData;
			return null;
		}

		public IEnumerable<Produsen> GetListProdusen()
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			IEnumerable<Produsen> list = db.Produsen.ToList();
			return list;
		}

		public Produsen AddProdusen(Produsen data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			if (data != null)
			{
				db.Produsen.Add(data);
				db.SaveChanges();
				db.Dispose();
				return data;
			}
			return null;
		}

		public Produsen GetProdusenById(string id_produsen)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			int id = int.Parse(id_produsen);
			Produsen getData = db.Produsen.FirstOrDefault(m => m.id_produsen == id);
			if (getData != null)
				return getData;
			return null;
		}

		public Produsen_Vaksin AddProdusenVaksin(Produsen_Vaksin data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			data.created_at = DateTime.Now;
			if (data != null)
			{
				if (db.Produsen_Vaksin.FirstOrDefault(m => m.no_registration == data.no_registration) != null)
				{
					return null;
				}
				db.Produsen_Vaksin.Add(data);
				db.SaveChanges();
				db.Dispose();
				return data;
			}
			else
			{
				db.Dispose();
				return null;
			}
		}

		public Produsen_Vaksin UpdateProdusenVaksin(Produsen_Vaksin data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			if (data != null)
			{
				Produsen_Vaksin dtVaksin = db.Produsen_Vaksin.FirstOrDefault(m => m.no_registration == data.no_registration);
				data.created_at = DateTime.Now;
				if (dtVaksin != null)
				{
					dtVaksin.status = data.status;
					dtVaksin.packaging = data.packaging;
					dtVaksin.total = data.total;
					db.SaveChanges();
					db.Dispose();
					return dtVaksin;
				}
				else
					throw new Exception("Data number registration not found");
			}
			return null;
		}

		public Feedback DeleteProdusenVaksin(string no_registration)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			Produsen_Vaksin dtVaksin = db.Produsen_Vaksin.FirstOrDefault(m => m.no_registration == no_registration);
			if (dtVaksin != null)
			{
				db.Produsen_Vaksin.Remove(dtVaksin);
				db.SaveChanges();
				return new Feedback { statusCode = HttpStatusCode.OK.ToString(), Description = "Data has been deleted" };
			}
			else
				return new Feedback
				{
					statusCode = HttpStatusCode.NotFound.ToString(),
					Description = "Failed to delete data",
					Message = "No Registration avaible on record"
				};
		}

		public bool DeleteListProdusenVaksin(IEnumerable<Produsen_Vaksin> list)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			try
			{
				db.Produsen_Vaksin.RemoveRange(list);
				db.SaveChanges();
				db.Dispose();
				return true;
			}
			catch
			{
				return false;
			}
		}
		// End

		// BPOM Service for RS
		public RumahSakit AddRS(RumahSakit data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			if (data != null)
			{
				db.RumahSakit.Add(data);
				db.SaveChanges();
				db.Dispose();
				return data;
			}
			return null;
		}

		public RumahSakit GetRS(string username)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			RumahSakit getData = db.RumahSakit.FirstOrDefault(m => m.username == username);
			if (getData != null)
				return getData;
			return null;
		}

		public IEnumerable<RS_Vaksin> GetVaksinRS()
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			IEnumerable<RS_Vaksin> listRS = db.RS_Vaksin.ToList();
			return listRS;
		}

		public RS_Vaksin GetVaksinRSDetails(string no_registration)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			RS_Vaksin getData = db.RS_Vaksin.FirstOrDefault(m => m.no_registration == no_registration);
			return getData;
		}

		public RS_Vaksin AddRSVaksin(RS_Vaksin data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			if (data != null)
			{
				db.RS_Vaksin.Add(data);
				db.SaveChanges();
				db.Dispose();
				return data;
			}
			return null;
		}

		public bool AddListRSVaksin(IEnumerable<RS_Vaksin> list)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			try
			{
				db.RS_Vaksin.AddRange(list);
				db.SaveChanges();
				db.Dispose();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public RS_Vaksin UpdateRSVaksin(RS_Vaksin data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			if (data != null)
			{
				RS_Vaksin dtVaksin = db.RS_Vaksin.FirstOrDefault(m => m.no_registration == data.no_registration);
				data.created_at = DateTime.Now;
				if (dtVaksin != null)
				{
					dtVaksin.ref_code = data.ref_code;
					dtVaksin.packaging = data.packaging;
					dtVaksin.total = data.total;
					dtVaksin.isReported = data.isReported;
					db.SaveChanges();
					db.Dispose();
					return dtVaksin;
				}
				else
					throw new Exception("Data number registration not found");
			}
			return null;
		}

		public bool UpdateListRSVaksin(IEnumerable<RS_Vaksin> list)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			if (list != null)
			{
				foreach (RS_Vaksin data in list)
				{
					RS_Vaksin newVaksin = db.RS_Vaksin.FirstOrDefault(m => m.no_registration == data.no_registration);
					newVaksin.id_produsen = data.id_produsen;
					newVaksin.ref_code = data.ref_code;
					newVaksin.isReported = data.isReported;
					newVaksin.packaging = data.packaging;
					newVaksin.total = data.total;
					db.SaveChanges();
				}
				return true;
			}
			else
				return false;
		}

		public Feedback DeleteRSVaksin(string no_registration)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			RS_Vaksin dtVaksin = db.RS_Vaksin.FirstOrDefault(m => m.no_registration == no_registration);
			if (dtVaksin != null)
			{
				db.RS_Vaksin.Remove(dtVaksin);
				db.SaveChanges();
				return new Feedback { statusCode = HttpStatusCode.OK.ToString(), Description = "Data has been deleted" };
			}
			else
			{
				return new Feedback
				{
					statusCode = HttpStatusCode.NotFound.ToString(),
					Description = "Failed to delete data",
					Message = "No Registration avaible on record"
				};
			}
		}

		public IEnumerable<RumahSakit> GetListRumahSakit()
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			IEnumerable<RumahSakit> listRS = db.RumahSakit.ToList();
			return listRS;
		}

		public RumahSakit GetDetailRumahSakit(string id)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			int idRS = int.Parse(id);
			RumahSakit getDataById = db.RumahSakit.FirstOrDefault(m => m.id == idRS);
			return getDataById;
		}

		public IEnumerable<BPOM_Log_Kedatangan_Vaksin> GetLogArrivalVaccine()
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			IEnumerable<BPOM_Log_Kedatangan_Vaksin> list = db.BPOM_Log_Kedatangan_Vaksin.ToList();
			return list;
		}

		public BPOM_Log_Kedatangan_Vaksin GetLogArrivalVaccineDetails(string no_registration)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			BPOM_Log_Kedatangan_Vaksin logByNoReg = db.BPOM_Log_Kedatangan_Vaksin.FirstOrDefault(m => m.no_registration == no_registration);
			return logByNoReg;
		}

		public Feedback ReportArrivalVaccine(BPOM_Log_Kedatangan_Vaksin data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			data.created_at = DateTime.Now;
			Feedback fb = new Feedback();
			if (data != null)
			{
				if (db.BPOM_Vaksin.FirstOrDefault(m => m.no_registration == data.no_registration) != null)
				{
					if (db.BPOM_Log_Kedatangan_Vaksin.FirstOrDefault(m => m.no_registration == data.no_registration) != null)
					{
						db.Dispose();
						fb.statusCode = HttpStatusCode.BadRequest.ToString();
						fb.Description = "No Registration already exits";
						fb.Message = "Failde to record data";
						return fb;
					}
					db.BPOM_Log_Kedatangan_Vaksin.Add(data);
					db.SaveChanges();
					db.Dispose();
					fb.statusCode = HttpStatusCode.OK.ToString();
					fb.Message = "Record data success";
					return fb;
				}
				else
				{
					db.Dispose();
					fb.statusCode = HttpStatusCode.BadRequest.ToString();
					fb.Description = "No Registration not found";
					fb.Message = "Failde to record data";
					return fb;
				}
			}
			else
			{
				db.Dispose();
				return null;
			}
		}

		public bool ReportListArrivalVaccine(IEnumerable<BPOM_Log_Kedatangan_Vaksin> list)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			if (list.Count() != 0)
			{
				db.BPOM_Log_Kedatangan_Vaksin.AddRange(list);
				db.SaveChanges();
				db.Dispose();
				return true;
			}
			return false;
		}

		public IEnumerable<BPOM_Log_Penggunaan_Vaksin> GetLogVaccineUse()
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			IEnumerable<BPOM_Log_Penggunaan_Vaksin> list = db.BPOM_Log_Penggunaan_Vaksin.ToList();
			return list;
		}

		public BPOM_Log_Penggunaan_Vaksin GetLogVaccineUseDetails(string id)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			int idLog = int.Parse(id);
			BPOM_Log_Penggunaan_Vaksin log = db.BPOM_Log_Penggunaan_Vaksin.FirstOrDefault(m => m.id == idLog);
			return log;
		}

		public Feedback ReportVaccineUse(BPOM_Log_Penggunaan_Vaksin data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			data.created_at = DateTime.Now;
			Feedback fb = new Feedback();
			if (db.Masyarakat.FirstOrDefault(m => m.nik == data.nik) != null)
			{
				db.BPOM_Log_Penggunaan_Vaksin.Add(data);
				db.SaveChanges();
				db.Dispose();
				fb.statusCode = HttpStatusCode.OK.ToString();
				fb.Message = "Reported was recorded";
				return fb;
			}
			else
			{
				fb.statusCode = HttpStatusCode.BadRequest.ToString();
				fb.Description = "NIK not found";
				fb.Message = "Failed to record data";
				db.Dispose();
				return fb;
			}
		}

		public bool ReportListVaccineUse(IEnumerable<BPOM_Log_Penggunaan_Vaksin> list)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			if (list != null)
			{
				db.BPOM_Log_Penggunaan_Vaksin.AddRange(list);
				db.SaveChanges();
				db.Dispose();
				return true;
			}
			return false;
		}

		public bool UpdateListVaccineUse(IEnumerable<BPOM_Log_Penggunaan_Vaksin> list)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			if (list != null)
			{
				foreach (BPOM_Log_Penggunaan_Vaksin item in list)
				{
					BPOM_Log_Penggunaan_Vaksin log = db.BPOM_Log_Penggunaan_Vaksin.FirstOrDefault(m => m.no_rek_medis == item.no_rek_medis);
					log.nik = item.nik;
					log.no_rek_medis = item.no_rek_medis;
					log.no_registration = item.no_registration;
					log.id_rumahsakit = item.id_rumahsakit;
					db.SaveChanges();
					db.Dispose();
				}
				return true;
			}
			return false;
		}
		// End

		// Masyarakat
		public Masyarakat GetMasyarakat(string username)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			Masyarakat getData = db.Masyarakat.FirstOrDefault(m => m.username == username);
			if (getData != null)
				return getData;
			return null;
		}

		public Masyarakat GetMasyarakatByNik(string nik)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			Masyarakat getData = db.Masyarakat.FirstOrDefault(m => m.nik == nik);
			if (getData != null)
				return getData;
			return null;
		}

		public IEnumerable<Masyarakat> GetListMasyarakat()
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			IEnumerable<Masyarakat> list = db.Masyarakat.ToList();
			return list;
		}

		public Masyarakat AddMasyarakat(Masyarakat data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			if (data != null)
			{
				db.Masyarakat.Add(data);
				db.SaveChanges();
			}
			db.Dispose();
			return data;
		}

		public bool UpdateMasyarakat(Masyarakat data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			Masyarakat getData = db.Masyarakat.FirstOrDefault(m => m.nik == data.nik);
			if (getData != null)
			{
				getData.name = data.name;
				getData.gender = data.gender;
				getData.date_of_birth = data.date_of_birth;
				db.SaveChanges();
				db.Dispose();
				return true;
			}
			db.Dispose();
			return false;
		}

		public bool DeleteMasyarakat(string nik)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			Masyarakat getData = db.Masyarakat.FirstOrDefault(m => m.nik == nik);
			if (getData != null)
			{
				db.Masyarakat.Remove(getData);
				db.SaveChanges();
				db.Dispose();
				return true;
			}
			db.Dispose();
			return false;
		}
		// End

		// Pasien
		public IEnumerable<Pasien> GetListPasien()
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			IEnumerable<Pasien> list = db.Pasien.ToList();
			return list;
		}

		public Pasien GetPasienByNoRek(string no_rek_medis)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			Pasien getData = db.Pasien.FirstOrDefault(m => m.no_rek_medis == no_rek_medis);
			db.Dispose();
			return getData;
		}

		public Pasien AddPasien(Pasien data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			if (data != null)
			{
				db.Pasien.Add(data);
				db.SaveChanges();
				db.Dispose();
				return data;
			}
			return null;
		}

		public bool EditPasien(Pasien data)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			Pasien newData = db.Pasien.FirstOrDefault(m => m.no_rek_medis == data.no_rek_medis);
			if (newData != null)
			{
				newData.no_registration = data.no_registration;
				newData.id_rumahsakit = data.id_rumahsakit;
				newData.nik = data.nik;
				newData.isReported = data.isReported;
				db.SaveChanges();
				db.Dispose();
				return true;
			}
			db.Dispose();
			return false;
		}

		public bool EditListPasien(IEnumerable<Pasien> list)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			if (list != null)
			{
				foreach (Pasien data in list)
				{
					Pasien newData = db.Pasien.FirstOrDefault(m => m.no_registration == data.no_registration);
					newData.no_registration = data.no_registration;
					newData.id_rumahsakit = data.id_rumahsakit;
					newData.nik = data.nik;
					newData.isReported = data.isReported;
					db.SaveChanges();
					db.Dispose();
				}
				return true;
			}
			else
				return false;
		}

		public bool DeletePasien(string no_rek_medis)
		{
			TrackingVaksinEntities db = new TrackingVaksinEntities();
			Pasien getData = db.Pasien.FirstOrDefault(m => m.no_rek_medis == no_rek_medis);
			if (getData != null)
			{
				db.Pasien.Remove(getData);
				db.SaveChanges();
				db.Dispose();
				return true;
			}
			db.Dispose();
			return false;
		}
		// End
	}
}
