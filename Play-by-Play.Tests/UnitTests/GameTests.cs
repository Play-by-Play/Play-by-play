using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Hubs.Models;
using Play_by_Play.Models;
using Should;
using Xunit;

namespace Play_by_Play.Tests.UnitTests {
	public class GameTests {
		[Fact]
		public void GameIsReadyWhen5PlayersFromEachTeamIsPlaced() {
			var game = SetupGame();

			var result = game.IsReadyForTactic();

			result.ShouldBeTrue();
		}

		[Fact]
		public void GameIsNotReadyWhenSomePlayerIsMissing() {
			var game = SetupGame();
			game.Board.GetArea(0,0).HomePlayers.RemoveAt(0);
			
			var result = game.IsReadyForTactic();

			result.ShouldBeFalse();
		}

		[Fact]
		public void GameIsNotReadyWhenOneGoalieIsMissing() {
			var game = SetupGame();
			game.Board.HomeGoalie = null;

			var result = game.IsReadyForTactic();

			result.ShouldBeFalse();
		}

		[Fact]
		public void ChangeInTurnAfterFirstPlay() {
			var game = SetupGame();
			var card = GetCard();

			game.CurrentTactic = card;
			game.ChangeTurn();

			game.IsHomeTurn.ShouldBeFalse();
		}

		[Fact]
		public void NoChangeInTurnAfterSecondPlay() {
			var game = SetupGame();
			var card = GetCard();

			game.CurrentTactic = card;
			game.ChangeTurn();

			game.IsHomeTurn.ShouldBeFalse();

			game.CurrentTactic = card;
			game.ChangeTurn();

			game.IsHomeTurn.ShouldBeFalse();
		}

		[Fact]
		public void ChangeInTurnAfterThirdPlay() {
			var game = SetupGame();
			game.ChangeTurn();

			game.IsHomeTurn.ShouldBeFalse();

			game.ChangeTurn();

			game.IsHomeTurn.ShouldBeFalse();

			game.ChangeTurn();

			game.IsHomeTurn.ShouldBeTrue();
		}

		[Fact]
		public void ChangeInPeriodAfterFouthPlay() {
			var game = SetupGame();

			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();

			game.Period.ShouldEqual(2);
		}

		[Fact]
		public void TurnResetAfterFouthPlay() {
			var game = SetupGame();

			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();

			game.Turn.ShouldEqual(1);
		}

		[Fact]
		public void ChangeInPeriodAfterEighthPlay() {
			var game = SetupGame();

			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();

			game.Period.ShouldEqual(3);
		}

		[Fact]
		public void TurnResetAfterEighthPlay() {
			var game = SetupGame();

			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();
			game.ChangeTurn();

			game.Turn.ShouldEqual(1);
		}

		[Fact]
		public void AllTurnsWorks() {
			var game = SetupGame();

			game.IsHomeTurn.ShouldBeTrue();
			game.ChangeTurn();
			game.IsHomeTurn.ShouldBeFalse();
			game.ChangeTurn();
			game.IsHomeTurn.ShouldBeFalse();
			game.ChangeTurn();
			game.IsHomeTurn.ShouldBeTrue();
			game.ChangeTurn();
			game.Period.ShouldEqual(2);

			game.IsHomeTurn = false;
			game.IsHomeTurn.ShouldBeFalse();
			game.ChangeTurn();
			game.IsHomeTurn.ShouldBeTrue();
			game.ChangeTurn();
			game.IsHomeTurn.ShouldBeTrue();
			game.ChangeTurn();
			game.IsHomeTurn.ShouldBeFalse();
			game.ChangeTurn();
			game.Period.ShouldEqual(3);

			game.IsHomeTurn = false;
			game.IsHomeTurn.ShouldBeFalse();
			game.ChangeTurn();
			game.IsHomeTurn.ShouldBeTrue();
			game.ChangeTurn();
			game.IsHomeTurn.ShouldBeTrue();
			game.ChangeTurn();
			game.IsHomeTurn.ShouldBeFalse();
			game.ChangeTurn();
			game.IsFinished.ShouldBeTrue();

			game.Turn.ShouldEqual(1);
		}

		private static TacticCard GetCard() {
			var nodes = new List<Node> {
				new Node {X = 0, Y = 0},
				new Node {X = 0, Y = 1},
				new Node {X = 0, Y = 2},
				new Node {X = 0, Y = 3},
				new Node {X = 1, Y = 0},
				new Node {X = 1, Y = 1},
				new Node {X = 1, Y = 2},
				new Node {X = 1, Y = 3}
			};

			var card = new TacticCard {
				Id = 5,
				Name = "Nailed",
				Difficulty = 3,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 2).First()
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
				Movements = new List<Movement>(),
				Passes = new List<Movement>(),
				Shot = nodes.Where(x => x.X == 0 && x.Y == 2).First()
			};
			return card;
		}

		private static Game SetupGame() {
			var game = new Game {
				Board = new GameBoard(),
				HomeUser = new GameUser("TestUser1", "123"),
				AwayUser = new GameUser("TestUser2", "123")
			};
			game.HomeUser.Oppenent = game.AwayUser;
			game.AwayUser.Oppenent = game.HomeUser;
			game.IsHomeTurn = true;
			game.Start();
			foreach (var player in game.HomeUser.Team.Line1) game.Board.PlacePlayer(player, 0, 2, true);
			foreach (var player in game.AwayUser.Team.Line1) game.Board.PlacePlayer(player, 1, 1, false);
			game.Board.HomeGoalie = game.HomeUser.Team.Goalies.First();
			game.Board.AwayGoalie = game.AwayUser.Team.Goalies.First();
			game.CurrentTactic = game.HomeUser.CurrentCards.First();
			return game;
		}

		private static void PlaceLine(List<Player> line, GameBoard board) {
			
		}
	}
}