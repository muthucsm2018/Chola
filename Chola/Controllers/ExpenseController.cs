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
    public class ExpenseController : Controller
    {
        #region CommonFunctions
       

        #endregion

        #region Expense

        public ActionResult ListExpense(string Success)
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

        private DataSet ReturnQueryExpense(string ExpenseName)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[5];
                sqlParams[0] = new SqlParameter("@ExpenseCode", SqlDbType.Int);
                sqlParams[0].Value = DBNull.Value;
                sqlParams[1] = new SqlParameter("@ExpenseName", SqlDbType.NVarChar, 100);
                if (Shared.ToString(ExpenseName).Length > 0)
                    sqlParams[1].Value = ExpenseName;
                else
                    sqlParams[1].Value = DBNull.Value;
               
                sqlParams[2] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[2].Value = WebSecurity.CurrentUserId;
                sqlParams[3] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[3].Value = "Expense";
                sqlParams[4] = new SqlParameter("@ParmType", SqlDbType.Int);
                sqlParams[4].Value = 2;

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Expense", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<ExpenseListModel> GetListExpense(string ExpenseName)
        {
            DataSet ds = ReturnQueryExpense(ExpenseName);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new ExpenseListModel
                {
                    ExpenseCode = Shared.ToString(dataRow.Field<Int32>("ExpenseCode")),
                    ExpenseName = dataRow.Field<string>("ExpenseName"),
                    Charge = Shared.ToString(dataRow.Field<decimal>("Charge")),
                    Remarks = dataRow.Field<string>("Remarks"),
                    TrxnDate = dataRow.Field<string>("CreatedDate")
                });
                return trxnList.ToList();
            }
            else

                return new List<ExpenseListModel>();
        }

        public JsonResult QueryExpenseList(string ExpenseName)
        {
            try
            {
                List<ExpenseListModel> query = GetListExpense(ExpenseName);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.ExpenseCode, x.ExpenseName, x.Charge, x.Remarks, x.TrxnDate })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion

        #region CreateEditExpense

        public ActionResult CreateExpense(string Success)
        {
            using (Entities context = new Entities())
            {
                CreateExpenseModel model = new CreateExpenseModel();                       

                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateExpense(CreateExpenseModel model, FormCollection frm)
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
                                _affectedRows = context.sp_frm_add_upd_Expense(
                                    (int?)null,
                                    model.ExpenseName.ToUpper(),
                                    Shared.ToDecimal(model.Charge),
                                    model.Remarks,
                                    WebSecurity.CurrentUserId,
                                    "Expense"
                                );

                                return RedirectToAction("ListExpense", "Expense", new { Success = "Successfully Added" });

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

        public ActionResult EditExpense(string ExpenseCode, string Success)
        {
            using (Entities context = new Entities())
            {
                CreateExpenseModel model = new CreateExpenseModel();

                try
                {
                    ViewBag.Message = Success;
                    int? _ExpenseCode = Shared.ToInt(ExpenseCode);

                    model = (from c in context.sp_frm_get_Expense(_ExpenseCode, null, WebSecurity.CurrentUserId, "Expense", 1, null, null)
                             select new CreateExpenseModel
                             {
                                 ExpenseCode = c.ExpenseCode,
                                 ExpenseName = Shared.ToString(c.ExpenseName),
                                 Charge = Shared.ToString(c.Charge),
                                 Remarks = c.Remarks
                             }).FirstOrDefault(i => i.ExpenseCode == _ExpenseCode);

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
        public ActionResult EditExpense(CreateExpenseModel model, FormCollection frm)
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
                                _affectedRows = context.sp_frm_add_upd_Expense(
                                     model.ExpenseCode,
                                     model.ExpenseName.ToUpper(),
                                     Shared.ToDecimal(model.Charge),
                                     model.Remarks,
                                     WebSecurity.CurrentUserId,
                                     "Expense"
                               );

                                return RedirectToAction("ListExpense", "Expense", new { Success = "Successfully Updated" });
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
