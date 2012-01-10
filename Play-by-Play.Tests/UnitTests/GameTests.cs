using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Hubs.Models;
using Play_by_Play.Models;
using Play_by_Play.Tests.Helpers;
using Should;
using Xunit;

namespace Play_by_Play.Tests.UnitTests.GameTests {
	public class IsReadyForTactic {
		[Fact]
		public void GameIsReadyWhen5PlayersFromEachTeamIsPlaced() {
			var game = Factory.SetupGame();

			var result = game.IsReadyForTactic();

			result.ShouldBeTrue();
		}

		[Fact]
		public void GameIsNotReadyWhenSomePlayerIsMissing() {
			var game = Factory.SetupGame();
			var area = game.Board.GetArea(0, 2);
			area.HomePlayers.RemoveAt(0);

			var result = game.IsReadyForTactic();

			result.ShouldBeFalse();
		}

		[Fact]
		public void GameIsNotReadyWhenOneGoalieIsMissing() {
			var game = Factory.SetupGame();
			game.Board.HomeGoalie = null;

			var result = game.IsReadyForTactic();

			result.ShouldBeFalse();
		}
	}

	public class ChangeTurnTests {
		[Fact]
		public void ChangeInTurnAfterFirstPlay() {
			var game = Factory.SetupGame();
			var card = Factory.GetCard();

			game.CurrentTactic = card;
			game.ChangeTurn();

			game.IsHomeTurn.ShouldBeFalse();
		}

		[Fact]
		public void NoChangeInTurnAfterSecondPlay() {
			var game = Factory.SetupGame();
			var card = Factory.GetCard();

			for (var i = 0; i < 2; i++) {
				game.CurrentTactic = card;
				game.ChangeTurn();

				game.IsHomeTurn.ShouldBeFalse();
			}
		}

		[Fact]
		public void ChangeInTurnAfterThirdPlay() {
			var game = Factory.SetupGame();
			game.ChangeTurn();

			for (var i = 0; i < 2; i++) {
				game.IsHomeTurn.ShouldBeFalse();
				game.ChangeTurn();
			}

			game.IsHomeTurn.ShouldBeTrue();
		}

		[Fact]
		public void ChangeInPeriodAfterFouthPlay() {
			var game = Factory.SetupGame();

			for (var i = 0; i < 4; i++) {
				game.ChangeTurn();
			}

			game.Period.ShouldEqual(2);
			game.Turn.ShouldEqual(1);
		}

		[Fact]
		public void ChangeInPeriodAfterEighthPlay() {
			var game = Factory.SetupGame();

			for (int i = 0; i < 8; i++) {
				game.ChangeTurn();
			}

			game.Period.ShouldEqual(3);
			game.Turn.ShouldEqual(1);
		}

		[Fact]
		public void AllTurnsWorks() {
			var game = Factory.SetupGame();

			for (var i = 1; i <= 3; i++) {
				game.IsHomeTurn = false;
				game.IsHomeTurn.ShouldBeFalse();
				game.ChangeTurn();
				game.IsHomeTurn.ShouldBeTrue();
				game.ChangeTurn();
				game.IsHomeTurn.ShouldBeTrue();
				game.ChangeTurn();
				game.IsHomeTurn.ShouldBeFalse();
				game.ChangeTurn();
				game.Period.ShouldEqual(i + 1);
				game.Turn.ShouldEqual(1);
			}
		}
	}
}
