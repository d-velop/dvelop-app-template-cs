using System;
using System.Collections.Generic;
using System.Linq;
using Dvelop.Domain.Repositories;

namespace Dvelop.Domain.Vacation
{
    public class VacationService : IVacationService
    {
        private readonly IVacationRepository _vacationRepository;

        public VacationService(IVacationRepository vacationRepository)
        {
            _vacationRepository = vacationRepository;
        }

        public Guid RequestVacation(VacationModel request)
        {
            if (request.From >= request.To)
            {
                throw new ArgumentException("Dates invalid");
            }
            return  _vacationRepository.AddVacation(request);
        }

        public bool UpdateVacation(VacationModel request)
        {
            if (request.From >= request.To)
            {
                throw new ArgumentException("Dates invalid");
            }

            return _vacationRepository.UpdateVacation(request);
        }

        public bool AcceptVacation(Guid id)
        {
            var vac = _vacationRepository.Vacations.FirstOrDefault(v => v.Id == id);
            if (vac == null)
            {
                throw new ArgumentException("invalid vacation request");
            }

            vac.State = VacationState.Accepted;
            _vacationRepository.UpdateVacation(vac);
            return true;
        }

        public bool RejectVacation(Guid id)
        {
            var vac = _vacationRepository.Vacations.FirstOrDefault(v => v.Id == id);
            if (vac == null)
            {
                throw new ArgumentException("invalid vacation request");
            }

            vac.State = VacationState.Declined;
            _vacationRepository.UpdateVacation(vac);
            return true;
        }

        public bool CancelVacation(Guid id)
        {
            var vac = _vacationRepository.Vacations.FirstOrDefault(v => v.Id == id);
            if (vac == null)
            {
                throw new ArgumentException("invalid vacation request");
            }

            vac.State = VacationState.Cancelled;
            _vacationRepository.UpdateVacation(vac);
            return true;
        }

        public IEnumerable<VacationModel> Vacations => _vacationRepository.Vacations;
    }
}
