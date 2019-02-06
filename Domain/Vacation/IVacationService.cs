using System;
using System.Collections.Generic;

namespace Dvelop.Domain.Vacation
{
    public interface IVacationService
    {
        Guid RequestVacation(VacationModel request);
        bool UpdateVacation(VacationModel vacationModel);
        bool AcceptVacation(Guid id);
        bool RejectVacation(Guid id);
        bool CancelVacation(Guid id);
        IEnumerable<VacationModel> Vacations { get; }
    }
}