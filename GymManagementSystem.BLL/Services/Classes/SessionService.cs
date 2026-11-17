using AutoMapper;
using GymManagementSystem.BLL.Helpers;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Data.Models.Enums;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(ct);
            if(!sessions.Any())
            {
                return [];
            }
            sessions = sessions.OrderByDescending(S => S.StartDate);
            var sessionsViewModel = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);
            foreach (var session in sessionsViewModel)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            }
            return sessionsViewModel;
        }
        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate)
                return Result.Validation("End date must be after start date.");
            if(model.StartDate <= DateTime.Now)
                return Result.Validation("Start date must be in the future.");
            var trainerRepo =  _unitOfWork.GetRepository<Trainer>();
            var trainer = await trainerRepo.GetByIdAsync(model.TrainerId, ct);
            if (trainer is null)
                return Result.NotFound("Trainer not found.");
            if (await _unitOfWork.SessionRepository.AnyAsync(S => S.TrainerId == model.TrainerId && (S.StartDate <= model.StartDate && S.EndDate >= model.StartDate) || (model.StartDate < S.StartDate && model.EndDate > S.StartDate), ct))
                return Result.Fail("That Trainer has another session at that time.");
            var categoryRepo = _unitOfWork.GetRepository<Category>();
            var category = await categoryRepo.GetByIdAsync(model.CategoryId, ct);
            if (category is null)
                return Result.NotFound("Category not found.");
            var isValidSpeciality=Enum.TryParse<Specialities>(category.CategoryName,true, out var categorySpeciality);
            if( !isValidSpeciality || trainer.Speciality != categorySpeciality)
                return Result.Validation("Trainer is not qualified for this category.");
            var session = _mapper.Map<Session>(model);
            _unitOfWork.SessionRepository.AddAsync(session);
            var result=await _unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.Ok() : Result.Fail("Failed to create session.");
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var categories =await _unitOfWork.GetRepository<Category>().GetAllAsync(ct:ct);
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(categories);
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var trainers =await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct:ct);
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(trainers);
        }

        public async Task<SessionViewModel?> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategoryAsync(sessionId, ct);
            if (session is null)
                return null;
            var sessionViewModel = _mapper.Map<SessionViewModel>(session);
            sessionViewModel.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId,ct);
            return sessionViewModel;

        }

        public async Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null)
                return null;
            if (!await IsValidSessionForUpdateAsync(session, ct))
                return null;
            return _mapper.Map<UpdateSessionViewModel>(session);

        }
        public async Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId,ct);
            if (session is null)
                return Result.NotFound("Session Not Found");
            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot Edit a session that already started");
            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            if (bookingCount > 0)
                return Result.Fail("Cannot Edit a Session that already has bookings");
            if (model.EndDate <= model.StartDate)
                return Result.Validation("End Date must be after Start Date");
            if (model.StartDate <= DateTime.Now)
                return Result.Validation("Start Date must be in the future");
            var trainerRepo = _unitOfWork.GetRepository<Trainer>();
            var trainer = await trainerRepo.GetByIdAsync(model.TrainerId, ct);

            if (trainer is null)
                return Result.NotFound("Trainer not found");

            var isUnavailable = await _unitOfWork.SessionRepository.AnyAsync(S => S.TrainerId == model.TrainerId && S.Id != session.Id && ((S.StartDate <= model.StartDate &&S.EndDate>=model.StartDate) ||(model.StartDate<=S.StartDate&&model.EndDate>=S.StartDate)),ct);
            if (isUnavailable)
                return Result.Fail("That Trainer has another session at the same time range");
            var categoryRepo = _unitOfWork.GetRepository<Category>();
            var category = await categoryRepo.GetByIdAsync(session.CategoryId, ct);
            var isValidSpeciality = Enum.TryParse<Specialities>(category.CategoryName, true,out var categorySpeciality);

            if (!isValidSpeciality || categorySpeciality != trainer.Speciality)
                return Result.Validation("Can't Update this session for this trainer");

            _mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;
            _unitOfWork.SessionRepository.UpdateAsync(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Update this session");
        }
        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null)
                return Result.NotFound("Session Not Found");
            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot remove ongoing session");
            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            if (bookingCount > 0)
                return Result.Fail("Cannot delete a session that has bookings");
            _unitOfWork.SessionRepository.DeleteAsync(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to delete a session");
        }
        private async Task<bool> IsValidSessionForUpdateAsync(Session session,CancellationToken ct = default)
        {
            if (session.StartDate <= DateTime.Now)
                return false;
            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            return bookingCount == 0;


        }

        
    }
}
