using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWishlist.Data;
using MyWishlist.Models;

namespace MyWishlist.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private List<Wish> _wishes;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
        }

        public List<Wish> ActualWishes => _wishes.FindAll(x => !x.IsCompleted).ToList();
        public List<Wish> CompletedWishes => _wishes.FindAll(x => x.IsCompleted).ToList();

        public async Task OnGetAsync()
        {
            _wishes = await _context.Wishes.AsNoTracking().Select(x => x).ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteWishAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!_signInManager.IsSignedIn(User))
            {
                return new ForbidResult();
            }

            var wish = await _context.Wishes.FindAsync(id);

            if (wish == null) return NotFound();
            _context.Wishes.Remove(wish);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Index");
        }
        
        public async Task<IActionResult> OnPostCompleteWishAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            if (!_signInManager.IsSignedIn(User))
            {
                return new ForbidResult();
            }

            var wish = await _context.Wishes.FindAsync(id);
            if (wish == null) return NotFound();
            wish.IsCompleted = true;
            _context.Attach(wish).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WishExists(wish.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToPage("/Index");
        }
        
        private bool WishExists(int id)
        {
            return _context.Wishes.Any(e => e.Id == id);
        }
    }
}
