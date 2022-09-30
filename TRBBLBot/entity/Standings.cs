using System.Collections.Generic;

namespace TRBBLBot.entity {
    public class Standings {
        private List<StandingsTeam> teams = new List<StandingsTeam>();

        public List<StandingsTeam> Teams {
            get => teams;
            set => teams = value;
        }

        public StandingsTeam CreateTeam(string teamName, string coachName) {
            var newTeam = new StandingsTeam();
            newTeam.Name = teamName;
            newTeam.Coach = coachName;
            teams.Add(newTeam);
            return newTeam;
        }
    }

    public class StandingsTeam {
        private string coach;
        private string name;
        private int score = 0;
        private int scoreAgainst = 0;
        private int wins = 0;
        private int loses = 0;
        private int draws = 0;
        public int Games {
            get => wins + loses + draws;
        }
        public int Points {
            get {
                return (wins * 3) + draws;
            }

        }
        public int Score {
            get => score;
            set => score = value;
        }
        public int ScoreAgainst {
            get => scoreAgainst;
            set => scoreAgainst = value;
        }

        public int TDD {
            get => score - scoreAgainst;
        }
        public int Wins {
            get => wins;
            set => wins = value;
        }
        public int Loses {
            get => loses;
            set => loses = value;
        }
        public int Draws {
            get => draws;
            set => draws = value;
        }
        public string Name {
            get => name;
            set => name = value;
        }
        public string Coach {
            get => coach;
            set => coach = value;
        }
    }
}
