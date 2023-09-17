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

namespace Chola.Controllers
{
    public class BillingController : Controller
    {
        #region CommonFunctions

        #endregion

        #region ListBills

        public ActionResult ListBills(string Success)
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

        private DataSet ReturnQueryTicket(string CustomerName)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@TourCode", SqlDbType.Int);
                sqlParams[0].Value = DBNull.Value;
                sqlParams[1] = new SqlParameter("@CustomerName", SqlDbType.NVarChar, 100);
                if (Shared.ToString(CustomerName).Length > 0)
                    sqlParams[1].Value = CustomerName;
                else
                    sqlParams[1].Value = DBNull.Value;

                sqlParams[2] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[2].Value = WebSecurity.CurrentUserId;
                sqlParams[3] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[3].Value = "ListTickets";

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Tours", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<TourListModel> GetListTicket(string CustomerName)
        {
            DataSet ds = ReturnQueryTicket(CustomerName);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new TourListModel
                {
                    TourCode = Shared.ToString(dataRow.Field<Int32>("TourCode")),
                    CustomerName = dataRow.Field<string>("CustomerName"),
                    BuyingPrice = Shared.ToString(dataRow.Field<decimal>("BuyingPrice")),
                    SellingPrice = Shared.ToString(dataRow.Field<decimal>("SellingPrice")),
                    TrxnDate = dataRow.Field<string>("TrxnDate"),
                    Remarks = dataRow.Field<string>("Remarks")
                });
                return trxnList.ToList();
            }
            else

                return new List<TourListModel>();
        }

        public JsonResult QueryTourList(string CustomerName)
        {
            try
            {
                List<TourListModel> query = GetListTicket(CustomerName);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.TourCode, x.TourCode, x.CustomerName, x.Remarks, String.Format("{0:n}", x.BuyingPrice), String.Format("{0:n}", x.SellingPrice), DateTime.Parse(x.TrxnDate).ToString("dd/MMM/yyyy"), x.TourCode })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult Invoice(string TourCode)
        {
            using (Entities context = new Entities())
            {
                int? _TourCode = Shared.ToInt(TourCode);
                var tour = (from c in context.Tours where c.TourCode == _TourCode select c).FirstOrDefault();

                ViewBag.CustomerName = Shared.ToString(tour.CustomerName);
                ViewBag.InvoiceNo = Shared.ToString(_TourCode);
                ViewBag.Remarks = Shared.ToString(tour.Remarks);
                ViewBag.InvoiceDate = Shared.ToString(tour.TrxnDate.Value.ToString("dd MMM yyyy"));                
                ViewBag.Amount = Shared.ToDecimal(tour.SellingPrice);

                var vatperc = Shared.ToString(System.Configuration.ConfigurationManager.AppSettings["Vat"]);
                var _total = Shared.ToDecimal(tour.SellingPrice) / Shared.ToDecimal(1.07);
                ViewBag.Total = String.Format("{0:n}", _total);
                decimal? taxamt = CalculateVat(_total, Shared.ToInt(vatperc));
                ViewBag.Vat = String.Format("{0:n}", taxamt);
                decimal? _grandtotal = Shared.ToDecimal(taxamt) + Shared.ToDecimal(_total);
                ViewBag.GrandTotal = String.Format("{0:n}", _grandtotal);

                return View();
            }
        }

        public decimal CalculateVat(decimal value, int vat)
        {
            return (value * vat) / 100;
        }

        #endregion

        #region CreateEditTour
        public ActionResult CreateBill(string Success)
        {
            using (Entities context = new Entities())
            {
                CreateTourModel model = new CreateTourModel();

                int maxId = 0;
                var _maxId = "";
                var count = context.Tours.AsEnumerable().Count();
                if (count > 0)
                {
                    try
                    {
                        maxId = context.Tours.Max(p => p.TourCode);
                        _maxId =  DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString("D2") + (Shared.ToInt(count) + 1).ToString("D4");
                    }

                    catch (Exception ex)
                    {
                        maxId = 0;
                    }
                }
                else
                {
                    _maxId = DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString("D2") + "0001";
                }
                maxId = maxId + 1;
                model.TourCode = Shared.ToInt(_maxId);

                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBill(CreateTourModel model, FormCollection frm)
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
                                _affectedRows = context.sp_frm_add_upd_Tours(
                                    model.TourCode,
                               Shared.ToString(model.CustomerName).ToUpper(),
                               Shared.ToString(model.Remarks).ToUpper(),
                                model.TrxnDate,
                                model.BuyingPrice,
                                model.SellingPrice,
                                WebSecurity.CurrentUserId,
                                "CreateTicket"
                                );

                                return RedirectToAction("ListBills", "Billing", new { Success = "Successfully Added" });
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

                }
                return View(model);
            }
        }

        public ActionResult EditBill(string TourCode, string Success)
        {
            using (Entities context = new Entities())
            {
                EditTourModel model = new EditTourModel();

                try
                {
                    ViewBag.Message = Success;
                    int? _tourCode = Shared.ToInt(TourCode);

                    model = (from c in context.sp_frm_get_Tours(_tourCode, null, WebSecurity.CurrentUserId, "ListTickets", null, null)
                             select new EditTourModel
                             {
                                 TourCode = c.TourCode,
                                 CustomerName = c.CustomerName,
                                 Remarks = c.Remarks,
                                 TrxnDate = DateTime.Parse(c.TrxnDate),
                                 BuyingPrice = Shared.ToDecimal(c.BuyingPrice),
                                 SellingPrice = Shared.ToDecimal(c.SellingPrice)

                             }).FirstOrDefault(i => i.TourCode == _tourCode);

                }
                catch (Exception ex)
                {
                    return View(model);
                }
                finally
                {
                    
                }
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditBill(EditTourModel model, FormCollection frm)
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
                                _affectedRows = context.sp_frm_add_upd_Tours(
                                   model.TourCode,
                                  Shared.ToString(model.CustomerName).ToUpper(),
                                  Shared.ToString(model.Remarks).ToUpper(),
                                   model.TrxnDate,
                                   model.BuyingPrice,
                                   model.SellingPrice,
                                   WebSecurity.CurrentUserId,
                                   "CreateBill"
                                );

                                return RedirectToAction("ListBills", "Billing", new { Success = "Successfully Updated" });
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
                   
                }
            }
        }
      
        #endregion

    }
}
