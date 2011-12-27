namespace Play_by_Play.Hubs.Models {
	public class GameScore {
		public int Home { get; private set; }
		public int Away { get; private set; }

		public void HomeGoal() {
			Home++;
		}

		public void AwayGoal() {
			Away++;
		}
	}
}