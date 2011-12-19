using System;
using System.Collections.Generic;
using System.Linq;

namespace Play_by_Play.Hubs.Models {
	public class BattleResult {
		private RandomGenerator _generator;
		private BattleResult() {}

		public BattleResult(List<Player> homePlayers, List<Player> awayPlayers, string type, bool homeAttack)
			: this(homePlayers, awayPlayers, type, homeAttack, new RandomGenerator()){}
		public BattleResult(List<Player> homePlayers, List<Player> awayPlayers, string type, bool homeAttack, RandomGenerator generator) {
			HomePlayers = homePlayers;
			AwayPlayers = awayPlayers;
			Type = type;
			IsHomeAttacking = homeAttack;
			_generator = generator;
			var succeeded = true;
			do {
				Execute();
				try {
					IsHomeWinner = IsHomeWinning();
				} catch(Exception) {
					succeeded = false;
				}
			} while (!succeeded);
		}

		public List<Player> HomePlayers { get; set; }
		public List<Player> AwayPlayers { get; set; }
		public int HomeModifier { get; set; }
		public int AwayModifier { get; set; }
		public bool IsHomePlayer { get; set; }
		public bool IsHomeAttacking { get; set; }
		public bool IsHomeWinner { get; private set; }
		public GameArea Area { get; set; }
		public string Type { get; set; }
		public int HomePlayersTotal { get { return TotalAttributes(HomePlayers, IsHomeAttacking); } }
		public int AwayPlayersTotal { get { return TotalAttributes(AwayPlayers, !IsHomeAttacking); } }

		public int HomeTotal { 
			get {
				var totalAttributes = TotalAttributes(HomePlayers, IsHomeAttacking);
				return HomeModifier != 1 && totalAttributes != 0
					? totalAttributes + HomeModifier + (IsHomeWinner ? 1 : 0)
				       	: 0;
			}
		}
		public int AwayTotal {
			get {
				var totalAttributes = TotalAttributes(AwayPlayers, !IsHomeAttacking);
				return AwayModifier != 1 && totalAttributes != 0
								? totalAttributes + AwayModifier + (!IsHomeWinner ? 1 : 0)
				       	: 0;
			}
		}

		public bool Success {
			get { return (IsHomeAttacking && IsHomeWinner) || (!IsHomeAttacking && !IsHomeWinner); }
		}

		private int TotalAttributes(IEnumerable<Player> players, bool isAttacking) {
			var sum = 0;
			if (Type.Equals(BattleType.FaceOff)) {
				sum = players.Sum(x => x.Offense);
			} else if (Type.Equals(BattleType.Scramble) || Type.Equals(BattleType.Pass)) {
				sum = players.Sum(x => isAttacking ? x.Offense : x.Defense);
			} else if (Type.Equals(BattleType.Shot)) {
				sum = IsHomeAttacking
								? (isAttacking
				      	   	? players.Sum(x => x.Offense)
				      	   	: players.Sum(x => x.Defense))
								: (isAttacking
				      	   	? players.Sum(x => x.Defense)
				      	   	: players.Sum(x => x.Offense));
			}

			return sum;
		}

		private void Execute() {
			var homeModifier = _generator.Next(1, 6);
			var awayModifier = _generator.Next(1, 6);

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
				IsHomeAttacking = IsHomeAttacking,
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
				IsHomeAttacking = IsHomeAttacking,
				Area = Area != null ? Area.Opposite : null,
				Type = Type
			};
			return result;
		}

		private bool IsHomeWinning() {
			if (HomeTotal != AwayTotal) 
				return HomeTotal > AwayTotal;

			// Face-off is tied
			var homeAttributes = TotalAttributes(HomePlayers, IsHomeAttacking);
			var awayAttributes = TotalAttributes(AwayPlayers, !IsHomeAttacking);
			if (homeAttributes == awayAttributes) {
				// Same attribute total, execute faceoff again
				throw new Exception();
			}
			return homeAttributes > awayAttributes;
		}
	}

	public class RandomGenerator {
		private static readonly Random Rnd = new Random();
		public int Next(int min, int max) {
			return Rnd.Next(min, max);
		}
	}
}