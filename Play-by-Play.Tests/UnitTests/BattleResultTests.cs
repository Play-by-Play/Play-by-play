using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Hubs.Models;
using Should;
using Xunit;
using System;

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
		public void OffensiveEqualsOffensivePlayerAttributeWhenHomeTeamAttacks() {
			var homePlayers = GetPlayers(1);
			var awayPlayers = GetPlayers(1);
			
			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.HomePlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void DefensiveEqualsDefensivePlayerAttributeWhenHomeTeamAttacks() {
			var homePlayers = GetPlayers(1);
			var awayPlayers = GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.AwayPlayersTotal.ShouldEqual(2);
		}

		[Fact]
		public void OffensiveEqualsAllOffensivePlayersAttributeWhenHomeTeamAttacks() {
			var homePlayers = GetPlayers(2);
			var awayPlayers = GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.HomePlayersTotal.ShouldEqual(3 * 2);
		}

		[Fact]
		public void DefensiveEqualsAllDefensivePlayersAttributeWhenHomeTeamAttacks() {
			var homePlayers = GetPlayers(1);
			var awayPlayers = GetPlayers(2);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.AwayPlayersTotal.ShouldEqual(2 * 2);
		}

		[Fact]
		public void OffensiveEqualsOffensivePlayerAttributeWhenAwayTeamAttacks() {
			var homePlayers = GetPlayers(1);
			var awayPlayers = GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false);

			result.AwayPlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void DefensiveEqualsDefensivePlayerAttributeWhenAwayTeamAttacks() {
			var homePlayers = GetPlayers(1);
			var awayPlayers = GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false);

			result.HomePlayersTotal.ShouldEqual(2);
		}

		[Fact]
		public void OffensiveEqualsAllOffensivePlayersAttributeWhenAwayTeamAttacks() {
			var homePlayers = GetPlayers(1);
			var awayPlayers = GetPlayers(2);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false);

			result.AwayPlayersTotal.ShouldEqual(3 * 2);
		}

		[Fact]
		public void DefensiveEqualsAllDefensivePlayersAttributeWhenAwayTeamAttacks() {
			var homePlayers = GetPlayers(2);
			var awayPlayers = GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false);

			result.HomePlayersTotal.ShouldEqual(2 * 2);
		}

		[Fact]
		public void GetHomeResultHasCorrectValues() {
			var homePlayers = GetPlayers(2);
			var awayPlayers = GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false).GetHomeResult();

			result.HomePlayersTotal.ShouldEqual(2 * 2);
			result.IsHomeAttacking.ShouldBeFalse();
		}

		[Fact]
		public void HomePlayerShotIsCorrect() {
			var shooter = GetPlayers(1);
			var goalie = GetPlayers(1, "G");

			var result = new BattleResult(shooter, goalie, BattleType.Shot, true);

			result.HomePlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void AwayPlayerShotIsCorrect() {
			var shooter = GetPlayers(1);
			var goalie = GetPlayers(1, "G");

			var result = new BattleResult(goalie, shooter, BattleType.Shot, false);

			result.AwayPlayersTotal.ShouldEqual(3);
		}

		[Fact(Skip = "Loop freezes")]
		public void FaceOffIsNeverTied() {
			var home = GetPlayers(1);
			var away = GetPlayers(1);
			Console.WriteLine("Tests started");
			for (int i = 0; i < 10; i++) {
				Console.WriteLine(string.Format("Test {0} of 100", i + 1));
				var result = new BattleResult(home, away, BattleType.FaceOff, true);

				result.HomeTotal.ShouldNotEqual(result.AwayTotal);
			}
		}

		private static List<Player> GetPlayers(int nr, string position = "C") {
			var players = new List<Player>();
			for (int i = 0; i < nr; i++) {
				players.Add(new Player {
					Defense = 2,
					Offense = 3,
					Position = position
				});
			}
			return players;
		}
	}
}