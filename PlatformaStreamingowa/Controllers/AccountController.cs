using CustomIdentity.Data;
using CustomIdentity.Models;
using CustomIdentity.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CustomIdentity.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext _context;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, AppDbContext context)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Main", "Home");
                }

                ModelState.AddModelError("", "Nieprawid³owa próba logowania");
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var addToRoleResult = await userManager.AddToRoleAsync(user, "User");
                    if (!addToRoleResult.Succeeded)
                    {
                        foreach (var error in addToRoleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Main", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Management()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccount(AccountVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return NotFound();
                }

                user.Name = model.Name;
                user.Email = model.Email;
                if (!string.IsNullOrEmpty(model.Has³o))
                {
                    var setPasswordResult = await userManager.RemovePasswordAsync(user);
                    if (setPasswordResult.Succeeded)
                    {
                        setPasswordResult = await userManager.AddPasswordAsync(user, model.Has³o);
                    }
                    if (!setPasswordResult.Succeeded)
                    {
                        foreach (var error in setPasswordResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View("Management", model);
                    }
                }

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("Management", model);
                }

                return RedirectToAction("Main", "Home");
            }

            return View("Management", model);
        }

        public IActionResult Watching(int filmId)
        {
            var film = GetFilmById(filmId);
            if (film == null)
            {
                return NotFound();
            }

            var ratings = _context.Oceny.Where(o => o.FilmId == filmId).Select(o => o.OcenaWartosc).ToList();
            double averageRating = ratings.Any() ? ratings.Average() : 0;

            var comments = _context.Comments.Where(c => c.FilmId == filmId).OrderByDescending(c => c.DateAdded).ToList();

            var model = new WatchingViewModel
            {
                Id = film.Id,
                Title = film.Tytul,
                Description = film.Opis,
                VideoUrl = film.LinkDoFilmu,
                Rating = averageRating,
                Comments = comments
            };

            return View("~/Views/Subpage/Watching.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> RateFilm(int filmId, int rating)
        {
            SaveFilmRating(filmId, rating);
            return RedirectToAction("Watching", new { filmId = filmId });
        }

        [HttpPost]
        public IActionResult AddComment(int filmId, string content)
        {
            var user = userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                FilmId = filmId,
                Content = content,
                DateAdded = DateTime.Now,
                Name = user.Name 
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            var updatedComments = _context.Comments
                .Where(c => c.FilmId == filmId)
                .OrderByDescending(c => c.DateAdded)
                .ToList();

            return PartialView("~/Views/Shared/_CommentsPartial.cshtml", updatedComments);
        }

        private Film GetFilmById(int filmId)
        {
            return _context.Filmy.FirstOrDefault(f => f.Id == filmId);
        }

        private void SaveFilmRating(int filmId, int rating)
        {
            var film = GetFilmById(filmId);

            if (film != null)
            {
                var ocena = new Ocena { OcenaWartosc = rating };
                film.Oceny.Add(ocena);

                _context.SaveChanges();
            }
        }
    }
}
