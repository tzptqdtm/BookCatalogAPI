using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCatalogAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookCatalogAPI.Data
{
    public static class SeedData
    {
        public static async Task Seed(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, EfBookDbContext context)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
            await SeedAuthorBooks(context);
            
        }
        private static async Task SeedUsers(UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync("admin@bookcatalog.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin@bookcatalog.com",
                    Email = "admin@bookcatalog.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }
            if (await userManager.FindByEmailAsync("user1@gmail.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "user1@gmail.com",
                    Email = "user1@gmail.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
            if (await userManager.FindByEmailAsync("user2@gmail.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "user2@gmail.com",
                    Email = "user2@gmail.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                var role = new IdentityRole
                {
                    Name = "Administrator"
                };
                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                var role = new IdentityRole
                {
                    Name = "Customer"
                };
                await roleManager.CreateAsync(role);
            }
        }

        private static async Task SeedAuthorBooks(EfBookDbContext context)
        {
            if (!context.Authors.Any())
            {
                var author = new Author
                {
                    Firstname = "Стивен",
                    Lastname = "Кинг",
                    Bio = "американский писатель, работающий в разнообразных жанрах, включая ужасы, триллер, " +
                          "фантастику, фэнтези, мистику, драму, детектив; получил прозвище «Король ужасов»."
                };
                await context.Authors.AddAsync(author);

                var books = new List<Book>
                {
                    new()
                    {
                        Title = "Зеленая миля",
                        Year = 2020,
                        Isbn = "978-5-17-118362-2",
                        AuthorId = 1,
                        Image = "ayVS3y6RQAByzPkUzZMi.jpg",
                        Summary = "Роман-событие, ставший лауреатом премии Брэма Стокера и вдохновивший Фрэнка Дарабонта " +
                                  "на создание культового фильма, в котором Том Хэнкс сыграл, возможно, свою лучшую роль. …" +
                                  "Стивен Кинг приглашает читателей в жуткий мир тюремного блока смертников, откуда уходят, " +
                                  "чтобы не вернуться, приоткрывает дверь последнего пристанища тех, кто преступил закон и " +
                                  "теперь отсчитывает последние часы... До предела обнажены нервы, накалены страсти и обострены" +
                                  " чувства. Уже ничего нельзя исправить — последняя черта совсем близко. Но еще можно понять " +
                                  "и посочувствовать тем, кому предстоит отправиться по зловещей Зеленой миле в никуда…",
                        Price = 1

                    },
                    new()
                    {
                        Title = "Бесплодные земли",
                        Year = 2017,
                        Isbn = "978-5-17-982575-3",
                        AuthorId = 1,
                        Image = "Gknvz83m44YBESnxekKu.jpg",
                        Summary = "Юный Роланд - последний благородный рыцарь в мире, «сдвинувшемся с места». Ему во что бы " +
                                  "то ни стало нужно найти Темную Башню - средоточие Силы, краеугольный камень мироздания. В " +
                                  "долгом и опасном пути его сопровождают люди из реального мира - мелкий воришка-наркоман и " +
                                  "женщина с раздвоенным сознанием. Путникам противостоит могущественный колдун - «человек в черном»," +
                                  " некогда предсказавший Роланду судьбу по картам Таро...",
                        Price = 1
                    }

                };

                await context.Books.AddRangeAsync(books);

                await context.SaveChangesAsync();
            }
        }

    }

}
