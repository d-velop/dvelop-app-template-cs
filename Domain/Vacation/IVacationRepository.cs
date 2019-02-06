using System;
using System.Collections.Generic;

namespace Dvelop.Domain.Vacation
{
    public interface IVacationRepository
    {
        Guid AddVacation(VacationModel vacation);
        bool UpdateVacation(VacationModel vacation);
        IEnumerable<VacationModel> Vacations { get; }
    }
}
