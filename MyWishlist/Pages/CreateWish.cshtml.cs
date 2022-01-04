using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyWishlist.Data;
using MyWishlist.Models;
using MyWishlist.Utils;

namespace MyWishlist.Pages
{
    public class CreateWish : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IImagesStorage _imagesStorage;
        private readonly IHostEnvironment _appEnvironment;

        public CreateWish(ApplicationDbContext context, IOptions<Constants> options, IHostEnvironment appEnvironment)
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

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormCollection wishImage)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Wish.IsCompleted = false;
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
                Wish.ImageLink = link;
            }

            _context.Wishes.Add(Wish);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}