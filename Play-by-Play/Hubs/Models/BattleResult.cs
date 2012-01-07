using System;
using System.Collections.Generic;
using System.Linq;

namespace Play_by_Play.Hubs.Models {
	public class BattleResult {
		private readonly RandomGenerator _generator;
		private BattleResult() { }

		public BattleResult(List<Player> homePlayers, List<Player> awayPlayers, string type, bool homeAttack)
			: this(homePlayers, awayPlayers, type, homeAttack, new RandomGenerator()) { }
		public BattleResult(List<Player> homePlayers, List<Player> awayPlayers, string type, bool homeAttack, RandomGenerator generator) {
			HomePlayers = homePlayers.Select(player => player.Clone()).ToList();
			AwayPlayers = awayPlayers.Select(player => player.Clone()).ToList();
			Type = type;
			IsHomeAttacking = homeAttack;
			_generator = generator;
			Execute();
		}

		public BattleResult GetHomeResult() {
			var result = new BattleResult {
				HomeModifier = HomeModifier,
				HomePlayers = HomePlayers,
				HomePlayersTotal = HomePlayersTotal,
				HomeTotal = HomeTotal,
				IsHomePlayer = true,
				AwayModifier = AwayModifier,
				AwayPlayers = AwayPlayers,
				AwayPlayersTotal = AwayPlayersTotal,
				AwayTotal = AwayTotal,
				IsHomeAttacking = IsHomeAttacking,
				Area = Area,
				Type = Type,
				Success = IsHomeAttacking ? IsHomeWinner : !IsHomeWinner
			};
			return result;
		}

		public BattleResult GetAwayResult() {
			var result = new BattleResult {
				HomeModifier = HomeModifier,
				HomePlayers = HomePlayers,
				HomePlayersTotal = HomePlayersTotal,
				HomeTotal = HomeTotal,
				IsHomePlayer = false,
				AwayModifier = AwayModifier,
				AwayPlayers = AwayPlayers,
				AwayPlayersTotal = AwayPlayersTotal,
				AwayTotal = AwayTotal,
				IsHomeAttacking = IsHomeAttacking,
				Area = Area != null ? Area.Opposite : null,
				Type = Type,
				Success = IsHomeAttacking ? !IsHomeWinner : IsHomeWinner
			};
			return result;
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
		public int HomePlayersTotal { get; private set; }
		public int AwayPlayersTotal { get; private set; }

		public int HomeTotal { get; private set; }
		public int AwayTotal { get; private set; }

		public bool Success { get; private set; }

		private void Execute() {
			HomePlayersTotal = TotalAttributes(HomePlayers, IsHomeAttacking);
			AwayPlayersTotal = TotalAttributes(AwayPlayers, !IsHomeAttacking);
			
			do{
				HomeModifier = _generator.Next(1, 6);
				AwayModifier = _generator.Next(1, 6);

				HomeTotal = HomeModifier != 1 && HomePlayersTotal != 0 || AwayPlayers.Count == 0
					? HomePlayersTotal + HomeModifier + HomePlayers.Count(player => player.Bonus == (IsHomeAttacking ? Bonus.Offense : Bonus.Defense)) + (IsHomeWinner ? 1 : 0)
									: 0;
				AwayTotal = AwayModifier != 1 && AwayPlayersTotal != 0 || HomePlayers.Count == 0
									? AwayPlayersTotal + AwayModifier + AwayPlayers.Count(player => player.Bonus == (!IsHomeAttacking ? Bonus.Offense : Bonus.Defense)) + (!IsHomeWinner ? 1 : 0)
									: 0;
			} while(HomeTotal == AwayTotal);

			IsHomeWinner = HomeTotal > AwayTotal;
			Success = IsHomeAttacking ? IsHomeWinner : !IsHomeWinner;
		}

		private int TotalAttributes(IEnumerable<Player> players, bool isAttacking) {
			var sum = 0;
			if (Type.Equals(BattleType.FaceOff)) {
				sum = players.Sum(x => x.Offense);
			} else if (Type.Equals(BattleType.Scramble) || Type.Equals(BattleType.Pass)) {
				sum = players.Sum(x => isAttacking ? x.Offense : x.Defense);
			} else if (Type.Equals(BattleType.Shot)) {
				sum = isAttacking
								? players.Sum(x => x.Offense)
								: players.Sum(x => x.Defense);
			}

			return sum;
		}
	}
}