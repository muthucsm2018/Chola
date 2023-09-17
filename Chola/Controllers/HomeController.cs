using Chola.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Chola.Controllers
{
    public class HomeController: Controller
    {
        public ActionResult RedirectPage()
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        // Get Home Page
                        string _homeURL = (from c in context.sp_frm_Security_get_UserHomePage(WebSecurity.CurrentUserId) select c).SingleOrDefault();
                        string _controller = Shared.ToString(_homeURL.Split('/')[1]);
                        string _pageName = Shared.ToString(_homeURL.Split('/')[2]);

                        return RedirectToAction(_pageName, _controller);
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

        private DataTable ReturnTodayTrxnDetails()
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@CustomerId", SqlDbType.Int);
                sqlParams[0].Value = WebSecurity.CurrentUserId;

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Transaction_Details", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery.Tables[0];
        }

        public ActionResult Index()
        {
            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        var _isAdmin = context.security_UsersInRoles.AsEnumerable().Count(p => p.UserID == WebSecurity.CurrentUserId && p.RoleID == 1);
                        if (_isAdmin == 0)
                            return RedirectToAction("Login", "Account");

                        var count = context.Status.AsEnumerable().Count();
                        if (count > 0)
                        {
                            ViewBag.StatusList = context.Status.ToList().Select(c => new SelectListItem
                            {
                                Value = c.StatusCode.ToString(),
                                Text = c.StatusName
                            });
                        }
                        else
                        {
                            ViewBag.StatusList = new SelectListItem
                            {
                                Text = "",
                                Value = ""
                            };
                        }

                        DataTable dt = ReturnTodayTrxnDetails();

                        ViewBag.OtherAmount = dt.Rows[0]["NoofOtherAmount"];
                        ViewBag.VisaAmount = dt.Rows[0]["NoofVisaAmount"];
                        ViewBag.TotalAmount = dt.Rows[0]["NoofTotalAmount"];
                        ViewBag.NoofCollections = dt.Rows[0]["NoofCollections"];
                        ViewBag.NoofDelivered = dt.Rows[0]["NoofDelivered"];
                        ViewBag.TotalExpense = dt.Rows[0]["NoofExpense"];
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

        private DataSet ReturnQueryBill(string BarCodeNo, string CustomerName, string PassportNo, string InvoiceNo, DateTime? TransactionFrom, DateTime? TransactionTo, string Status)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[11];
                sqlParams[0] = new SqlParameter("@TransactionID", SqlDbType.Int);
                sqlParams[0].Value = DBNull.Value;
                sqlParams[1] = new SqlParameter("@BarCodeNo", SqlDbType.NVarChar, 256);
                if (Shared.ToString(BarCodeNo).Length > 0)
                    sqlParams[1].Value = BarCodeNo;
                else
                    sqlParams[1].Value = DBNull.Value;
                sqlParams[2] = new SqlParameter("@CustomerName", SqlDbType.NVarChar, 256);
                if (Shared.ToString(CustomerName).Length > 0)
                    sqlParams[2].Value = CustomerName;
                else
                    sqlParams[2].Value = DBNull.Value;
                sqlParams[3] = new SqlParameter("@PassportNo", SqlDbType.NVarChar, 50);
                if (Shared.ToString(PassportNo).Length > 0)
                    sqlParams[3].Value = PassportNo;
                else
                    sqlParams[3].Value = DBNull.Value;
                sqlParams[4] = new SqlParameter("@InvoiceNo", SqlDbType.NVarChar, 50);
                if (Shared.ToString(InvoiceNo).Length > 0)
                    sqlParams[4].Value = InvoiceNo;
                else
                    sqlParams[4].Value = DBNull.Value;
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
                sqlParams[7] = new SqlParameter("@StatusCode", SqlDbType.Int);
                if (Shared.ToString(Status).Length > 0)
                    sqlParams[7].Value = Shared.ToInt(Status);
                else
                    sqlParams[7].Value = DBNull.Value;
                sqlParams[8] = new SqlParameter("@CreatedBy", SqlDbType.Int);
                sqlParams[8].Value = DBNull.Value;
                sqlParams[9] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[9].Value = WebSecurity.CurrentUserId;
                sqlParams[10] = new SqlParameter("@PageName", SqlDbType.VarChar, 50);
                sqlParams[10].Value = "ListBills";

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Transactions", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<TransactionListModel> GetListBill(string BarCodeNo, string CustomerName, string PassportNo, string InvoiceNo, DateTime? TransactionFrom, DateTime? TransactionTo, string Status)
        {
            DataSet ds = ReturnQueryBill(BarCodeNo, CustomerName, PassportNo, InvoiceNo, TransactionFrom, TransactionTo, Status);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new TransactionListModel
                {
                    TransactionID = Shared.ToString(dataRow.Field<Int32>("TransactionID")),
                    DocumentName = dataRow.Field<string>("DocumentName"),
                    CustomerName = dataRow.Field<string>("CustomerName"),
                    InvoiceNumber = dataRow.Field<string>("InvoiceNumber"),
                    PassportNumber = dataRow.Field<string>("PassportNumber"),
                    ContactNo = dataRow.Field<string>("ContactNo"),                    
                    StatusName = dataRow.Field<string>("StatusName"),
                    PaidAmount = Shared.ToString(dataRow.Field<decimal>("PaidAmount")),
                    TransactionDate = Shared.ToString(dataRow.Field<DateTime>("TransactionDate").ToString("MMM dd yyyy h:mm tt")),
                });
                return trxnList.ToList();
            }
            else

                return new List<TransactionListModel>();
        }

        public JsonResult QueryBillList(string BarCodeNo, string CustomerName, string PassportNo, string InvoiceNo, string daterange, string Status)
        {
            try
            {
                //02/02/2016 - 02/02/2016
                DateTime? _startdate = null;
                DateTime? _enddate = null;

                if (Shared.ToString(daterange).Length > 0)
                {
                    _startdate = DateTime.Parse(daterange.Split('|')[0].Trim());
                    _enddate = DateTime.Parse(daterange.Split('|')[1].Trim());
                }


                List<TransactionListModel> query = GetListBill(BarCodeNo, CustomerName, PassportNo, InvoiceNo, _startdate, _enddate, "1");

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.TransactionID, x.InvoiceNumber, x.DocumentName, x.PaidAmount, x.CustomerName, x.PassportNumber, x.StatusName, x.TransactionDate, x.TransactionID, x.TransactionID })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private DataSet ReturnQueryEntry(string OthersTypeCode)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[5];
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
                sqlParams[4].Value = 1;

                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Miscellaneous", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<EntryListModel> GetListEntry(string OthersTypeCode)
        {
            DataSet ds = ReturnQueryEntry(OthersTypeCode);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new EntryListModel
                {
                    OthersCode = Shared.ToString(dataRow.Field<Int32>("OthersCode")),
                    OthersTypeName = dataRow.Field<string>("OtherTypeName"),
                    Charge = Shared.ToString(dataRow.Field<decimal>("Charge")),
                    Remarks = dataRow.Field<string>("Remarks"),
                    CreatedBy = dataRow.Field<string>("CreatedBy"),
                    CreatedDate = dataRow.Field<string>("CreatedDate"),
                    UpdatedBy = dataRow.Field<string>("UpdatedBy"),
                    UpdatedDate = dataRow.Field<string>("UpdatedDate")
                });
                return trxnList.ToList();
            }
            else

                return new List<EntryListModel>();
        }

        public JsonResult QueryEntryList(string OthersTypeCode)
        {
            try
            {
                List<EntryListModel> query = GetListEntry(OthersTypeCode);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.OthersCode, x.OthersTypeName, x.Charge, x.Remarks, x.CreatedBy, x.CreatedDate, x.UpdatedBy, x.UpdatedDate })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
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
                sqlParams[4].Value = 1;

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
                    CreatedBy = dataRow.Field<string>("CreatedBy"),
                    CreatedDate = dataRow.Field<string>("CreatedDate"),
                    UpdatedBy = dataRow.Field<string>("UpdatedBy"),
                    UpdatedDate = dataRow.Field<string>("UpdatedDate")
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
                    aaData = query.Select(x => new[] { x.ExpenseCode, x.ExpenseName, x.Charge, x.Remarks, x.CreatedBy, x.CreatedDate, x.UpdatedBy, x.UpdatedDate })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult Contact()
        {
            return View();
        }

        #region Menu       

        public IList<ChildMenu> GetChildMenus(int? parentid)
        {
            IList<ChildMenu> result;

            using (Entities context = new Entities())
            {
                result = (from du in context.sp_frm_Security_TreeViewMenu(WebSecurity.CurrentUserId, "SUBMENU", parentid)
                          select new ChildMenu
                          {
                              PageID = du.PageID,
                              PageTitle = du.PageTitle,
                              NavigateUrl = du.PageURL
                          }).ToList();
            }

            return result;
        }

        public ActionResult Menu()
        {
            IEnumerable<ParentMenu> menu = null;

            using (Entities context = new Entities())
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {
                        menu = (from app in context.sp_frm_Security_TreeViewMenu(WebSecurity.CurrentUserId, "MAIN", 0)
                                select new ParentMenu
                                {
                                    ID = app.PageID,
                                    Text = app.PageTitle,
                                    PageUrl = app.PageURL,
                                    childs = GetChildMenus(app.PageID)
                                }).AsEnumerable();
                    }
                    catch
                    {

                    }
                    finally
                    {
                        var _isAdmin = context.security_UsersInRoles.AsEnumerable().Count(p => p.UserID == WebSecurity.CurrentUserId && p.RoleID == 1);
                        if (_isAdmin > 0)
                            ViewBag.UserAuth = "true";
                        else
                            ViewBag.UserAuth = "false";
                    }
                    return PartialView(menu);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        #endregion

        #region CommonFunctions

        #endregion    
    }
}