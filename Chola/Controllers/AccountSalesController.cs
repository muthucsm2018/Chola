using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Chola.Models;
using WebMatrix.WebData;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Chola.Controllers
{
    public class AccountSalesController : Controller
    {      
        #region CommonFunctions  

        
       
        #endregion   

        #region AccountPayment

        [HttpPost]
        public ActionResult GetVendors(string VendorCode)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    List<SelectListItem> vendors_list = null;
                    int? _vendorcode = Shared.ToInt(VendorCode);

                    var vendors = (from c in context.sp_frm_get_parm_Acc_Vendors(WebSecurity.CurrentUserId, "", null)
                                   select new SelectListItem
                                   {
                                       Text = c.VendorName,
                                       Value = Shared.ToString(c.VendorCode),
                                       Selected = c.VendorCode.Equals(_vendorcode)
                                   }).ToList();

                    if (vendors != null)
                    {
                        vendors_list = (List<SelectListItem>)vendors.ToList();
                        return Json(new { success = true, response = vendors_list });
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

        private static DataTable ReturnTrxnDetails(string VendorCode)
        {
            DataSet dsQuery = new DataSet();

            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@VendorCode", SqlDbType.Int);
            if (Shared.ToString(VendorCode).Length > 0)
                sqlParams[0].Value = Shared.ToInt(VendorCode);
            else
                sqlParams[0].Value = DBNull.Value;

            dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_AccSales_Users", sqlParams);

            return dsQuery.Tables[0];
        }

        public ActionResult AccountPayment()
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    AccountPaymentModel model = new AccountPaymentModel();

                    try
                    {
                        ViewBag.Message = "";
                    }
                    catch
                    {

                    }
                    finally
                    {
                        ViewBag.VendorList = (from c in context.sp_frm_get_parm_Acc_Vendors(WebSecurity.CurrentUserId, "", null)
                                              select new SelectListItem
                                              {
                                                  Text = c.VendorName,
                                                  Value = Shared.ToString(c.VendorCode)
                                              }).ToList();

                    }
                    model.PaymentDate = DateTime.Now;
                    model.PaymentBy = "CASH";
                    DataTable dt = ReturnTrxnDetails(null);
                    ViewBag.NetAmount = Shared.ToString(dt.Rows[0]["Charge"]);
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        private DataSet ReturnQueryAccountPayment(string VendorCode)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@VendorCode", SqlDbType.Int);
                if (Shared.ToString(VendorCode).Length > 0)
                    sqlParams[0].Value = Shared.ToInt(VendorCode);
                else
                    sqlParams[0].Value = DBNull.Value;

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_AccSales_Payments", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<AccountPaymentListModel> GetListAccountPayment(string VendorCode)
        {
            DataSet ds = ReturnQueryAccountPayment(VendorCode);
            DataTable dt = ReturnTrxnDetails(VendorCode);
            var NetAmount = Shared.ToString(dt.Rows[0]["Charge"]);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new AccountPaymentListModel
                {
                    TableKey = Shared.ToString(dataRow.Field<Int32>("TableKey")),
                    VendorName = Shared.ToString(dataRow.Field<string>("VendorName")),
                    PaymentDate = dataRow.Field<string>("PaymentDate"),
                    PaymentBy = dataRow.Field<string>("PaymentBy"),
                    PaymentAmount = Shared.ToString(dataRow.Field<double?>("PaymentAmount")),
                    BalanceAmount = NetAmount
                });
                return trxnList.ToList();
            }
            else

                return new List<AccountPaymentListModel>();
        }

        public JsonResult AccountPaymentList(string VendorCode)
        {
            try
            {
                List<AccountPaymentListModel> query = GetListAccountPayment(VendorCode);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.TableKey, x.VendorName, x.PaymentDate, x.PaymentBy, x.PaymentAmount,  x.BalanceAmount })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetAccountPaymentDetails(string TableKey)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    AccountPaymentModel model = new AccountPaymentModel();
                    int? _tablekey = Shared.ToInt(TableKey);

                    model = (from c in context.Acc_SalesPayments
                             where c.TableKey == _tablekey
                             select new AccountPaymentModel
                             {
                                 TableKey = c.TableKey,
                                 VendorCode = c.VendorCode,
                                 PaymentDate = c.PaymentDate,
                                 PaymentBy = c.PaymentBy,
                                 PaymentAmount = c.PaymentAmount
                             }).FirstOrDefault();

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
        public ActionResult GetNetAmount(string VendorCode)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    DataTable dt = ReturnTrxnDetails(VendorCode);
                    var model = Shared.ToString(dt.Rows[0]["Charge"]);

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
        public ActionResult AccountPayment(AccountPaymentModel model, FormCollection frm)
        {
            int _affectedRows = 0;
            using (Entities context = new Entities())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        _affectedRows = context.sp_frm_add_upd_AccSalesPayment(
                                    Shared.ToInt(model.VendorCode),
                                    model.PaymentDate,
                                    model.PaymentBy,
                                    Shared.ToDouble(model.PaymentAmount)
                                    );
                        model.TableKey = null;
                        model.VendorCode = null;
                        model.PaymentDate = DateTime.Now;
                        model.PaymentBy = "CASH";
                        model.PaymentAmount = 0;
                    }
                    ViewBag.Message = "Successfully Added!";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
                }
                finally
                {

                    ViewBag.VendorList = (from c in context.sp_frm_get_parm_Acc_Vendors(WebSecurity.CurrentUserId, "", null)
                                          select new SelectListItem
                                          {
                                              Text = c.VendorName,
                                              Value = Shared.ToString(c.VendorCode)
                                          }).ToList();

                }
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult DeleteAccountPayment(string TableKey)
        {
            int? _tablekey = null;

            using (Entities context = new Entities())
            {
                try
                {
                    if (Shared.ToString(TableKey).Length > 0)
                    {
                        _tablekey = Shared.ToInt(TableKey);

                        context.Database.ExecuteSqlCommand("delete from Acc_SalesPayments WHERE TableKey = " + TableKey + "");
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

        #endregion

        #region CustomerPayment

        [HttpPost]
        public ActionResult GetCustomers(string RefNo)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    List<SelectListItem> Customers_list = null;

                    var Customers = (from c in context.sp_frm_get_parm_Customers(WebSecurity.CurrentUserId, "")
                                   select new SelectListItem
                                   {
                                       Text = c.CustomerName,
                                       Value = Shared.ToString(c.RefNo),
                                       Selected = c.RefNo.Equals(RefNo)
                                   }).ToList();

                    if (Customers != null)
                    {
                        Customers_list = (List<SelectListItem>)Customers.ToList();
                        return Json(new { success = true, response = Customers_list });
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

        private static DataTable ReturnTrxnDetailsCustomer(string RefNo)
        {
            DataSet dsQuery = new DataSet();

            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@RefNo", SqlDbType.NVarChar, 50);
            if (Shared.ToString(RefNo).Length > 0)
                sqlParams[0].Value = Shared.ToString(RefNo);
            else
                sqlParams[0].Value = DBNull.Value;

            dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_AccSales_Customers", sqlParams);

            return dsQuery.Tables[0];
        }

        public ActionResult CustomerPayment()
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    CustomerPaymentModel model = new CustomerPaymentModel();

                    try
                    {
                        ViewBag.Message = "";
                    }
                    catch
                    {

                    }
                    finally
                    {
                        ViewBag.CustomerList = (from c in context.sp_frm_get_parm_Customers(WebSecurity.CurrentUserId, "")
                                              select new SelectListItem
                                              {
                                                  Text = c.CustomerName,
                                                  Value = Shared.ToString(c.RefNo)
                                              }).ToList();

                    }
                    model.PaymentDate = DateTime.Now;
                    model.PaymentBy = "CASH";
                    DataTable dt = ReturnTrxnDetailsCustomer(null);
                    ViewBag.NetAmount = Shared.ToString(dt.Rows[0]["Charge"]);
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        private DataSet ReturnQueryCustomerPayment(string RefNo)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@RefNo", SqlDbType.NVarChar, 50);
                if (Shared.ToString(RefNo).Length > 0)
                    sqlParams[0].Value = Shared.ToString(RefNo);
                else
                    sqlParams[0].Value = DBNull.Value;

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Customer_Payments", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<CustomerPaymentListModel> GetListCustomerPayment(string RefNo)
        {
            DataSet ds = ReturnQueryCustomerPayment(RefNo);
            DataTable dt = ReturnTrxnDetailsCustomer(RefNo);
            var NetAmount = Shared.ToString(dt.Rows[0]["Charge"]);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new CustomerPaymentListModel
                {
                    PaymentID = Shared.ToString(dataRow.Field<Int32>("PaymentID")),
                    RefNo = Shared.ToString(dataRow.Field<string>("RefNo")),
                    CustomerName = Shared.ToString(dataRow.Field<string>("CustomerName")),
                    PaymentDate = Shared.ToString(dataRow.Field<string>("PaymentDate")),
                    PaymentBy = dataRow.Field<string>("PaymentBy"),
                    PaymentAmount = Shared.ToString(dataRow.Field<decimal?>("Amount")),
                    BalanceAmount = NetAmount
                });
                return trxnList.ToList();
            }
            else

                return new List<CustomerPaymentListModel>();
        }

        public JsonResult CustomerPaymentList(string RefNo)
        {
            try
            {
                List<CustomerPaymentListModel> query = GetListCustomerPayment(RefNo);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.PaymentID, x.RefNo, x.CustomerName, x.PaymentDate, x.PaymentBy, x.PaymentAmount, x.BalanceAmount })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetCustomerPaymentDetails(string PaymentID)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    CustomerPaymentModel model = new CustomerPaymentModel();
                    int? _tablekey = Shared.ToInt(PaymentID);

                    model = (from c in context.CustomerAccount_Payment
                             where c.PaymentID == _tablekey
                             select new CustomerPaymentModel
                             {
                                 PaymentID = c.PaymentID,
                                 RefNo = c.RefNo,
                                 PaymentDate = c.TrxnDate,
                                 PaymentBy = c.PaymentBy,
                                 PaymentAmount = c.Amount
                             }).FirstOrDefault();

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
        public ActionResult GetNetAmountCustomer(string RefNo)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    DataTable dt = ReturnTrxnDetailsCustomer(RefNo);
                    var model = Shared.ToString(dt.Rows[0]["Charge"]);

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
        public ActionResult CustomerPayment(CustomerPaymentModel model, FormCollection frm)
        {
            int _affectedRows = 0;
            using (Entities context = new Entities())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        _affectedRows = context.sp_frm_add_upd_CustomerPayment(
                                    Shared.ToInt(model.PaymentID),
                                    model.RefNo,
                                    Shared.ToDecimal(model.PaymentAmount),
                                    model.PaymentDate,
                                    model.PaymentBy
                                    );
                        model.PaymentID = null;
                        model.RefNo = null;
                        model.PaymentDate = DateTime.Now;
                        model.PaymentBy = "CASH";
                        model.PaymentAmount = 0;
                    }
                    ViewBag.Message = "Successfully Added!";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
                }
                finally
                {

                    ViewBag.CustomerList = (from c in context.sp_frm_get_parm_Customers(WebSecurity.CurrentUserId, "")
                                          select new SelectListItem
                                          {
                                              Text = c.CustomerName,
                                              Value = Shared.ToString(c.RefNo)
                                          }).ToList();

                }
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult DeleteCustomerPayment(string PaymentID)
        {
            int? _tablekey = null;

            using (Entities context = new Entities())
            {
                try
                {
                    if (Shared.ToString(PaymentID).Length > 0)
                    {
                        _tablekey = Shared.ToInt(PaymentID);

                        context.Database.ExecuteSqlCommand("delete from CustomerAccount_Payment WHERE PaymentID = " + PaymentID + "");
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

        #endregion

    }
}
