using DTHStore.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DTHStore.Controllers
{
    public class WeaponController : Controller
    {
        dbCsgoStoreDataContext db = new dbCsgoStoreDataContext();
        // GET: Weapon
        public ActionResult Index(int? page, string searchString)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            ViewBag.Keyword = searchString;

            var weaponListQuery = db.weapons.Select(w => w);

            if (!string.IsNullOrEmpty(searchString))
            {
                weaponListQuery = weaponListQuery.Where(w =>
                    w.weapon_name.Contains(searchString) ||
                    w.category.Contains(searchString)
                );
            }

            var weaponList = weaponListQuery.OrderBy(w => w.weapon_name).ToPagedList(pageNumber, pageSize);

            return View(weaponList);
        }
        public ActionResult Index_customer(int? page, string searchString)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            ViewBag.Keyword = searchString;

            var weaponListQuery = db.weapons.Select(w => w);

            if (!string.IsNullOrEmpty(searchString))
            {
                weaponListQuery = weaponListQuery.Where(w =>
                    w.weapon_name.Contains(searchString) ||
                    w.category.Contains(searchString)
                );
            }

            var weaponList = weaponListQuery.OrderBy(w => w.weapon_name).ToPagedList(pageNumber, pageSize);

            return View(weaponList);
        }
        public ActionResult Detail(int id)
        {
            var weapon = db.weapons.FirstOrDefault(w => w.weapon_id == id);
            if (weapon == null)
            {
                return HttpNotFound();
            }
            return View(weapon);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var weapon = db.weapons.FirstOrDefault(w => w.weapon_id == id);
            if (weapon == null)
            {
                return HttpNotFound();
            }
            return View(weapon);
        }

        [HttpPost]
        public ActionResult Edit(weapon updatedWeapon)
        {
            var weapon = db.weapons.FirstOrDefault(w => w.weapon_id == updatedWeapon.weapon_id);
            if (weapon == null)
            {
                return HttpNotFound();
            }

            weapon.weapon_name = updatedWeapon.weapon_name;
            weapon.category = updatedWeapon.category;
            weapon.price = updatedWeapon.price;
            weapon.damage = updatedWeapon.damage;
            weapon.fire_rate = updatedWeapon.fire_rate;
            weapon.recoil = updatedWeapon.recoil;
            weapon.magazine_size = updatedWeapon.magazine_size;
            weapon.penetration_power = updatedWeapon.penetration_power;
            weapon.image_url = updatedWeapon.image_url;

            db.SubmitChanges();

            return RedirectToAction("Index");
        }
    }
}