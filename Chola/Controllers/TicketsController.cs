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
    public class TicketsController : Controller
    {
        #region CommonFunctions


        #endregion

        #region ListTickets
       

        public List<TicketCustomersListModel> GetListSaleItems(Int64 ReceiptNo)
        {
            using (Entities context = new Entities())
            {

                    var trxnList = context.Ticket_Customers.Where( s=> s.TicketCode == ReceiptNo).AsEnumerable().Select(dataRow => new TicketCustomersListModel
                    {
                        TicketCode = dataRow.TicketCode,
                        CustomerCode = dataRow.CustomerCode,
                        CustomerName = dataRow.CustomerName,
                        Description = dataRow.Description,
                        Remarks = dataRow.Remarks,
                        BuyingPrice = dataRow.BuyingPrice,
                        SellingPrice = dataRow.SellingPrice

                    });
                    return trxnList.ToList();
            }
        }

        public JsonResult TicketItemsListView(string TicketCode)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    List<TicketCustomersListModel> query = new List<TicketCustomersListModel>();
                    Int64 _ticketcode = Int64.Parse(TicketCode);
                    query = context.Ticket_Customers.Where(s => s.TicketCode == _ticketcode).AsEnumerable().Select(dataRow => new TicketCustomersListModel
                    {
                        TicketCode = dataRow.TicketCode,
                        CustomerCode = dataRow.CustomerCode,
                        CustomerName = dataRow.CustomerName,
                        Description = dataRow.Description,
                        Remarks = dataRow.Remarks,
                        BuyingPrice = dataRow.BuyingPrice,
                        SellingPrice = dataRow.SellingPrice
                    }).ToList();

                    return Json(new
                    {
                        aaData = query.Select(x => new[] { x.CustomerName, x.Description, x.Remarks, Shared.ToString(x.BuyingPrice), Shared.ToString(x.SellingPrice) })
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Result = "ERROR", Message = ex.Message });
                }
            }
        }

        public JsonResult TicketItemsList(string TicketCode)
        {
            try
            {
                List<TicketCustomersListModel> query = GetListSaleItems(Int64.Parse(TicketCode));

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.TicketCode + "|" + x.CustomerCode, x.CustomerName, x.Description, x.Remarks, String.Format("{0:n}", Shared.ToString(x.BuyingPrice)), String.Format("{0:n}", Shared.ToString(x.SellingPrice)) })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult ListTickets(string Success)
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

        [HttpPost]
        public ActionResult AddTicketItem(string TicketCode, string CustomerName, string Description, string Remarks, string BuyingPrice, string SellingPrice)
        {

            using (Entities context = new Entities())
            {
                try
                {
                    if (Shared.ToString(TicketCode).Length > 0)
                    {
                        if (Shared.ToString(CustomerName).Length == 0)
                        {
                            return Json(new { success = false, response = "Customer Name is Required!" });
                        }
                        if (Shared.ToString(Description).Length == 0)
                        {
                            return Json(new { success = false, response = "Description is Required!" });
                        }
                        if (Shared.ToString(Remarks).Length == 0)
                        {
                            return Json(new { success = false, response = "Remarks is Required!" });
                        }
                        if (Shared.ToString(BuyingPrice).Length == 0)
                        {
                            return Json(new { success = false, response = "Buying Price is Required!" });
                        }
                        if (Shared.ToString(SellingPrice).Length == 0)
                        {
                            return Json(new { success = false, response = "Selling Price is Required!" });
                        }

                        Ticket_Customers tbl = new Ticket_Customers();
                        tbl.TicketCode = Int64.Parse(TicketCode);
                        tbl.CustomerName = CustomerName.ToUpper();
                        tbl.Description = Description.Replace("\n", "<br />").ToUpper();
                        tbl.Remarks = Remarks.ToUpper();
                        tbl.BuyingPrice = Shared.ToDecimal(BuyingPrice);
                        tbl.SellingPrice = Shared.ToDecimal(SellingPrice);

                        context.Ticket_Customers.Add(tbl);
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
        public ActionResult DeleteTicketItem(string TicketCode)
        {
            Int64? _ticketcode = null;
            int? _customercode = null;

            using (Entities context = new Entities())
            {
                try
                {
                    if (Shared.ToString(TicketCode).Length > 0)
                    {
                        _ticketcode = Int64.Parse(TicketCode.Split('|')[0]);
                        _customercode = Shared.ToInt(TicketCode.Split('|')[1]);

                        var report = (from d in context.Ticket_Customers
                                      where d.TicketCode == _ticketcode && d.CustomerCode == _customercode
                                      select d).Single();
                        context.Ticket_Customers.Remove(report);
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
        public ActionResult GetTicketDetails(string TicketCode)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    TicketCustomersListModel model = new TicketCustomersListModel();

                    model = context.Ticket_Customers.Where(s => s.TicketCode == Shared.ToInt(TicketCode)).Select(dataRow => new TicketCustomersListModel
                    {
                        CustomerName = dataRow.CustomerName,
                        Description = dataRow.Description,
                        Remarks = dataRow.Remarks,
                        BuyingPrice = dataRow.BuyingPrice,
                        SellingPrice = dataRow.SellingPrice
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

        private DataSet ReturnQueryTicket(string CustomerName, string RefNo)
        {
            DataSet dsQuery = new DataSet();

            try
            {
                SqlParameter[] sqlParams = new SqlParameter[5];
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
                sqlParams[4] = new SqlParameter("@RefNo", SqlDbType.NVarChar, 50);
                if (Shared.ToString(RefNo).Length > 0)
                    sqlParams[4].Value = Shared.ToString(RefNo);
                else
                    sqlParams[4].Value = DBNull.Value;
                dsQuery = db_data_access.FetchRS(CommandType.StoredProcedure, "sp_frm_get_Tickets", sqlParams);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, Shared.ToString(ex.InnerException).Length > 0 ? ex.InnerException.Message : ex.Message);
            }

            return dsQuery;
        }

        public List<TicketListModel> GetListTicket(string VendorName, string RefNo)
        {
            DataSet ds = ReturnQueryTicket(VendorName, RefNo);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var trxnList = ds.Tables[0].AsEnumerable().Select(dataRow => new TicketListModel
                {
                    TicketCode = Shared.ToString(dataRow.Field<Int64>("TicketCode")),
                    CompanyName = dataRow.Field<string>("VendorName"),
                    Charge = Shared.ToString(dataRow.Field<decimal?>("Amount")),
                    TrxnDate = dataRow.Field<string>("TrxnDate"),
                    DueDate = dataRow.Field<string>("DueDate")
                });
                return trxnList.ToList();
            }
            else

                return new List<TicketListModel>();
        }

        public JsonResult QueryTicketList(string VendorName, string RefNo)
        {
            try
            {
                List<TicketListModel> query = GetListTicket(VendorName, RefNo);

                return Json(new
                {
                    aaData = query.Select(x => new[] { x.TicketCode, x.CompanyName, String.Format("{0:n}", x.Charge), x.TrxnDate, x.DueDate, x.TicketCode, x.TicketCode })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult Invoice(string BillId)
        {
            using (Entities context = new Entities())
            {

                
                    Int64? _TicketCode = Int64.Parse(BillId);
                    var ticket = (from c in context.Tickets where c.TicketCode == _TicketCode select c).FirstOrDefault();
                    var customer = (from c in context.Acc_Vendors where c.VendorCode == ticket.VendorCode select c).FirstOrDefault();

                    ViewBag.CustomerName = Shared.ToString(customer.VendorName);
                    ViewBag.ContactNo = Shared.ToString(ticket.ContactNo);
                    ViewBag.InvoiceNo = Shared.ToString(_TicketCode);
                    ViewBag.Addr1 = Shared.ToString(customer.Address1);
                    ViewBag.Addr2 = Shared.ToString(customer.Address2);
                    ViewBag.Addr3 = Shared.ToString(customer.Address3);
                    ViewBag.TaxID = Shared.ToString(customer.TaxID);
                    ViewBag.DueDate = Shared.ToString(ticket.DueDate.Value.ToString("dd MMM yyyy"));
                    ViewBag.InvoiceDate = Shared.ToString(ticket.TrxnDate.Value.ToString("dd MMM yyyy"));

                    var trxnList = context.Ticket_Customers.Where(s => s.TicketCode == _TicketCode).AsEnumerable().Select(dataRow => new TicketCustomersListModel
                    {
                        TicketCode = dataRow.TicketCode,
                        CustomerCode = dataRow.CustomerCode,
                        CustomerName = dataRow.CustomerName,
                        Description = dataRow.Description,
                        Remarks = dataRow.Remarks,
                        SellingPrice = dataRow.SellingPrice
                    }).ToList();

                    ViewBag.Customers = trxnList;
                    var result = context.Ticket_Customers.Where(s => s.TicketCode == _TicketCode).Sum(a => a.SellingPrice);
                    ViewBag.Amount = result;
                    ViewBag.AmountinWords = DecimalToWords(Shared.ToDecimal(result));


                return View();
            }
        }

        public string DecimalToWords(decimal number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + DecimalToWords(Math.Abs(number));

            string words = "";

            int intPortion = (int)number;
            decimal fraction = (number - intPortion) * 100;
            int decPortion = (int)fraction;

            words = NumberToWords(intPortion);
            if (decPortion > 0)
            {
                words += " and ";
                words += NumberToWords(decPortion);
            }
            return words;
        }

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        #endregion

        #region CreateEditTicket
        public ActionResult CreateTicket(string Success)
        {
            using (Entities context = new Entities())
            {
                CreateTicketModel model = new CreateTicketModel();

                ViewBag.VendorList = (from c in context.sp_frm_get_parm_Acc_Vendors(WebSecurity.CurrentUserId, "Vendors", null)
                                      select new SelectListItem
                                      {
                                          Text = c.VendorName,
                                          Value = Shared.ToString(c.VendorCode)
                                      }).ToList();
                ViewBag.CustomerList = (from c in context.sp_frm_get_parm_Customers(WebSecurity.CurrentUserId, "Vendors")
                                        select new SelectListItem
                                        {
                                            Text = c.CustomerName,
                                            Value = Shared.ToString(c.RefNo)
                                        }).ToList();
                Int64 maxId = 0;
                var _maxId = "";
                var count = context.Tickets.AsEnumerable().Count();
                if (count > 0)
                {
                    try
                    {
                        maxId = context.Tickets.Max(p => p.TicketCode);
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
                model.TicketCode = Int64.Parse(_maxId);

                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTicket(CreateTicketModel model, FormCollection frm)
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
                        if (Shared.ToInt(model.VendorCode) == 0) ModelState.AddModelError("VendorCode", "Customer Name is Required.");
                        if (!model.DueDate.HasValue) ModelState.AddModelError("DueDate", "Due Date is Required.");

                        if (ModelState.IsValid)
                        {
                            try
                            {
                                _affectedRows = context.sp_frm_add_upd_Tickets(
                                    model.TicketCode,
                                    Shared.ToInt(model.VendorCode),                               
                                    DateTime.Now,
                                    model.DueDate,
                                    WebSecurity.CurrentUserId,                               
                                    "CreateTicket", model.RefNo,
                                    model.ContactNo
                                );

                                return RedirectToAction("ListTickets", "Tickets", new { Success = "Successfully Added" });
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
                    ViewBag.VendorList = (from c in context.sp_frm_get_parm_Acc_Vendors(WebSecurity.CurrentUserId, "Vendors", null)
                                          select new SelectListItem
                                          {
                                              Text = c.VendorName,
                                              Value = Shared.ToString(c.VendorCode)
                                          }).ToList();
                    ViewBag.CustomerList = (from c in context.sp_frm_get_parm_Customers(WebSecurity.CurrentUserId, "Vendors")
                                            select new SelectListItem
                                            {
                                                Text = c.CustomerName,
                                                Value = Shared.ToString(c.RefNo)
                                            }).ToList();

                }
                return View(model);
            }
        }

        public ActionResult EditTicket(string TicketId, string Success)
        {
            using (Entities context = new Entities())
            {
                EditTicketModel model = new EditTicketModel();

                try
                {
                    ViewBag.Message = Success;
                    Int64? _Ticketid = Int64.Parse(TicketId);

                    model = (from c in context.sp_frm_get_Tickets(_Ticketid, null, WebSecurity.CurrentUserId, "ListTickets", null, null, null)
                             select new EditTicketModel
                             {
                                 TicketCode = c.TicketCode,
                                 VendorCode = Shared.ToInt(c.VendorCode),
                                 DueDate = DateTime.Parse(c.DueDate), 
                                 Charge = Shared.ToString(c.Amount),
                                 RefNo = Shared.ToString(c.RefNo),
                                 ContactNo = Shared.ToString(c.ContactNo)
                                 
                             }).FirstOrDefault(i => i.TicketCode == _Ticketid);

                    var count = context.Acc_Vendors.AsEnumerable().Count(p => p.VendorCode == model.VendorCode);
                    if (count > 0)
                    {
                        var _exist = (from c in context.Acc_Vendors where c.VendorCode == model.VendorCode select c).FirstOrDefault();

                        model.Addr1 = Shared.ToString(_exist.Address1);
                        model.Addr2 = Shared.ToString(_exist.Address2);
                        model.Addr3 = Shared.ToString(_exist.Address3);
                        model.TaxID = Shared.ToString(_exist.TaxID);
                    }
                    else
                    {

                    }

                }
                catch (Exception ex)
                {
                    return View(model);
                }
                finally
                {
                    ViewBag.VendorList = (from c in context.sp_frm_get_parm_Acc_Vendors(WebSecurity.CurrentUserId, "Vendors", null)
                                          select new SelectListItem
                                          {
                                              Text = c.VendorName,
                                              Value = Shared.ToString(c.VendorCode)
                                          }).ToList();
                    ViewBag.CustomerList = (from c in context.sp_frm_get_parm_Customers(WebSecurity.CurrentUserId, "Vendors")
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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditTicket(EditTicketModel model, FormCollection frm)
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
                        if (!model.DueDate.HasValue) ModelState.AddModelError("DueDate", "Due Date is Required.");
                        if (ModelState.IsValid)
                        {
                            try
                            {
                                _affectedRows = context.sp_frm_add_upd_Tickets(
                                   model.TicketCode,
                               Shared.ToInt(model.VendorCode),                               
                               DateTime.Now,
                               model.DueDate,
                                WebSecurity.CurrentUserId,
                                "CreateTicket",
                                model.RefNo,
                                model.ContactNo
                                );

                                return RedirectToAction("ListTickets", "Tickets", new { Success = "Successfully Updated" });
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
                    ViewBag.VendorList = (from c in context.sp_frm_get_parm_Acc_Vendors(WebSecurity.CurrentUserId, "Vendors", null)
                                          select new SelectListItem
                                          {
                                              Text = c.VendorName,
                                              Value = Shared.ToString(c.VendorCode)
                                          }).ToList();
                    ViewBag.CustomerList = (from c in context.sp_frm_get_parm_Customers(WebSecurity.CurrentUserId, "Vendors")
                                            select new SelectListItem
                                            {
                                                Text = c.CustomerName,
                                                Value = Shared.ToString(c.RefNo)
                                            }).ToList();

                }
            }
        }

        [HttpPost]
        public ActionResult GetCustomer(string CustomerID)
        {
            using (Entities context = new Entities())
            {
                try
                {
                    string addr1 = "";
                    string addr2 = ""; string addr3 = ""; string taxid = "";

                    int _documenttype = Shared.ToInt(CustomerID);
                    var count = context.Acc_Vendors.AsEnumerable().Count(p => p.VendorCode == _documenttype);
                    if (count > 0)
                    {
                        var _exist = (from c in context.Acc_Vendors where c.VendorCode == _documenttype select c).FirstOrDefault();
                        
                        addr1 = Shared.ToString(_exist.Address1);
                        addr2 = Shared.ToString(_exist.Address2);
                        addr3 = Shared.ToString(_exist.Address3);
                        taxid = Shared.ToString(_exist.TaxID);
                    }
                    else
                    {

                    }

                    return Json(new { success = true, Address1 = addr1, Address2 = addr2, Address3 = addr3, TaxID = taxid });
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
