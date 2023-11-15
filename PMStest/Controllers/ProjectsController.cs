
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.EnterpriseServices;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PMStest.Models;
using Activity = PMStest.Models.Activity;

namespace PMStest.Controllers
{
    public class ProjectsController : Controller
    {
        private pms_testEntities1 db = new pms_testEntities1();

        // GET: Projects
        public ActionResult Index()
        {
            return View(db.Project.Where(q => q.IsDelete != true).ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View(new Project());
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {

                //first time create
                project.CreateDate = DateTime.Now;
                project.UpdateDate = DateTime.Now;
                project.IsDelete = false;

                AddParamAllActivity(project.Activity, project);
                //add some year
                db.Project.Add(project);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(project);
        }

        private void AddParamAllActivity(ICollection<Models.Activity> activity,Project project) {
            foreach (var item in activity)
            {
                item.CreateDate = project.CreateDate;
                item.UpdateDate = project.UpdateDate;
                item.IsDelete = false;
                AddParamAllActivity(item.Activity1,project);
            }
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Where(q => q.ID == id).Include(p => p.Activity.Where(o => o.IsDelete != true).ToList()).First();
            if (project == null)
            {
                return HttpNotFound();
            }
            project.Activity = RemoveChilden(project.Activity, 1);
            
            return View(project);
        }

        private ICollection<Models.Activity> RemoveChilden(ICollection<Models.Activity> activity,int LvCheck) {
            if (activity.Where(q => q.Lv != LvCheck).Count() > 0)
            {
                activity = activity.Where(q => q.Lv == LvCheck).ToList();
            }

            foreach (var item in activity)
            {
                item.Activity1 = RemoveChilden(item.Activity1, LvCheck+1);
            }

            return activity;
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Project project)
        {
            if (ModelState.IsValid)
            {
                //edit update
                project.UpdateDate = DateTime.Now;

                SaveChilden(project.Activity, project);

                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(project);
        }

        private void AddParamAllActivityUpdate(ICollection<Models.Activity> activity, Project project)
        {
            foreach (var item in activity)
            {
                item.CreateDate = project.UpdateDate;
                item.UpdateDate = project.UpdateDate;
                item.IsDelete = false;
                item.ProjectID = project.ID;
                AddParamAllActivityUpdate(item.Activity1, project);
            }
        }

        private void SaveChilden(ICollection<Models.Activity> activity,Project project) { 
            foreach (var item in activity)
            {
                if (item.CreateDate != null)
                {
                    SaveChilden(item.Activity1,project);

                    item.UpdateDate = project.UpdateDate;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else {
                    item.CreateDate = project.UpdateDate;
                    item.UpdateDate = project.UpdateDate;
                    item.IsDelete = false;

                    AddParamAllActivityUpdate(item.Activity1, project);
                    //add some year
                    db.Activity.Add(item);
                    db.SaveChanges();
                }
            }
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Project.Find(id);
            project.IsDelete = true;
            db.Entry(project).State = EntityState.Modified;
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

        [HttpPost, ActionName("AddActivityEdit")]
        public ActionResult AddActivityEdit(Project project)
        {
            if (project.IsAddActivity == true)
            {
                Activity mainItem = new Activity();
                mainItem.Lv = 1;
                mainItem.ProjectID = project.ID;
                project.Activity.Add(mainItem);
            }
            else
            {
                FindIsAddActivity(project.Activity);
            }


            return View("Edit", project);
        }

        [HttpPost, ActionName("AddActivityCreate")]
        public ActionResult AddActivityCreate(Project project)
        {
            if (project.IsAddActivity == true)
            {
                Activity mainItem = new Activity();
                mainItem.Lv = 1;
                project.Activity.Add(mainItem);
            }
            else
            {
                FindIsAddActivity(project.Activity);
            }


            return View("Create", project);
        }

        private void FindIsAddActivity(ICollection<Activity> activity)
        {
            foreach (var item in activity) {
                if(item.IsAddActivity == true) {
                    Activity newItem = new Activity();
                    newItem.Lv = item.Lv + 1;
                    item.Activity1.Add(newItem);
                    return;
                }
                FindIsAddActivity(item.Activity1);
            }
        }

        [HttpPost, ActionName("DeleteThisActivityCreate")]
        public ActionResult DeleteThisActivityCreate(Project project)
        {
            project.Activity = ClearDeleteThisActivity(project.Activity);
            return View("Create", project);
        }

        private ICollection<Activity> ClearDeleteThisActivity(ICollection<Activity> activity)
        {
            if (activity.Where(q => q.IsDeleteThisActivity == true).Count() > 0)
            {
                activity = activity.Where(q => q.IsDeleteThisActivity != true).ToList();
            }
            else {
                foreach (var item in activity)
                {
                    item.Activity1 = ClearDeleteThisActivity(item.Activity1);
                }
            }
            return activity;
        }

        [HttpPost, ActionName("DeleteThisActivityEdit")]
        public ActionResult DeleteThisActivityEdit(Project project)
        {
            project.Activity = ClearDeleteThisActivityEdit(project.Activity,1);
            return View("Edit", project);
        }

        private ICollection<Models.Activity> ClearDeleteThisActivityEdit(ICollection<Models.Activity> activity,int LvCheck)
        {
            if (activity.Where(q => q.Lv != LvCheck).Count() > 0) {
                activity = activity.Where(q => q.Lv == LvCheck).ToList();
            }
            if (activity.Where(q => q.IsDeleteThisActivity == true).Count() > 0)
            {
                foreach (var item in activity)
                {
                    if (item.IsDeleteThisActivity == true) {
                        item.IsDelete = true;
                        item.Activity1 = SetIsDeleteAllChilden(item.Activity1);
                    }
                }
            }
            else
            {
                foreach (var item in activity)
                {
                    item.Activity1 = ClearDeleteThisActivityEdit(item.Activity1,LvCheck+1);
                }
            }
            return activity;
        }

        private ICollection<Models.Activity> SetIsDeleteAllChilden(ICollection<Models.Activity> activity)
        {
            foreach (var item in activity) {
                item.IsDelete = true;
                item.Activity1 = SetIsDeleteAllChilden(item.Activity1);
            }
            return activity;
        }
    }
}
