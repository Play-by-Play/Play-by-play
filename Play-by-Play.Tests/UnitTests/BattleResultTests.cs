using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Hubs.Models;
using Play_by_Play.Tests.Fakes;
using Play_by_Play.Tests.Helpers;
using Should;
using Xunit;
using System;

namespace Play_by_Play.Tests.UnitTests{
	public class BattleResultTests {
		[Fact]
		public void HomePlayersTotal_WhenFaceOff_OnlyOffensiveAttributesAreUsed() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.FaceOff, true);

			result.HomePlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void HomePlayersTotal_WhenHomeAttacksAndOnePlayer_ItIsOffensiveAttribute() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.HomePlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void HomePlayersTotal_WhenHomeAttacksAndManyPlayers_ItIsSumOfOffentiveAttributes() {
			var homePlayers = Factory.GetPlayers(2);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.HomePlayersTotal.ShouldEqual(3 * 2);
		}

		[Fact]
		public void HomePlayersTotal_WhenAwayAttacksAndOnePlayer_ItIsDefensiveAttribute() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false);

			result.HomePlayersTotal.ShouldEqual(2);
		}

		[Fact]
		public void HomePlayersTotal_WhenAwayAttacksAndManyPlayers_ItIsSumOfDefensiveAttributes() {
			var homePlayers = Factory.GetPlayers(2);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false);

			result.HomePlayersTotal.ShouldEqual(2 * 2);
		}

		[Fact]
		public void HomePlayersTotal_WhenHomeShoots_ItIsOffensiveAttribute() {
			var shooter = Factory.GetPlayers(1);
			var goalie = Factory.GetPlayers(1, "G");
			var area = Factory.GetArea(0, 0);

			var result = new BattleResult(shooter, goalie, BattleType.Shot, area, true);

			result.HomePlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void HomePlayersTotal_WhenAwayShootsFromLeft_ItIsGoaliesOffensiveAttribute() {
			var homePlayers = Factory.GetPlayers(1, "G");
			var awayPlayers = Factory.GetPlayers(1);
			var area = Factory.GetArea(1, 3);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Shot, area, false);

			result.HomePlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void HomePlayersTotal_WhenAwayShootsFromRight_ItIsGoaliesDefensiveAttribute() {
			var homePlayers = Factory.GetPlayers(1, "G");
			var awayPlayers = Factory.GetPlayers(1);
			var area = Factory.GetArea(0, 3);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Shot, area, false);

			result.HomePlayersTotal.ShouldEqual(2);
		}

		[Fact]
		public void AwayPlayersTotal_WhenFaceOff_ItIsOffensiveAttribute() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.FaceOff, true);

			result.AwayPlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void AwayPlayersTotal_WhenHomeAttacksAndOnePlayer_ItIsDefensiveAttribute() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.AwayPlayersTotal.ShouldEqual(2);
		}

		[Fact]
		public void AwayPlayersTotal_WhenHomeAttacksAndManyPlayers_ItIsSumOfDefensiveAttributes() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(2);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.AwayPlayersTotal.ShouldEqual(2 * 2);
		}

		[Fact]
		public void AwayPlayersTotal_WhenAwayAttacksAndOnePlayer_ItIsOffensiveAttribute() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false);

			result.AwayPlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void AwayPlayersTotal_WhenAwayAttacksAndManyPlayers_ItIsSumOfOffensiveAttribute() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(2);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false);

			result.AwayPlayersTotal.ShouldEqual(3 * 2);
		}

		[Fact]
		public void AwayPlayersTotal_WhenAwayShoots_ItIsOffensiveAttribute() {
			var shooter = Factory.GetPlayers(1);
			var goalie = Factory.GetPlayers(1, "G");
			var area = Factory.GetArea(0, 0);

			var result = new BattleResult(goalie, shooter, BattleType.Shot, area, false);

			result.AwayPlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void AwayPlayersTotal_WhenHomeShootsFromLeft_ItItGoaliesOffensiveAttribute() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1, "G");
			var area = Factory.GetArea(0, 0);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Shot, area, true);

			result.AwayPlayersTotal.ShouldEqual(3);
		}

		[Fact]
		public void AwayPlayersTotal_WhenHomeShootsFromRight_ItIsGoaliesDefensiceAttribute() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1, "G");
			var area = Factory.GetArea(1, 0);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Shot, area, true);

			result.AwayPlayersTotal.ShouldEqual(2);
		}

		[Fact]
		public void AwayTotal_WhenFaceOff_ItIsOffensiveAttribute() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.FaceOff, true, new FakeGenerator());

			result.AwayTotal.ShouldEqual(6);
		}

		[Fact]
		public void AwayTotal_WhenHomeAttacksAndOnePlayer_ItIsSumOfDefensiveAttributeAndModifier() {
			var homePlayers = Factory.GetPlayers(1);
			var awayPlayers = Factory.GetPlayers(1);

			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true, new FakeGenerator());

			result.AwayTotal.ShouldEqual(5);
		}

		public class AwayTotal_Tests {

			[Fact]
			public void AwayTotal_AwayTotal_WhenHomeTeamAttacks_DefensiveEqualsAllAwayPlayers() {
				var homePlayers = Factory.GetPlayers(1);
				var awayPlayers = Factory.GetPlayers(2);

				var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true, new FakeGenerator());

				result.AwayTotal.ShouldEqual(7);
			}

			[Fact]
			public void AwayTotal_OffensiveEqualsOffensivePlayerAttributeWhenAwayTeamAttacks() {
				var homePlayers = Factory.GetPlayers(1);
				var awayPlayers = Factory.GetPlayers(1);

				var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false, new FakeGenerator());

				result.AwayTotal.ShouldEqual(6);
			}

			[Fact]
			public void AwayTotal_OffensiveEqualsAllOffensivePlayersAttributeWhenAwayTeamAttacks() {
				var homePlayers = Factory.GetPlayers(1);
				var awayPlayers = Factory.GetPlayers(2);

				var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false, new FakeGenerator());

				result.AwayTotal.ShouldEqual(9);
			}

			[Fact]
			public void AwayTotal_AwayPlayerShotIsCorrect() {
				var shooter = Factory.GetPlayers(1);
				var goalie = Factory.GetPlayers(1, "G");
				var area = Factory.GetArea(1, 3);

				var result = new BattleResult(goalie, shooter, BattleType.Shot, area, false, new FakeGenerator());

				result.AwayTotal.ShouldEqual(6);
			}
		}

		public class IsHomeAttackingTests {
			[Fact]
			public void GetHomeResultHasCorrectValue() {
				var homePlayers = Factory.GetPlayers(2);
				var awayPlayers = Factory.GetPlayers(1);

				var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false).GetHomeResult();

				result.IsHomeAttacking.ShouldBeFalse();
			}

			[Fact]
			public void GetAwayResultHasCorrectValue() {
				var homePlayers = Factory.GetPlayers(1);
				var awayPlayers = Factory.GetPlayers(1);

				var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, false).GetAwayResult();

				result.IsHomeAttacking.ShouldBeFalse();
			}
		}

		public class ClearTests {
			[Fact]
			public void BonusIsNotResettedWhenGameBoardIsCleared() {
				var home = Factory.GetPlayers(1, "LW").First();
				home.Bonus = Bonus.Offense;
				var away = Factory.GetPlayers(1);
				var result = new BattleResult(new List<Player> {home}, away, BattleType.Scramble, true);

				home.Bonus = Bonus.None;

				result.HomePlayers.First().Bonus.ShouldEqual(Bonus.Offense);
			}
		}
	}
}