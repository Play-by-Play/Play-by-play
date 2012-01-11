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
				return Areas.SelectMany(area => area.AwayPlayers);
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
				area.AddHomePlayer(player);
			else
				area.AddAwayPlayer(player);
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
			if (!homePlayerAttacks) {
				//Reverse tactic
				tacticCard = tacticCard.Reverse();
			}
			var battles = new List<BattleResult>();
			var area = Areas.Single(x => x.X == tacticCard.StartNode.X && x.Y == tacticCard.StartNode.Y);
			var result = new BattleResult(area.HomePlayers, area.AwayPlayers, BattleType.Scramble, area, homePlayerAttacks) {Area = area};
			if(result.Success)
				Movement(tacticCard, homePlayerAttacks, area, tacticCard.StartNode);
			battles.Add(result);
			if (!result.Success)
				return battles;

			foreach (var pass in tacticCard.Passes) {
				var node = pass.End;
				area = Areas.Single(x => x.X == node.X && x.Y == node.Y);

				var homePlayers = new List<Player>(area.HomePlayers);
				var awayPlayers = new List<Player>(area.AwayPlayers);

				result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, area, homePlayerAttacks) {
					Area = area
				};

				if (result.Success) {
					// Is there any player movin to that area?
					Movement(tacticCard, homePlayerAttacks, area, node);
				}
				battles.Add(result);
				if (!result.Success)
					return battles;
			}

			//Shot
			area = Areas.Single(x => x.X == tacticCard.Shot.X && x.Y == tacticCard.Shot.Y);

			if (homePlayerAttacks) {
				var shooter = area.HomePlayers.First();
				var goalie = AwayGoalie;

				var battleResult = new BattleResult(new List<Player> {shooter}, new List<Player> {goalie}, BattleType.Shot, area, true);
				battles.Add(battleResult);

			} else {
				var shooter = area.AwayPlayers.First();
				var goalie = HomeGoalie;

				var battleResult = new BattleResult(new List<Player> { goalie }, new List<Player> { shooter }, BattleType.Shot, area, false);
				battles.Add(battleResult);
			}
			return battles;
		}

		private void Movement(TacticCard tacticCard, bool homePlayerAttacks, GameArea area, Node node) {
			var movement = tacticCard.Movements.SingleOrDefault(x => x.Start.X == node.X && x.Start.Y == node.Y);
			if (movement != null) {
				var nextArea = Areas.Single(x => x.X == movement.End.X && x.Y == movement.End.Y);
				if (homePlayerAttacks) {
					var player = area.HomePlayers.First();
					area.HomePlayers.Remove(player);
					nextArea.AddHomePlayer(player);
				}
				else {
					var player = area.AwayPlayers.First();
					area.AwayPlayers.Remove(player);
					nextArea.AddAwayPlayer(player);
				}
			}
		}

		public void ClearBoard() {
			foreach (var area in Areas) {
				area.Clear();
			}

			
		}
	}
}