using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using TRBBLBot.entity;

namespace TRBBLBot.service {
    public class ScheduleService {

        public List<Schedule> convertToSchedules(JToken token) {
            var schedules = new List<Schedule>();

            var idmatch = (int)token["cols"]["idmatch"];
            var idcompetition = (int)token["cols"]["idcompetition"];
            var round = (int)token["cols"]["round"];
            var idorigin = (int)token["cols"]["idorigin"];
            var idteam_home = (int)token["cols"]["idteam_home"];
            var team_name_home = (int)token["cols"]["team_name_home"];
            var score_home = (int)token["cols"]["score_home"];
            var idrace_home = (int)token["cols"]["idrace_home"];
            var logo_home = (int)token["cols"]["logo_home"];
            var idteam_away = (int)token["cols"]["idteam_away"];
            var team_name_away = (int)token["cols"]["team_name_away"];
            var idrace_away = (int)token["cols"]["idrace_away"];
            var score_away = (int)token["cols"]["score_away"];
            var logo_away = (int)token["cols"]["logo_away"];
            var idcoach_home = (int)token["cols"]["idcoach_home"];
            var coach_name_home = (int)token["cols"]["coach_name_home"];
            var idcoach_away = (int)token["cols"]["idcoach_away"];
            var coach_name_away = (int)token["cols"]["coach_name_away"];
            var schedule_origin_id = (int)token["cols"]["schedule_origin_id"];
            var userdate = (int)token["cols"]["userdate"];
            var userdate_changed = (int)token["cols"]["userdate_changed"];

            //Convert to Objects
            foreach(var row in token["rows"]) {
                var values = row.Values<string>().ToList();
                var schedule = new Schedule();
                schedule.IdMatch = values[idmatch];
                schedule.IdCompetition = values[idcompetition];
                schedule.Round = values[round];
                schedule.IdOrigin = values[idorigin];
                schedule.IdTeamHome = values[idteam_home];
                schedule.TeamNameHome = values[team_name_home];
                schedule.ScoreHome = values[score_home];
                schedule.IdRaceHome = values[idrace_home];
                schedule.LogoHome = values[logo_home];
                schedule.IdTeamAway = values[idteam_away];
                schedule.TeamNameAway = values[team_name_away];
                schedule.IdRaceAway = values[idrace_away];
                schedule.ScoreAway = values[score_away];
                schedule.LogoAway = values[logo_away];
                schedule.IdCoachHome = values[idcoach_home];
                schedule.CoachNameHome = values[coach_name_home];
                schedule.IdCoachAway = values[idcoach_away];
                schedule.CoachNameAway = values[coach_name_away];
                schedule.ScheduleOriginId = values[schedule_origin_id];
                schedule.UserDate = values[userdate];
                schedule.UserDateChanged = values[userdate_changed];

                schedules.Add(schedule);
            }
            return schedules;
        }

        public List<Schedule> filterCurrentSeason(List<Schedule> allSchedules) {
            var filteredSchedules = new List<Schedule>();

            allSchedules = allSchedules.OrderByDescending(s => s.ScheduleOriginId).ToList();

            var currentId = int.Parse(allSchedules.First().ScheduleOriginId);
            foreach(var schedule in allSchedules) {
                if(int.Parse(schedule.ScheduleOriginId) == currentId) {
                    filteredSchedules.Add(schedule);
                    currentId--;
                } else {
                    break;
                }
            }
            return filteredSchedules;
        }
    }
}
