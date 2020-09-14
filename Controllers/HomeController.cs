using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MvcIdentity.Extensions;
using MvcIdentity.Models;
using MvcIndentity.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace MvcIdentity.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        private MvcIdentityContext db = new MvcIdentityContext();
        private ApplicationRoleManager _roleManager;
        private ApplicationUserManager _userManager;
 
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ??
                    HttpContext.GetOwinContext().
                    Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ActionResult BusinessForm()
        {
            var ss = new Dictionary<int, string>() { { 1, "ss" }, { 2, "ss" } };

            var op = TempData["op"];
            var model = new BusinessMode();
            if(op != null)
            {
                var worktype = TempData["worktype"];
                model.Operrators.Add(UserManager.FindByName((string)op));
                model.Authors = db.Authors.ToList();
                model.State = "1," + worktype.ToString();
                return View(model);
            }
            var user = this.HttpContext.User.Identity;
            if( user.IsAuthenticated )
            {
                //ログイン済み
                model.Operrators = new List<ApplicationUser>();
                model.Operrators.Add(UserManager.FindByName(user.Name));
                model.State = "0";
            }
            else
            {
                model.Operrators = context.Users.ToList();
                model.State = "0";
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult BusinessForm(string action, string op, int worktype, IEnumerable<MemberInfo> members)
        {
            //最初の文字でアクションを決める。
            db.Database.Log = (log) => Debug.WriteLine(log);
            var model = new BusinessMode();
            model.Operrators = new List<ApplicationUser>();
            model.Operrators.Add(UserManager.FindByName(op ?? this.HttpContext.User.Identity.Name));
            var tw =  this.HttpContext.User.Identity.GetMyPage();
            switch (action)
            {
                case "0":
                    string workType = "";
                    switch (worktype)
                    {
                        case 1:
                            workType = "マスタメンテナンス";
                            break;
                        case 2:
                            workType = "トランザクションメンテナンス";
                            break;
                        case 99:
                            workType = "その他";
                            break;
                    }
                    ViewBag.WorkType = workType;
                    model.State = worktype.ToString();
                    break;
                case "1":
                    switch (worktype)
                    {
                        case 1:
                            //"著者";
                            model.Authors = db.Authors.ToList();
                            break;
                        case 2:// メンバー一覧
                            model.MemberInfos = db.Members.Select( d => new MemberInfo
                            {
                                Id = d.Id,
                                Email = d.Email,
                                Name = d.Name
                            }).ToList();
                            break;
                        case 99:
                            workType = "その他";
                            break;
                    }
                    model.State = "1," + worktype.ToString();

                    break;
                case "1,2":// メンバー更新
                    try
                    {
                        //データの妥当性検査をスキップする
                        //db.Configuration.AutoDetectChangesEnabled = false;
                        db.Configuration.ValidateOnSaveEnabled = false;
                        Member member;
                        if (ModelState.IsValid)
                        {
                            foreach (var p in members)
                            {
                                member = db.Members.Find(p.Id);
                                if (member.Name != p.Name || member.Email != p.Email)
                                {
                                    member.Name = p.Name;
                                    member.Email = p.Email;
                                    db.Entry(member).State = EntityState.Modified;
                                }
                            }
                            db.SaveChanges();
                            model.MemberInfos = members.ToList();
                            model.State = action;
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Debug.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                            }
                        }

                    }
                    finally
                    {
                        //re-enable detection of changes
                        //db.Configuration.AutoDetectChangesEnabled = true;
                        db.Configuration.ValidateOnSaveEnabled = true;
                    }
                    break;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult JsonTest(IEnumerable<JsonSample> sample)
        {
            var res =  sample.ToList();
            res[0].Results[0].Score += 10;
            res[0].Results[1].Score += 10;
            return Json(res);
        }
        public ActionResult Index()
        {
            Response.AppendCookie(new HttpCookie("email")
            {
                Value = "thats@ex2p.net",
                Expires = DateTime.Now.AddHours(1)
            });
            ViewBag.BackImage = $"~/Images/jumbo{DateTime.Now.Second % 5 + 1}.jpg";

            var orgData = new List<JsonSample>()
            {
                new JsonSample
                {
                    Grade = 1,Class = 2, Name = "受刑者", Results = new List<Result>(){ new Result { Score=65, Subject = "数学" },new Result{ Score=77, Subject = "理科" } }
                }
            };
            ViewBag.Json = orgData;
            //var post_data = {
            //    Grade: 1,
            //    Class: 2,
            //    Name: "受験者",
            //    Results:
            //[
            //        { Subject: "数学", Score: 64 },
            //        { Subject: "国語", Score: 78 },
            //        { Subject: "英語", Score: 58 }
            //    ]
            //};
           return View(orgData);
        }

        [MyAuthorize]
        public ActionResult About(string email)
        {
            ViewBag.Message = $"Your application description {email}.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult RoleIndex()
        {
            var roles = RoleManager.Roles.OrderBy(m => m.Name);
            var ss = roles.ToList();
            using (var context = new ApplicationDbContext())
            {
                var sql = $"SELECT Id, Name, RoleFlags FROM AspNetRoles ORDER BY Name";
                var dat = context.Database.SqlQuery<ApplicationRole>(sql);
                var res = dat.ToList();
                return PartialView("_RoleIndex", res);
            }
        }
    }
}