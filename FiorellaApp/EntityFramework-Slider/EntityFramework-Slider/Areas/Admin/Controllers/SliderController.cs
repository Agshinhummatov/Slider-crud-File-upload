using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;      // bu interface vasitesi ile biz gedib WWw.root un icine cata bilecik
        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Sliders.Where(m => !m.SoftDelete).ToListAsync();
            return View(sliders);
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {

            if (id == null) return BadRequest();
            //badrequest routingde falan nese sehvlik olduqda chixan exseptiondur
            //meselen routingden sehven axtariw edilen id silinerse bu zaman chixan errordur



            //gelen id li elementi bazadan tapmaq uchun edirik bunu
            Slider? slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);


            //eyer kimse sehv regem yazibsa Url-e
            //NotFound-tapilmadi deye Exception 
            if (slider is null) return NotFound();
            //gelen id bazamizda yoxdursa chican error

            return View(slider);
        }


        //[HttpGet]-datani goturende 

        [HttpGet]
        public IActionResult Create()     /*async-elemirik cunku data gelmir databazadan*/
        {

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create( Slider slider)
        {
            try
            {

                if (!ModelState.IsValid)    // create eden zaman input null olarsa, yeni isvalid deyilse(isvalid olduqda data daxil edilir) view a qayit
                {
                    return View();    // lahiyenin ustune vurub arxa fonda null ucun olan  baglmaq lazimdi  yeni   <Nullable>disable</Nullable> elemek lazimdir 
                }

                // comente atmasam eger asqiqdaki if atmaliyam comente  hazir extationlara cixartmisam

                //if (!slider.Photo.ContentType.Contains("image/")){             /// yoxlariki ContetntType yeni yuklediyin filenin tipu nedir bozde sert qoyuruqki image/ olsun 2 ci sert qoyasaq yanina yazaciq
                //    ModelState.AddModelError("Photo", "Flie type must be image");
                //    return View();
                //}

                //if (slider.Photo.Length/1024 >200)   // Length/1024 biz deyirki lensini 1024 de bolub onun nece kb(mb) olduqu cixardiriq ve sert qoyuruqki 200 kbdan asaqi olanlari yuklemek olar ancaq 
                //{
                //    ModelState.AddModelError("Photo", "Photo size must be max 200kb");
                //    return View();
                //}



                if (!slider.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();
                }

                if (!slider.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View();
                }




                string fileName = Guid.NewGuid().ToString() + "_" + slider.Photo.FileName; // Guid.NewGuid() bu neynir bir id kimi dusune birerik hemise ferqli herifler verir mene ki men sekilin name qoyanda o ferqli olsun tostring ele deyirem yeni random oalraq ferlqi ferqli sekil adi gelecek  ve  slider.Photo.FileName; ordan gelen ada birslerdir 

                /*   string path = Path.Combine(_env.WebRootPath, "img", fileName); */  // Path.Combine nedir www.root kimi olan yoldur _env.WebRootPath bi www.rootun icindeyem hansi file qoyacam "img" file qoyacam demekdi  ve fileName yeni adini // path hara qoyduqmu gosderir mene yeni www.rootun icinde img folderin icine get yuxaride yazdiqim file name qoy yeni adini 

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                using (FileStream stream = new(path, FileMode.Create))    ///  using() neynir elaqeni qirir yeni bu www.rootun icinde getdi daha sonra deyiremki elaqeni qir bunu yazmasam exception cixacaq mutleq elaqeni qirlamiyam arxa planda qebz collectionu isleyir ve qirandan sonra silir arxa planda biz gormurk bunu
                {
                    await slider.Photo.CopyToAsync(stream);     // deyiremki yuxardaki yaratdiqimi   copy to ele streami  yeni icindeki path yeni fiziki olaraq lahiyemin icine www.rootun icine axi pathde yazmisam  kopyalayir atir ora upload edende sekli   // FileStream  bi neyinir fiziki olaraq kompyuterimde  file crate ede bilim deye bunu yazaliyam ve kansruktoru bizden data isdeyir  // FileMode ise ora ne edirikse tutaqki Createdise . crate yaziriq 
                }

                slider.Image = fileName;

                await _context.Sliders.AddAsync(slider);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));



            }
            catch (Exception ex)
            {

                throw;
            }

           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {

                if (id == null) return BadRequest();

                Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

                if (slider is null) return NotFound();

                //string path = Path.Combine(_env.WebRootPath, "img", slider.Image);     /// men path yolu ile yoxladim img folderin  icindeki slider.image  yeni  bu adda sekkil varmi ve tapir hemin sekli stiring kimi 

                //if (System.IO.File.Exists(path))  // bu yoxlayirki ele bir file var www.rootun icinde yeni lahiyemde //System.IO ise bir usingdir onu yazmasam admin panelde oxumur
                //{
                //    System.IO.File.Delete(path);   // burda ise tapdiqim file silirem yeni image www.root un icindeki image 
                //}


                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", slider.Image);

                FileHelper.DeleteFile(path);

                _context.Sliders.Remove(slider);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception)
            {

                throw;
            }


           
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

            if (slider is null) return NotFound();

            return View(slider);

        }
    }
}
