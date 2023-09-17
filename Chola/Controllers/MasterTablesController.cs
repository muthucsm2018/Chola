using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Chola.Models;
using WebMatrix.WebData;
using System.Globalization;

namespace Chola.Controllers
{
    public class MasterTablesController : Controller
    {
        #region CommonFunctions

       
        #endregion

        #region Documents

        public ActionResult Documents()
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        ViewBag.Message = "";
                    }
                    catch
                    {

                    }
                    finally
                    {
                        ViewBag.VisaList = GetVisaList();
                        ViewBag.CountryList = GetCountryList();
                    }

                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        private DataSet ReturnQueryDocuments(string DocumentName, string Nationality)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[0].Value = WebSecurity.CurrentUserId;
                sqlParams[1] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[1].Value = "Documents";
                sqlParams[2] = new SqlParameter("@DocumentName", SqlDbType.VarChar, 256);
                sqlParams[2].Value = DocumentName;
                sqlParams[3] = new SqlParameter("@Nationality", SqlDbType.VarChar, 50);
                sqlParams[3].Value = Nationality;

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Documents", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<DocumentsModel> GetListDocuments(string DocumentName, string Nationality)
        {
            DataSet ds = ReturnQueryDocuments(DocumentName, Nationality);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new DocumentsModel
                     {
                         DocumentType = Shared.ToString(dataRow.Field<Int32>("DocumentType")),
                         DocumentName = dataRow.Field<string>("DocumentName"),
                         Nationality = dataRow.Field<string>("Nationality"),
                         VisaType = dataRow.Field<string>("VisaType"),
                         Duration = dataRow.Field<string>("Duration"),
                         VisaCost = Shared.ToString(dataRow.Field<decimal>("VisaCost")),
                         Charge = Shared.ToString(dataRow.Field<decimal>("Charge")),
                         ProcessingDays = Shared.ToString(dataRow.Field<Int32>("ProcessingDays")),
                         SortOrder = Shared.ToString(dataRow.Field<Int32>("SortOrder"))
                     });
                return trxnList.ToList();
            }
            else

                return new List<DocumentsModel>();
        }

        public JsonResult DocumentsList(string DocumentName, string Nationality)
        {
            try
            {
                List<DocumentsModel> query = GetListDocuments(DocumentName, Nationality);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.DocumentType, x.DocumentType, x.DocumentName, x.Nationality, x.VisaType, x.Duration, x.VisaCost, x.Charge, x.ProcessingDays, x.SortOrder })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SaveDocument(string DocumentType, string DocumentName, string Nationality, string VisaType, string Duration, string VisaCost, string Charge, string ProcessingDays, string SortOrder, string mode)
        {
            int _affectedRows = 0;
            using (Entities context = new Entities())
            {
                try
                {
                    context.sp_frm_add_upd_Document(
                               Shared.ToInt(DocumentType),
                                DocumentName.ToUpper(),
                                Nationality.ToUpper(),
                                VisaType.ToUpper(),
                                Duration.ToUpper(),
                                Shared.ToDecimal(VisaCost),
                                Shared.ToDecimal(Charge),
                                Shared.ToInt(ProcessingDays),
                                Shared.ToInt(SortOrder),
                                WebSecurity.CurrentUserId,
                                "Documents"
                                );

                    return Json(new { success = true, response = Shared.ToString(mode) == "Create" ? "Successfully Added!" : "Successfully Updated!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }
        }        

        [HttpPost]
        public ActionResult CheckExistsDocument(string DocumentName, string Nationality, string VisaType, string Duration)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    var _exists = context.Document.Any(x => x.DocumentName == DocumentName && x.Nationality == Nationality && x.VisaType == VisaType && x.Duration == Duration);
                    if (_exists)
                    {
                        return Json(new { success = false, response = "Record Already Exists." });
                    }
                    else
                    {
                        return Json(new { success = true });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }
        }

        [HttpPost]
        public ActionResult GenerateVisaList()
        {
            using (Entities context = new Entities())
            {
                try
                {
                    RegionInfo country = new RegionInfo(new CultureInfo("en-US", false).LCID);
                    List<SelectListItem> countryNames = new List<SelectListItem>();
                    List<SelectListItem> countryNamesFiltered = new List<SelectListItem>();

                    //To get the Country Names from the CultureInfo installed in windows
                    foreach (CultureInfo cul in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                    {
                        country = new RegionInfo(new CultureInfo(cul.Name, false).LCID);
                        countryNames.Add(new SelectListItem() { Text = country.DisplayName.ToUpper() + " VISA", Value = country.DisplayName.ToUpper() + " VISA" });
                    }
                    //Assigning all Country names to IEnumerable
                    IEnumerable<SelectListItem> nameAdded = countryNames.GroupBy(x => x.Text).Select(x => x.FirstOrDefault()).ToList<SelectListItem>().OrderBy(x => x.Text);
                    countryNamesFiltered = nameAdded.ToList();

                    if (countryNames != null)
                    {
                        return Json(new { success = true, response = countryNamesFiltered });
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
        public ActionResult GenerateCountryList()
        {
            using (Entities context = new Entities())
            {
                try
                {
                    RegionInfo country = new RegionInfo(new CultureInfo("en-US", false).LCID);
                    List<SelectListItem> countryNames = new List<SelectListItem>();
                    List<SelectListItem> countryNamesFiltered = new List<SelectListItem>();

                    //To get the Country Names from the CultureInfo installed in windows
                    foreach (CultureInfo cul in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                    {
                        country = new RegionInfo(new CultureInfo(cul.Name, false).LCID);
                        countryNames.Add(new SelectListItem() { Text = country.DisplayName.ToUpper(), Value = country.DisplayName.ToUpper() });
                    }
                    //Assigning all Country names to IEnumerable
                    IEnumerable<SelectListItem> nameAdded = countryNames.GroupBy(x => x.Text).Select(x => x.FirstOrDefault()).ToList<SelectListItem>().OrderBy(x => x.Text);
                    countryNamesFiltered = nameAdded.ToList();
                    countryNamesFiltered.Add(new SelectListItem { Text = "OTHER", Value = "OTHER" });

                    if (countryNames != null)
                    {
                        return Json(new { success = true, response = countryNamesFiltered });
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
               
        public List<SelectListItem> GetVisaList()
        {
            using (Entities context = new Entities())
            {
                try
                {
                    RegionInfo country = new RegionInfo(new CultureInfo("en-US", false).LCID);
                    List<SelectListItem> countryNames = new List<SelectListItem>();
                    List<SelectListItem> countryNamesFiltered = new List<SelectListItem>();

                    //To get the Country Names from the CultureInfo installed in windows
                    foreach (CultureInfo cul in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                    {
                        country = new RegionInfo(new CultureInfo(cul.Name, false).LCID);
                        countryNames.Add(new SelectListItem() { Text = country.DisplayName.ToUpper() + " VISA", Value = country.DisplayName.ToUpper() + " VISA" });
                    }
                    //Assigning all Country names to IEnumerable
                    IEnumerable<SelectListItem> nameAdded = countryNames.GroupBy(x => x.Text).Select(x => x.FirstOrDefault()).ToList<SelectListItem>().OrderBy(x => x.Text);
                    countryNamesFiltered = nameAdded.ToList();

                   return countryNamesFiltered;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException.Message);
                }
            }
        }

        public List<SelectListItem> GetCountryList()
        {
            using (Entities context = new Entities())
            {
                try
                {
                    RegionInfo country = new RegionInfo(new CultureInfo("en-US", false).LCID);
                    List<SelectListItem> countryNames = new List<SelectListItem>();
                    List<SelectListItem> countryNamesFiltered = new List<SelectListItem>();

                    //To get the Country Names from the CultureInfo installed in windows
                    foreach (CultureInfo cul in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                    {
                        country = new RegionInfo(new CultureInfo(cul.Name, false).LCID);
                        countryNames.Add(new SelectListItem() { Text = country.DisplayName.ToUpper(), Value = country.DisplayName.ToUpper() });
                    }
                    //Assigning all Country names to IEnumerable
                    IEnumerable<SelectListItem> nameAdded = countryNames.GroupBy(x => x.Text).Select(x => x.FirstOrDefault()).ToList<SelectListItem>().OrderBy(x => x.Text);
                    countryNamesFiltered = nameAdded.ToList();
                    countryNamesFiltered.Add(new SelectListItem { Text = "OTHER", Value = "OTHER" });

                   return countryNamesFiltered;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException.Message);
                }
            }
        }

        #endregion

        #region ServiceTypes

        public ActionResult ServiceTypes()
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        ViewBag.Message = "";
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

        private DataSet ReturnQueryServiceTypes(string OthersTypeName)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[0].Value = WebSecurity.CurrentUserId;
                sqlParams[1] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[1].Value = "ServiceTypes";
                sqlParams[2] = new SqlParameter("@OthersTypeName", SqlDbType.VarChar, 256);
                sqlParams[2].Value = OthersTypeName;

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_OtherTypes", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<ServiceTypesModel> GetListServiceTypes(string OthersTypeName)
        {
            DataSet ds = ReturnQueryServiceTypes(OthersTypeName); 

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new ServiceTypesModel
                {
                    OthersTypeCode = Shared.ToString(dataRow.Field<Int32>("OthersTypeCode")),
                    OthersTypeName = dataRow.Field<string>("OtherTypeName")
                });
                return trxnList.ToList();
            }
            else

                return new List<ServiceTypesModel>();
        }

        public JsonResult ServiceTypesList(string OthersTypeName)
        {
            try
            {
                List<ServiceTypesModel> query = GetListServiceTypes(OthersTypeName);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.OthersTypeCode, x.OthersTypeCode, x.OthersTypeName })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SaveServiceType(string OthersTypeCode, string OthersTypeName, string mode)
        {
            int _affectedRows = 0;
            using (Entities context = new Entities())
            {
                try
                {
                    _affectedRows = context.sp_frm_add_upd_OthersType(
                               Shared.ToInt(OthersTypeCode),
                                OthersTypeName.ToUpper(),
                                WebSecurity.CurrentUserId,
                                "ServiceTypes"
                                );

                    return Json(new { success = true, response = Shared.ToString(mode) == "Create" ? "Successfully Added!" : "Successfully Updated!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }
        }

        [HttpPost]
        public ActionResult CheckExistsServiceType(string OthersTypeName)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    var _exists = context.OthersType.Any(x => x.OtherTypeName == OthersTypeName);
                    if (_exists)
                    {
                        return Json(new { success = false, response = "Name Already Exists." });
                    }
                    else
                    {
                        return Json(new { success = true });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }
        }

        #endregion

        #region AccountVendors

        public ActionResult AccountVendors()
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        ViewBag.Message = "";
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

        private DataSet ReturnQueryAccountVendors(string VendorName)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[0].Value = WebSecurity.CurrentUserId;
                sqlParams[1] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[1].Value = "Vendors";
                sqlParams[2] = new SqlParameter("@VendorName", SqlDbType.VarChar, 100);
                sqlParams[2].Value = VendorName;


                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Acc_Vendors", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<VendorsModel> GetListAccountVendors(string VendorName)
        {
            DataSet ds = ReturnQueryAccountVendors(VendorName);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new VendorsModel
                {
                    VendorCode = Shared.ToString(dataRow.Field<Int32>("VendorCode")),
                    VendorName = dataRow.Field<string>("VendorName"),
                    Address1 = dataRow.Field<string>("Address1"),
                    Address2 = dataRow.Field<string>("Address2"),
                    Address3 = dataRow.Field<string>("Address3"),
                    TaxID = dataRow.Field<string>("TaxID"),
                    Status = Shared.ToString(dataRow.Field<Int16>("Status"))
                });
                return trxnList.ToList();
            }
            else

                return new List<VendorsModel>();
        }

        public JsonResult AccountVendorsList(string VendorName)
        {
            try
            {
                List<VendorsModel> query = GetListAccountVendors(VendorName);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.VendorCode, x.VendorCode, x.VendorName, x.Address1, x.Address2, x.Address3, x.TaxID, x.Status, "" })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SaveAccountVendor(string VendorCode, string VendorName, string Address1, string Address2, string Address3, string TaxID, string Status, string mode)
        {
            int _affectedRows = 0;
            using (Entities context = new Entities())
            {
                try
                {
                    _affectedRows = context.sp_frm_add_upd_Acc_Vendors(
                               Shared.ToInt(VendorCode),
                                VendorName.ToUpper(),
                                Address1,
                                Address2,
                                Address3,
                                TaxID,
                                Status == "True" ? true : false,
                                WebSecurity.CurrentUserId,
                                "Vendors"
                                );

                    return Json(new { success = true, response = Shared.ToString(mode) == "Create" ? "Successfully Added!" : "Successfully Updated!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }
        }

        [HttpPost]
        public ActionResult CheckExistsAccountVendor(string VendorName)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    var _exists = context.Acc_Vendors.Any(x => x.VendorName == VendorName);
                    if (_exists)
                    {
                        return Json(new { success = false, response = "Vendor Name Already Exists." });
                    }
                    else
                    {
                        return Json(new { success = true });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }
        }

        #endregion

        #region Customers

        public ActionResult Customers()
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        ViewBag.Message = "";
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

        private DataSet ReturnQueryCustomers(string CustomerName)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[0].Value = WebSecurity.CurrentUserId;
                sqlParams[1] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[1].Value = "Customers";
                sqlParams[2] = new SqlParameter("@CustomerName", SqlDbType.VarChar, 100);
                sqlParams[2].Value = CustomerName;


                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Customers", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<CustomersModel> GetListCustomers(string CustomerName)
        {
            DataSet ds = ReturnQueryCustomers(CustomerName);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new CustomersModel
                {
                    RefNo = Shared.ToString(dataRow.Field<string>("RefNo")),
                    CustomerName = dataRow.Field<string>("CustomerName"),
                    OpeningBalance = dataRow.Field<decimal?>("OpeningBalance")
                });
                return trxnList.ToList();
            }
            else

                return new List<CustomersModel>();
        }

        public JsonResult CustomersList(string CustomerName)
        {
            try
            {
                List<CustomersModel> query = GetListCustomers(CustomerName);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.RefNo, x.RefNo, x.CustomerName, Shared.ToString(x.OpeningBalance) })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SaveCustomer(string RefNo, string CustomerName, string OpeningBalance, string mode)
        {
            int _affectedRows = 0;
            using (Entities context = new Entities())
            {
                try
                {
                    _affectedRows = context.sp_frm_add_upd_Customers(
                               Shared.ToString(RefNo).ToUpper(),
                               Shared.ToString(CustomerName).ToUpper(),
                                Shared.ToDecimal(OpeningBalance),
                                WebSecurity.CurrentUserId,
                                "Customers"
                                );

                    return Json(new { success = true, response = Shared.ToString(mode) == "Create" ? "Successfully Added!" : "Successfully Updated!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }
        }

        [HttpPost]
        public ActionResult CheckExistsCustomer(string CustomerName)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    var _exists = context.CustomerAccount.Any(x => x.CustomerName == CustomerName);
                    if (_exists)
                    {
                        return Json(new { success = false, response = "Customer Name Already Exists." });
                    }
                    else
                    {
                        return Json(new { success = true });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }
        }

        #endregion

        #region Flights

        public ActionResult Flights()
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        ViewBag.Message = "";
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

        private DataSet ReturnQueryFlights(string FlightName)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[0].Value = WebSecurity.CurrentUserId;
                sqlParams[1] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[1].Value = "Flights";
                sqlParams[2] = new SqlParameter("@FlightName", SqlDbType.VarChar, 100);
                sqlParams[2].Value = FlightName;


                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Flights", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<FlightsModel> GetListFlights(string FlightName)
        {
            DataSet ds = ReturnQueryFlights(FlightName);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new FlightsModel
                {
                    FlightCode = Shared.ToString(dataRow.Field<Int32>("FlightCode")),
                    FlightName = dataRow.Field<string>("FlightName"),                   
                    Status = Shared.ToString(dataRow.Field<Int16>("Status"))
                });
                return trxnList.ToList();
            }
            else

                return new List<FlightsModel>();
        }

        public JsonResult FlightsList(string FlightName)
        {
            try
            {
                List<FlightsModel> query = GetListFlights(FlightName);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.FlightCode, x.FlightCode, x.FlightName, x.Status })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SaveFlight(string FlightCode, string FlightName, string Status, string mode)
        {
            int _affectedRows = 0;
            using (Entities context = new Entities())
            {
                try
                {
                    _affectedRows = context.sp_frm_add_upd_Flights(
                               Shared.ToInt(FlightCode),
                                FlightName.ToUpper(),                               
                                Status == "True" ? true : false,
                                WebSecurity.CurrentUserId,
                                "Flights"
                                );

                    return Json(new { success = true, response = Shared.ToString(mode) == "Create" ? "Successfully Added!" : "Successfully Updated!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }
        }

        [HttpPost]
        public ActionResult CheckExistsFlight(string FlightName)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    var _exists = context.Flights.Any(x => x.FlightName == FlightName);
                    if (_exists)
                    {
                        return Json(new { success = false, response = "Flight Name Already Exists." });
                    }
                    else
                    {
                        return Json(new { success = true });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, response = Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message });
                }
            }
        }

        #endregion
    }
}
