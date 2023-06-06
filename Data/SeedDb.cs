using Microsoft.AspNetCore.Identity;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            var user = await _userHelper.GetUserByEmailAsync("ldvssr@icloud.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Licínio",
                    LastName = "Rosário",
                    Email = "ldvssr@icloud.com",
                    UserName = "ldvssr@icloud.com",
                    PhoneNumber = "919999999"
                };

                var result = await _userHelper.AddUserAsync(user, "Passw0rd");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

            }

            if (!_context.Products.Any())
            {
                AddProduct("iPhone X", user);
                AddProduct("Magic Mouse", user);
                AddProduct("iWatch Series 4", user);
                AddProduct("iPad Mini", user);
                await _context.SaveChangesAsync();
            }

        }

        private void AddProduct(string name, User user)
        {
            _context.Products.Add(new Entities.Product
            {
                Name = name,
                Price = _random.Next(1000),
                IsAvailable = true,
                Stock = _random.Next(100),
                User = user
            });
        }
    }
}
