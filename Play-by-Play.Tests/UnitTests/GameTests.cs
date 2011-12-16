using System.Linq;
using Play_by_Play.Hubs.Models;
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

		private static Game SetupGame() {
			var game = new Game {
				Board = new GameBoard(),
				HomeUser = new GameUser("TestUser1", "123"),
				AwayUser = new GameUser("TestUser2", "123")
			};
			game.Start();
			foreach (var player in game.HomeUser.Team.Line1) game.Board.PlacePlayer(player, 0, 0, true);
			foreach (var player in game.AwayUser.Team.Line1) game.Board.PlacePlayer(player, 0, 0, false);
			game.Board.HomeGoalie = game.HomeUser.Team.Goalies.First();
			game.Board.AwayGoalie = game.AwayUser.Team.Goalies.First();
			game.CurrentTactic = game.HomeUser.CurrentCards.First();
			return game;
		}
	}
}