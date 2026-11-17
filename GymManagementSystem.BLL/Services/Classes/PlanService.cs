using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class PlanService: IPlanService
    {
        private readonly IUnitOfWork unitOfWork;
        public PlanService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans =await unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            var plansViewModel = plans.Select(p => new PlanViewModel()
            {
                Id = p.Id,
                Name=p.Name,
                IsActive=p.IsActive,
                Price=p.Price,
                Description=p.Description,
                DurationDays=p.DurationDays
            });
            return plansViewModel;

        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            return plan == null ? null : new PlanViewModel()
            {
                Id = plan.Id,
                Name = plan.Name,
                IsActive = plan.IsActive,
                Price = plan.Price,
                Description = plan.Description,
                DurationDays = plan.DurationDays
            };

        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if(plan is null || !plan.IsActive)return null;
            if(await HasActiveMembershipsAsync(planId, ct)) return null;
            return new UpdatePlanViewModel()
            {

                PlanName = plan.Name,
                Price = plan.Price,
                Description = plan.Description,
                DurationDays = plan.DurationDays
            };
        }

        public async Task<bool> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if(plan is null) return false;
            if(plan.IsActive && await HasActiveMembershipsAsync(planId, ct)) return false;
            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
            unitOfWork.GetRepository<Plan>().UpdateAsync(plan);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result>0;
        }

        public async Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if(plan is null) return false;
            if(await HasActiveMembershipsAsync(id, ct)) return false;
            plan.Price = model.Price;
            plan.Description = model.Description;
            plan.DurationDays = model.DurationDays;
            plan.UpdatedAt = DateTime.Now;
            unitOfWork.GetRepository<Plan>().UpdateAsync(plan);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result>0;
        }
        private async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct = default)
        {
            return await unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == planId && m.EndDate > DateTime.Now, ct);
        }
    }
}

        
    

