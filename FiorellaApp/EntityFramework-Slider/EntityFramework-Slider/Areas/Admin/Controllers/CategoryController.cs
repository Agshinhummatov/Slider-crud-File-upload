using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly AppDbContext _context;
        public CategoryController(ICategoryService categoryService, AppDbContext context)
        {
            _categoryService = categoryService;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _categoryService.GetAll());
        }


        [HttpGet]
        public IActionResult Create()     /*async-elemirik cunku data gelmir databazadan*/ //sadece indexe gedir hansiki inputa data elavve edib category yaradacaq
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)     // bazaya neyise save edirik deye asinxron olmalidi
        {
            try
            {
                if (!ModelState.IsValid)    // create eden zaman input null olarsa, yeni isvalid deyilse(isvalid olduqda data daxil edilir) view a qayit
                {
                    return View();    // lahiyenin ustune vurub arxa fonda null ucun olan  baglmaq lazimdi  yeni   <Nullable>disable</Nullable> elemek lazimdir 
                }

                var existData = await _context.Categories.FirstOrDefaultAsync(m => m.Name.Trim().ToLower() == category.Name.Trim().ToLower());
                // yoxlayiriq bize gelen yeni inputa yazilan name databazada varsa  error chixartmaq uchun

                if (existData is not null) /*gelen data databazamizda varsa yeni null deyilse*/
                {
                    ModelState.AddModelError("Name", "This data already exist!"); // bu inputa name daxil etmeyende error chixartsin. Name propertisinin altinda. buradaki Name hemin input-un adidir.

                    return View();
                }

                //int num = 1;
                //int num2 = 0;                 ozumuz error cixardib yoxladiqki Error indexine gedir ya yox try catch erroru yaxlayir ya yox
                //int result = num / num2;

                await _context.Categories.AddAsync(category);  //bazadaki categorie tablesine category ni add edir liste
                await _context.SaveChangesAsync();    //save edir bazaya 
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

               
                return RedirectToAction("Error", new { msj = ex.Message }); // eror mesajimizi diger seyfeye yoneldir "Error" yazdiqimiz indexe yoneldir
            }




          



        }


        public IActionResult Error( string msj) // RedirectToAction("Error", new { msj = ex.Message }); gonderdiyimiz parametiri qebul edirik
        {
            ViewBag.error = msj; //viewbag ile gonderirik datani erroun indexine orda qebul edecik
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest(); // id nulldusa eger retrun ele badRequest'i

                    
            Category category = await _context.Categories.FindAsync(id);  // category find edir yeni datani tapir 

            if (category is null) return NotFound(); // find edenden sonra yoxla bele bir categrya yoxdusa retrun edir note fondu


            _context.Categories.Remove(category); // remove ele elimdeki categoryani
            await _context.SaveChangesAsync();  // daha sonra data bazaya save et
            return RedirectToAction(nameof(Index)); // indexsine gonder delete edenden sonra


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int? id)
        {
            if (id is null) return BadRequest(); // id nulldusa eger retrun ele badRequest'i


            Category category = await _context.Categories.FindAsync(id);  // category find edir

            if (category is null) return NotFound(); // find edenden sonra yoxla bele bir categrya yoxdusa retrun edir note fondu


            category.SoftDelete = true; // deyiremki category delete edende softdelete ture dusecek bizde softdelete false olanlari gosderirik syahida
            await _context.SaveChangesAsync();  // daha sonra data bazaya save et
            return RedirectToAction(nameof(Index)); // indexsine gonder delete edenden sonra


        }


        [HttpGet]
      
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest(); // id nulldusa eger retrun ele badRequest'i


            Category category = await _context.Categories.FindAsync(id);  // category find edir

            if (category is null) return NotFound(); /// find edenden sonra yoxla bele bir categrya yoxdusa retrun edir note fondu


            return View(category);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Category category)
        {
            try
            {

                if (id is null) return BadRequest(); // id nulldusa eger retrun ele badRequest'i


                Category dbcategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id); // category find edir AsNoTracking() bunu ona gore yaziramki data base ile elaqeni qirsin yeni bu gelir update ede bilmir bunu yazanda  ise update edir

                if (dbcategory is null) return NotFound(); /// find edenden sonra yoxla bele bir categrya yoxdusa retrun edir note fondu

                if (dbcategory.Name.Trim().ToLower() == category.Name.Trim().ToLower())  /// bu yoxlayirki mene gelen name data bazadaki name beraberdise sen bunu data base requset gonderme
                {
                    return RedirectToAction(nameof(Index));   //ifin icine girir ve dayandirir methodu indexine retrun edir
                }

                /* dbcategory.Name = category.Name;  */ //   dbcategory.Name data bazamdki name mene gelen nemae beraber ele  category.Name  methoda viewdan gonderirik submit edende

                _context.Categories.Update(category);  // buda ona gore yaziriqki biz  dbcategory.Name = category.Name bundan istifade etmeyek tutaqki 4 dene input geldi gelib bir bir beraberlesdirmeliyem amma bunu yazanda ehtiyyac yoxdur

                await _context.SaveChangesAsync(); //data bazaya save edirik

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {


                return RedirectToAction("Error", new { msj = ex.Message }); // eror mesajimizi diger seyfeye yoneldir "Error" yazdiqimiz indexe yoneldir
            }



        }


        [HttpGet]

        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest(); // id nulldusa eger retrun ele badRequest'i


            Category category = await _context.Categories.FindAsync(id);  // category find edir

            if (category is null) return NotFound(); /// find edenden sonra yoxla bele bir categrya yoxdusa retrun edir note fondu


            return View(category);

        }

    }
}
