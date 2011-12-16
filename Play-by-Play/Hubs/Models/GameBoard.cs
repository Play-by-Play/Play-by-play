using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Models;

namespace Play_by_Play.Hubs.Models {
	public class GameBoard {
		public List<GameArea> Areas { get; set; }

		public Player HomeGoalie { get; set; }
		public Player AwayGoalie { get; set; }

		public Player HomeFaceoff { get; set; }
		public Player AwayFaceoff { get; set; }

		public IEnumerable<Player> HomePlayers {
			get {
				return Areas.SelectMany(area => area.HomePlayers);
			}
		}

		public IEnumerable<Player> AwayPlayers {
			get {
				return Areas.SelectMany(area => area.HomePlayers);
			}
		}

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
			foreach (var area in Areas) {
				area.Opposite = Areas.Single(x => x.X == 1 - area.X && x.Y == 3 - area.Y);
			}
		}

		public void PlacePlayer(Player player, int x, int y, bool isHome) {
			if (!isHome) {
				x = 1 - x;
				y = 3 - y;
			}
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

		public List<BattleResult> ExecuteTactic(TacticCard tacticCard, bool homePlayerAttacks) {
			if (homePlayerAttacks) {
				//Reverse tactic
				tacticCard = tacticCard.Reverse();
			}
			var battles = new List<BattleResult>();
			var area = Areas.Single(x => x.X == tacticCard.StartNode.X && x.Y == tacticCard.StartNode.Y);
			battles.Add(new BattleResult(area.HomePlayers, area.AwayPlayers){Type = "Scramble"});
			

			foreach (var pass in tacticCard.Passes) {
				var node = pass.End;
				area = Areas.Single(x => x.X == node.X && x.Y == node.Y);

				var homePlayers = new List<Player>(area.HomePlayers);
				var awayPlayers = new List<Player>(area.AwayPlayers);

				// Is there any player movin to that area?
				var movement = tacticCard.Movements.SingleOrDefault(x => x.End.X == node.X && x.End.Y == node.Y);
				if (movement != null) {
					var prevArea = Areas.Single(x => x.X == movement.Start.X && x.Y == movement.Start.Y);
					if(homePlayerAttacks)
						homePlayers.Add(prevArea.HomePlayers.First());
					else
						awayPlayers.Add(prevArea.AwayPlayers.First());
				}

				battles.Add(new BattleResult(homePlayers, awayPlayers) {
					Area = area
				});
			}

			//Shot
			area = Areas.Single(x => x.X == tacticCard.Shot.X && x.Y == tacticCard.Shot.Y);
			var shooter = homePlayerAttacks
			              	? area.HomePlayers.First()
			              	: area.AwayPlayers.First();
			var goalie = homePlayerAttacks
			             	? AwayGoalie
			             	: HomeGoalie;

			battles.Add(new BattleResult(new List<Player> {shooter}, new List<Player> {goalie}){Type = "Shot"});

			return battles;
		}
	}
}