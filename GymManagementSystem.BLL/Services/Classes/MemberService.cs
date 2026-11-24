using AutoMapper;
using GymManagementSystem.BLL.Services.AttachmentService;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork,IMapper mapper, IAttachmentService attachmentService )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var repo=_unitOfWork.GetRepository<Member>();
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email, ct);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone, ct);
            if (emailExists || phoneExists)
            {
                return false;
            }
            var photo = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MembersPictures",ct);
            if (string.IsNullOrEmpty(photo))
                return false;
            var member = _mapper.Map<CreateMemberViewModel, Member>(model);
            member.Photo = photo;

            
        repo.AddAsync(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            if (result == 0) 
            {
                _attachmentService.Delete(member.Photo,"MembersPictures");
                return false;
            }
            return true;
        }

            

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any())
            {
                return [];
            }
            var memberViewModels = _mapper.Map<IEnumerable<MemberViewModel>>(members);
            return memberViewModels;
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var member =await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member is null)
                return null;
            var memberViewModel = _mapper.Map<MemberViewModel>(member);
            var membership = await _unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(M => M.MemberId == memberId && M.EndDate > DateTime.Now, ct: ct);
            if (membership is not null)
            {
                var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(membership.PlanId, ct);
                memberViewModel.PlanName = plan?.Name!;
                memberViewModel.MembershipStartDate = membership.CreatedAt.ToShortDateString();
                memberViewModel.MembershipEndDate = membership.EndDate.ToShortDateString();
            }
            return memberViewModel;

        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(H => H.MemberId == memberId, ct: ct);
            if (healthRecord is null)
                return null;
            return _mapper.Map<HealthRecordViewModel>(healthRecord);
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member= await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member is null)
                return null;
            return _mapper.Map<MemberToUpdateViewModel>(member);
        }

        public async Task<bool> RemoveMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if(member is null)
                return false;
            var FutureSessions=await _unitOfWork.GetRepository<Booking>().AnyAsync(B => B.MemberId == memberId && B.Session.StartDate > DateTime.Now, ct);
            if(FutureSessions)
                return false;
            _unitOfWork.GetRepository<Member>().DeleteAsync(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            if (result > 0)
            {
                if (!string.IsNullOrEmpty(member.Photo))
                    _attachmentService.Delete(member.Photo, "MembersPictures");
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateMemberAsync(int memberId, MemberToUpdateViewModel model, CancellationToken ct = default)
        {

            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if ( member is null)
            {
                return false;
            }
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(M => M.Email == model.Email && M.Id != memberId, ct);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(M => M.Phone == model.Phone && M.Id != memberId, ct);
            if (emailExists || phoneExists)
            {
                return false;
            }
            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.Street = model.Street;
            member.Address.City = model.City;
            member.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Member>().UpdateAsync(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result>0;
            
            

        }
    }
}
