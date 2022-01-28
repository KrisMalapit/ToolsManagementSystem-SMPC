using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ToolsManagementSystem.Models;
using ToolsManagementSystem.DAL;
using ToolsManagementSystem.Models.View_Model;
using PagedList;
using System.Text.RegularExpressions;
using Microsoft.Reporting.WebForms;
using System.Linq.Dynamic;
using System.Globalization;

namespace ToolsManagementSystem.Controllers
{
    [CustomAuthorize]

    public class ItemController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();
        ReportParameter[] p;
        // GET: /Items/
        public ActionResult Index(int? page, string searchString)
        {
            //ViewBag.Columns = ItemColumns();
            //ViewBag.SearchStr = searchString;

            //var items = db.Items.Where(i => i.Status == "Active" || i.Status == "InActive").ToList();
            //var items2 = items;

            //int limit = 15;

            //    if (!String.IsNullOrEmpty(searchString))
            //    {
            //        items = db.Items.Where(i => i.Description.ToUpper().Contains(searchString.ToUpper()) || i.ItemCode.ToUpper().Contains(searchString.ToUpper()) ||
            //            i.SerialNo.ToUpper().Contains(searchString.ToUpper()) || i.Description2.ToUpper().Contains(searchString.ToUpper()) || i.EquipmentType.ToUpper().Contains(searchString.ToUpper())
            //            ).Where(b => b.Status == "Active" || b.Status == "InActive").ToList();
            //    }
            //switch (page)
            //{
            //    case null:
            //        items2 = items.OrderBy(i => i.id).Skip(0).Take(limit).ToList();
            //        break;
            //    case 1:
            //        items2 = items.OrderBy(i => i.id).Skip(0).Take(limit).ToList();
            //        break;
            //    default:
            //        int offset = (Convert.ToInt32(page) - 1) * limit;
            //        items2 = items.OrderBy(i => i.id).Skip(offset).Take(limit).ToList();
            //        break;
            //}

            //var itemList = new List<ItemViewModel>();
            //var itemList2 = new List<ItemViewModel>();

            //foreach (var item in items)
            //{
            //    var goalModel = new ItemViewModel(item, 0); //0 is for dummy purpose only
            //    itemList.Add(goalModel);

            //}
            //foreach (var item2 in items2)
            //{
            //    var goalModel = new ItemViewModel(item2,TotalInv(item2.id));
            //    itemList2.Add(goalModel);
            //}
            //ViewBag.ItemList = itemList2;
            //int pageSize = 15;
            //int pageNumber = (page ?? 1);
            //return View(itemList.OrderBy(i => i.id).ToPagedList(pageNumber, pageSize));
            return View();



        }

        public List<SelectListItem> ItemColumns()
        {
            List<SelectListItem> itemCol = new List<SelectListItem>();
            itemCol.Add(new SelectListItem { Text = "Item Code", Value = "ItemCode", Selected = true });
            itemCol.Add(new SelectListItem { Text = "Description", Value = "Description" });
            itemCol.Add(new SelectListItem { Text = "Description 2", Value = "Description2" });
            itemCol.Add(new SelectListItem { Text = "Shelf No", Value = "ShelfNo" });
            itemCol.Add(new SelectListItem { Text = "Category", Value = "Category" });
            itemCol.Add(new SelectListItem { Text = "Equipment Type", Value = "EquipmentType" });
            return itemCol;
        }

        // GET: /Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }



            ViewBag.Category = item.Category;
            ViewBag.UOM = item.UOM;
            ViewBag.ID = id;
            ViewBag.Inventory = Convert.ToString(TotalInv(item.id));

            var model = db.ItemDetails.Where(i => i.ItemID == id).Where(i => i.Status == "Active").OrderBy(i => i.Description);
            if (model != null)
            {

                ViewBag.Qty = "0";

            }
            else
            {
                int totalqty = model.Sum(i => i.Qty);
                ViewBag.Qty = totalqty.ToString();

            }



            return View(item);


        }

        // GET: /Items/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,ItemCode,Description,Description2,ModelType,SerialNo,ShelfNo,Category,Location,UOM,EquipmentType")] Item item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Items.Add(item);
                    db.SaveChanges();
                    int lastid = db.Items.Max(a => a.id);
                    return RedirectToAction("Details", new { id = lastid });
                }
            }
            catch (Exception e)
            {

                ModelState.AddModelError("ItemCode", e.InnerException.InnerException.Message);
                return View(item);
            }


            return View(item);
        }

        // GET: /Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: /Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,ItemCode,Description,Description2,ModelType,SerialNo,ShelfNo,Category,Location,UOM,Status,EquipmentType")] Item item)
        {
            string stat = "";
            if (item.Status == "false")
            {
                stat = "Active";
            }
            else
            {
                stat = "InActive";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    item.Status = stat;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = item.id });
                }
                catch (Exception)
                {
                    ModelState.AddModelError("ItemCode", "Item Code already exist");
                    return View(item);
                }

            }
            return View(item);
        }

        ////Check If Item Code Existing
        //public ActionResult IsTagAvailble(string Tag)
        //{

        //        try
        //        {
        //            var tag = db.TABLE_NAME.Single(m => m.Tag == Tag);
        //            return Json(false, JsonRequestBehavior.AllowGet);
        //        }
        //        catch (Exception)
        //        {
        //            return Json(true, JsonRequestBehavior.AllowGet);
        //        }

        //}
        // GET: /Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: /Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            item.Status = "Deleted";
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult ItemDetails(int offset, int limit, string search, string sort, string order, int itemid, string filter)
        {

            DateTime dval = new DateTime();
            Boolean hasDateDelivered = false;

            var totalcount = 0;
            var model = db.ItemDetails.Select(b => new
            {
                EntryType = b.ToolStatus == "Serviceable" ? "Positive Adjustment" : "Negative Adjustment",
                b.id,
                b.ItemID,
                b.FACardNo,
                b.PropertyNo,
                b.Description,
                b.Description2,
                b.PO,
                b.SerialNo,
                b.Qty,
                b.ModelType,
                b.UoM,
                UnitCost = b.UnitCost.ToString(),
                b.ShelfNo,
                DateAdjusted = b.DateAdjusted.ToString(),
                b.ToolStatus,
                b.Status,
                b.ReferenceNo,
                b.Notes,
                DateDelivered = b.DateDelivered.Month.ToString() + "/" + b.DateDelivered.Day.ToString() + "/" + b.DateDelivered.Year.ToString()
            }).Where(b => b.Status == "Active");

            model = model.Where(i => i.ItemID == itemid).OrderBy(i => i.Description);

            //dynamic filter starts here
            string o = "";
            string q = filter;
            string strFilter = "";


            if (!string.IsNullOrEmpty(q))
            {
                q = filter.Replace("\\", "");
                string[] res = new string[(q.Length) - 1];
                if (q.Contains(","))
                {
                    var elements = q.Split(',');
                    int j = 0;
                    for (int i = 0; i < elements.Count(); i++)
                    {
                        o = "";
                        o = elements[i].Replace(':', '=').Replace("{", "").Replace("}", "").Replace(@"\", "");
                        string colSearch = o.Before("=").Replace('"', ' ').Replace(" ", "");
                        string colval = o.After("=");

                        //if (colSearch == "DateDelivered")
                        //{
                        //    colval = colval.Substring(1, 10);
                        //    dval = DateTime.Parse(colval);
                        //    hasDateDelivered = true;
                        //}
                        //else
                        //{
                        if (colSearch == "Qty")
                        {
                            colval = colval.Replace("\"", ""); //remove escape character "\"
                        }

                        if (!string.IsNullOrEmpty(strFilter))
                        {
                            strFilter = (colSearch != "Qty") ? (strFilter + " && " + colSearch + ".Contains(" + colval + ")") : (strFilter + " && " + "(" + colSearch + "=" + colval + ")");
                        }
                        else
                        {
                            strFilter = (colSearch != "Qty") ? (colSearch + ".Contains(" + colval + ")") : "(" + colSearch + "=" + colval + ")";

                        }
                        //}

                    }

                }

                else
                {

                    o = "";
                    o = q.Replace(':', '=').Replace("{", "").Replace("}", "").Replace(@"\", "");
                    string colSearch = o.Before("=").Replace('"', ' ').Replace(" ", "");
                    string colval = o.After("=");

                    //if (colSearch == "DateDelivered")
                    //{
                    //    colval = colval.Substring(1, 10);
                    //    dval = DateTime.Parse(colval);
                    //    hasDateDelivered = true;
                    //}
                    //else
                    //{

                    if (colSearch == "Qty")
                    {
                        colval = colval.Replace("\"", ""); //remove escape character "\"
                    }


                    strFilter = (colSearch != "Qty") ? (colSearch + ".Contains(" + colval + ")") : "(" + colSearch + "=" + colval + ")";
                    //}


                }

            }
            else
            {
                if (!string.IsNullOrEmpty(search))
                {
                    model = model.Where(b => b.Description.Contains(search) || b.FACardNo.Contains(search) || b.PropertyNo.Contains(search) || b.Description.Contains(search) || b.Description2.Contains(search)
                        || b.PO.Contains(search) || b.SerialNo.Contains(search));
                }
            }

            string filterQuery = strFilter;

            if (!string.IsNullOrEmpty(filterQuery))
            {
                model = model.Where(filterQuery);
            }


            //if (hasDateDelivered)
            //{
            //    model = model.Where("DateDelivered = @0", dval);
            //}


            totalcount = model.Count();
            model = model.OrderBy(i => i.id).Skip(offset).Take(limit);



            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "EntryType":
                        switch (order)
                        {
                            case "asc":
                                model = model.OrderBy(i => i.ToolStatus).Skip(offset).Take(limit);
                                break;
                            case "desc":
                                model = model.OrderByDescending(i => i.ToolStatus).Skip(offset).Take(limit);
                                break;
                        }
                        break;
                    case "PO":
                        switch (order)
                        {
                            case "asc":
                                model = model.OrderBy(i => i.PO).Skip(offset).Take(limit);
                                break;
                            case "desc":
                                model = model.OrderByDescending(i => i.PO).Skip(offset).Take(limit);
                                break;
                        }
                        break;
                    case "Qty":
                        switch (order)
                        {
                            case "asc":
                                model = model.OrderBy(i => i.Qty).Skip(offset).Take(limit);
                                break;
                            case "desc":
                                model = model.OrderByDescending(i => i.Qty).Skip(offset).Take(limit);
                                break;
                        }
                        break;
                    case "UnitCost":
                        switch (order)
                        {
                            case "asc":
                                model = model.OrderBy(i => i.UnitCost).Skip(offset).Take(limit);
                                break;
                            case "desc":
                                model = model.OrderByDescending(i => i.UnitCost).Skip(offset).Take(limit);
                                break;
                        }
                        break;
                    case "UoM":
                        switch (order)
                        {
                            case "asc":
                                model = model.OrderBy(i => i.UoM).Skip(offset).Take(limit);
                                break;
                            case "desc":
                                model = model.OrderByDescending(i => i.UoM).Skip(offset).Take(limit);
                                break;
                        }
                        break;
                    case "SerialNo":
                        switch (order)
                        {
                            case "asc":
                                model = model.OrderBy(i => i.SerialNo).Skip(offset).Take(limit);
                                break;
                            case "desc":
                                model = model.OrderByDescending(i => i.SerialNo).Skip(offset).Take(limit);
                                break;
                        }

                        break;
                    case "DateDelivered":
                        switch (order)
                        {
                            case "asc":
                                model = model.OrderBy(i => i.DateDelivered).Skip(offset).Take(limit);
                                break;
                            case "desc":
                                model = model.OrderByDescending(i => i.DateDelivered).Skip(offset).Take(limit);
                                break;
                        }
                        break;
                }
            }


            var modelItem = new
            {
                total = totalcount,
                rows = model.ToList(),
            };
            return Json(modelItem, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSub(FixedAssetInput item, bool isMulti, string DateDelivereds)
        {
            item.DateDelivered = DateTime.ParseExact(DateDelivereds, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            string s = item.ReferenceNo;
            var Y = s.Split('-');
            string lno = Y[1].ToString();

            ItemDetail x = new ItemDetail();

            if (isMulti)
            {
                int cnt = item.SubQty;
                for (int i = 1; i <= cnt; i++)
                {
                    x.ItemID = item.ItemID;
                    x.FACardNo = item.SubFACardNo;
                    x.PropertyNo = item.SubPropertyNo;
                    x.Description = item.SubDescription;
                    x.Description2 = item.SubDescription2;
                    x.PO = item.SubPO;
                    if (i == 1)
                    {
                        x.SerialNo = item.SubSerialNo;
                    }
                    else
                    {
                        x.SerialNo = "";
                    }


                    x.Qty = 1;
                    x.ModelType = item.SubModelType;
                    x.UoM = item.SubUoM;
                    x.UnitCost = Convert.ToDecimal(item.SubUnitCost);
                    x.ShelfNo = item.SubShelfNo;
                    x.ToolStatus = item.SubToolStatus;
                    x.Notes = item.Notes;
                    x.ReferenceNo = item.ReferenceNo;
                    x.DateDelivered = item.DateDelivered;
                    x.DateCreated = DateTime.Now;
                    db.ItemDetails.Add(x);
                    db.SaveChanges();
                }
            }
            else
            {
                x.ItemID = item.ItemID;
                x.FACardNo = item.SubFACardNo;
                x.PropertyNo = item.SubPropertyNo;
                x.Description = item.SubDescription;
                x.Description2 = item.SubDescription2;
                x.PO = item.SubPO;
                x.SerialNo = item.SubSerialNo;
                x.Qty = item.SubQty;
                x.ModelType = item.SubModelType;
                x.UoM = item.SubUoM;
                x.UnitCost = Convert.ToDecimal(item.SubUnitCost);
                x.ShelfNo = item.SubShelfNo;
                x.ToolStatus = item.SubToolStatus;
                x.Notes = item.Notes;
                x.ReferenceNo = item.ReferenceNo;
                x.DateDelivered = item.DateDelivered;
                x.DateCreated = DateTime.Now;
                db.ItemDetails.Add(x);
                db.SaveChanges();
            }





            NoSeries ns = db.NoSeries.SingleOrDefault(v => v.Code == "ADJ");
            ns.LastNo = lno;
            db.SaveChanges();



            ItemLog itl = new ItemLog();
            itl.ItemID = item.ItemID;
            itl.Module = "ItemDetail";
            itl.EntryType = "Create";
            itl.Quantity = item.SubQty;
            db.ItemLogs.Add(itl);
            db.SaveChanges();

            Log log = new Log();
            log.descriptions = "Added record id:" + x.id + " Table [ItemDetail]";
            log.otherinfo = "Qty : " + item.SubQty.ToString() + "Serial No : " + item.SubSerialNo + "UnitCost : " + item.SubUnitCost;
            db.Logs.Add(log);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = item.ItemID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSub(FixedAssetInput item, bool isMulti,string DateDelivereds)
        {
            item.DateDelivered = DateTime.ParseExact(DateDelivereds, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            DateTime itmdtls = db.ItemDetails.Where(a => a.id == item.appid).Select(a => a.DateCreated).FirstOrDefault();
            ItemDetail x = new ItemDetail();

            if (isMulti)
            {
                int cnt = item.SubQty;
                for (int i = 1; i <= cnt; i++)
                {

                    if (i == 1)
                    {
                        x.ItemID = item.ItemID;
                        x.id = item.appid;
                        x.FACardNo = item.SubFACardNo;
                        x.PropertyNo = item.SubPropertyNo;
                        x.Description = item.SubDescription;
                        x.Description2 = item.SubDescription2;
                        x.PO = item.SubPO;
                        x.SerialNo = item.SubSerialNo;
                        x.Qty = 1;
                        x.ModelType = item.SubModelType;
                        x.UoM = item.SubUoM;
                        x.UnitCost = Convert.ToDecimal(item.SubUnitCost);
                        x.ShelfNo = item.SubShelfNo;
                        x.ToolStatus = item.SubToolStatus;
                        x.Notes = item.Notes;
                        x.ReferenceNo = item.ReferenceNo;
                        x.DateDelivered = item.DateDelivered;
                        x.DateCreated = itmdtls;
                        db.Entry(x).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        x.ItemID = item.ItemID;
                        x.FACardNo = item.SubFACardNo;
                        x.PropertyNo = item.SubPropertyNo;
                        x.Description = item.SubDescription;
                        x.Description2 = item.SubDescription2;
                        x.PO = item.SubPO;
                        x.SerialNo = item.SubSerialNo;
                        x.Qty = 1;
                        x.ModelType = item.SubModelType;
                        x.UoM = item.SubUoM;
                        x.UnitCost = Convert.ToDecimal(item.SubUnitCost);
                        x.ShelfNo = item.SubShelfNo;
                        x.ToolStatus = item.SubToolStatus;
                        x.Notes = item.Notes;
                        x.ReferenceNo = item.ReferenceNo;
                        x.DateDelivered = item.DateDelivered;
                        x.DateCreated = itmdtls;
                        db.ItemDetails.Add(x);
                        db.SaveChanges();
                    }



                }
            }
            else
            {
                x.ItemID = item.ItemID;
                x.id = item.appid;
                x.FACardNo = item.SubFACardNo;
                x.PropertyNo = item.SubPropertyNo;
                x.Description = item.SubDescription;
                x.Description2 = item.SubDescription2;
                x.PO = item.SubPO;
                x.SerialNo = item.SubSerialNo;
                x.Qty = item.SubQty;
                x.ModelType = item.SubModelType;
                x.UoM = item.SubUoM;
                x.UnitCost = Convert.ToDecimal(item.SubUnitCost);
                x.ShelfNo = item.SubShelfNo;
                x.ToolStatus = item.SubToolStatus;
                x.Notes = item.Notes;
                x.ReferenceNo = item.ReferenceNo;
                x.DateDelivered = item.DateDelivered;
                x.DateCreated = itmdtls;
                db.Entry(x).State = EntityState.Modified;
                db.SaveChanges();
            }




            ItemLog itl = new ItemLog();
            itl.ItemID = item.ItemID;
            itl.Module = "ItemDetail";
            itl.EntryType = "Modify";
            itl.Quantity = item.SubQty;
            db.ItemLogs.Add(itl);
            db.SaveChanges();

            Log log = new Log();
            log.descriptions = "Modified record id:" + x.id + " Table [ItemDetail]";
            log.otherinfo = "Qty : " + item.SubQty.ToString() + "Serial No : " + item.SubSerialNo + "UnitCost : " + item.SubUnitCost;
            db.Logs.Add(log);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = item.ItemID });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteSub(string itemid, string remarks)
        {

            int i = 0;
            int[] res = new int[(itemid.Length) - 1];
            JsonArray result = new JsonArray();

            try
            {
                var elements = itemid.Split(',');
                foreach (var item in elements)
                {
                    i = Convert.ToInt32(item);
                    ItemDetail subitem = db.ItemDetails.Find(i);
                    subitem.Status = "Deleted";
                    subitem.Remarks = remarks;
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();

                    ItemLog itl = new ItemLog();
                    itl.ItemID = subitem.ItemID;
                    itl.Module = "ItemDetail";
                    itl.EntryType = "Delete";
                    itl.Quantity = subitem.Qty;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();

                    Log log = new Log();
                    log.descriptions = "Deleted record id:" + item + " Table [ItemDetail]";
                    db.Logs.Add(log);
                    db.SaveChanges();
                }

                result.message = "success";

            }
            catch (Exception e)
            {
                result.message = "fail";
                throw;
            }


            return Json(result);


        }
        public JsonResult SearchItem_Json(string q)
        {

            var x = db.Items.ToList();

            var model = db.Items.Select(b => new
            {
                id = b.id,
                text = b.ItemCode + " " + b.Description + " | " + b.Description2,
                status = b.Status
            }).Where(b => b.status == "Active");

            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(b => b.text.Contains(q));
            }


            var modelItem = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelItem, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OverDueList(int? page, string dateRange)
        {
            ViewBag.DateRange = dateRange;

            var items = OverDueItems(dateRange);
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            //if (!String.IsNullOrEmpty(dateRange))
            //{
            //    if (pageNumber > 1)
            //    {
            //        pageNumber = 1;
            //    }
            //}
            return View(items.OrderByDescending(i => i.Description).ToPagedList(pageNumber, pageSize));

        }
        public ActionResult dataOverDueList(int draw, int start, int length, string strcode, int noCols, string dateRange)
        {
            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;

            var items = OverDueItems(dateRange);
            var v = items.AsEnumerable();

            recordsTotal = v.Count();



            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.ItemCode.ToUpper().Contains(searchValue.ToUpper()) ||
                    a.Description.ToUpper().Contains(searchValue.ToUpper()) ||
                    a.RefNo.ToUpper().Contains(searchValue.ToUpper()) ||
                    a.DateIssued.ToString().Contains(searchValue.ToUpper()) ||
                    a.DueDate.ToString().Contains(searchValue.ToUpper()) ||
                    a.EmpID.ToString().Contains(searchValue.ToUpper()) ||
                    a.EmployeeName.ToString().Contains(searchValue.ToUpper()) ||
                    a.QtyIssued.ToString().Contains(searchValue.ToUpper()) ||
                    a.QtyReturn.ToString().Contains(searchValue.ToUpper())
                );
            }


            DateTime dval = new DateTime();
            DateTime dval2 = new DateTime();
            Boolean hasDateIssued = false;
            Boolean hasDueDate = false;

            string strFilter = "";
            for (int i = 0; i < noCols; i++)
            {
                string colval = Request["columns[" + i + "][search][value]"];
                if (colval != "")
                {
                    colval = colval.ToUpper();
                    string colSearch = Request["columns[" + i + "][data]"];
                    if (colSearch == "DateIssued")
                    {
                        dval = DateTime.Parse(colval);
                        hasDateIssued = true;
                    }
                    else if (colSearch == "DueDate")
                    {
                        dval2 = DateTime.Parse(colval);
                        hasDueDate = true;
                    }
                    else
                    {
                        if (strFilter == "")
                        {
                            if (colval == "*")
                            {
                                strFilter = "(" + colSearch + " != " + "" + ")";
                            }
                            else
                            {

                                //strFilter = colSearch + ".Contains(" + "\"" + colval + "\"" + ")";
                                strFilter = (colSearch != "QtyIssued" && colSearch != "QtyReturn") ? colSearch + ".ToUpper().Contains(" + "\"" + colval + "\"" + ")" : "(" + colSearch + "=" + colval + ")";
                            }
                        }
                        else
                        {
                            //strFilter = strFilter + " && " + colSearch + ".Contains(" + "\"" + colval + "\"" + ")";
                            strFilter = (colSearch != "QtyIssued" && colSearch != "QtyReturn") ? strFilter + " && " + colSearch + ".ToUpper().Contains(" + "\"" + colval + "\"" + ")" : strFilter + " && " + "(" + colSearch + "=" + colval + ")";
                        }
                    }
                    if (strFilter != "")
                    {
                        v = v.Where(strFilter);
                    }
                }
            }

            if (hasDateIssued)
            {
                v = v.Where("DateIssued = @0", dval);
            }
            if (hasDueDate)
            {
                v = v.Where("DueDate = @0", dval2);
            }

            recordsFiltered = v.Count();


            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                string col = Request["columns[" + sortColumn + "][data]"];
                v = v.OrderBy(col + " " + sortColumnDir);
            }


            var x = v.ToList();
            if (pageSize < 0)
            {
                pageSize = recordsFiltered;
            }
            var allCustomer = v.Skip(skip).Take(pageSize).ToList();

            var model = new
            {
                draw = draw,
                recordsFiltered = recordsFiltered,
                recordsTotal = recordsTotal,
                data = allCustomer
            };
            return Json(model, JsonRequestBehavior.AllowGet);

        }
        public int OverDueCount(string dateRange)
        {

            var items = OverDueItems(dateRange);
            var model = new
            {
                SumQty = items
                       .Select(a => a.QtyIssued)
                       .DefaultIfEmpty(0)
                       .Sum(),
                SumReturn = items
                        .Select(a => a.QtyReturn)
                        .DefaultIfEmpty(0)
                        .Sum(),
            };


            //ViewBag.SumQty = model.SumQty.ToString();
            //ViewBag.SumQtyAdj = model.SumQtyAdj.ToString();
            //ViewBag.Unreturn = (model.SumQtyUnreturn - model.SumQty - model.SumQtyAdj).ToString();

            int c = model.SumQty - model.SumReturn;
            return c;
        }
        public int OpenCount(string accType)
        {



            int c = 0;

            switch (accType)
            {
                case "EBC": c = db.AFBorrowers.Where(a => a.Status == "Active").Where(a => a.DocStatus == 0).Count();
                    break;
                case "EAC": c = db.AFEmployees.Where(a => a.Status == "Active").Where(a => a.DocStatus == 0).Count();
                    break;
                case "FA":
                    c = db.AFFAs.Where(a => a.Status == "Active").Where(a => a.DocStatus == 0).Count();
                    break;
            }


            
            return c;
        }
        public ActionResult UnReturn(int? page, int id)
        {
            var items = ViewUnReturn(id);
            ViewBag.ID = id;
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(items.OrderBy(i => i.RefNo).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ItemTracking(int? page, int id)
        {
            var items = ViewItemTracking(id, null, null, null);



            var model = new
            {
                SumQty = items
                       .Select(a => a.Qty)
                       .DefaultIfEmpty(0)
                       .Sum(),
                SumQtyAdj = items
                        .Select(a => a.QtyAdj)
                        .DefaultIfEmpty(0)
                        .Sum(),
                SumQtyDeployed = items
                        .Where(a => a.entrytype == "Positive Adjustment" || a.entrytype == "Negative Adjustment")
                        .Select(a => a.Qty)
                        .DefaultIfEmpty(0)
                        .Sum(),
                SumQtyLostUnreturned = items
                    .Where(a => a.entrytype == "Un-Returned")
                    .Select(a => a.QtyLostUnreturned)
                    .DefaultIfEmpty(0)
                    .Sum(),
                WarehouseDamaged = items
                   .Where(a => a.entrytype == "Negative Adjustment")
                   .Select(a => a.Qty * (-1))
                   .DefaultIfEmpty(0)
                   .Sum(),
            };


            ViewBag.SumQty = model.SumQty.ToString();
            ViewBag.SumQtyAdj = (model.SumQtyAdj + model.WarehouseDamaged).ToString();
            ViewBag.Deployed = (model.SumQtyDeployed - model.SumQty - model.SumQtyAdj - model.SumQtyLostUnreturned).ToString();
            ViewBag.LostUnreturn = model.SumQtyLostUnreturned.ToString();
            ViewBag.ItemId = id.ToString();

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(items.OrderByDescending(i => i.Date).ToPagedList(pageNumber, pageSize));
        }




        public List<ItemTrackingViewModel> ViewItemTracking(int id, DateTime? AsOf, DateTime? FromDate, DateTime? ToDate)
        {

            if (AsOf != null)
            {

            }
            if (FromDate != null && ToDate != null)
            {

            }

            //ItemDetails
            var items = db.ItemDetails
                .Where(i => i.Status == "Active" && i.ItemID == id)
                .Select(i => new ItemTrackingViewModel()
            {
                TransDate = i.DateDelivered
                ,
                ItemID = i.ItemID
               ,
                RefNo = i.ReferenceNo
               ,
                EmployeeName = ""
               ,
                Date = i.DateCreated
               ,
                DateAdjusted = i.DateAdjusted
               ,
                ItemCode = i.Items.ItemCode
               ,
                Description = i.Items.Description
               ,
                SerialNo = i.SerialNo
               ,
                PO = i.PO
                ,
                PropertyNo = i.PropertyNo
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty =
                 (
                i.ToolStatus == "Serviceable" ? i.Qty : i.Qty * (-1)
                 )
                ,
                QtyAdj = 0
                ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.UnitCost * i.Qty
                ,
                ToolStatus = i.ToolStatus
                ,
                entrytype =
                 (
                i.ToolStatus == "Serviceable" ? "Positive Adjustment" : "Negative Adjustment"
                 )
                ,
                Remarks = i.Notes
                ,
                Status = i.Status
                ,
                HeadStatus = i.Status
                ,
                DocStatus = 1
            })
                //AFEmployee
            .Concat(db.AFEmployeeIssues
            .Where(a => a.AFEmployees.Status == "Active" || a.AFEmployees.Status == "Transferred")
            .Where(a => a.Status == "Active" || a.Status == "Transferred")
            .Where(a => a.Quantity * (-1) != 0)
            .Where(a => a.ItemID == id)
            .Where(a => a.AFEmployees.DocStatus != 0)
            .Select(i => new ItemTrackingViewModel()
            {
                TransDate = i.AFEmployees.DateReleased
                ,
                ItemID = i.ItemID
                ,
                RefNo = i.AFEmployees.EACNo
                ,
                EmployeeName = i.AFEmployees.Employees.LastName + ", " + i.AFEmployees.Employees.FirstName
                ,
                Date = i.DateIssued
                ,
                DateAdjusted = i.Created_At
               ,
                ItemCode = i.Items.ItemCode
                ,
                Description = i.Items.Description
                ,
                SerialNo = i.SerialNo
                ,
                PO = i.PO
                ,
                PropertyNo = (
                    i.Items.Category == "CME" ? "" : i.Items.ItemDetails.Select(a => a.PropertyNo).FirstOrDefault()
                )
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty = i.Quantity * (-1)
                ,
                QtyAdj = 0
                ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.Quantity * i.UnitCost
                ,
                ToolStatus = "Serviceable"
                ,
                entrytype = "Issued"
                ,
                Remarks = i.Remarks
                ,
                Status = i.Status
                ,
                HeadStatus = i.AFEmployees.Status
                ,
                DocStatus = i.AFEmployees.DocStatus

            }))
            .Concat(db.AFEmployeeReturns
            .Where(a => a.Status == "Active" && a.AFEmployeeIssues.ItemID == id)
            .Where(a => a.DocStatus == 1)
            .Select(a => new ItemTrackingViewModel()
            {
                TransDate = a.DatePosted
                ,
                ItemID = a.AFEmployeeIssues.ItemID
                ,
                RefNo = a.AFEmployeeIssues.AFEmployees.EACNo
                ,
                EmployeeName = a.AFEmployeeIssues.AFEmployees.Employees.LastName + ", " + a.AFEmployeeIssues.AFEmployees.Employees.FirstName
                ,
                Date = a.DateReturned
                ,
                DateAdjusted = a.Created_At
               ,
                ItemCode = a.AFEmployeeIssues.Items.ItemCode
                ,
                Description = a.AFEmployeeIssues.Items.Description
                ,
                SerialNo = a.AFEmployeeIssues.SerialNo
                ,
                PO = a.AFEmployeeIssues.PO
                ,
                PropertyNo = (
                    a.AFEmployeeIssues.Items.Category == "CME" ? "" : a.AFEmployeeIssues.Items.ItemDetails.Select(x => x.PropertyNo).FirstOrDefault()
                )
                ,
                Location = a.AFEmployeeIssues.Items.Location
                ,
                UoM = a.AFEmployeeIssues.UoM
                ,
                Qty =
                 (
                a.ToolStatus == "Serviceable" ? a.Quantity : 0
                 )
                ,
                QtyAdj = (
                    a.ToolStatus == "Serviceable" ? 0 : (a.ToolStatus == "Lost_Unreturned" ? 0 : a.Quantity)
                    )
                ,
                QtyLostUnreturned = (a.ToolStatus == "Lost_Unreturned" ? a.Quantity : 0)
                ,
                UnitCost = a.AFEmployeeIssues.UnitCost
                ,
                Amount = a.Quantity * a.AFEmployeeIssues.UnitCost
                ,
                ToolStatus = a.ToolStatus
                ,
                entrytype = (
                        a.ToolStatus == "Serviceable" ? "Returned" : (a.ToolStatus == "Lost_Unreturned" ? "Un-Returned" : "Damaged")
                 )
                ,
                Remarks = a.Remarks
                ,
                Status = a.Status
                ,
                HeadStatus = a.Status
                ,
                DocStatus = a.DocStatus
            })
            )
            .Concat(db.AFEmployeeIssues
            .Where(a => a.AFEmployees.Status == "Transferred" || a.AFEmployees.Status == "Active")
            .Where(a => a.Status == "Transferred")
            .Where(a => (a.Quantity - a.QuantityAdj) != 0)
            .Where(a => a.ItemID == id)
            .Where(a => a.AFEmployees.DocStatus != 0)
            .Select(i => new ItemTrackingViewModel() //for transfer offsetting EAC
            {
                TransDate = i.AFEmployees.DateReleased
                ,
                ItemID = i.ItemID
                ,
                RefNo = i.AFEmployees.EACNo
                ,
                EmployeeName = i.AFEmployees.Employees.LastName + ", " + i.AFEmployees.Employees.FirstName
                ,
                Date = i.DateIssued
                ,
                DateAdjusted = i.Created_At
               ,
                ItemCode = i.Items.ItemCode
                ,
                Description = i.Items.Description
                ,
                SerialNo = i.SerialNo
                ,
                PO = i.PO
                ,
                PropertyNo = (
                    i.Items.Category == "CME" ? "" : i.Items.ItemDetails.Select(a => a.PropertyNo).FirstOrDefault()
                )
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty = i.Quantity - i.QuantityAdj
                ,
                QtyAdj = 0
                 ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.Amount
                ,
                ToolStatus = "Serviceable"
                ,
                entrytype = "Transferred"
                ,
                Remarks = i.Remarks
                ,
                Status = i.Status
                ,
                HeadStatus = i.AFEmployees.Status
                ,
                DocStatus = i.AFEmployees.DocStatus
            })
            )
                //AFBorrower
            .Concat(db.AFBorrowerIssues
            .Where(a => a.AFBorrowers.Status == "Active" || a.AFBorrowers.Status == "Transferred")
            .Where(a => a.Status == "Active" || a.Status == "Transferred")
            .Where(a => (a.Quantity * (-1)) != 0)
            .Where(a => a.ItemID == id)
            .Where(a => a.AFBorrowers.DocStatus != 0)
            .Select(i => new ItemTrackingViewModel()
            {
                TransDate = i.AFBorrowers.DateReleased
                ,
                ItemID = i.ItemID
                ,
                RefNo = i.AFBorrowers.EBCNo
                ,
                EmployeeName = i.AFBorrowers.Employees.LastName + ", " + i.AFBorrowers.Employees.FirstName
                ,
                Date = i.DateIssued
                ,
                DateAdjusted = i.Created_At
               ,
                ItemCode = i.Items.ItemCode
                ,
                Description = i.Items.Description
                ,
                SerialNo = i.SerialNo
                ,
                PO = i.PO
                ,
                PropertyNo = (
                     i.Items.Category == "CME" ? "" : i.Items.ItemDetails.Select(a => a.PropertyNo).FirstOrDefault()
                 )
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty = i.Quantity * (-1)
                ,
                QtyAdj = 0
                 ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.Quantity * i.UnitCost
                ,
                ToolStatus = "Serviceable"
                ,
                entrytype = "Issued"
                ,
                Remarks = i.Remarks
                ,
                Status = i.Status
                ,
                HeadStatus = i.AFBorrowers.Status
                ,
                DocStatus = i.AFBorrowers.DocStatus
            })
            )
            .Concat(db.AFBorrowerReturns
            .Where(a => a.Status == "Active" && a.AFBorrowerIssues.ItemID == id)
            .Where(a => a.DocStatus == 1)
            .Select(a => new ItemTrackingViewModel()
            {
                TransDate = a.DatePosted
                ,
                ItemID = a.AFBorrowerIssues.ItemID
                ,
                RefNo = a.AFBorrowerIssues.AFBorrowers.EBCNo
                ,
                EmployeeName = a.AFBorrowerIssues.AFBorrowers.Employees.LastName + ", " + a.AFBorrowerIssues.AFBorrowers.Employees.FirstName
                ,
                Date = a.DateReturned
                ,
                DateAdjusted = a.DateReturned
               ,
                ItemCode = a.AFBorrowerIssues.Items.ItemCode
                ,
                Description = a.AFBorrowerIssues.Items.Description
                ,
                SerialNo = a.AFBorrowerIssues.SerialNo
                ,
                PO = a.AFBorrowerIssues.PO
                ,
                PropertyNo = (
                    a.AFBorrowerIssues.Items.Category == "CME" ? "" : a.AFBorrowerIssues.Items.ItemDetails.Select(x => x.PropertyNo).FirstOrDefault()
                )
                    //PropertyNo = "Prop"
                ,
                Location = a.AFBorrowerIssues.Items.Location
                ,
                UoM = a.AFBorrowerIssues.UoM
                ,
                Qty =
                 (
                a.ToolStatus == "Serviceable" ? a.Quantity : 0
                 )
                ,
                QtyAdj = (
                   a.ToolStatus == "Serviceable" ? 0 : (a.ToolStatus == "Lost_Unreturned" ? 0 : a.Quantity)
                   )
                    ,
                QtyLostUnreturned = (a.ToolStatus == "Lost_Unreturned" ? a.Quantity : 0)
                    ,
                UnitCost = a.AFBorrowerIssues.UnitCost
                ,
                Amount = a.Quantity * a.AFBorrowerIssues.UnitCost
                ,
                ToolStatus = a.ToolStatus
                ,
                entrytype = (
                    a.ToolStatus == "Serviceable" ? "Returned" : (a.ToolStatus == "Lost_Unreturned" ? "Un-Returned" : "Damaged")
                 )
                ,
                Remarks = a.Remarks
                ,
                Status = a.Status
                ,
                HeadStatus = a.Status
                ,
                DocStatus = a.DocStatus
            }))
            .Concat(db.AFBorrowerIssues
            .Where(a => a.AFBorrowers.Status == "Active" || a.AFBorrowers.Status == "Transferred")
            .Where(a => a.Status == "Transferred")
            .Where(a => (a.Quantity - a.QuantityAdj) != 0)
            .Where(a => a.ItemID == id)
            .Where(a => a.AFBorrowers.DocStatus != 0)
            .Select(i => new ItemTrackingViewModel() // for transfer offsetting EBC
            {
                TransDate = i.AFBorrowers.DateReleased
                ,
                ItemID = i.ItemID
                ,
                RefNo = i.AFBorrowers.EBCNo
                ,
                EmployeeName = i.AFBorrowers.Employees.LastName + ", " + i.AFBorrowers.Employees.FirstName
                ,
                Date = i.DateIssued
                ,
                DateAdjusted = i.Created_At
               ,
                ItemCode = i.Items.ItemCode
                ,
                Description = i.Items.Description
                ,
                SerialNo = i.SerialNo
                ,
                PO = i.PO
                ,
                PropertyNo = (
                     i.Items.Category == "CME" ? "" : i.Items.ItemDetails.Select(a => a.PropertyNo).FirstOrDefault()
                 )
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty = i.Quantity - i.QuantityAdj
                ,
                QtyAdj = 0
                ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.Quantity * i.UnitCost
                ,
                ToolStatus = "Serviceable"
                ,
                entrytype = "Transferred"
                ,
                Remarks = i.Remarks
                ,
                Status = i.Status
                ,
                HeadStatus = i.AFBorrowers.Status
                ,
                DocStatus = i.AFBorrowers.DocStatus
            })
            )




            //AFFA

            .Concat(db.AFFAIssues
            .Where(a => a.AFFAs.Status == "Active" || a.AFFAs.Status == "Transferred")
            .Where(a => a.Status == "Active" || a.Status == "Transferred")
            .Where(a => (a.Quantity * (-1)) != 0)
            .Where(a => a.ItemID == id)
            .Where(a => a.AFFAs.DocStatus != 0)
            .Select(i => new ItemTrackingViewModel()
            {
                TransDate = i.AFFAs.DateReleased
                ,
                ItemID = i.ItemID
                ,
                RefNo = i.AFFAs.FAAFNo
                ,
                EmployeeName = i.AFFAs.Employees.LastName + ", " + i.AFFAs.Employees.FirstName
                ,
                Date = i.DateIssued
                ,
                DateAdjusted = i.Created_At
               ,
                ItemCode = i.Items.ItemCode
                ,
                Description = i.Items.Description
                ,
                SerialNo = i.SerialNo
                ,
                PO = i.PO
                ,
                PropertyNo = (
                     i.Items.Category == "CME" ? "" : i.Items.ItemDetails.Select(a => a.PropertyNo).FirstOrDefault()
                 )
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty = i.Quantity * (-1)
                ,
                QtyAdj = 0
                ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.Quantity * i.UnitCost
                ,
                ToolStatus = "Serviceable"
                ,
                entrytype = "Issued"
                ,
                Remarks = i.Remarks
                ,
                Status = i.Status
                ,
                HeadStatus = i.AFFAs.Status
                ,
                DocStatus = i.AFFAs.DocStatus
            }))
            .Concat(db.AFFAReturns
            .Where(a => a.Status == "Active" && a.AFFAIssues.ItemID == id)
            .Where(a => a.DocStatus == 1)
            .Select(a => new ItemTrackingViewModel()
            {
                TransDate = a.DatePosted
                ,
                ItemID = a.AFFAIssues.ItemID
                ,
                RefNo = a.AFFAIssues.AFFAs.FAAFNo
                ,
                EmployeeName = a.AFFAIssues.AFFAs.Employees.LastName + ", " + a.AFFAIssues.AFFAs.Employees.FirstName
                ,
                Date = a.DateReturned
                ,
                DateAdjusted = a.DateReturned
               ,
                ItemCode = a.AFFAIssues.Items.ItemCode
                ,
                Description = a.AFFAIssues.Items.Description
                ,
                SerialNo = a.AFFAIssues.SerialNo
                ,
                PO = a.AFFAIssues.PO
                ,
                PropertyNo = (
                     a.AFFAIssues.Items.Category == "CME" ? "" : a.AFFAIssues.Items.ItemDetails.Select(x => x.PropertyNo).FirstOrDefault()
                 )
                ,
                Location = a.AFFAIssues.Items.Location
                ,
                UoM = a.AFFAIssues.UoM
                ,
                Qty =
                 (
                a.ToolStatus == "Serviceable" ? a.Quantity : 0
                 )
                 ,
                QtyAdj = (
                    a.ToolStatus == "Serviceable" ? 0 : (a.ToolStatus == "Lost_Unreturned" ? 0 : a.Quantity)
                    )
                    ,
                QtyLostUnreturned = (a.ToolStatus == "Lost_Unreturned" ? a.Quantity : 0)
                ,
                UnitCost = a.AFFAIssues.UnitCost
                ,
                Amount = a.Quantity * a.AFFAIssues.UnitCost
                ,
                ToolStatus = a.ToolStatus
                ,
                entrytype = (
                a.ToolStatus == "Serviceable" ? "Returned" : (a.ToolStatus == "Lost_Unreturned" ? "Un-Returned" : "Damaged")
                 )
                ,
                Remarks = a.Remarks
                ,
                Status = a.Status
                ,
                HeadStatus = a.Status
                ,
                DocStatus = a.DocStatus
            }))
            .Concat(db.AFFAIssues
            .Where(a => a.AFFAs.Status == "Active" || a.AFFAs.Status == "Transferred")
            .Where(a => a.Status == "Transferred")
            .Where(a => (a.Quantity - a.QuantityAdj) != 0)
            .Where(a => a.ItemID == id)
            .Where(a => a.AFFAs.DocStatus != 0)
            .Select(i => new ItemTrackingViewModel() //for transferred offsetting
            {
                TransDate = i.AFFAs.DateReleased
                ,
                ItemID = i.ItemID
                ,
                RefNo = i.AFFAs.FAAFNo
                ,
                EmployeeName = i.AFFAs.Employees.LastName + ", " + i.AFFAs.Employees.FirstName
                ,
                Date = i.DateIssued
                ,
                DateAdjusted = i.Created_At
               ,
                ItemCode = i.Items.ItemCode
                ,
                Description = i.Items.Description
                ,
                SerialNo = i.SerialNo
                ,
                PO = i.PO
                ,
                PropertyNo = (
                i.Items.Category == "CME" ? "" : i.Items.ItemDetails.Select(a => a.PropertyNo).FirstOrDefault()
                 )
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty = i.Quantity - i.QuantityAdj
                ,
                QtyAdj = 0
                ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.Amount
                ,
                ToolStatus = "Serviceable"
                ,
                entrytype = "Transferred"
                ,
                Remarks = i.Remarks
                ,
                Status = i.Status
                ,
                HeadStatus = i.AFFAs.Status
                ,
                DocStatus = i.AFFAs.DocStatus
            }))
            .ToList();



            if (AsOf != null)
            {

                var d1 = DateTime.Parse(AsOf.ToString());
                items = items.Where("TransDate <= @0", d1).ToList();



            }



            return items;
        }


        public int TotalInv(int id)
        {

            var items = ViewItemTracking(id, null, null, null);

            var model = new
            {
                total_inv = items
                       .Select(a => a.Qty)
                       .DefaultIfEmpty(0)
                       .Sum()

            };


            return model.total_inv;


        }

        public List<ItemUnReturnViewModel> ViewUnReturn(int id)
        {
            var model = db.AFEmployeeIssues
            .Where(b => b.Status == "Active" || b.Status == "Transferred")
            .Where(b => b.ItemID == id)
            .Where(b => b.AFEmployees.DocStatus != 0)
            .Select(b => new ItemUnReturnViewModel()
            {
                ItemID = b.ItemID,
                DateIssued = b.DateIssued,
                RefNo = b.AFEmployees.EACNo,
                SerialNo = b.SerialNo,
                EmpID = b.AFEmployees.Employees.EmpId,
                EmployeeName = b.AFEmployees.Employees.LastName + "," + b.AFEmployees.Employees.FirstName,
                QtyIssued = (
                           b.Status == "Active" ? b.Quantity : b.QuantityAdj
                     ),
                QtyReturn = db.AFEmployeeReturns
                            .Where(a => a.DocStatus == 1)
                           .Where(a => a.Status == "Active")
                           .Where(a => a.ToolStatus != "Lost_Unreturned")
                           .Where(a => a.AFEmployeeIssueID == b.id)
                           .Select(a => a.Quantity)
                           .DefaultIfEmpty(0)
                           .Sum(),
                QtyLostUnreturned = db.AFEmployeeReturns
                           .Where(a => a.DocStatus == 1)
                           .Where(a => a.Status == "Active")
                           .Where(a => a.ToolStatus == "Lost_Unreturned")
                           .Where(a => a.AFEmployeeIssueID == b.id)
                           .Select(a => a.Quantity)
                           .DefaultIfEmpty(0)
                           .Sum(),
                Status = b.Status,
            })

                //AFBorrower
            .Concat(db.AFBorrowerIssues
            .Where(b => b.Status == "Active" || b.Status == "Transferred")
            .Where(b => b.ItemID == id)
            .Where(b => b.AFBorrowers.DocStatus != 0)
            .Select(b => new ItemUnReturnViewModel()
            {
                ItemID = b.ItemID,
                DateIssued = b.DateIssued,
                RefNo = b.AFBorrowers.EBCNo,
                SerialNo = b.SerialNo,
                EmpID = b.AFBorrowers.Employees.EmpId,
                EmployeeName = b.AFBorrowers.Employees.LastName + "," + b.AFBorrowers.Employees.FirstName,
                QtyIssued = (
                           b.Status == "Active" ? b.Quantity : b.QuantityAdj
                     ),
                QtyReturn = db.AFBorrowerReturns
                           .Where(a => a.DocStatus == 1)
                           .Where(a => a.Status == "Active")
                           .Where(a => a.ToolStatus != "Lost_Unreturned")
                           .Where(a => a.AFBorrowerIssueID == b.id)
                           .Select(a => a.Quantity)
                           .DefaultIfEmpty(0)
                           .Sum(),
                QtyLostUnreturned = db.AFBorrowerReturns
                         .Where(a => a.DocStatus == 1)
                         .Where(a => a.Status == "Active")
                         .Where(a => a.ToolStatus == "Lost_Unreturned")
                         .Where(a => a.AFBorrowerIssueID == b.id)
                         .Select(a => a.Quantity)
                         .DefaultIfEmpty(0)
                         .Sum(),
                Status = b.Status,
            }))
                //AFFA
             .Concat(db.AFFAIssues
            .Where(b => b.Status == "Active" || b.Status == "Transferred")
            .Where(b => b.ItemID == id)
            .Where(b => b.AFFAs.DocStatus != 0)
             .Select(b => new ItemUnReturnViewModel()
             {
                 ItemID = b.ItemID,
                 DateIssued = b.DateIssued,
                 RefNo = b.AFFAs.FAAFNo,
                 SerialNo = b.SerialNo,
                 EmpID = b.AFFAs.Employees.EmpId,
                 EmployeeName = b.AFFAs.Employees.LastName + "," + b.AFFAs.Employees.FirstName,
                 QtyIssued = (
                            b.Status == "Active" ? b.Quantity : b.QuantityAdj
                      ),
                 QtyReturn = db.AFFAReturns
                            .Where(a => a.DocStatus == 1)
                            .Where(a => a.Status == "Active")
                            .Where(a => a.ToolStatus != "Lost_Unreturned")
                            .Where(a => a.AFFAIssueID == b.id)
                            .Select(a => a.Quantity)
                            .DefaultIfEmpty(0)
                            .Sum(),
                 QtyLostUnreturned = db.AFFAReturns
                             .Where(a => a.DocStatus == 1)
                             .Where(a => a.Status == "Active")
                             .Where(a => a.ToolStatus == "Lost_Unreturned")
                             .Where(a => a.AFFAIssueID == b.id)
                             .Select(a => a.Quantity)
                             .DefaultIfEmpty(0)
                             .Sum(),
                 Status = b.Status,
             }))
            .ToList();


            model = model.Where(b => b.QtyIssued != (b.QtyReturn + b.QtyLostUnreturned)).ToList();

            return model;


        }
        public List<ItemUnReturnViewModel> OverDueItems(string dateRange)
        {
            var model = db.AFBorrowerIssues
                .Where(b => b.AFBorrowers.DocStatus != 0)
                .Select(b => new ItemUnReturnViewModel()
            {
                id = b.id,
                ItemCode = b.Items.ItemCode,
                Description = b.Items.Description + " | " + b.Items.Description2,
                DateIssued = b.DateIssued,
                DueDate = b.DueDate,
                RefNo = b.AFBorrowers.EBCNo,
                SerialNo = b.SerialNo,
                EmpID = b.AFBorrowers.Employees.EmpId,
                EmployeeName = b.AFBorrowers.Employees.LastName + "," + b.AFBorrowers.Employees.FirstName,
                QtyIssued = (
                           b.Status == "Active" ? b.Quantity : b.QuantityAdj
                     ),
                QtyReturn = db.AFBorrowerReturns
                           .Where(a => a.Status == "Active")
                           .Where(a => a.AFBorrowerIssueID == b.id)
                           .Select(a => a.Quantity)
                           .DefaultIfEmpty(0)
                           .Sum(),
                Status = b.Status,
            })
            .Where(b => b.Status == "Active" || b.Status == "Transferred");

            model = model.Where(b => b.QtyIssued != b.QtyReturn);

            DateTime baseDate = DateTime.Now;
            var thisDay = DateTime.Now;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            var thisWeekEnd = thisWeekStart.AddDays(6).AddSeconds(-1);
            var firstDayOfMonth = new DateTime(baseDate.Year, baseDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            switch (dateRange)
            {
                case "all":
                    model = model.Where(b => b.DueDate <= thisDay);
                    //default:
                    break;
                case "today":
                    model = model.Where(b => b.DueDate == thisDay);
                    break;
                case "week":
                    model = model.Where(b => b.DueDate >= thisWeekStart && b.DueDate <= thisWeekEnd);
                    break;
                case "month":
                    model = model.Where(b => b.DueDate >= firstDayOfMonth && b.DueDate <= lastDayOfMonth);
                    break;
            }

            return model.ToList();


        }
        [HttpPost]
        public JsonResult ChangeStatJson(int id, string stat)
        {
            JsonArray res = new JsonArray();
            Item item = db.Items.Find(id);
            item.Status = stat;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            res.status = "success";
            return Json(res);
        }
        public JsonResult SearchItem_UoM(string q)
        {

            var model = db.UnitOfMeasures.Select(b => new
            {
                id = b.Code,
                text = b.Code + " | " + b.Description,

            });
            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(b => b.text.Contains(q));
            }

            var modelItem = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelItem, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchItem_EquipmentType(string q)
        {

            var model = db.EquipmentTypes.Select(b => new
            {
                id = b.Code,
                text = b.Code + " | " + b.Description,

            });
            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(b => b.text.Contains(q));
            }

            var modelItem = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelItem, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetNoSeries(string strcode)
        {
            var noseries = db.NoSeries.Select(n => new
            {
                code = n.Code,
                text = n.LastNo,
                status = "success"
            }).Where(n => n.code == strcode);

            return Json(noseries, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public PartialViewResult FilterItems(int? page, FormCollection form)
        //{

        //    int a = 0;
        //    int b = 0;
        //    int cnt = form.AllKeys.Count() / 2;

        //    string[] col = new string[cnt];
        //    string[] colval = new string[cnt];



        //    foreach (var key in form.AllKeys)
        //    {
        //        if (key.StartsWith("keyfld"))
        //        {
        //            col[a] = form[key];
        //            a++;
        //        }
        //        if (key.StartsWith("keyval"))
        //        {
        //            colval[b] = form[key];
        //            b++;
        //        }
        //    }


        //    var items = db.Items.ToList();
        //    for (int j = 0; j < cnt; j++)
        //    {
        //        switch (col[j])
        //        {
        //            case "ItemCode":
        //                items = items.Where(p => p.ItemCode.ToUpper().Contains(colval[j].ToUpper())).ToList();
        //                break;
        //            case "Description":
        //                items = items.Where(p => p.Description.ToUpper().Contains(colval[j].ToUpper())).ToList();   
        //                break;
        //            case "Description2":
        //                items = items.Where(p => p.Description2.ToUpper().Contains(colval[j].ToUpper())).ToList();
        //                break;
        //            case "ShelfNo":

        //                var itemd = db.ItemDetails.ToList();
        //                itemd = itemd.Where(i => i.ShelfNo != null && i.ShelfNo.ToUpper().Contains(colval[j].ToUpper())).ToList();

        //                var xx = itemd.Count();
        //                string[] itmcode = new string[xx];

        //                a = 0;
        //                foreach (var itemss in itemd)
        //                {
        //                    itmcode[a] = itemss.ItemID.ToString();
        //                    a++;
        //                }

        //                //items = items.Where(p => p.id == xxx).ToList();
        //                items = items.Where(p => itmcode.Contains(p.id.ToString())).ToList();
        //                //items = items.Where(p => p.ItemDetails.ShelfNo.ToUpper().Contains(colval[j].ToUpper())).ToList();

        //                break;
        //            case "Category":
        //                items = items.Where(p => p.Category.ToUpper().Contains(colval[j].ToUpper())).ToList();
        //                break;
        //            case "EquipmentType":
        //                items = items.Where(p => p.EquipmentType != null && p.EquipmentType.ToUpper().Contains(colval[j].ToUpper())).ToList();
        //                break;
        //        }

        //    }


        //    ViewBag.Columns = ItemColumns();
        //    var items2 = items;

        //    //int limit = 50;

        //    //switch (page)
        //    //{
        //    //    case null:
        //    //        items2 = items.OrderBy(i => i.id).Skip(0).Take(limit).ToList();
        //    //        break;
        //    //    case 1:
        //    //        items2 = items.OrderBy(i => i.id).Skip(0).Take(limit).ToList();
        //    //        break;
        //    //    default:
        //    //        int offset = (Convert.ToInt32(page) - 1) * limit;
        //    //        items2 = items.OrderBy(i => i.id).Skip(offset).Take(limit).ToList();
        //    //        break;
        //    //}




        //    var itemList = new List<ItemViewModel>();
        //    var itemList2 = new List<ItemViewModel>();

        //    foreach (var item in items)
        //    {
        //        var goalModel = new ItemViewModel(item, 0); //0 is for dummy purpose only
        //        itemList.Add(goalModel);

        //    }
        //    foreach (var item2 in items2)
        //    {
        //        var goalModel = new ItemViewModel(item2, TotalInv(item2.id));
        //        itemList2.Add(goalModel);

        //    }
        //    ViewBag.ItemList = itemList2;



        //    var x = items;

        //    int pageSize = 5000;
        //    int pageNumber = (page ?? 1);



        //    var lst = itemList.OrderBy(i => i.id).ToPagedList(pageNumber, pageSize);
        //    return PartialView("_ItemListPartial", lst);

        //}

        public ActionResult PrintUnreturnItems(int? id)
        {
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\UItems.rdlc";
            dt = new ReportController().PrintUnreturnItems(id);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("UItems", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult PrintUnreturnItemsExcel(int? id)
        {
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\UItems.rdlc";
            dt = new ReportController().PrintUnreturnItems(id);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("UItems", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult PrintItemLogs(int? id)
        {
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ItemLogs.rdlc";
            dt = new ReportController().PrintItemLogs(id);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("ItemLog", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult PrintItemLogsExcel(int? id)
        {
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ItemLogs.rdlc";
            dt = new ReportController().PrintItemLogs(id);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("ItemLog", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult PrintItemTracking(int? id, string rptType)
        {
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ItemTracking.rdlc";
            dt = new ReportController().PrintItemTracking(id);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("ItemTracking", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            if (rptType == "pdf")
            {
                bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            }
            else
            {
                bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            }


            return new FileContentResult(bytes, mimeType);


        }

        public ActionResult PrintInventory(string rptType)
        {
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Inventory.rdlc";
            dt = new ReportController().PrintInventory();

            viewer.LocalReport.DataSources.Add(new ReportDataSource("Inventory", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            if (rptType == "pdf")
            {
                bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            }
            else
            {
                bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            }
            return new FileContentResult(bytes, mimeType);
        }


        public ActionResult indexData(int draw, int start, int length, string strcode, int noCols)
        {

            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;

            var v = db.Items.Where(i => i.Status == "Active" || i.Status == "InActive")
                .Select(a => new
                {
                    id = a.id,
                    ItemCode = a.ItemCode,
                    Description = a.Description,
                    Description2 = a.Description2,
                    Inventory = 0,
                    ShelfNo = a.ShelfNo,
                    Category = a.Category,
                    EquipmentType = a.EquipmentType,
                    Status = a.Status == "Active" ? "No" : "Yes"
                });

            recordsTotal = v.Count();


            string strFilter = "";
            for (int i = 0; i < noCols; i++)
            {
                string colval = Request["columns[" + i + "][search][value]"];
                if (colval != "")
                {
                    string colSearch = Request["columns[" + i + "][data]"];
                    colSearch = colSearch == "Blocked" ? "Status" : colSearch;

                    if (strFilter == "")
                    {
                        if (colval == "*")
                        {
                            strFilter = "(" + colSearch + " != " + "" + ")";
                        }
                        else
                        {

                            strFilter = (colSearch != "Inventory") ? colSearch + ".Contains(" + "\"" + colval + "\"" + ")" : "(" + colSearch + "=" + colval + ")";
                        }
                    }
                    else
                    {
                        strFilter = (colSearch != "Inventory") ? strFilter + " && " + colSearch + ".Contains(" + "\"" + colval + "\"" + ")" : strFilter + " && " + "(" + colSearch + "=" + colval + ")";
                    }

                    if (strFilter != "")
                    {
                        v = v.Where(strFilter);
                    }
                }
            }


            recordsFiltered = v.Count();


            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                //for make sort simpler we will add System.Linq.Dynamic reference
                string col = Request["columns[" + sortColumn + "][data]"];
                v = v.OrderBy(col + " " + sortColumnDir);
            }


            if (pageSize < 0)
            {
                pageSize = recordsFiltered;
            }



            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(b => b.ItemCode.Contains(searchValue) || b.Description.Contains(searchValue)
                            || b.Description2.Contains(searchValue) || b.ShelfNo.Contains(searchValue)
                            || b.Category.Contains(searchValue) || b.EquipmentType.Contains(searchValue)
                            || b.Status.Contains(searchValue));
            }

            var allCustomer = v.Skip(skip).Take(pageSize).ToList().Select(a => new
            {
                id = a.id,
                ItemCode = a.ItemCode,
                Description = a.Description,
                Description2 = a.Description2,
                Inventory = TotalInv(a.id),
                ShelfNo = a.ShelfNo,
                Category = a.Category,
                EquipmentType = a.EquipmentType,
                Blocked = a.Status
            });



            var model = new
            {
                draw = draw,
                recordsFiltered = recordsFiltered,
                recordsTotal = recordsTotal,
                data = allCustomer
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SetNewDueDate(string itemid, DateTime duedate)
        {

            int i = 0;
            int[] res = new int[(itemid.Length) - 1];
            JsonArray result = new JsonArray();

            try
            {
                var elements = itemid.Split(',');
                foreach (var item in elements)
                {
                    i = Convert.ToInt32(item);
                    AFBorrowerIssue subitem = db.AFBorrowerIssues.Find(i);
                    //string workorder = "";
                    //string purpose = "";

                    //workorder = string.IsNullOrEmpty(subitem.WorkOrder) ? "N/A" : subitem.WorkOrder;
                    //purpose = string.IsNullOrEmpty(subitem.Purpose) ? "N/A" : subitem.Purpose;

                    //subitem.WorkOrder = workorder;
                    //subitem.Purpose = purpose;
                    subitem.DueDate = duedate;

                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();

                    ItemLog itl = new ItemLog();
                    itl.ItemID = subitem.id;
                    itl.Module = "EBC";
                    itl.EntryType = "Update Due Date";
                    itl.Quantity = 0;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();

                    Log log = new Log();
                    log.descriptions = "Update EBC Record. AFBorrower id:" + item;
                    db.Logs.Add(log);
                    db.SaveChanges();
                }



                result.message = "success";

            }
            catch (Exception e)
            {
                result.message = e.Message;

            }


            return Json(result);
        }


    }
}
