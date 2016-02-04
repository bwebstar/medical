namespace Medical.Migrations
{
    using Medical.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Medical.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Medical.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var doctors = new List<Doctor>
            {
                new Doctor {FirstName = "Gregory", MiddleName = "Arnold", LastName = "House"},
                new Doctor {FirstName = "Doggie", MiddleName = "Malcom", LastName = "Houser"},
                new Doctor {FirstName = "Charles", MiddleName = "Samual", LastName = "Xavier"}
            };
            doctors.ForEach(d => context.Doctors.AddOrUpdate(n => n.LastName, d));
            context.SaveChanges();

            var patients = new List<Patient>
            {
                new Patient {OHIP="1234567890", FirstName = "Fred", MiddleName = "Reginald", LastName = "Flintstone", DOB = DateTime.Parse("1955-01-12"), ExpYrVisits = 3, DoctorID = 1 },
                new Patient {OHIP="0987654321", FirstName = "Barney", MiddleName = "Henry", LastName = "Rubble", DOB = DateTime.Parse("1965-04-23"), ExpYrVisits = 5, DoctorID = 2 },
                new Patient {OHIP="6543210987", FirstName = "Bob", MiddleName = "James", LastName = "Jones", DOB = DateTime.Parse("1975-10-08"), ExpYrVisits = 1, DoctorID = 3 },
            };
            patients.ForEach(p => context.Patients.AddOrUpdate(o => o.OHIP, p));
            context.SaveChanges();

            // Add Admin and Manager Roles
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //Create Admin Role if it does not exist
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var roleresult = roleManager.Create(new IdentityRole("Admin"));
            }

            //Create Manager Role if it does not exist
            if (!context.Roles.Any(r => r.Name == "Manager"))
            {
                var roleresult = roleManager.Create(new IdentityRole("Manager"));
            }

            //Create a few generic users
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            for (int i = 1; i <= 4; i++)
            {
                var user = new ApplicationUser
                {
                    UserName = string.Format("user{0}@outlook.com", i.ToString()),
                    Email = string.Format("user{0}@outlook.com", i.ToString())
                };
                if (!context.Users.Any(u => u.UserName == user.UserName))
                    manager.Create(user, "password");
            }

            // Now the Manager User named manager1 with password
            var amanageruser = new ApplicationUser
            {
                UserName = "manager1@outlook.com",
                Email = "manager1@outlook.com"
            };
            if (!context.Users.Any(u => u.UserName == "manager1@outlook.com"))
            {
                manager.Create(amanageruser, "Password123!");
                manager.AddToRole(amanageruser.Id, "Manager");
            }

            // Now the Admin User named admin1 with password
            var adminuser = new ApplicationUser
            {
                UserName = "admin1@outlook.com",
                Email = "admin1@outlook.com"
            };
            if (!context.Users.Any(u => u.UserName == "admin1@outlook.com"))
            {
                manager.Create(adminuser, "Password123!");
                manager.AddToRole(adminuser.Id, "Admin");
                // Optionally add them to the manager role as well
                manager.AddToRole(adminuser.Id, "Manager");
            }

        }
    }
}
