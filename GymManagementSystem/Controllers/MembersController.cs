using GymManagementSystem.BLL.Services.AttachmentService;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;
        public MembersController(IMemberService memberService, IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllMembersAsync(ct: ct);
            return View(members);
        }
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member= await _memberService.GetMemberDetailsAsync(id, ct: ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));

            }
            return View(member);
        }
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var healthRecord = await _memberService.GetMemberHealthRecordAsync(id, ct: ct);
            if (healthRecord is null)
            {
                TempData["ErrorMessage"] = "Health record not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(healthRecord);
        }
        #region Create User
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _memberService.CreateMemberAsync(model, ct);
            if (result)
            {
                TempData["SuccessMessage"] = "Member created successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create member.";
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion


        #region Edit Member
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            // Implement logic to retrieve member details for editing
            var member = await _memberService.GetMemberToUpdateAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute]int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _memberService.UpdateMemberAsync(id, model, ct);
            if (result == false)
            {
                TempData["ErrorMessage"] = "Failed to update member";

                return View(model);
            }
            TempData["SuccessMessage"] = "Member updated successfully.";
            return RedirectToAction(nameof(Index));

        }
        #endregion

        #region Delete Member
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsAsync(id,ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _memberService.RemoveMemberAsync(id, ct);
            if (result)
            {
                TempData["SuccessMessage"] = "Member deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete member.";
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion

        public async Task<IActionResult> Picture(int id)
        {
            var member = await _memberService.GetMemberDetailsAsync(id);
            if (member is null || string.IsNullOrEmpty(member.Photo))
                return NotFound();
            var result = _attachmentService.GetFile(member.Photo, "MembersPictures");
            if (result is null)
                return NotFound();
            return File(result.Value.Stream, result.Value.ContentType);
        }
    }
}
