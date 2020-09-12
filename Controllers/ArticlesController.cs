using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MvcIdentity.Extensions;
using MvcIdentity.Models;

namespace MvcIdentity.Controllers
{
    public class ArticlesController : Controller
    {
        private MvcIdentityContext db = new MvcIdentityContext();

        // GET: Articles
        public ActionResult Index()
        {
            ViewBag.Title = "作品Index";
            return View(db.Articles.ToList());
        }
        public ActionResult _Index()
        {
            return PartialView("_Index", db.Articles.ToList());
        }

        // GET: Articles/Details/5
        [MyAuthorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            //using (var context = new MvcIdentityContext())
            //{
            //    var sql = $"SELECT Body FROM Comments where articleid = @articleid";
            //    var param1 = new SqlParameter("articleid", id);
            //    var dat = context.Database.SqlQuery<ArtCom>(sql, param1);
            //    var res = dat.ToList();
            //    ViewBag.Com = res;
            //}
            return View(article);
        }

        // GET: Articles/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Create";
            //作者一覧の作成
            SetAuthorsToBag();
            return View("CreEdit");
        }

        // POST: Articles/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Url,Title,Category,Description,Viewcount,Published,Released, Authors")] Article article, int[] auth)
        {
            if (ModelState.IsValid)
            {
                foreach (var a in auth)
                {
                    var auy = db.Authors.Find(a);
                    if (auy != null)
                    {
                        auy.Articles.Add(article);
                        db.Entry(auy).State = EntityState.Modified;
                    }
                }
                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Create";
            SetAuthorsToBag();
            return View("CreEdit", article);
        }
        [MyAuthorize(Roles = "Admin")]
        // GET: Articles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = "Edit";
            SetAuthorsToBag();
            return View("CreEdit", article);
        }

        // POST: Articles/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Url,Title,Category,Description,Viewcount,Published,Released,Timestamp")] Article article, int[] auth)
        {
            if (ModelState.IsValid)
            {
                using (var context = new MvcIdentityContext())
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        var sql = "DELETE FROM AuthorArticles where Article_Id = @articleid";
                        var param1 = new SqlParameter("articleid", article.Id);
                        var res = await context.Database.ExecuteSqlCommandAsync(sql, param1);
                        if (auth != null)
                        {
                            foreach (var a in auth)
                            {
                                var auy = context.Authors.Find(a);
                                if (auy != null)
                                {
                                    auy.Articles.Add(article);
                                    context.Entry(auy).State = EntityState.Modified;
                                }
                            }
                        }
                        context.Entry(article).State = EntityState.Modified;
                        context.SaveChanges();
                        trans.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                        trans.Rollback();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                        trans.Rollback();
                    }
                }
            }
            ViewBag.Title = "Edit";
            SetAuthorsToBag();
            return View("CreEdit", article);
        }

        // GET: Articles/Delete/5
        [MyAuthorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Articles/Delete/5
        [MyAuthorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, Byte[] Timestamp)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            var comm = db.Comments
                .Where(c => c.ArticleId == id)
                .Select(c => c);
            db.Comments.RemoveRange(comm);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //作者一覧の作成
        public void SetAuthorsToBag()
        {
            var auth = db.Authors
                .Select(a => a);
            ViewBag.Authors = auth.ToList();

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
