using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;       
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClubController(IClubRepository clubRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _clubRepository = clubRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Club> clubs = await _clubRepository.GetAll();
            return View(clubs);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Club club = await _clubRepository.GetById(id);
            return View(club);
        }

        public IActionResult Create()
        {
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var createClubViewModel = new CreateClubViewModel { AppUserId = curUserId };
            return View(createClubViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubVm)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(clubVm.Image);

                var club = new Club
                {
                    Title = clubVm.Title,
                    Description = clubVm.Description,
                    AppUserId = clubVm.AppUserId,
                    Address = new Address
                    {
                        City = clubVm.Address.City,
                        State = clubVm.Address.State,
                        Street = clubVm.Address.Street
                    }, 
                    Image = result.Url.ToString()
                };
                _clubRepository.AddClub(club);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }
            return View(clubVm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetById(id);
            if (club == null)
            {
                return View("Error");
            }
            var clubVM = new EditClubViewModel
            {
                Title = club.Title,
                Description = club.Description,
                AddressId = club.AddressId,
                Address = club.Address,
                URL = club.Image,
                ClubCategory = club.ClubCategory
            };
            return View(clubVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit");
                return View("Edit", clubVM);
            }
            var userClub = await _clubRepository.GetByIdAsyncNoTracking(id);
            if (userClub != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userClub.Image);
                } 
                catch (Exception e)
                {
                    ModelState.AddModelError(e.Message, "Could not delete the image");
                    return View(clubVM);
                }
                var photoResult = await _photoService.AddPhotoAsync(clubVM.Image);
                var club = new Club
                {
                    Id = id,
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    AddressId = clubVM.AddressId,
                    Address = clubVM.Address,
                    Image = photoResult.Url.ToString()
                };
                _clubRepository.UpdateClub(club);
                return RedirectToAction("Index");
            }
            else
            {
                return View(clubVM);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var clubDetail = await _clubRepository.GetById(id);
            if (clubDetail != null)
            {
                return View(clubDetail);
            }
            else
            {
                return View("Error");
            }

        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            var clubDetail = await _clubRepository.GetById(id);
            if (clubDetail == null) return View("Error");
            _clubRepository.DeleteClub(clubDetail);
            return RedirectToAction("Index");
        }
    }
}
