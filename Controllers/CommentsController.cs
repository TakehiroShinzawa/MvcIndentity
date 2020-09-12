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
using System.Runtime.InteropServices;

namespace MvcIndentity.Controllers
{
    public class CommentsController : Controller
    {
        private MvcIdentityContext db = new MvcIdentityContext();

        // GET: Comments
        public async Task<ActionResult> Index(int? articleId)
        {
            if (articleId == null)
                return HttpNotFound();
            var article = await db.Articles.FindAsync(articleId);
            if( article == null)
            {
                return HttpNotFound();
            }
            var comments = db.Comments.Where( c=> c.ArticleId == articleId).Include(c => c.Article);
            //var com = from dt in db.Comments
            //          where dt.ArticleId == articleId
            //          select dt;
            ViewBag.ArticleName = article.Title;
            ViewBag.ArticleId = article.Id;
            return View("_List", await comments.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public async Task<ActionResult> Create(int? articleId)
        {
            if (articleId == null)
                return HttpNotFound();
            var article = await db.Articles.FindAsync(articleId);
            if (article == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArticleId = new SelectList(db.Articles, "Id", "Url", articleId);
            ViewBag.ArticleName = article.Title;
            ViewBag.ArtId = articleId;
            ViewBag.Title = "Create";
            return View("CreEdit");
        }

        // POST: Comments/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Body,Updated,ArticleId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.Article = db.Articles.Find(comment.ArticleId);
                db.Comments.Add(comment);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Articles", new { Id = comment.ArticleId });
            }

            ViewBag.ArticleId = new SelectList(db.Articles, "Id", "Url", comment.ArticleId);
            ViewBag.ArticleName = comment.Article.Title;
            ViewBag.ArtId = comment.ArticleId;
            ViewBag.Title = "Create";

            return View("CreEdit");
        }

        // GET: Comments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArticleId = new SelectList(db.Articles, "Id", "Url", comment.ArticleId);
            ViewBag.ArticleName = comment.Article.Title;
            ViewBag.ArtId = comment.ArticleId;
            ViewBag.Title = "Edit";
            return View("CreEdit", comment);
        }

        // POST: Comments/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Body,Updated,ArticleId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Articles", new { Id = comment.ArticleId });
            }
            ViewBag.ArticleId = new SelectList(db.Articles, "Id", "Url", comment.ArticleId);
            ViewBag.ArticleName = comment.Article.Title;
            ViewBag.ArtId = comment.ArticleId;
            ViewBag.Title = "Edit";
            return View("CreEdit", comment);
        }

        // GET: Comments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Comment comment = await db.Comments.FindAsync(id);
            var articleId = comment.ArticleId;
            db.Comments.Remove(comment);
            await db.SaveChangesAsync();
            return RedirectToAction("Details", "Articles", new { Id = articleId });
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
