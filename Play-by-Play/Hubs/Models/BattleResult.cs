using System;
using System.Collections.Generic;
using System.Linq;

namespace Play_by_Play.Hubs.Models {
	public class BattleResult {
		private BattleResult() {}
		public BattleResult(List<Player> homePlayers, List<Player> awayPlayers ) {
			HomePlayers = homePlayers;
			AwayPlayers = awayPlayers;

			Execute();
		}

		public List<Player> HomePlayers { get; set; }
		public List<Player> AwayPlayers { get; set; }
		public int HomeModifier { get; set; }
		public int AwayModifier { get; set; }
		public bool IsHomePlayer { get; set; }

		public int HomeTotal { 
			get {
				if (HomeModifier == 1)
					return 0;

				var playerSum = HomePlayers.Sum(x => x.Offense);
				return playerSum + HomeModifier;
			}
		}
		public int AwayTotal {
			get {
				if (AwayModifier == 1)
					return 0;

				var playerSum = AwayPlayers.Sum(x => x.Offense);
				return playerSum + AwayModifier;
			}
		}

		private void Execute() {
			var rnd = new Random();
			var homeModifier = rnd.Next(1, 6);
			var awayModifier = rnd.Next(1, 6);

			if (HomeTotal == AwayTotal && homeModifier == awayModifier) {
				Execute();
				return;
			}

			HomeModifier = homeModifier;
			AwayModifier = awayModifier;
		}

		public BattleResult GetHomeResult() {
			var result = new BattleResult {
				HomeModifier = HomeModifier,
				HomePlayers = HomePlayers,
				IsHomePlayer = true,
				AwayModifier = AwayModifier,
				AwayPlayers = AwayPlayers
			};
			return result;
		}

		public BattleResult GetAwayResult() {
			var result = new BattleResult {
				HomeModifier = HomeModifier,
				HomePlayers = HomePlayers,
				IsHomePlayer = false,
				AwayModifier = AwayModifier,
				AwayPlayers = AwayPlayers
			};
			return result;
		}
	}
}