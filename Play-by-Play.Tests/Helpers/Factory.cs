using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Hubs.Models;
using Play_by_Play.Models;

namespace Play_by_Play.Tests.Helpers {
	public static class Factory {
		public static List<Player> GetPlayers(int nr, string position = "C") {
			var players = new List<Player>();
			for (int i = 0; i < nr; i++) {
				players.Add(new Player {
					Offense = 3,
					Defense = 2,
					Position = position
				});
			}
			return players;
		}

		public static GameArea GetArea(int x, int y) {
			return new GameArea {
				X = x,
				Y = y
			};
		}

		public static TacticCard GetCard() {
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

		public static Game SetupGame() {
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

		public static void PlaceLine(List<Player> line, GameBoard board) {

		}
	}
}