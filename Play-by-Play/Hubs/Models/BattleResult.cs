using System;
using System.Collections.Generic;
using System.Linq;

namespace Play_by_Play.Hubs.Models {
	public class BattleResult {
		private BattleResult() {}
		public BattleResult(List<Player> homePlayers, List<Player> awayPlayers, string type, bool homeAttack) {
			HomePlayers = homePlayers;
			AwayPlayers = awayPlayers;
			Type = type;
			IsHomeAttacking = homeAttack;

			Execute();
			IsHomeWinner = IsHomeWinning();
		}

		public List<Player> HomePlayers { get; set; }
		public List<Player> AwayPlayers { get; set; }
		public int HomeModifier { get; set; }
		public int AwayModifier { get; set; }
		public bool IsHomePlayer { get; set; }
		public bool IsHomeAttacking { private get; set; }
		public bool IsHomeWinner { get; private set; }
		public GameArea Area { get; set; }
		public string Type { get; set; }

		public int HomeTotal { 
			get {
				return HomeModifier != 1
				       	? TotalAttributes(HomePlayers) + HomeModifier
				       	: 0;
			}
		}
		public int AwayTotal {
			get {
				return AwayModifier != 1
				       	? TotalAttributes(AwayPlayers) + AwayModifier
				       	: 0;
			}
		}

		public bool Success {
			get { return (IsHomePlayer && IsHomeWinner) || (!IsHomePlayer && !IsHomeWinner); }
		}

		private int TotalAttributes(IEnumerable<Player> players) {
			var sum = 0;
			if (Type.Equals(BattleType.FaceOff)) {
				sum = players.Sum(x => x.Offense);
			} else if (Type.Equals(BattleType.Scramble) || Type.Equals(BattleType.Pass)) {
				sum = players.Sum(x => IsHomeAttacking ? x.Offense : x.Defense);
			} else if (Type.Equals(BattleType.Shot)) {
				sum = IsHomePlayer
				      	? (IsHomeAttacking
				      	   	? players.Sum(x => x.Offense)
				      	   	: players.Sum(x => x.Defense))
				      	: (IsHomeAttacking
				      	   	? players.Sum(x => x.Defense)
				      	   	: players.Sum(x => x.Offense));
			}

			return sum;
		}

		private void Execute() {
			var rnd = new Random();
			var homeModifier = rnd.Next(1, 6);
			var awayModifier = rnd.Next(1, 6);

			HomeModifier = homeModifier;
			AwayModifier = awayModifier;
		}

		public BattleResult GetHomeResult() {
			var result = new BattleResult {
				HomeModifier = HomeModifier,
				HomePlayers = HomePlayers,
				IsHomePlayer = true,
				AwayModifier = AwayModifier,
				AwayPlayers = AwayPlayers,
				Area = Area,
				Type = Type
			};
			return result;
		}

		public BattleResult GetAwayResult() {
			var result = new BattleResult {
				HomeModifier = HomeModifier,
				HomePlayers = HomePlayers,
				IsHomePlayer = false,
				AwayModifier = AwayModifier,
				AwayPlayers = AwayPlayers,
				Area = Area != null ? Area.Opposite : null,
				Type = Type
			};
			return result;
		}

		private bool IsHomeWinning() {
			if (HomeTotal != AwayTotal) 
				return HomeTotal > AwayTotal;

			// Face-off is tied
			var homeAttributes = TotalAttributes(HomePlayers);
			var awayAttributes = TotalAttributes(AwayPlayers);
			if (homeAttributes == awayAttributes) {
				// Same attribute total, execute faceoff again
				Execute();
				return IsHomeWinning();
			}
			return homeAttributes > awayAttributes;
		}
	}
}