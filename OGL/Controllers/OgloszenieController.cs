using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Repozytorium.IRepo;
using Repozytorium.Models;
using Repozytorium.Repo;
using Microsoft.AspNet.Identity;
using PagedList;

namespace OGL.Controllers
{
    public class OgloszenieController : Controller
    {

        private readonly IOgloszenieRepo _repo;
        public OgloszenieController(IOgloszenieRepo repo)
        {
            _repo = repo;
        }


        // GET: Ogloszenie
        //Ogloszenie/Index?page=3
        public ActionResult Index(int? page, string sortOrder)
        {
            int currentPage = page ?? 1;
            int naStronie = 3;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSort = String.IsNullOrEmpty(sortOrder) ? "IdAsc" : "";
            ViewBag.DataDodaniaSort = sortOrder == "DataDodania" ? "DataDodaniaAsc" : "DataDodania";
            ViewBag.TrescSort = sortOrder == "TrescAsc" ? "Tresc" : "TrescAsc";
            ViewBag.TytulSort = sortOrder == "TytulAsc" ? "Tytul" : "TytulAsc";

            var ogloszenia = _repo.PobierzOgloszenia();
            switch (sortOrder)
            {
                case "DataDodania":
                    ogloszenia = ogloszenia.OrderByDescending(s => s.DataDodania);
                    break;
                case "DataDodaniaAsc":
                    ogloszenia = ogloszenia.OrderBy(s => s.DataDodania);
                    break;
                case "Tytul":
                    ogloszenia = ogloszenia.OrderByDescending(s => s.Tytul);
                    break;
                case "TytulAsc":
                    ogloszenia = ogloszenia.OrderBy(s => s.Tytul);
                    break;
                case "Tresc":
                    ogloszenia = ogloszenia.OrderByDescending(s => s.Tresc);
                    break;
                case "TrescAsc":
                    ogloszenia = ogloszenia.OrderBy(s => s.Tresc);
                    break;
                case "IdAsc":
                    ogloszenia = ogloszenia.OrderBy(s => s.Id);
                    break;
                default:    // id descending
                    ogloszenia = ogloszenia.OrderByDescending(s => s.Id);
                    break;
            }

            return View(ogloszenia.ToPagedList<Ogloszenie>(currentPage, naStronie));
        }



        //// GET: Ogloszenie/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ogloszenie ogloszenie = _repo.GetOgloszenieById((int)id);
            if (ogloszenie == null)
            {
                return HttpNotFound();
            }
            return View(ogloszenie);
        }

        // GET: Ogloszenie/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //// POST: Ogloszenie/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Tresc,Tytul")] Ogloszenie ogloszenie)
        {
            if (ModelState.IsValid)
            {
                ogloszenie.UzytkownikId = User.Identity.GetUserId();
                ogloszenie.DataDodania = DateTime.Now;
                try
                {
                    _repo.Dodaj(ogloszenie);
                    _repo.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    return View(ogloszenie);
                }
            }
            return View(ogloszenie);
        }

        // GET: Ogloszenie/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ogloszenie ogloszenie = _repo.GetOgloszenieById((int)id);
            if (ogloszenie == null)
            {
                return HttpNotFound();
            }
            else if(ogloszenie.UzytkownikId != User.Identity.GetUserId() && !(User.IsInRole("Admin") || User.IsInRole("Pracownik")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(ogloszenie);
        }

        //// POST: Ogloszenie/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,Tresc,Tytul,DataDodania,UzytkownikId")] Ogloszenie ogloszenie)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //ogloszenie.UzytkownikId = "fsksk";
                    _repo.Aktualizuj(ogloszenie);
                    _repo.SaveChanges();
                }
                catch (Exception)
                {
                    ViewBag.Blad = true;
                    return View(ogloszenie);
                }
                ViewBag.Blad = false;
            }
            return View(ogloszenie);
        }

        // GET: Ogloszenie/Delete/5
        [Authorize]
        public ActionResult Delete(int? id, bool? blad)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ogloszenie ogloszenie = _repo.GetOgloszenieById((int)id);
            if (ogloszenie == null)
            {
                return HttpNotFound();
            } else if (ogloszenie.UzytkownikId != User.Identity.GetUserId() && !User.IsInRole("Admin")){
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            } 
            if(blad !=null)
            {
                ViewBag.Blad = true;
            }
            return View(ogloszenie);
        }

        //// POST: Ogloszenie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _repo.UsunOgloszenie(id);
            try
            {
                _repo.SaveChanges();
            }
            catch
            {
                return RedirectToAction("Delete", new { id = id, blad = true });
            }
            return RedirectToAction("Index");
        }
        //GET: /Ogloszenie/
        public ActionResult Partial(int? page)
        {
            int currentPage = page ?? 1;
            int naStronie = 3;
            var ogloszenia = _repo.PobierzOgloszenia();
            ogloszenia = ogloszenia.OrderByDescending(d => d.DataDodania);
            return PartialView("Index", ogloszenia.ToPagedList<Ogloszenie>(currentPage,naStronie));
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
