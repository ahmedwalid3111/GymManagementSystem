using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _memberRepo;
        private readonly IGenericRepository<Membership> _membershipRepo;
        private readonly IGenericRepository<Plan> _planRepo;
        private readonly IGenericRepository<HealthRecord> _healthRecordRepo;
        private readonly IGenericRepository<Booking> _bookingRepo;
        public MemberService(IGenericRepository<Member> memberRepo,IGenericRepository<Membership> membershipRepo,IGenericRepository<Plan> planRepo,IGenericRepository<HealthRecord> healthRecordRepo,IGenericRepository<Booking> bookingRepo)
        {
            _memberRepo = memberRepo;
            _membershipRepo = membershipRepo;
            _planRepo = planRepo;
            _healthRecordRepo = healthRecordRepo;
            _bookingRepo = bookingRepo;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExists = await _memberRepo.AnyAsync(m => m.Email == model.Email, ct);
            var phoneExists = await _memberRepo.AnyAsync(m => m.Phone == model.Phone, ct);
            if (emailExists || phoneExists)
            {
                return false;
            }
            var member = new Member()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street
                },
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                HealthRecord = new HealthRecord()
                {
                    Height=model.HealthRecordViewModel.Height,
                    Weight = model.HealthRecordViewModel.Weight,
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Note = model.HealthRecordViewModel.Note
                }

            };
            var result = await _memberRepo.AddAsync(member, ct);
            return result > 0;

        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _memberRepo.GetAllAsync(ct: ct);
            if (!members.Any())
            {
                return [];
            }
            var memberViewModels = members.Select(m => new MemberViewModel()
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                Photo = m.Photo,
                Gender = m.Gender
            }).ToList();
            return memberViewModels;
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var member =await _memberRepo.GetByIdAsync(memberId, ct);
            if (member is null)
                return null;
            var memberViewModel = new MemberViewModel()
            {
                
                Name = member.Name,
                Email = member.Email,
                Gender = member.Gender,
                Phone = member.Phone,
                Photo = member.Photo,
                Address = $"{member.Address.BuildingNumber}, {member.Address.Street}, {member.Address.City}",
                DateOfBirth = member.DateOfBirth.ToShortDateString()

            };
            var membership = await _membershipRepo.FirstOrDefaultAsync(M => M.MemberId == memberId && M.EndDate > DateTime.Now, ct: ct);
            if (membership is not null)
            {
                var plan = await _planRepo.GetByIdAsync(membership.PlanId, ct);
                memberViewModel.PlanName = plan?.Name!;
                memberViewModel.MembershipStartDate = membership.CreatedAt.ToShortDateString();
                memberViewModel.MembershipEndDate = membership.EndDate.ToShortDateString();
            }
            return memberViewModel;

        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var healthRecord = await _healthRecordRepo.FirstOrDefaultAsync(H => H.MemberId == memberId, ct: ct);
            if (healthRecord is null)
                return null;
            return new HealthRecordViewModel()
            {
                Height = healthRecord.Height,
                Weight = healthRecord.Weight,
                BloodType = healthRecord.BloodType,
                Note = healthRecord.Note
            };
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member= await _memberRepo.GetByIdAsync(memberId, ct);
            if (member is null)
                return null;
            return new MemberToUpdateViewModel()
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Photo = member.Photo,
                BuildingNumber = member.Address.BuildingNumber,
                City = member.Address.City,
                Street = member.Address.Street
            };
        }

        public async Task<bool> RemoveMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _memberRepo.GetByIdAsync(memberId, ct);
            if(member is null)
                return false;
            var FutureSessions=await _bookingRepo.AnyAsync(B => B.MemberId == memberId && B.Session.StartDate > DateTime.Now, ct);
            if(FutureSessions)
                return false;
            var result = await _memberRepo.DeleteAsync(member, ct);
            return result>0;    
        }

        public async Task<bool> UpdateMemberAsync(int memberId, MemberToUpdateViewModel model, CancellationToken ct = default)
        {

            var member = await _memberRepo.GetByIdAsync(memberId, ct);
            if ( member is null)
            {
                return false;
            }
            var emailExists = await _memberRepo.AnyAsync(M => M.Email == model.Email && M.Id != memberId, ct);
            var phoneExists = await _memberRepo.AnyAsync(M => M.Phone == model.Phone && M.Id != memberId, ct);
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
            var result = await _memberRepo.UpdateAsync(member, ct);
            return result>0;
            
            

        }
    }
}
