using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyWishlist.Data;
using MyWishlist.Models;
using MyWishlist.Utils;

namespace MyWishlist.Pages
{
    public class EditWish : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IImagesStorage _imagesStorage;
        private readonly IHostEnvironment _appEnvironment;

        public EditWish(ApplicationDbContext context, IOptions<Constants> options, IHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
#if DEBUG
            _imagesStorage = new StubImagesStorage();
#else
            _imagesStorage = new CloudinaryImagesStorage(options);
#endif
        }

        [BindProperty] public Wish Wish { get; set; }
        [BindProperty] public string Price { get; set; }
        [BindProperty] public string Currency { get; set; }
        [BindProperty] public bool DeleteImage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Wish = await _context.Wishes.FirstOrDefaultAsync(m => m.Id == id);

            if (Wish == null)
            {
                return NotFound();
            }

            Price = Wish.PriceAmount;
            Currency = Wish.PriceCurrency;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormCollection wishImage)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Price != null)
            {
                Wish.PriceAmount = Price;
                Wish.PriceCurrency = Currency;
            }

            var file = wishImage.Files.FirstOrDefault();

            if (file != null)
            {
                if (Path.GetExtension(file.FileName).ToLower() != ".jpeg"
                    && Path.GetExtension(file.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(file.FileName).ToLower() != ".gif"
                    && Path.GetExtension(file.FileName).ToLower() != ".bmp"
                    && Path.GetExtension(file.FileName).ToLower() != ".png")
                {
                    return Page();
                }

                var newFileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
                var imagePath = _appEnvironment.ContentRootPath + "/" + newFileName;

                await using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var link = _imagesStorage.Upload(imagePath);
                System.IO.File.Delete(imagePath);
                if (Wish.ImageLink != null)
                {
                    _imagesStorage.Delete(Wish.ImageLink);
                }

                Wish.ImageLink = link;
            }
            else if (DeleteImage)
            {
                if (Wish.ImageLink != null)
                {
                    _imagesStorage.Delete(Wish.ImageLink);
                }

                Wish.ImageLink = null;
            }

            _context.Attach(Wish).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WishExists(Wish.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool WishExists(int id)
        {
            return _context.Wishes.Any(e => e.Id == id);
        }
    }
}