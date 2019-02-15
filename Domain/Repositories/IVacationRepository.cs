using System;
using System.Collections.Generic;
using Dvelop.Domain.Vacation;

namespace Dvelop.Domain.Repositories
{
    public interface IVacationRepository
    {
        Guid AddVacation(VacationModel vacation);
        bool UpdateVacation(VacationModel vacation);
        IEnumerable<VacationModel> Vacations { get; }
    }
}
