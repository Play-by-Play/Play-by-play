using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Hubs.Models;
using Play_by_Play.Tests.Helpers;
using Should;
using Xunit;
using System;

namespace Play_by_Play.Tests.UnitTests.BattleResultTests {

	public class HomePlayersTotalTests {
		[Fact]
		public void FaceOffUsesOnlyOffensiveAttributes() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.FaceOff, true);

			result.HomePlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void OffensiveEqualsOffensivePlayerAttributeWhenHomeTeamAttacks() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.HomePlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void OffensiveEqualsAllOffensivePlayersAttributeWhenHomeTeamAttacks() {
			var homePlayers = Factory.GetPlayers(2);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.HomePlayersTotal.ShouldEqual(3 * 2);
		}

		[Fact]
		public void DefensiveEqualsDefensivePlayerAttributeWhenAwayTeamAttacks() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false);

			result.HomePlayersTotal.ShouldEqual(2);
		}

		[Fact]
		public void DefensiveEqualsAllDefensivePlayersAttributeWhenAwayTeamAttacks() {
			var homePlayers = Factory.GetPlayers(2);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false);

			result.HomePlayersTotal.ShouldEqual(2 * 2);
		}

		[Fact]
		public void GetHomeResultHasCorrectValues() {
			var homePlayers = Factory.GetPlayers(2);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false).GetHomeResult();

			result.HomePlayersTotal.ShouldEqual(2 * 2);
		}

		[Fact]
		public void HomePlayerShotIsCorrect() {
			var shooter = Factory.GetPlayers(1);
			var goalie = Factory.GetPlayers(1, "G");

			var result = new BattleResult(shooter, goalie, BattleType.Shot, true);

			result.HomePlayersTotal.ShouldEqual(3);
		}
	}

	public class AwayPlayersTotalTest {
		[Fact]
		public void FaceOffUsesOnlyOffensiveAttributes() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.FaceOff, true);

			result.AwayPlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void DefensiveEqualsDefensivePlayerAttributeWhenHomeTeamAttacks() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.AwayPlayersTotal.ShouldEqual(2);
		}

		[Fact]
		public void DefensiveEqualsAllDefensivePlayersAttributeWhenHomeTeamAttacks() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(2);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.AwayPlayersTotal.ShouldEqual(2 * 2);
		}

		[Fact]
		public void OffensiveEqualsOffensivePlayerAttributeWhenAwayTeamAttacks() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false);

			result.AwayPlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void OffensiveEqualsAllOffensivePlayersAttributeWhenAwayTeamAttacks() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(2);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false);

			result.AwayPlayersTotal.ShouldEqual(3 * 2);
		}

		[Fact]
		public void AwayPlayerShotIsCorrect() {
			var shooter = Factory.GetPlayers(1);
			var goalie = Factory.GetPlayers(1, "G");

			var result = new BattleResult(goalie, shooter, BattleType.Shot, false);

			result.AwayPlayersTotal.ShouldEqual(3);
		}
	}

	public class IsHomeAttackingTests {
		[Fact]
		public void GetHomeResultHasCorrectValues() {
			var homePlayers = Factory.GetPlayers(2);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false).GetHomeResult();

			result.IsHomeAttacking.ShouldBeFalse();
		}
	}
}