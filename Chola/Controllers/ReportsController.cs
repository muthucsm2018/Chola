using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Chola.Filters;
using Chola.Models;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.IO;
using System.Data.Objects.SqlClient;
using System.Globalization;
using System.Data.Entity;

namespace Chola.Controllers
{
    public class ReportsController : Controller
    {
        #region CommonFunctions
        

        #endregion

        #region OverAllReport

        private DataTable ReturnTrxnDetails(DateTime? TransactionFrom, DateTime? TransactionTo)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@CustomerId", SqlDbType.Int);
                sqlParams[0].Value = WebSecurity.CurrentUserId;
                sqlParams[1] = new SqlParameter("@TransactionFrom", SqlDbType.DateTime);
                if (TransactionFrom != null)
                    sqlParams[1].Value = TransactionFrom;
                else
                    sqlParams[1].Value = DBNull.Value;
                sqlParams[2] = new SqlParameter("@TransactionTo", SqlDbType.DateTime);
                if (TransactionFrom != null)
                    sqlParams[2].Value = TransactionTo;
                else
                    sqlParams[2].Value = DBNull.Value;

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Transaction_Details_OverAll", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery.Tables[0];
        }

        public ActionResult OverallReport()
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        DataTable dt = ReturnTrxnDetails(null, null);

                        ViewBag.OtherAmount = dt.Rows[0]["NoofOtherAmount"];
                        ViewBag.VisaAmount = dt.Rows[0]["NoofVisaAmount"];
                        ViewBag.TotalAmount = dt.Rows[0]["NoofTotalAmount"];
                        ViewBag.TotalExpense = dt.Rows[0]["NoofExpense"];
                    }
                    catch
                    {

                    }
                    
                    CultureInfo provider = CultureInfo.InvariantCulture;

                    ViewBag.Date = DateTime.Now.ToString("MM/dd/yyyy") + "|" + DateTime.Now.ToString("MM/dd/yyyy");
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        [HttpPost]
        public ActionResult OverallReport(string daterange, FormCollection frm)
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        //02/02/2016 - 02/02/2016
                        DateTime? _startdate = null;
                        DateTime? _enddate = null;
                        string format = "d";
                        CultureInfo provider = CultureInfo.InvariantCulture;

                        if (Shared.ToString(daterange).Length > 0)
                        {
                            _startdate = DateTime.ParseExact(daterange.Split('|')[0].Trim(), format, provider);
                            _enddate = DateTime.ParseExact(daterange.Split('|')[1].Trim(), format, provider);
                        }

                        DataTable dt = ReturnTrxnDetails(_startdate, _enddate);

                        ViewBag.OtherAmount = dt.Rows[0]["NoofOtherAmount"];
                        ViewBag.VisaAmount = dt.Rows[0]["NoofVisaAmount"];
                        ViewBag.TotalAmount = dt.Rows[0]["NoofTotalAmount"];
                        ViewBag.TotalExpense = dt.Rows[0]["NoofExpense"];
                    }
                    catch(Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message.ToString());
                    }
                    ViewBag.Date = daterange;
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }      

        #endregion

        #region Bills

        public ActionResult Bills(string Success)
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
                    finally
                    {

                        ViewBag.CreatedByList = context.sp_frm_get_Parm_Users(WebSecurity.CurrentUserId, "").ToList().Select(c => new SelectListItem
                        {
                            Value = c.UserId.ToString(),
                            Text = c.UserName
                        }); 

                        var _isAdmin = context.security_UsersInRoles.AsEnumerable().Count(p => p.UserID == WebSecurity.CurrentUserId && p.RoleID == 1);
                        if (_isAdmin > 0)
                            ViewBag.UserAuth = "true";
                        else
                            ViewBag.UserAuth = "false";
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        private DataSet ReturnQueryBill(DateTime? TransactionFrom, DateTime? TransactionTo, string CustomerName)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[6];
                sqlParams[0] = new SqlParameter("@TourCode", SqlDbType.Int);
                sqlParams[0].Value = DBNull.Value;
              
                sqlParams[1] = new SqlParameter("@CustomerName", SqlDbType.NVarChar, 256);
                if (Shared.ToString(CustomerName).Length > 0)
                    sqlParams[1].Value = CustomerName;
                else
                    sqlParams[1].Value = DBNull.Value;
              
                sqlParams[2] = new SqlParameter("@TransactionFrom", SqlDbType.DateTime);
                if (TransactionFrom != null)
                    sqlParams[2].Value = TransactionFrom;
                else
                    sqlParams[2].Value = DBNull.Value;
                sqlParams[3] = new SqlParameter("@TransactionTo", SqlDbType.DateTime);
                if (TransactionFrom != null)
                    sqlParams[3].Value = TransactionTo;
                else
                    sqlParams[3].Value = DBNull.Value;
               
                sqlParams[4] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[4].Value = WebSecurity.CurrentUserId;
                sqlParams[5] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[5].Value = "ListBills";

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Tours", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<TourListModel> GetListBill(DateTime? TransactionFrom, DateTime? TransactionTo, string CustomerName)
        {
            DataSet ds = ReturnQueryBill(TransactionFrom, TransactionTo, CustomerName);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new TourListModel
                {
                    TourCode = Shared.ToString(dataRow.Field<Int32>("TourCode")),
                    CustomerName = dataRow.Field<string>("CustomerName"),
                    Remarks = dataRow.Field<string>("Remarks"),
                    TrxnDate = dataRow.Field<string>("TrxnDate"),
                    //Shared.ToString(dataRow.Field<DateTime>("TrxnDate").ToString("MMM dd yyyy")),
                    BuyingPrice = Shared.ToString(dataRow.Field<decimal>("BuyingPrice")),
                    SellingPrice = Shared.ToString(dataRow.Field<decimal>("SellingPrice")),
                    CreatedBy = dataRow.Field<string>("CreatedBy")
                });
                return trxnList.ToList();
            }
            else

                return new List<TourListModel>();
        }

        public JsonResult QueryBillList(string daterange, string CustomerName)
        {
            try
            {
                //02/02/2016 - 02/02/2016
                DateTime? _startdate = null;
                DateTime? _enddate = null;
                string format = "d";
                CultureInfo provider = CultureInfo.InvariantCulture;

                if (Shared.ToString(daterange).Length > 0)
                {
                    _startdate = DateTime.ParseExact(daterange.Split('|')[0].Trim(), format, provider);
                    _enddate = DateTime.ParseExact(daterange.Split('|')[1].Trim(), format, provider);
                }

                List<TourListModel> query = GetListBill(_startdate, _enddate, CustomerName);
                var vatperc = Shared.ToString(System.Configuration.ConfigurationManager.AppSettings["Vat"]);
                
                return Json(new
                {
                    aaData = query.Select(x => new[] { x.TourCode, DateTime.Parse(x.TrxnDate).ToString("dd/MMM/yyyy"), x.CustomerName, x.Remarks,
                        String.Format("{0:n}", x.BuyingPrice),
                        ///String.Format("{0:n}", x.SellingPrice),
                         //String.Format("{0:n}", CalculateVat((Shared.ToDecimal(x.SellingPrice) / Shared.ToDecimal(1.07)), Shared.ToInt(vatperc))),
                         String.Format("{0:n}", ReturnTotal(CalculateVat((Shared.ToDecimal(x.SellingPrice)/ Shared.ToDecimal(1.07)), Shared.ToInt(vatperc)), (Shared.ToDecimal(x.SellingPrice) / Shared.ToDecimal(1.07)))),
                         String.Format("{0:n}", ReturnProfit(ReturnTotal(CalculateVat((Shared.ToDecimal(x.SellingPrice)/ Shared.ToDecimal(1.07)), Shared.ToInt(vatperc)), Shared.ToDecimal(x.SellingPrice)/ Shared.ToDecimal(1.07)) , Shared.ToDecimal(x.BuyingPrice))),
                        x.CreatedBy })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public decimal ReturnTotal(decimal value, decimal vat)
        {
            return value + vat;
        }

        public decimal ReturnProfit(decimal total, decimal buy)
        {
            return total - buy;
        }

        public decimal CalculateVat(decimal value, int vat)
        {
            return (value * vat) / 100;
        }

        #endregion

        #region Miscellaneous

        public ActionResult Miscellaneous(string Success)
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
                    var count = context.OthersType.AsEnumerable().Count();
                    if (count > 0)
                    {
                        ViewBag.ServiceList = context.OthersType.ToList().Select(c => new SelectListItem
                        {
                            Value = c.OthersTypeCode.ToString(),
                            Text = c.OtherTypeName
                        });
                    }
                    else
                    {
                        ViewBag.ServiceList = new SelectListItem
                        {
                            Text = "",
                            Value = ""
                        };
                    }

                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        private DataSet ReturnQueryEntry(string OthersTypeCode, DateTime? TransactionFrom, DateTime? TransactionTo)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[7];
                sqlParams[0] = new SqlParameter("@OthersCode", SqlDbType.Int);
                sqlParams[0].Value = DBNull.Value;
                sqlParams[1] = new SqlParameter("@OthersTypeCode", SqlDbType.Int);
                if (Shared.ToString(OthersTypeCode).Length > 0)
                    sqlParams[1].Value = Shared.ToInt(OthersTypeCode);
                else
                    sqlParams[1].Value = DBNull.Value;

                sqlParams[2] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[2].Value = WebSecurity.CurrentUserId;
                sqlParams[3] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[3].Value = "ListEntries";
                sqlParams[4] = new SqlParameter("@ParmType", SqlDbType.Int);
                sqlParams[4].Value = 2;
                sqlParams[5] = new SqlParameter("@TransactionFrom", SqlDbType.DateTime);
                if (TransactionFrom != null)
                    sqlParams[5].Value = TransactionFrom;
                else
                    sqlParams[5].Value = DBNull.Value;
                sqlParams[6] = new SqlParameter("@TransactionTo", SqlDbType.DateTime);
                if (TransactionFrom != null)
                    sqlParams[6].Value = TransactionTo;
                else
                    sqlParams[6].Value = DBNull.Value;

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Miscellaneous", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<EntryListModel> GetListEntry(string OthersTypeCode, DateTime? TransactionFrom, DateTime? TransactionTo)
        {
            DataSet ds = ReturnQueryEntry(OthersTypeCode, TransactionFrom, TransactionTo);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new EntryListModel
                {
                    OthersCode = Shared.ToString(dataRow.Field<Int32>("OthersCode")),
                    OthersTypeName = dataRow.Field<string>("OtherTypeName"),
                    Charge = Shared.ToString(dataRow.Field<decimal>("Charge")),
                    Remarks = dataRow.Field<string>("Remarks"),
                    TrxnDate = dataRow.Field<string>("CreatedDate"),
                    CreatedBy = dataRow.Field<string>("CreatedBy"),
                    UpdatedBy = dataRow.Field<string>("UpdatedBy"),
                    UpdatedDate = dataRow.Field<string>("UpdatedDate")
                });
                return trxnList.ToList();
            }
            else

                return new List<EntryListModel>();
        }

        public JsonResult QueryEntryList(string OthersTypeCode, string daterange)
        {
            try
            {
                DateTime? _startdate = null;
                DateTime? _enddate = null;
                string format = "d";
                CultureInfo provider = CultureInfo.InvariantCulture;

                if (Shared.ToString(daterange).Length > 0)
                {
                    _startdate = DateTime.ParseExact(daterange.Split('|')[0].Trim(), format, provider);
                    _enddate = DateTime.ParseExact(daterange.Split('|')[1].Trim(), format, provider);
                }

                List<EntryListModel> query = GetListEntry(OthersTypeCode, _startdate, _enddate);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.OthersTypeName, x.Charge, x.Remarks, x.TrxnDate, x.CreatedBy, x.UpdatedBy, x.UpdatedDate })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion        

        #region Tickets

        public ActionResult Tickets(string Success)
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        ViewBag.Message = Success;
                        ViewBag.VendorList = (from c in context.sp_frm_get_parm_Acc_Vendors(WebSecurity.CurrentUserId, "Vendors", null)
                                              select new SelectListItem
                                              {
                                                  Text = c.VendorName,
                                                  Value = Shared.ToString(c.VendorCode)
                                              }).ToList();
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

        private DataSet ReturnQueryTicket(string CustomerName, string PassportNo, DateTime? TransactionFrom, DateTime? TransactionTo)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[6];
                sqlParams[0] = new SqlParameter("@TicketCode", SqlDbType.Int);
                sqlParams[0].Value = DBNull.Value;
                sqlParams[1] = new SqlParameter("@VendorName", SqlDbType.NVarChar, 256);
                if (Shared.ToString(CustomerName).Length > 0)
                    sqlParams[1].Value = CustomerName;
                else
                    sqlParams[1].Value = DBNull.Value;
               
                sqlParams[2] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[2].Value = WebSecurity.CurrentUserId;
                sqlParams[3] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[3].Value = "ListTickets";
                sqlParams[4] = new SqlParameter("@TransactionFrom", SqlDbType.DateTime);
                if (TransactionFrom != null)
                    sqlParams[4].Value = TransactionFrom;
                else
                    sqlParams[4].Value = DBNull.Value;
                sqlParams[5] = new SqlParameter("@TransactionTo", SqlDbType.DateTime);
                if (TransactionFrom != null)
                    sqlParams[5].Value = TransactionTo;
                else
                    sqlParams[5].Value = DBNull.Value;

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Tickets_Report", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<TicketListReportModel> GetListTicket(string CustomerName, string PassportNo, DateTime? TransactionFrom, DateTime? TransactionTo)
        {
            DataSet ds = ReturnQueryTicket(CustomerName, PassportNo, TransactionFrom, TransactionTo);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new TicketListReportModel
                {
                    TicketCode = Shared.ToString(dataRow.Field<Int32>("TicketCode")),
                    TrxnDate = dataRow.Field<string>("TrxnDate"),
                    CustomerName = dataRow.Field<string>("CustomerName"),
                    Description = dataRow.Field<string>("Description"),
                    Remarks = dataRow.Field<string>("Remarks"),
                    BuyingPrice = Shared.ToString(dataRow.Field<decimal>("BuyingPrice")),
                    SellingPrice = Shared.ToString(dataRow.Field<decimal>("SellingPrice")),
                    CreatedBy = dataRow.Field<string>("CreatedBy"),
                    UpdatedDate = dataRow.Field<string>("UpdatedDate")

                });
                return trxnList.ToList();
            }
            else
                return new List<TicketListReportModel>();
        }

        public JsonResult QueryTicketList(string CustomerName, string PassportNo, string daterange)
        {
            try
            {
                DateTime? _startdate = null;
                DateTime? _enddate = null;
                string format = "d";
                CultureInfo provider = CultureInfo.InvariantCulture;

                if (Shared.ToString(daterange).Length > 0)
                {
                    _startdate = DateTime.ParseExact(daterange.Split('|')[0].Trim(), format, provider);
                    _enddate = DateTime.ParseExact(daterange.Split('|')[1].Trim(), format, provider);
                }

                List<TicketListReportModel> query = GetListTicket(CustomerName, PassportNo, _startdate, _enddate);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.TicketCode, x.TrxnDate, x.CustomerName, x.Description, x.Remarks, x.BuyingPrice, x.SellingPrice, String.Format("{0:n}", ReturnProfit(Shared.ToDecimal(x.SellingPrice), Shared.ToDecimal(x.BuyingPrice))), x.CreatedBy, x.UpdatedDate })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion

        #region Expense

        public ActionResult Expense(string Success)
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

        private DataSet ReturnQueryExpense(string ExpenseName, DateTime? TransactionFrom, DateTime? TransactionTo)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[7];
                sqlParams[0] = new SqlParameter("@ExpenseCode", SqlDbType.Int);
                sqlParams[0].Value = DBNull.Value;
                sqlParams[1] = new SqlParameter("@ExpenseName", SqlDbType.NVarChar, 100);
                if (Shared.ToString(ExpenseName).Length > 0)
                    sqlParams[1].Value = Shared.ToString(ExpenseName);
                else
                    sqlParams[1].Value = DBNull.Value;

                sqlParams[2] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[2].Value = WebSecurity.CurrentUserId;
                sqlParams[3] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[3].Value = "Expense";
                sqlParams[4] = new SqlParameter("@ParmType", SqlDbType.Int);
                sqlParams[4].Value = 2;
                sqlParams[5] = new SqlParameter("@TransactionFrom", SqlDbType.DateTime);
                if (TransactionFrom != null)
                    sqlParams[5].Value = TransactionFrom;
                else
                    sqlParams[5].Value = DBNull.Value;
                sqlParams[6] = new SqlParameter("@TransactionTo", SqlDbType.DateTime);
                if (TransactionFrom != null)
                    sqlParams[6].Value = TransactionTo;
                else
                    sqlParams[6].Value = DBNull.Value;

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Expense", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<ExpenseListModel> GetListExpense(string ExpenseName, DateTime? TransactionFrom, DateTime? TransactionTo)
        {
            DataSet ds = ReturnQueryExpense(ExpenseName, TransactionFrom, TransactionTo);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new ExpenseListModel
                {
                    ExpenseCode = Shared.ToString(dataRow.Field<Int32>("ExpenseCode")),
                    ExpenseName = dataRow.Field<string>("ExpenseName"),
                    Charge = Shared.ToString(dataRow.Field<decimal>("Charge")),
                    Remarks = dataRow.Field<string>("Remarks"),
                    TrxnDate = dataRow.Field<string>("CreatedDate"),
                    CreatedBy = dataRow.Field<string>("CreatedBy"),
                    UpdatedBy = dataRow.Field<string>("UpdatedBy"),
                    UpdatedDate = dataRow.Field<string>("UpdatedDate")
                });
                return trxnList.ToList();
            }
            else

                return new List<ExpenseListModel>();
        }

        public JsonResult QueryExpenseList(string ExpenseName, string daterange)
        {
            try
            {
                DateTime? _startdate = null;
                DateTime? _enddate = null;
                string format = "d";
                CultureInfo provider = CultureInfo.InvariantCulture;

                if (Shared.ToString(daterange).Length > 0)
                {
                    _startdate = DateTime.ParseExact(daterange.Split('|')[0].Trim(), format, provider);
                    _enddate = DateTime.ParseExact(daterange.Split('|')[1].Trim(), format, provider);
                }

                List<ExpenseListModel> query = GetListExpense(ExpenseName, _startdate, _enddate);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.ExpenseName, x.Charge, x.Remarks, x.TrxnDate, x.CreatedBy, x.UpdatedBy, x.UpdatedDate })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion        

        #region CustomerAccountReport

        public ActionResult CustomerAccountReport(string Success)
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        ViewBag.Message = Success;
                        ViewBag.CustomerList = (from c in context.sp_frm_get_parm_Customers(WebSecurity.CurrentUserId, "Vendors")
                                              select new SelectListItem
                                              {
                                                  Text = c.CustomerName,
                                                  Value = Shared.ToString(c.RefNo)
                                              }).ToList();
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

        private DataSet ReturnQueryCustomer(string RefNo, DateTime? TransactionFrom, DateTime? TransactionTo, string PaymentBy)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[6];
                
                sqlParams[0] = new SqlParameter("@RefNo", SqlDbType.NVarChar, 50);
                if (Shared.ToString(RefNo).Length > 0)
                    sqlParams[0].Value = RefNo;
                else
                    sqlParams[0].Value = DBNull.Value;

                sqlParams[1] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[1].Value = WebSecurity.CurrentUserId;
                sqlParams[2] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[2].Value = "Customers";
                sqlParams[3] = new SqlParameter("@TransactionFrom", SqlDbType.DateTime);
                if (TransactionFrom != null)
                    sqlParams[3].Value = TransactionFrom;
                else
                    sqlParams[3].Value = DBNull.Value;
                sqlParams[4] = new SqlParameter("@TransactionTo", SqlDbType.DateTime);
                if (TransactionFrom != null)
                    sqlParams[4].Value = TransactionTo;
                else
                    sqlParams[4].Value = DBNull.Value;

                sqlParams[5] = new SqlParameter("@PaymentBy", SqlDbType.NChar, 20);
                if (Shared.ToString(PaymentBy).Length > 0)
                    sqlParams[5].Value = PaymentBy;
                else
                    sqlParams[5].Value = DBNull.Value;
                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Customer_Report", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<CustomerListReportModel> GetListCustomer(string RefNo, DateTime? TransactionFrom, DateTime? TransactionTo, string PaymentBy)
        {
            DataSet ds = ReturnQueryCustomer(RefNo, TransactionFrom, TransactionTo, PaymentBy);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new CustomerListReportModel
                {
                    RefNo = dataRow.Field<string>("RefNo"),
                    CustomerName = dataRow.Field<string>("CustomerName"),
                    OpeningBalance = Shared.ToString(dataRow.Field<decimal?>("OpeningBalance")),
                    Debit = Shared.ToString(dataRow.Field<decimal?>("Debit")),
                    Credit = Shared.ToString(dataRow.Field<decimal?>("Credit"))
                });
                return trxnList.ToList();
            }
            else
                return new List<CustomerListReportModel>();
        }

        public JsonResult QueryCustomerList(string RefNo, string daterange, string PaymentBy)
        {
            try
            {
                DateTime? _startdate = null;
                DateTime? _enddate = null;
                string format = "d";
                CultureInfo provider = CultureInfo.InvariantCulture;

                if (Shared.ToString(daterange).Length > 0)
                {
                    _startdate = DateTime.ParseExact(daterange.Split('|')[0].Trim(), format, provider);
                    _enddate = DateTime.ParseExact(daterange.Split('|')[1].Trim(), format, provider);
                }

                List<CustomerListReportModel> query = GetListCustomer(RefNo, _startdate, _enddate, PaymentBy);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.RefNo, x.CustomerName, String.Format("{0:n}", x.OpeningBalance), String.Format("{0:n}", x.Debit), String.Format("{0:n}", x.Credit), String.Format("{0:n}", ReturnOutStanding(Shared.ToDecimal(x.OpeningBalance), Shared.ToDecimal(x.Debit), Shared.ToDecimal(x.Credit))) })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public decimal ReturnOutStanding(decimal balance, decimal debit, decimal credit)
        {
            return ((balance + credit) - debit);
        }

        #endregion

    }
}
