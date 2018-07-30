namespace Repozytorium.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Repozytorium.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Repozytorium.Models.OglContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Repozytorium.Models.OglContext context)
        {
            //  This method will be called after migrating to the latest version.
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.


            //Do debugowania metody seed
            /*if (System.Diagnostics.Debugger.IsAttached == false)
                System.Diagnostics.Debugger.Launch();*/

            SeedRoles(context);
            SeedUsers(context);
            SeedOgloszenia(context);
            SeedKategorie(context);
            SeedOgloszenia_Kategoria(context);
        }

       
        private void SeedRoles(OglContext context)
        {
            var roleManager = new RoleManager<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(new RoleStore<IdentityRole>());
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new 
                    Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name= "Admin";
                roleManager.Create(role);
            }
        }

        private void SeedUsers(OglContext context)
        {
            var store = new UserStore<Uzytkownik>(context);
            var manager = new UserManager<Uzytkownik>(store);
            if(!context.Users.Any(u=>u.UserName=="Admin"))
            {
                var user = new Uzytkownik { UserName = "Admin", Wiek = 12 };
                var adminresult = manager.Create(user, "12345678");
                if (adminresult.Succeeded)
                    manager.AddToRole(user.Id, "Admin");
            }
        }

        private void SeedOgloszenia(OglContext context)
        {
            var idUzytownika = context.Set<Uzytkownik>()
                .Where(u => u.UserName == "Admin")
                .FirstOrDefault().Id;
            for(int i=1;i<=10;i++)
            {
                var ogl = new Ogloszenie()
                {
                    Id = i,
                    UzytkownikId = idUzytownika,
                    Tresc = "Tre�� Og�oszenia" + i.ToString(),
                    Tytul = "Tytu� Og�oszenia" + i.ToString(),
                    DataDodania = DateTime.Now.AddDays(-i)
                };
                context.Set<Ogloszenie>().AddOrUpdate(ogl);
            }
            context.SaveChanges();
        }

        private void SeedKategorie(OglContext context)
        {
            for (int i = 1; i <= 10; i++)
            {
                var kat = new Kategoria()
                {
                    Id = i,
                    Nazwa = "Nazwa kategorii" + i.ToString(),
                    Tresc = "Tre�� Og�oszenia" + i.ToString(),
                    MetaTytul = "Tytu� Kategorii" + i.ToString(),
                    MetaOpis = "Opis Kategorii" + i.ToString(),
                    MetaSlowa = "S�owa kluczowe do kategorii" + i.ToString(),
                    ParentId = i
                };
                context.Set<Kategoria>().AddOrUpdate(kat);
            }
            context.SaveChanges();
        }

        private void SeedOgloszenia_Kategoria(OglContext context)
        {
            for (int i = 1; i < 10; i++)
            {
                var okat = new Ogloszenie_Kategoria()
                {
                    Id = i,
                    OgloszenieId = i / 2 + 1,
                    KategoriaId = i / 2 + 2
                };
                context.Set<Ogloszenie_Kategoria>().AddOrUpdate(okat);
            }
            context.SaveChanges();
        }
    }
}
