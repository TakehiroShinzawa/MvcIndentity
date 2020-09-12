using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcIdentity.Models;

namespace MvcIndentity.Controllers
{
    public class AuthorsController : Controller
    {
        private MvcIdentityContext db = new MvcIdentityContext();

        // GET: Authors
        public async Task<ActionResult> Index()
        {
            return View(await db.Authors.ToListAsync());
        }

        // GET: Authors/Details/5
        public async Task<ActionResult> Details(int? id, string op, string ret)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = await db.Authors.FindAsync(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            ViewBag.Return = ret ?? "";
            ViewBag.Operator = op ?? "";
            return View(author);
        }

        // GET: Authors/Create
        public ActionResult Create( string op, string ret)
        {
            ViewBag.Title = "Create";
            ViewBag.Return = ret ?? "";
            ViewBag.Operator = op ?? "";
            return View("CreEdit");
        }

        // POST: Authors/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Email,Birth")] Author author, string op, string ret)
        {
            ViewBag.Return = ret ?? "";
            ViewBag.Operator = op ?? "";
            if (ModelState.IsValid)
            {
                db.Authors.Add(author);
                await db.SaveChangesAsync();
                if (ret == null)
                    return RedirectToAction("Index");
                else
                {
                    TempData["op"] = op;
                    TempData["worktype"] = 1;
                    return RedirectToAction(ret, "Home");
                }
            }
            ViewBag.Title = "Create";
            return View("CreEdit",author);
        }

        // GET: Authors/Edit/5
        public async Task<ActionResult> Edit(int? id, string op, string ret)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = await db.Authors.FindAsync(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = "Edit";
            ViewBag.Return = ret ?? "";
            ViewBag.Operator = op ?? "";
            return View("CreEdit",author);
        }

        // POST: Authors/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Email,Birth")] Author author, string op, string ret)
        {
            ViewBag.Return = ret ?? "";
            ViewBag.Operator = op ?? "";
            if (ModelState.IsValid)
            {
                db.Entry(author).State = EntityState.Modified;
                await db.SaveChangesAsync();
                if (ret == null)
                    return RedirectToAction("Index");
                else
                {
                    TempData["op"] = op;
                    TempData["worktype"] = 1;
                    return RedirectToAction(ret, "Home");
                }
            }
            ViewBag.Title = "Edit";
            return View("CreEdit", author);
        }

        // GET: Authors/Delete/5
        public async Task<ActionResult> Delete(int? id, string op, string ret)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = await db.Authors.FindAsync(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            ViewBag.Return = ret ?? "";
            ViewBag.Operator = op ?? "";
            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id, string op, string ret)
        {
            Author author = await db.Authors.FindAsync(id);
            db.Authors.Remove(author);
            await db.SaveChangesAsync();
            ViewBag.Return = ret ?? "";
            ViewBag.Operator = op ?? "";
            if (ret == null)
                return RedirectToAction("Index");
            else
            {
                TempData["op"] = op;
                TempData["worktype"] = 1;
                return RedirectToAction(ret, "Home");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
