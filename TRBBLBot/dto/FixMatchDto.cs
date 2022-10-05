namespace TRBBLBot.dto {
    public class FixMatchDto {
        string competition;
        string scheduleId;
        public string Competition {
            get => competition;
            set => competition = value;
        }
        public string ScheduleId {
            get => scheduleId;
            set => scheduleId = value;
        }
    }
}
