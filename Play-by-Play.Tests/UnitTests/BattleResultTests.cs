using System.Collections.Generic;
using Play_by_Play.Hubs.Models;
using Should;
using Xunit;

namespace Play_by_Play.Tests.UnitTests {
	public class BattleResultTests {

		[Fact]
		public void FaceOffUsesOnlyOffensiveAttributes() {
			var homePlayers = GetPlayers(1);
			var awayPlayers = GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.FaceOff, true);

			result.HomePlayersTotal.ShouldEqual(3);
			result.AwayPlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void OffensiveEqualsOffensivePlayerAttribute() {
			var homePlayers = GetPlayers(1);
			var awayPlayers = GetPlayers(1);
			
			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.HomePlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void DefensiveEqualsDefensivePlayerAttribute() {
			var homePlayers = GetPlayers(1);
			var awayPlayers = GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.AwayPlayersTotal.ShouldEqual(2);
		}

		[Fact]
		public void OffensiveEqualsAllOffensivePlayersAttribute() {
			var homePlayers = GetPlayers(2);
			var awayPlayers = GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.HomePlayersTotal.ShouldEqual(3 * 2);
		}

		[Fact]
		public void DefensiveEqualsAllDefensivePlayersAttribute() {
			var homePlayers = GetPlayers(1);
			var awayPlayers = GetPlayers(2);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.AwayPlayersTotal.ShouldEqual(2 * 2);
		}

		private static List<Player> GetPlayers(int nr) {
			var players = new List<Player>();
			for (int i = 0; i < nr; i++) {
				players.Add(new Player {
					Defense = 2,
					Offense = 3,
					Position = Position.C
				});
			}
			return players;
		}
	}
}