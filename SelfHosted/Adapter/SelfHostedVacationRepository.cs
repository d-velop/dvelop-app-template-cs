using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dvelop.Domain.Repositories;
using Dvelop.Domain.Vacation;

namespace Dvelop.SelfHosted.Adapter
{
    public class SelfHostedVacationRepository : IVacationRepository
    {
        private readonly ITenantRepository _tenantRepository;

        private readonly ConcurrentDictionary<string, Dictionary<Guid,VacationModel>> _vacationDatabase = new ConcurrentDictionary<string, Dictionary<Guid,VacationModel>>();

        public SelfHostedVacationRepository(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
            var defaultValues = _vacationDatabase.GetOrAdd("0", new Dictionary<Guid, VacationModel>());
            var value1 = new VacationModel
            {
                Id = Guid.NewGuid(),
                Comment = "Some vacation makes me happy",
                From = DateTime.UtcNow,
                To =  DateTime.UtcNow.AddDays(14)            
            };
            defaultValues.Add(value1.Id, value1);
        }

        public Guid AddVacation(VacationModel vacation)
        {
            var id = Guid.NewGuid();
            var tenant = _tenantRepository.TenantId;
            var vacationModels = _vacationDatabase.GetOrAdd(tenant, new Dictionary<Guid,VacationModel>());
            vacation.Id = id;
            if (vacationModels.ContainsKey(id))
            {
                throw new Exception($"key {id} already exists");
            }
            vacationModels[id] = vacation;
            return id;

        }

        public bool UpdateVacation(VacationModel vacation)
        {
            var tenant = _tenantRepository.TenantId;
            var vacationModels = _vacationDatabase.GetOrAdd(tenant, new Dictionary<Guid,VacationModel>());
            if (!vacationModels.ContainsKey(vacation.Id))
            {
                throw new Exception($"key {vacation.Id} does not exist");
            }
            vacationModels[vacation.Id] = vacation;
            return true;
        }

        public IEnumerable<VacationModel> Vacations => _vacationDatabase.GetOrAdd(_tenantRepository.TenantId, new Dictionary<Guid, VacationModel>()).Values;
    }
}
