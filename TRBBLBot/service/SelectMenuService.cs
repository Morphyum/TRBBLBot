using Discord;
using TRBBLBot.dto;
using System.Linq;
using System.Threading.Tasks;

namespace TRBBLBot.service {
    public class SelectMenuService {
        private ScheduleService scheduleService;

        public ScheduleService ScheduleService {
            get => scheduleService;
            set => scheduleService = value;
        }

        public async Task<Modal> handleFixMatchAsync(FixMatchDto dto) {
            var schedule = await ScheduleService.getSchedulesAsync(dto.Competition);
            var match = schedule.Find(m => m.ScheduleOriginId.Equals(dto.ScheduleId));

            var mb = new ModalBuilder()
                     .WithTitle("Enter new score")
                     .WithCustomId("fix_match_modal")
                     .AddTextInput("ScheduleId(DO NOT CHANGE)", "schedule_id", TextInputStyle.Short, "new score", 0, 99, false, match.ScheduleOriginId)
                     .AddTextInput(match.TeamNameHome, "home_score", TextInputStyle.Short, "new score", 1, 2, true)
                     .AddTextInput(match.TeamNameAway, "away_score", TextInputStyle.Short, "new score", 1, 2, true);
            return mb.Build();
        }
    }
}
