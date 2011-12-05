using System.Collections.Generic;
using System.Linq;

namespace Play_by_Play.Hubs.Models {
	public class GameBoard {
		public List<GameArea> Areas { get; set; }

		public Player HomeGoalie { get; set; }
		public Player AwayGoalie { get; set; }

		public Player HomeFaceoff { get; set; }
		public Player AwayFaceoff { get; set; }

		public GameBoard() {
			Areas = new List<GameArea>();
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 2; j++) {
					Areas.Add(new GameArea {
						X = j,
						Y = i
					});
				}
			}
		}

		public void PlacePlayer(Player player, int x, int y, bool isHome) {
			var area = GetArea(x, y);
			if(isHome)
				area.HomePlayers.Add(player);
			else
				area.AwayPlayers.Add(player);
		}

		public GameArea GetArea(int x, int y) {
			return Areas.Single(area => area.X == x && area.Y == y);
		}

		public bool IsReadyForFaceoff() {
			return HomeFaceoff != null &&
			       HomeGoalie != null &&
			       AwayFaceoff != null &&
			       AwayGoalie != null;
		}
	}
}