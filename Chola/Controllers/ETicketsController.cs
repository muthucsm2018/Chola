using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Chola.Models;
using WebMatrix.WebData;

namespace Chola.Controllers
{
    public class ETicketsController : Controller
    {
        #region CommonFunctions
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion

        #region ListETickets


        public List<ETicketPassengerListModel> GetETicketPassengers(int ETicketCode)
        {
            using (Entities context = new Entities())
            {

                    var trxnList = context.ETicket_Passengers.Where( s=> s.ETicketCode == ETicketCode).AsEnumerable().Select(dataRow => new ETicketPassengerListModel
                    {
                        ETicketCode = Shared.ToString(dataRow.ETicketCode),
                        PassengerCode = Shared.ToString(dataRow.PassengerCode),
                        PassengerName = dataRow.PassengerName,
                        PaxType = dataRow.PaxType,
                        TicketNo = dataRow.TicketNo

                    });
                    return trxnList.ToList();
            }
        }

        public List<ETicketItineryListModel> GetETicketItineries(int ETicketCode)
        {
            using (Entities context = new Entities())
            {

                var trxnList = context.ETicket_Itinery.Where(s => s.ETicketCode == ETicketCode).AsEnumerable().Select(dataRow => new ETicketItineryListModel
                {
                    ETicketCode = Shared.ToString(dataRow.ETicketCode),
                    ItineryCode = Shared.ToString(dataRow.ItineryCode),
                    Flight = dataRow.Flight,
                    FlightNo = dataRow.FlightNo,
                    Depart = dataRow.Depart,
                    DepartDate = string.Format("{0:ddd, MMM d, yyyy}", dataRow.DepartDate),
                    DepartTer = Shared.ToString(dataRow.DepartTer),
                    DepartTime = dataRow.DepartTime,
                    Arrive = dataRow.Arrive,
                    ArriveDate = string.Format("{0:ddd, MMM d, yyyy}", dataRow.ArriveDate),
                    ArriveTime = dataRow.ArriveTime,
                    ArriveTer = Shared.ToString(dataRow.ArriveTer),
                    Class = dataRow.Class,
                    Baggage = dataRow.Baggage

                });
                return trxnList.ToList();
            }
        }


        public JsonResult ETicketPassengersList(string ETicketCode)
        {
            try
            {
                List<ETicketPassengerListModel> query = GetETicketPassengers(Shared.ToInt(ETicketCode));

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.ETicketCode + "|" + x.PassengerCode, x.PassengerName, x.PaxType, x.TicketNo })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult ETicketItineriesList(string ETicketCode)
        {
            try
            {
                List<ETicketItineryListModel> query = GetETicketItineries(Shared.ToInt(ETicketCode));

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.ETicketCode + "|" + x.ItineryCode, x.Flight + " " + x.FlightNo, x.Depart, x.DepartDate + "</br>" + x.DepartTime + "</br>" + x.DepartTer, x.Arrive, x.ArriveDate + "</br>" + x.ArriveTime + "</br>" + x.ArriveTer, x.Class, x.Baggage })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        public ActionResult ListETickets(string Success)
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        ViewBag.Message = Success;                       
                    }
                    catch
                    {

                    }                   

                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        [HttpPost]
        public ActionResult AddETicketPassenger(string ETicketCode, string PassengerName, string PaxType, string TicketNo)
        {

            using (Entities context = new Entities())
            {
                try
                {
                    if (Shared.ToString(ETicketCode).Length > 0)
                    {
                        if (Shared.ToString(PassengerName).Length == 0)
                        {
                            return Json(new { success = false, response = "Passenger Name is Required!" });
                        }
                        if (Shared.ToString(TicketNo).Length == 0)
                        {
                            return Json(new { success = false, response = "Airline PNR is Required!" });
                        }
                       

                        ETicket_Passengers tbl = new ETicket_Passengers();
                        tbl.ETicketCode = Shared.ToInt(ETicketCode);
                        tbl.PassengerName = PassengerName.ToUpper();
                        tbl.PaxType = PaxType;
                        tbl.TicketNo = TicketNo.ToUpper();                       

                        context.ETicket_Passengers.Add(tbl);
                        context.SaveChanges();                       
                    }

                    return Json(new { success = true, response = "Successfully Added!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }

        }

        [HttpPost]
        public ActionResult DeleteETicketPassenger(string ETicketCode)
        {
            int? _ETicketcode = null;
            int? _passengercode = null;

            using (Entities context = new Entities())
            {
                try
                {
                    if (Shared.ToString(ETicketCode).Length > 0)
                    {
                        _ETicketcode = Shared.ToInt(ETicketCode.Split('|')[0]);
                        _passengercode = Shared.ToInt(ETicketCode.Split('|')[1]);

                        var qry = (from d in context.ETicket_Passengers
                                      where d.ETicketCode == _ETicketcode && d.PassengerCode == _passengercode
                                      select d).Single();
                        context.ETicket_Passengers.Remove(qry);
                        context.SaveChanges();
                    }

                    return Json(new { success = true, response = "Successfully Deleted!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }

        }

        [HttpPost]
        public ActionResult AddETicketItinery(string ETicketCode, string Flight, string FlightNo, string Depart, string DepartTer, DateTime? DepartDate, string DepartTime, string Arrive, string ArriveTer, DateTime? ArriveDate, string ArriveTime, string Class, string Baggage)
        {

            using (Entities context = new Entities())
            {
                try
                {
                    if (Shared.ToString(ETicketCode).Length > 0)
                    {
                        if (Shared.ToString(FlightNo).Length == 0)
                        {
                            return Json(new { success = false, response = "Flight No is Required!" });
                        }
                        if (Shared.ToString(DepartDate).Length == 0)
                        {
                            return Json(new { success = false, response = "Departure Date is Required!" });
                        }
                        if (Shared.ToString(DepartTime).Length == 0)
                        {
                            return Json(new { success = false, response = "Departure Time is Required!" });
                        }

                        if (Shared.ToString(ArriveDate).Length == 0)
                        {
                            return Json(new { success = false, response = "Arrive Date is Required!" });
                        }
                        if (Shared.ToString(ArriveTime).Length == 0)
                        {
                            return Json(new { success = false, response = "Arrive Time is Required!" });
                        }

                        ETicket_Itinery tbl = new ETicket_Itinery();
                        tbl.ETicketCode = Shared.ToInt(ETicketCode);
                        tbl.Flight = Flight;
                        tbl.FlightNo = FlightNo.ToUpper();
                        tbl.Depart = Depart;
                        tbl.DepartTer = DepartTer.ToUpper(); ;
                        tbl.DepartDate = DepartDate;
                        tbl.DepartTime = DepartTime.ToUpper();
                        tbl.Arrive = Arrive;
                        tbl.ArriveTer = ArriveTer.ToUpper(); ;
                        tbl.ArriveDate = ArriveDate;
                        tbl.ArriveTime = ArriveTime.ToUpper();
                        tbl.Class = Class;
                        tbl.Baggage = Baggage;

                        context.ETicket_Itinery.Add(tbl);
                        context.SaveChanges();
                    }

                    return Json(new { success = true, response = "Successfully Added!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }

        }

        [HttpPost]
        public ActionResult DeleteETicketItinery(string ETicketCode)
        {
            int? _ETicketcode = null;
            int? _itinerycode = null;

            using (Entities context = new Entities())
            {
                try
                {
                    if (Shared.ToString(ETicketCode).Length > 0)
                    {
                        _ETicketcode = Shared.ToInt(ETicketCode.Split('|')[0]);
                        _itinerycode = Shared.ToInt(ETicketCode.Split('|')[1]);

                        var qry = (from d in context.ETicket_Itinery
                                   where d.ETicketCode == _ETicketcode && d.ItineryCode == _itinerycode
                                   select d).Single();
                        context.ETicket_Itinery.Remove(qry);
                        context.SaveChanges();
                    }

                    return Json(new { success = true, response = "Successfully Deleted!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }

        }

        [HttpPost]
        public ActionResult GetETicketPassengerDetails(string ETicketCode)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    ETicketPassengerListModel model = new ETicketPassengerListModel();

                    model = context.ETicket_Passengers.Where(s => s.ETicketCode == Shared.ToInt(ETicketCode)).Select(dataRow => new ETicketPassengerListModel
                    {
                        PassengerName = dataRow.PassengerName,
                        PaxType = dataRow.PaxType,
                        TicketNo = dataRow.TicketNo
                    }).SingleOrDefault();

                    if (model != null)
                    {
                        return Json(new { success = true, response = model });
                    }
                    else
                        return Json(new { success = true, response = " " });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }
        }

        [HttpPost]
        public ActionResult GetETicketItineryDetails(string ETicketCode)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    ETicketItineryListModel model = new ETicketItineryListModel();

                    model = context.ETicket_Itinery.Where(s => s.ETicketCode == Shared.ToInt(ETicketCode)).Select(dataRow => new ETicketItineryListModel
                    {
                        Flight = dataRow.Flight,
                        FlightNo = dataRow.FlightNo,
                        Depart = dataRow.Depart,
                        DepartDate = Shared.ToString(dataRow.DepartDate),
                        DepartTime = dataRow.DepartTime,
                        DepartTer = dataRow.DepartTer,
                        Arrive = dataRow.Arrive,
                        ArriveDate = Shared.ToString(dataRow.ArriveDate),
                        ArriveTime = dataRow.ArriveTime,
                        ArriveTer = dataRow.ArriveTer,
                        Class = dataRow.Class,
                        Baggage = dataRow.Baggage
                    }).SingleOrDefault();

                    if (model != null)
                    {
                        return Json(new { success = true, response = model });
                    }
                    else
                        return Json(new { success = true, response = " " });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }
        }

        private DataSet ReturnQueryETicket(string CustomerName, string AirlinePNR)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[5];
                sqlParams[0] = new SqlParameter("@ETicketCode", SqlDbType.Int);
                sqlParams[0].Value = DBNull.Value;
                sqlParams[1] = new SqlParameter("@CustomerName", SqlDbType.NVarChar, 256);
                if (Shared.ToString(CustomerName).Length > 0)
                    sqlParams[1].Value = CustomerName;
                else
                    sqlParams[1].Value = DBNull.Value;
               
                sqlParams[2] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[2].Value = WebSecurity.CurrentUserId;
                sqlParams[3] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[3].Value = "ListETickets";
                sqlParams[4] = new SqlParameter("@AirlinePNR", SqlDbType.NVarChar, 50);
                if (Shared.ToString(AirlinePNR).Length > 0)
                    sqlParams[4].Value = Shared.ToString(AirlinePNR);
                else
                    sqlParams[4].Value = DBNull.Value;
                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_ETickets", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<ETicketListModel> GetListETicket(string CustomerName, string AirlinePNR)
        {
            DataSet ds = ReturnQueryETicket(CustomerName, AirlinePNR);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new ETicketListModel
                {
                    ETicketCode = Shared.ToString(dataRow.Field<Int32>("ETicketCode")),
                    CustomerName = dataRow.Field<string>("CustomerName"),
                    Prefix = dataRow.Field<string>("Prefix"),
                    AirlinePNR = dataRow.Field<string>("AirlinePNR"),
                    RefNo = dataRow.Field<string>("RefNo"),
                    CreatedBy = dataRow.Field<string>("CreatedBy"),
                    UpdatedDate = dataRow.Field<string>("UpdatedDate")
                });
                return trxnList.ToList();
            }
            else

                return new List<ETicketListModel>();
        }

        public JsonResult QueryETicketList(string CustomerName, string AirlinePNR)
        {
            try
            {
                List<ETicketListModel> query = GetListETicket(CustomerName, AirlinePNR);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.ETicketCode, x.Prefix + x.CustomerName , x.AirlinePNR, x.RefNo, x.CreatedBy, x.UpdatedDate, x.ETicketCode, x.ETicketCode })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult Invoice(string ETicketCode)
        {
            using (Entities context = new Entities())
            {
                int? _ETicketCode = Shared.ToInt(ETicketCode);
                var ETicket = (from c in context.ETickets where c.ETicketCode == _ETicketCode select c).FirstOrDefault();

                ViewBag.CustomerName = Shared.ToString(ETicket.Prefix) + Shared.ToString(ETicket.CustomerName);
                ViewBag.AirlinePNR = Shared.ToString(ETicket.AirlinePNR);
                ViewBag.RefNo = Shared.ToString(ETicket.RefNo);

                //ViewBag.DueDate = Shared.ToString(ETicket.DueDate.Value.ToString("dd MMM yyyy"));

                var psgrList = context.ETicket_Passengers.Where(s => s.ETicketCode == _ETicketCode).AsEnumerable().Select(dataRow => new ETicketPassengerListModel
                {
                    PassengerName = dataRow.PassengerName,
                    PaxType = dataRow.PaxType,
                    TicketNo = dataRow.TicketNo
                }).ToList();

                ViewBag.Passengers = psgrList;

                var itiList = context.ETicket_Itinery.Where(s => s.ETicketCode == _ETicketCode).AsEnumerable().Select(dataRow => new ETicketItineryListModel
                {
                    Flight = dataRow.FlightNo,
                    Depart = dataRow.Depart + "</br>" + string.Format("{0:ddd, MMM d, yyyy}", dataRow.DepartDate) + "</br>" + dataRow.DepartTime + "</br>" + " - " + dataRow.DepartTer,
                    DepartDate = string.Format("{0:ddd, MMM d, yyyy}", dataRow.DepartDate),
                    DepartTime = dataRow.DepartTime,
                    Arrive = dataRow.Arrive + "</br>" + string.Format("{0:ddd, MMM d, yyyy}", dataRow.ArriveDate) + "</br>" + dataRow.ArriveTime + "</br>" + " - " + dataRow.ArriveTer,
                    ArriveDate = string.Format("{0:ddd, MMM d, yyyy}", dataRow.ArriveDate),
                    ArriveTime = dataRow.ArriveTime,
                    Class = dataRow.Class,
                    Baggage = dataRow.Baggage

                }).ToList();

                ViewBag.Itineries = itiList;
                ViewBag.Flight = context.ETicket_Itinery.Where(s => s.ETicketCode == _ETicketCode).AsEnumerable().Select(a => a.Flight).FirstOrDefault();
                return View();
            }
        }

        #endregion

        #region CreateEditETicket
        public ActionResult CreateETicket(string Success)
        {
            using (Entities context = new Entities())
            {
                ETicketModel model = new ETicketModel();
               
                int maxId = 0;
                var _maxId = "";
                var count = context.ETickets.AsEnumerable().Count();
                if (count > 0)
                {
                    try
                    {
                        maxId = context.ETickets.Max(p => p.ETicketCode);
                        _maxId = DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString("D2") + (Shared.ToInt(count) + 1).ToString("D4") + Shared.ToString(WebSecurity.CurrentUserId);
                    }

                    catch (Exception ex)
                    {
                        maxId = 0;
                    }
                }
                else
                {
                    _maxId = DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString("D2") + "0001" + Shared.ToString(WebSecurity.CurrentUserId);
                }
                maxId = maxId + 1;
                model.ETicketCode = Shared.ToInt(_maxId);

                List<SelectListItem> PaxType = new List<SelectListItem>();
                PaxType.Add(new SelectListItem() { Text = "Adult", Value = "Adult" });
                PaxType.Add(new SelectListItem() { Text = "Child", Value = "Child" });
                PaxType.Add(new SelectListItem() { Text = "Infant", Value = "Infant" });

                this.ViewBag.PaxType = new SelectList(PaxType, "Value", "Text");

                List<SelectListItem> Flight = (from p in context.Flights.AsEnumerable().Where(a => a.Status == 1)
                                  select new SelectListItem
                                  {
                                      Text = p.FlightName,
                                      Value = p.FlightName
                                  }).ToList();
                               

                //List<SelectListItem> Flight = new List<SelectListItem>();
                //Flight.Add(new SelectListItem() { Text = "IndiGo", Value = "IndiGo" });
                //Flight.Add(new SelectListItem() { Text = "Air India Express", Value = "Air India Express" });
                //Flight.Add(new SelectListItem() { Text = "Air India", Value = "Air India" });
                //Flight.Add(new SelectListItem() { Text = "Singapore Airlines", Value = "Singapore Airlines" });
                //Flight.Add(new SelectListItem() { Text = "Scoot", Value = "Scoot" });                
                //Flight.Add(new SelectListItem() { Text = "Air New Zealand", Value = "Air New Zealand" });
                //Flight.Add(new SelectListItem() { Text = "ANA", Value = "ANA" });
                //Flight.Add(new SelectListItem() { Text = "Go Air", Value = "Go Air" });
                //Flight.Add(new SelectListItem() { Text = "Vistara", Value = "Vistara" });

                this.ViewBag.Flight = new SelectList(Flight, "Value", "Text");

                ViewBag.Depart = (from c in context.Airports
                                  select new SelectListItem
                                  {
                                      Text = c.AirportName,
                                      Value = c.AirportName
                                  }).ToList();

                ViewBag.Arrive = (from c in context.Airports
                                  select new SelectListItem
                                  {
                                      Text = c.AirportName,
                                      Value = c.AirportName
                                  }).ToList();
                                

                List<SelectListItem> Prefix = new List<SelectListItem>();
                Prefix.Add(new SelectListItem() { Text = "Mr.", Value = "Mr." });
                Prefix.Add(new SelectListItem() { Text = "Ms.", Value = "Ms." });
                Prefix.Add(new SelectListItem() { Text = "Mrs.", Value = "Mrs." });

                this.ViewBag.Prefix = new SelectList(Prefix, "Value", "Text");

                List<SelectListItem> Class = new List<SelectListItem>();
                Class.Add(new SelectListItem() { Text = "Economy Class", Value = "Economy Class" });
                Class.Add(new SelectListItem() { Text = "First Class", Value = "First Class" });
                Class.Add(new SelectListItem() { Text = "Business Class", Value = "Business Class" });

                this.ViewBag.Class = new SelectList(Class, "Value", "Text");

                ViewBag.Baggage = (from c in context.Baggages
                                   select new SelectListItem
                                  {
                                      Text = c.BaggageName,
                                      Value = c.BaggageName
                                  }).ToList();
                               

                model.RefNo = RandomString(9);
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateETicket(ETicketModel model, FormCollection frm)
        {
            using (Entities context = new Entities())
            {
                int _affectedRows = 0;
                try
                {
                    // Has permission to view the page?
                    if (!Request.IsAuthenticated)
                    {
                        return RedirectToAction("LogOn", "Account");
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            try
                            {
                                _affectedRows = context.sp_frm_add_upd_ETickets(
                                    model.ETicketCode,
                                    model.Prefix,
                                    model.CustomerName.ToUpper(),
                                    model.RefNo,
                                    model.AirlinePNR.ToUpper(),
                                    WebSecurity.CurrentUserId,
                                    "CreateETicket"
                                ); ;

                                return RedirectToAction("ListETickets", "ETickets", new { Success = "Successfully Added" });
                            }
                            catch (Exception e)
                            {
                                ModelState.AddModelError(string.Empty, e.Message);
                            }

                        }
                        else
                        {
                            return View(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);

                    return View(model);
                }

                finally
                {
                    List<SelectListItem> PaxType = new List<SelectListItem>();
                    PaxType.Add(new SelectListItem() { Text = "Adult", Value = "Adult" });
                    PaxType.Add(new SelectListItem() { Text = "Child", Value = "Child" });
                    PaxType.Add(new SelectListItem() { Text = "Infant", Value = "Infant" });

                    this.ViewBag.PaxType = new SelectList(PaxType, "Value", "Text");

                    List<SelectListItem> Flight = (from p in context.Flights.AsEnumerable().Where(a => a.Status == 1)
                                                   select new SelectListItem
                                                   {
                                                       Text = p.FlightName,
                                                       Value = p.FlightName
                                                   }).ToList();

                    this.ViewBag.Flight = new SelectList(Flight, "Value", "Text");

                    ViewBag.Depart = (from c in context.Airports
                                      select new SelectListItem
                                      {
                                          Text = c.AirportName,
                                          Value = c.AirportName
                                      }).ToList();

                    ViewBag.Arrive = (from c in context.Airports
                                      select new SelectListItem
                                      {
                                          Text = c.AirportName,
                                          Value = c.AirportName
                                      }).ToList();


                    List<SelectListItem> Prefix = new List<SelectListItem>();
                    Prefix.Add(new SelectListItem() { Text = "Mr.", Value = "Mr." });
                    Prefix.Add(new SelectListItem() { Text = "Ms.", Value = "Ms." });
                    Prefix.Add(new SelectListItem() { Text = "Mrs.", Value = "Mrs." });

                    this.ViewBag.Prefix = new SelectList(Prefix, "Value", "Text");

                    List<SelectListItem> Class = new List<SelectListItem>();
                    Class.Add(new SelectListItem() { Text = "Economy Class", Value = "Economy Class" });
                    Class.Add(new SelectListItem() { Text = "First Class", Value = "First Class" });
                    Class.Add(new SelectListItem() { Text = "Business Class", Value = "Business Class" });

                    this.ViewBag.Class = new SelectList(Class, "Value", "Text");

                    ViewBag.Baggage = (from c in context.Baggages
                                       select new SelectListItem
                                       {
                                           Text = c.BaggageName,
                                           Value = c.BaggageName
                                       }).ToList();

                }
                return View(model);
            }
        }

        public ActionResult EditETicket(string ETicketCode, string Success)
        {
            using (Entities context = new Entities())
            {
                ETicketModel model = new ETicketModel();

                try
                {
                    ViewBag.Message = Success;
                    int? _ETicketCode = Shared.ToInt(ETicketCode);

                    model = (from c in context.sp_frm_get_ETickets(_ETicketCode, null, null, WebSecurity.CurrentUserId, "ListETickets", null, null, null)
                             select new ETicketModel
                             {
                                 ETicketCode = c.ETicketCode,
                                 CustomerName = Shared.ToString(c.CustomerName).ToUpper(),
                                 Prefix = Shared.ToString(c.Prefix), 
                                 AirlinePNR = Shared.ToString(c.AirlinePNR).ToUpper() ,
                                 RefNo = Shared.ToString(c.RefNo)
                                 
                             }).FirstOrDefault();                   

                }
                catch (Exception ex)
                {
                    return View(model);
                }
                finally
                {
                    List<SelectListItem> PaxType = new List<SelectListItem>();
                    PaxType.Add(new SelectListItem() { Text = "Adult", Value = "Adult" });
                    PaxType.Add(new SelectListItem() { Text = "Child", Value = "Child" });
                    PaxType.Add(new SelectListItem() { Text = "Infant", Value = "Infant" });

                    this.ViewBag.PaxType = new SelectList(PaxType, "Value", "Text");

                    List<SelectListItem> Flight = (from p in context.Flights.AsEnumerable().Where(a => a.Status == 1)
                                                   select new SelectListItem
                                                   {
                                                       Text = p.FlightName,
                                                       Value = p.FlightName
                                                   }).ToList();

                    this.ViewBag.Flight = new SelectList(Flight, "Value", "Text");

                    ViewBag.Depart = (from c in context.Airports
                                      select new SelectListItem
                                      {
                                          Text = c.AirportName,
                                          Value = c.AirportName
                                      }).ToList();

                    ViewBag.Arrive = (from c in context.Airports
                                      select new SelectListItem
                                      {
                                          Text = c.AirportName,
                                          Value = c.AirportName
                                      }).ToList();


                    List<SelectListItem> Prefix = new List<SelectListItem>();
                    Prefix.Add(new SelectListItem() { Text = "Mr.", Value = "Mr." });
                    Prefix.Add(new SelectListItem() { Text = "Ms.", Value = "Ms." });
                    Prefix.Add(new SelectListItem() { Text = "Mrs.", Value = "Mrs." });

                    this.ViewBag.Prefix = new SelectList(Prefix, "Value", "Text");

                    List<SelectListItem> Class = new List<SelectListItem>();
                    Class.Add(new SelectListItem() { Text = "Economy Class", Value = "Economy Class" });
                    Class.Add(new SelectListItem() { Text = "First Class", Value = "First Class" });
                    Class.Add(new SelectListItem() { Text = "Business Class", Value = "Business Class" });

                    this.ViewBag.Class = new SelectList(Class, "Value", "Text");

                    ViewBag.Baggage = (from c in context.Baggages
                                       select new SelectListItem
                                       {
                                           Text = c.BaggageName,
                                           Value = c.BaggageName
                                       }).ToList();


                }
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditETicket(ETicketModel model, FormCollection frm)
        {
            using (Entities context = new Entities())
            {
                int _affectedRows = 0;
                try
                {
                    // Has permission to view the page?
                    if (!Request.IsAuthenticated)
                    {
                        return RedirectToAction("LogOn", "Account");
                    }
                    else
                    {
                        //if (!model.DueDate.HasValue) ModelState.AddModelError("DueDate", "Due Date is Required.");
                        if (ModelState.IsValid)
                        {
                            try
                            {
                                _affectedRows = context.sp_frm_add_upd_ETickets(
                                    model.ETicketCode,
                                    model.Prefix,                               
                                    model.CustomerName,
                                    model.RefNo,
                                    model.AirlinePNR,
                                    WebSecurity.CurrentUserId,
                                   "CreateETicket"                              
                                );

                                return RedirectToAction("ListETickets", "ETickets", new { Success = "Successfully Updated" });
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);

                                return View(model);
                            }

                        }
                        else
                        {
                            return View(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);

                    return View(model);
                }

                finally
                {
                    List<SelectListItem> PaxType = new List<SelectListItem>();
                    PaxType.Add(new SelectListItem() { Text = "Adult", Value = "Adult" });
                    PaxType.Add(new SelectListItem() { Text = "Child", Value = "Child" });
                    PaxType.Add(new SelectListItem() { Text = "Infant", Value = "Infant" });

                    this.ViewBag.PaxType = new SelectList(PaxType, "Value", "Text");

                    List<SelectListItem> Flight = (from p in context.Flights.AsEnumerable().Where(a => a.Status == 1)
                                                   select new SelectListItem
                                                   {
                                                       Text = p.FlightName,
                                                       Value = p.FlightName
                                                   }).ToList();

                    this.ViewBag.Flight = new SelectList(Flight, "Value", "Text");

                    ViewBag.Depart = (from c in context.Airports
                                      select new SelectListItem
                                      {
                                          Text = c.AirportName,
                                          Value = c.AirportName
                                      }).ToList();

                    ViewBag.Arrive = (from c in context.Airports
                                      select new SelectListItem
                                      {
                                          Text = c.AirportName,
                                          Value = c.AirportName
                                      }).ToList();


                    List<SelectListItem> Prefix = new List<SelectListItem>();
                    Prefix.Add(new SelectListItem() { Text = "Mr.", Value = "Mr." });
                    Prefix.Add(new SelectListItem() { Text = "Ms.", Value = "Ms." });
                    Prefix.Add(new SelectListItem() { Text = "Mrs.", Value = "Mrs." });

                    this.ViewBag.Prefix = new SelectList(Prefix, "Value", "Text");

                    List<SelectListItem> Class = new List<SelectListItem>();
                    Class.Add(new SelectListItem() { Text = "Economy Class", Value = "Economy Class" });
                    Class.Add(new SelectListItem() { Text = "First Class", Value = "First Class" });
                    Class.Add(new SelectListItem() { Text = "Business Class", Value = "Business Class" });

                    this.ViewBag.Class = new SelectList(Class, "Value", "Text");

                    ViewBag.Baggage = (from c in context.Baggages
                                       select new SelectListItem
                                       {
                                           Text = c.BaggageName,
                                           Value = c.BaggageName
                                       }).ToList();



                }
            }
        }

        #endregion

    }
}
