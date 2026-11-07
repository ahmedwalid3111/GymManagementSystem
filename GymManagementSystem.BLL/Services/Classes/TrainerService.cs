using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.TrainerViewModels;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IGenericRepository<Trainer> _trainerRepository;
        private readonly IGenericRepository<Session> _sessionRepository;
        public TrainerService(IGenericRepository<Trainer> trainerRepository, IGenericRepository<Session> sessionRepository)
        {
            _trainerRepository = trainerRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            if (await _trainerRepository.AnyAsync(t => t.Email == model.Email, ct)) return false;
            if (await _trainerRepository.AnyAsync(t => t.Phone == model.Phone, ct)) return false;
            var entity = new Trainer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street=model.Street,
                    City=model.City
                },
                DateOfBirth=model.DateOfBirth,
                Speciality=model.Specialities

            };
            var result = await _trainerRepository.AddAsync(entity,ct);
            return result > 0;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers= await _trainerRepository.GetAllAsync(ct: ct);
            var trainersViewModel = trainers.Select(t => new TrainerViewModel()
            {
                Id = t.Id,
                Name = t.Name,
                Phone = t.Phone,
                Email = t.Email,
                Gender = t.Gender.ToString(),
                Address = $"{t.Address.BuildingNumber} - {t.Address.Street} - {t.Address.City}",
                DateOfBirth = t.DateOfBirth.ToShortDateString(),
                Specialities = t.Speciality.ToString()
            });
            return trainersViewModel;
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            return trainer is null ? null : new TrainerViewModel()
            {
                Id=trainer.Id,
                Name=trainer.Name,
                Email=trainer.Email,
                Phone=trainer.Phone,
                Gender = trainer.Gender.ToString(),
                Address = $"{trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}",
                DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
                Specialities = trainer.Speciality.ToString()
            };
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            return trainer is null ? null : new TrainerToUpdateViewModel()
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Specialities = trainer.Speciality,
                BuildingNumber = trainer.Address.BuildingNumber,
                Street = trainer.Address.Street,
                City = trainer.Address.City
            };
        }

        public async Task<bool> RemoveTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer =  await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer is null) return false;
            var hasFutureSessions = await _sessionRepository.AnyAsync(s => s.TrainerId == trainerId && s.StartDate > DateTime.Now, ct);
            if (hasFutureSessions) return false;
            var result = await _trainerRepository.DeleteAsync(trainer, ct);
            return result > 0;
        }

        public async Task<bool> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer is null) return false;
            if (await _trainerRepository.AnyAsync(t => t.Email == model.Email && t.Id != trainerId, ct)) return false;
            if (await _trainerRepository.AnyAsync(t => t.Phone == model.Phone && t.Id != trainerId, ct)) return false;
            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Speciality = model.Specialities;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.Street = model.Street;
            trainer.Address.City = model.City;
            trainer.UpdatedAt = DateTime.Now;
            var result =await _trainerRepository.UpdateAsync(trainer, ct);
            return result > 0;
        }
    }
}
