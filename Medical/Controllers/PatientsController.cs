using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Medical.Models;
using System.Data.Entity.Infrastructure;

namespace Medical.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Patients
        public ActionResult Index(string sortOrder, string SearchString, int? DoctorID)
        {
            PopulateDropDownlist();

            //Enabled Sorting and Filter

            // Sort using action links in the view
            // "?" and ":" are equivalent to writing and if else statement "?" is the if and ":" is the else
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            ViewBag.DOBSortParm = sortOrder == "DOB" ? "DOB desc" : "DOB";
            ViewBag.DoctorSortParm = sortOrder == "Doctor" ? "Doctor desc" : "Doctor";
            ViewBag.VisitSortParm = sortOrder == "Visit" ? "Visit desc" : "Visit";

            // Linq query always start with includes - Uses eager loading of full doctor object
            var patients = db.Patients
                .Include(p => p.Doctor);

            // Doctor Filter
            // if a DoctorID parameter is past to index page, via filtering in the index view, than it has value and runs this query
            if (DoctorID.HasValue)
                patients = patients.Where(p => p.DoctorID == DoctorID);

            // Patient Filter
            // if a SearchString parameter is past to index page, via filtering in the index view, than it runs this query
            if (!String.IsNullOrEmpty(SearchString))
                patients = patients.Where(p => p.LastName.ToUpper().Contains(SearchString.ToUpper()) || p.FirstName.ToUpper().Contains(SearchString.ToUpper()));

            // Sort Odering
            switch (sortOrder)
            { 
                case "Name desc":
                    patients = patients
                        .OrderByDescending(p => p.LastName)
                        .ThenByDescending(p => p.FirstName);
                    break;

                case "DOB":
                    patients = patients
                        .OrderBy(p => p.DOB);
                    break;

                case "DOB desc":
                    patients = patients
                        .OrderByDescending(p => p.DOB);
                    break;

                case "Doctor":
                    patients = patients
                        .OrderBy(p => p.DoctorID);
                    break;

                case "Doctor desc":
                    patients = patients
                        .OrderByDescending(p => p.DoctorID);
                    break;

                case "Visit":
                    patients = patients
                        .OrderBy(p => p.ExpYrVisits);
                    break;

                case "Visit desc":
                    patients = patients
                        .OrderByDescending(p => p.ExpYrVisits);
                    break;

                default:
                    patients = patients
                        .OrderBy(p => p.LastName)
                        .ThenBy(p => p.FirstName);
                    break;
            }

            // All the above queries are not mutually exclusive - if the parameter is present it run the query if not it doesn't

            return View(patients.ToList());
        }

        // GET: Patients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // GET: Patients/Create
        public ActionResult Create()
        {
            //Replaced default with function to populate dropdown list using doctors full name and title in order of last name 
            PopulateDropDownlist();
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create([Bind(Include = "ID,OHIP,FirstName,MiddleName,LastName,DOB,ExpYrVisits,DoctorID")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Patients.Add(patient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //Replaced default with function to populate dropdown list using patient intial assigment to doctor is remembered even if the create doesn't complete
            PopulateDropDownlist(patient.DoctorID);
            return View(patient);
        }

        // GET: Patients/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }

            //Replaced default with function to populate dropdown list using patients doctor as assigned
            PopulateDropDownlist(patient.DoctorID);
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Edit([Bind(Include = "ID,OHIP,FirstName,MiddleName,LastName,DOB,ExpYrVisits,Timestamp,DoctorID")] Patient patient)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(patient).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateConcurrencyException ex) // Added for Concurrency
            {
                var entry = ex.Entries.Single();
                var clientValues = (Patient)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry == null)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save changes. Patient record was deleted by anohter user.");
                }
                else
                {
                    var databaseValues = (Patient)databaseEntry.ToObject();
                    if (databaseValues.FirstName != clientValues.FirstName)
                        ModelState.AddModelError("FirstName", "Current value: " + databaseValues.FirstName);
                    if (databaseValues.MiddleName != clientValues.MiddleName)
                        ModelState.AddModelError("MiddleName", "Current value: " + databaseValues.MiddleName);
                    if (databaseValues.LastName != clientValues.LastName)
                        ModelState.AddModelError("LastName", "Current value: " + databaseValues.LastName);
                    if (databaseValues.OHIP != clientValues.OHIP)
                        ModelState.AddModelError("OHIP", "Current value: " + databaseValues.OHIP);
                    if (databaseValues.DOB != clientValues.DOB)
                        ModelState.AddModelError("DOB", "Current value: " + String.Format("{0:d}", databaseValues.DOB));
                    if (databaseValues.ExpYrVisits != clientValues.ExpYrVisits)
                        ModelState.AddModelError("ExpYrVisits", "Current value: " + databaseValues.ExpYrVisits);
                    if (databaseValues.DoctorID != clientValues.DoctorID)
                        ModelState.AddModelError("DoctorID", "Current value: " + db.Doctors.Find(databaseValues.DoctorID).FullName);
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user after you recieved your values. The edit operation was cancelled and the current values in the database have been displayed. If you still want to edit this record, click the 'Save' button again. Otherwise click the 'Back to List' hyperlink.");
                    patient.Timestamp = databaseValues.Timestamp;
                }
            }

            //Replaced default with function to populate dropdown list using patients doctor as assigned
            PopulateDropDownlist(patient.DoctorID);
            return View(patient);
        }

        // GET: Patients/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Patient patient = db.Patients.Find(id);
            db.Patients.Remove(patient);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //New function added to create sorted dropdown list with doctors full name and titles
        // Uses summary property "FormalName" in the doctors model class
        private void PopulateDropDownlist(object selectedDoctor = null)
        {
            var dQuery = from d in db.Doctors
                         orderby d.LastName, d.FirstName
                         select d;
            ViewBag.DoctorID = new SelectList(dQuery, "ID", "FormalName", selectedDoctor);
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
