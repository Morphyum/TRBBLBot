namespace TRBBLBot.service {
    public class WelcomeService {
        private string welcomeText = "First things firsts, please make sure you check out the<#572155952078258228> and the pinned messages in <#473044083330514944>. These will hopefully let you work out if it's the league for you. We're just heading into S7 and sorting out leagues at the moment.  If you're all happy with the rules etc... then you can find the sign up sheet in both <#473733975509041163> and pinned in <#753677775591702590>." +
                    "\nAs well as the league we also run side competitions.The main one is the perpetual ladder which is great for finding random games.They're just for fun but do allow you to play more between league games. Help on joining the ladder can be found at <#479662400640122890>. " +
                    "\nOther than that just look around and get involved.We've got plenty of opportunity to chat outside of just scheduling games so please feel free to get involved.If you've got any questions feel free to ask but make sure you include the <@&473036877524369459> tag so we know the question is there." +
                    "\n\nAlso check out the new coach primer pinned in this channel.https://docs.google.com/document/d/1";

        public string WelcomeText {
            get => welcomeText;
            set => welcomeText = value;
        }

        public void LoadWelcome() {
            welcomeText = System.IO.File.ReadAllText("Saves/Welcome.txt");
        }

        public void SaveWelcome() {
            System.IO.File.WriteAllText("Saves/Welcome.txt", welcomeText);
        }
    }
}
