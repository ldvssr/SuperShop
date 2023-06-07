using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;

        public ProductsController(
            IProductRepository productRepository,
            IUserHelper userHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper            
            )            
        {
            _productRepository = productRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(_productRepository.GetAll().OrderBy(p => p.Name));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {

                    var guid = Guid.NewGuid().ToString();

                    var file = $"{guid}.jpg";
                    
                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\products",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/products/{file}";
                }

                var product = _converterHelper.ToProduct(model, path, true);

                //TODO: MODIFICAR PARA O USER QUE ESTIVER LOGADO
                product.User = await _userHelper.GetUserByEmailAsync("ldvssr@icloud.com");
                await _productRepository.CreateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // if (id == null) return NotFound();
            if (id == null) return RedirectToAction(nameof(Index));

            // var product = await _context.Products.FindAsync(id);
            // var product = _repository.GetProduct(id.Value);
            var product = await _productRepository.GetByIdAsync(id.Value);

            if (product == null) return RedirectToAction(nameof(Index));

            var productViewModal =
                _converterHelper.ToProductViewModel(product);

            return View(productViewModal);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl;

                    if(model.ImageFile != null && model.ImageFile.Length > 0)
                    {

                        var guid = Guid.NewGuid().ToString();

                        var file = $"{guid}.jpg";

                        path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\images\\products",
                            file);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }
                        path = $"~/images/products/{file}";
                    }
                    var product = _converterHelper.ToProduct(model, path, false);
                    //TODO: MODIFICAR PARA O USER QUE ESTIVER LOGADO
                    product.User = await _userHelper.GetUserByEmailAsync("ldvssr@icloud.com");
                    await _productRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _productRepository.ExistAsync(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            await _productRepository.DeleteAsync(product);
            return RedirectToAction(nameof(Index));
        }
    }
}
