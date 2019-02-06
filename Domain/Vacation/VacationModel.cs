using System;

namespace Dvelop.Domain.Vacation
{
    public class VacationModel
    {
        public VacationModel()
        {
            State = VacationState.New;
        }
        public VacationState State { get; set; }
        public Guid Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public VacationType Type { get; set; }
        public string Comment { get; set; }
    }

    public enum VacationState
    {
        New = 0,
        Accepted = 1,
        Declined = 2,
        Cancelled = 3
    }

    public enum VacationType
    {
        Annual,
        Special,
        Compensatory
    }
}