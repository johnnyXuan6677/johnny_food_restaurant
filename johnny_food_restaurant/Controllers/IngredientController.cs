using johnny_food_restaurant.Data;
using johnny_food_restaurant.Models;
using Microsoft.AspNetCore.Mvc;

namespace johnny_food_restaurant.Controllers
{

    public class IngredientController : Controller
    {
        private Repository<Ingredient> ingredients;
        //判別ingredients的差異
        public IngredientController(ApplicationDbContext context)
        {
            ingredients = new Repository<Ingredient>(context);
        }
        public async Task<IActionResult> Index()
        {
            return View(await ingredients.GetAllAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(await ingredients.GetByIdAsync(id, new QueryOption<Ingredient>() { Includes = "ProductIngredients.Product" }));
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        //Ingredient/Create
        [HttpPost]
        // XSRF/CSRF 預防
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IngredientId, Name")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                await ingredients.AddAsync(ingredient);
                return RedirectToAction("Index");
            }
            return View(ingredient);
        }

        //Ingredient/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return View(await ingredients.GetByIdAsync(id,new QueryOption<Ingredient> { Includes = "ProductIngredients.Product"})); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Ingredient ingredient)
        {
            await ingredients.DeleteAsync(ingredient.IngredientId);
            return RedirectToAction("Index");
        }
    }
}
