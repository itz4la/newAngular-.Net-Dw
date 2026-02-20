using api.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
    {
    [Route("api/[controller]")]
    [ApiController]
    public class SeedDataController : ControllerBase
        {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedDataController(ApplicationContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
            {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            }

        [HttpPost("seed")]
        public async Task<ActionResult> SeedDatabase()
            {
            try
                {
                await SeedRolesAsync();
                await SeedGenresAsync();
                await SeedUsersAsync();
                await SeedBooksAsync();
                await SeedLoansAsync();

                return Ok(new
                    {
                    message = "Database seeded successfully!",
                    data = new
                        {
                        roles = 2,
                        genres = 4,
                        users = 3,
                        books = 20,
                        loans = "Multiple loans created"
                        }
                    });
                }
            catch (Exception ex)
                {
                return BadRequest(new { message = "Error seeding database", error = ex.Message });
                }
            }

        [HttpDelete("clear")]
        public async Task<ActionResult> ClearDatabase()
            {
            try
                {
                _context.Loans.RemoveRange(_context.Loans);
                _context.Books.RemoveRange(_context.Books);
                _context.Genres.RemoveRange(_context.Genres);

                var users = await _userManager.Users.ToListAsync();
                foreach (var user in users)
                    {
                    await _userManager.DeleteAsync(user);
                    }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Database cleared successfully!" });
                }
            catch (Exception ex)
                {
                return BadRequest(new { message = "Error clearing database", error = ex.Message });
                }
            }

        private async Task SeedRolesAsync()
            {
            if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

            if (!await _roleManager.RoleExistsAsync("Client"))
                {
                await _roleManager.CreateAsync(new IdentityRole("Client"));
                }
            }

        private async Task SeedGenresAsync()
            {
            if (await _context.Genres.AnyAsync())
                return;

            var genres = new List<Genre>
                {
                new Genre { Name = "Fiction" },
                new Genre { Name = "Science Fiction" },
                new Genre { Name = "Mystery" },
                new Genre { Name = "Biography" }
                };

            _context.Genres.AddRange(genres);
            await _context.SaveChangesAsync();
            }

        private async Task SeedUsersAsync()
            {
            if (await _userManager.Users.AnyAsync())
                return;

            var adminUser = new ApplicationUser
                {
                UserName = "admin@library.com",
                Email = "admin@library.com",
                EmailConfirmed = true
                };

            var result1 = await _userManager.CreateAsync(adminUser, "Admin@123");
            if (result1.Succeeded)
                {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                }

            var client1 = new ApplicationUser
                {
                UserName = "john.doe@library.com",
                Email = "john.doe@library.com",
                EmailConfirmed = true
                };

            var result2 = await _userManager.CreateAsync(client1, "Client@123");
            if (result2.Succeeded)
                {
                await _userManager.AddToRoleAsync(client1, "Client");
                }

            var client2 = new ApplicationUser
                {
                UserName = "jane.smith@library.com",
                Email = "jane.smith@library.com",
                EmailConfirmed = true
                };

            var result3 = await _userManager.CreateAsync(client2, "Client@123");
            if (result3.Succeeded)
                {
                await _userManager.AddToRoleAsync(client2, "Client");
                }
            }

        private async Task SeedBooksAsync()
            {
            if (await _context.Books.AnyAsync())
                return;

            var genres = await _context.Genres.ToListAsync();

            var books = new List<Book>
                {
                new Book { Title = "To Kill a Mockingbird", Author = "Harper Lee", Description = "A classic of modern American literature", GenreId = genres[0].Id, PublishedDate = new DateTime(1960, 7, 11), CoverImageUrl = "https://example.com/mockingbird.jpg" },
                new Book { Title = "1984", Author = "George Orwell", Description = "A dystopian social science fiction novel", GenreId = genres[1].Id, PublishedDate = new DateTime(1949, 6, 8), CoverImageUrl = "https://example.com/1984.jpg" },
                new Book { Title = "Pride and Prejudice", Author = "Jane Austen", Description = "A romantic novel of manners", GenreId = genres[0].Id, PublishedDate = new DateTime(1813, 1, 28), CoverImageUrl = "https://example.com/pride.jpg" },
                new Book { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Description = "The story of the mysteriously wealthy Jay Gatsby", GenreId = genres[0].Id, PublishedDate = new DateTime(1925, 4, 10), CoverImageUrl = "https://example.com/gatsby.jpg" },
                new Book { Title = "Dune", Author = "Frank Herbert", Description = "A science fiction novel set in the distant future", GenreId = genres[1].Id, PublishedDate = new DateTime(1965, 8, 1), CoverImageUrl = "https://example.com/dune.jpg" },
                new Book { Title = "The Catcher in the Rye", Author = "J.D. Salinger", Description = "A story about teenage rebellion", GenreId = genres[0].Id, PublishedDate = new DateTime(1951, 7, 16), CoverImageUrl = "https://example.com/catcher.jpg" },
                new Book { Title = "Sherlock Holmes", Author = "Arthur Conan Doyle", Description = "Detective fiction featuring Sherlock Holmes", GenreId = genres[2].Id, PublishedDate = new DateTime(1887, 11, 1), CoverImageUrl = "https://example.com/holmes.jpg" },
                new Book { Title = "The Hobbit", Author = "J.R.R. Tolkien", Description = "A fantasy novel and children's book", GenreId = genres[0].Id, PublishedDate = new DateTime(1937, 9, 21), CoverImageUrl = "https://example.com/hobbit.jpg" },
                new Book { Title = "Foundation", Author = "Isaac Asimov", Description = "A science fiction novel about the fall of civilization", GenreId = genres[1].Id, PublishedDate = new DateTime(1951, 6, 1), CoverImageUrl = "https://example.com/foundation.jpg" },
                new Book { Title = "Steve Jobs", Author = "Walter Isaacson", Description = "Biography of Apple co-founder Steve Jobs", GenreId = genres[3].Id, PublishedDate = new DateTime(2011, 10, 24), CoverImageUrl = "https://example.com/jobs.jpg" },
                new Book { Title = "The Da Vinci Code", Author = "Dan Brown", Description = "A mystery thriller novel", GenreId = genres[2].Id, PublishedDate = new DateTime(2003, 3, 18), CoverImageUrl = "https://example.com/davinci.jpg" },
                new Book { Title = "Harry Potter and the Sorcerer's Stone", Author = "J.K. Rowling", Description = "A fantasy novel about a young wizard", GenreId = genres[0].Id, PublishedDate = new DateTime(1997, 6, 26), CoverImageUrl = "https://example.com/harry.jpg" },
                new Book { Title = "The Martian", Author = "Andy Weir", Description = "A science fiction novel about an astronaut stranded on Mars", GenreId = genres[1].Id, PublishedDate = new DateTime(2011, 9, 27), CoverImageUrl = "https://example.com/martian.jpg" },
                new Book { Title = "Gone Girl", Author = "Gillian Flynn", Description = "A psychological thriller mystery novel", GenreId = genres[2].Id, PublishedDate = new DateTime(2012, 6, 5), CoverImageUrl = "https://example.com/gonegirl.jpg" },
                new Book { Title = "Becoming", Author = "Michelle Obama", Description = "A memoir by former First Lady Michelle Obama", GenreId = genres[3].Id, PublishedDate = new DateTime(2018, 11, 13), CoverImageUrl = "https://example.com/becoming.jpg" },
                new Book { Title = "The Lord of the Rings", Author = "J.R.R. Tolkien", Description = "An epic high-fantasy novel", GenreId = genres[0].Id, PublishedDate = new DateTime(1954, 7, 29), CoverImageUrl = "https://example.com/lotr.jpg" },
                new Book { Title = "Neuromancer", Author = "William Gibson", Description = "A science fiction novel that launched the cyberpunk genre", GenreId = genres[1].Id, PublishedDate = new DateTime(1984, 7, 1), CoverImageUrl = "https://example.com/neuromancer.jpg" },
                new Book { Title = "And Then There Were None", Author = "Agatha Christie", Description = "A mystery novel about ten strangers on an island", GenreId = genres[2].Id, PublishedDate = new DateTime(1939, 11, 6), CoverImageUrl = "https://example.com/agatha.jpg" },
                new Book { Title = "Einstein: His Life and Universe", Author = "Walter Isaacson", Description = "A biography of Albert Einstein", GenreId = genres[3].Id, PublishedDate = new DateTime(2007, 4, 10), CoverImageUrl = "https://example.com/einstein.jpg" },
                new Book { Title = "The Hunger Games", Author = "Suzanne Collins", Description = "A dystopian novel set in a post-apocalyptic nation", GenreId = genres[0].Id, PublishedDate = new DateTime(2008, 9, 14), CoverImageUrl = "https://example.com/hunger.jpg" }
                };

            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();
            }

        private async Task SeedLoansAsync()
            {
            if (await _context.Loans.AnyAsync())
                return;

            var users = await _userManager.Users.Where(u => u.Email != "admin@library.com").ToListAsync();
            var books = await _context.Books.ToListAsync();

            if (!users.Any() || !books.Any())
                return;

            var loans = new List<Loan>
                {
                new Loan
                    {
                    BookId = books[0].Id,
                    UserId = users[0].Id,
                    LoanDate = DateTime.Now.AddDays(-10),
                    DueDate = DateTime.Now.AddDays(4),
                    Status = LoanStatus.Active
                    },
                new Loan
                    {
                    BookId = books[1].Id,
                    UserId = users[0].Id,
                    LoanDate = DateTime.Now.AddDays(-5),
                    DueDate = DateTime.Now.AddDays(9),
                    Status = LoanStatus.Active
                    },
                new Loan
                    {
                    BookId = books[2].Id,
                    UserId = users[1].Id,
                    LoanDate = DateTime.Now.AddDays(-20),
                    DueDate = DateTime.Now.AddDays(-6),
                    Status = LoanStatus.Overdue
                    },
                new Loan
                    {
                    BookId = books[3].Id,
                    UserId = users[1].Id,
                    LoanDate = DateTime.Now.AddDays(-30),
                    DueDate = DateTime.Now.AddDays(-16),
                    ReturnDate = DateTime.Now.AddDays(-15),
                    Status = LoanStatus.Returned
                    },
                new Loan
                    {
                    BookId = books[4].Id,
                    UserId = users[0].Id,
                    LoanDate = DateTime.Now.AddDays(-7),
                    DueDate = DateTime.Now.AddDays(7),
                    Status = LoanStatus.Active
                    },
                new Loan
                    {
                    BookId = books[5].Id,
                    UserId = users[1].Id,
                    LoanDate = DateTime.Now.AddDays(-25),
                    DueDate = DateTime.Now.AddDays(-11),
                    ReturnDate = DateTime.Now.AddDays(-10),
                    Status = LoanStatus.Returned
                    }
                };

            _context.Loans.AddRange(loans);
            await _context.SaveChangesAsync();
            }
        }
    }
