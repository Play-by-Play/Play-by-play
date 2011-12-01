using SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace Play_by_Play.Hubs {
	[HubName("game")]
	public class GameConnection : Hub, IDisconnect {
		private static Dictionary<string, GameUser> users = new Dictionary<string, GameUser>(StringComparer.OrdinalIgnoreCase);
		private static Dictionary<string, Game> games = new Dictionary<string, Game>(StringComparer.OrdinalIgnoreCase);

		public void Send(string message) {
			var user = users.Values.FirstOrDefault(x => x.ClientId == Context.ClientId);
			if (user == null) return;
			Clients.addChatMessage(user.Name, message);
		}

		public void GetUser() {

			Caller.getUser(users.FirstOrDefault(x => x.Value.ClientId.Equals(Context.ClientId)).Value);
		}

		public void CreateUser(string username) {
			var userExists = users.FirstOrDefault(x => x.Key.Equals(username)).Value != null;
			if (!userExists) {
				var user = new GameUser(username, GetMD5Hash(username)) {
					ClientId = Context.ClientId
				};

				users[username] = user;

				Caller.Name = user.Name;
				Caller.Id = user.Id;
				Caller.Hash = user.Hash;

				Caller.addUser(user);

				return;
			}
			Caller.usernameExists();
		}

		public void CreateGame() {
			if (!users.ContainsKey(Caller.Name)) return;
			if (games.Values.Count(x => x.HomeUser.Name == Caller.Name || (x.AwayUser != null && x.AwayUser.Name == Caller.Name)) > 0) return;
			var user = users[Caller.Name];
			Game game = new Game {
				HomeUser = user
			};
			games[game.Id] = game;

			Caller.gameId = game.Id;

			Clients.addGame(game);
		}

		public void GetGames() {
			var activeGames = games.Values.Where(x => x.AwayUser == null).ToList();
			Caller.gameList(activeGames);
		}

		public void JoinGame(string gameId) {
			var game = games.FirstOrDefault(x => x.Key.Equals(gameId)).Value;

			if (game == null) return;

			var user = users.FirstOrDefault(x => x.Key.Equals(Caller.Name)).Value;

			game.AwayUser = user;
			game.Start();

			Caller.gameId = game.Id;
			Caller.startGame(game);
			Caller.addActionMessage("Game against " + game.HomeUser.Name + " has started");

			Clients[game.HomeUser.ClientId].startGame(game);
			Clients[game.HomeUser.ClientId].addActionMessage("Game against " + game.AwayUser.Name + " has started");
			Clients.removeGame(game.Id);
		}

		public void GetTacticCards(int amount) {
			var game = games.Values.First(x => x.AwayUser.ClientId.Equals(Context.ClientId) || x.HomeUser.ClientId.Equals(Context.ClientId));
			var tactics = game.GenerateTactics(amount);

			var writer = new JavaScriptSerializer();
			var result = writer.Serialize(tactics);
			Caller.createTacticCards(result);
		}

		public void GetPlayers() {
			var game = games.Values.First(x => x.AwayUser.ClientId.Equals(Context.ClientId) || x.HomeUser.ClientId.Equals(Context.ClientId));
			var isHome = game.HomeUser.ClientId == Context.ClientId;
			if (isHome)
				Caller.addPlayers(game.HomeUser.Team, game.AwayUser.Team);
			else
				Caller.addPlayers(game.AwayUser.Team, game.HomeUser.Team);
		}

		public void PlacePlayer(int playerId, string areaName) {
			var username = Caller.Name;
			var user = users[Caller.Name] as GameUser;
			if (user == null)
				throw new Exception("User does not exist");
			var game = games.Values.First(z => z.AwayUser == user || z.HomeUser == user);
			var coords = GameArea.GetCoords(areaName);
			var oppositeX = 1 - coords[0];
			var oppositeY = 3 - coords[1];
			var isHome = user == game.HomeUser;
			var opponentId = isHome
												? game.AwayUser.ClientId
												: game.HomeUser.ClientId;

			var player = user.Team.Players.FirstOrDefault(p => p.Id == playerId);

			var debugFlip = false;
			if (player == null && Caller.DebugMode) {
				player = isHome
				         	? game.AwayUser.Team.Players.First(p => p.Id == playerId)
				         	: game.HomeUser.Team.Players.First(p => p.Id == playerId);

				debugFlip = true;
			}
			game.Board.PlacePlayer(player, coords[0], coords[1], isHome);
			string name = GameArea.GetAreaName(oppositeX, oppositeY);
			if (!debugFlip) return;
			Clients[opponentId].placeOpponentPlayer(playerId, name);
		}

		public void AbortGame(Game game) {
			if (game.HomeUser != null) {
				Clients[game.HomeUser.ClientId].abortGame();
				game.HomeUser = null;
			}

			if (game.AwayUser != null) {
				Clients[game.AwayUser.ClientId].abortGame();
				game.AwayUser = null;
			}

			Clients.removeGame(game.Id);
			games.Remove(game.Id);
		}

		private string GetMD5Hash(string username) {
			return String.Join("", MD5.Create()
				.ComputeHash(Encoding.Default.GetBytes(username))
				.Select(b => b.ToString("x2")));
		}

		public void Disconnect() {
			var user = users.Values.FirstOrDefault(x => x.ClientId == Context.ClientId);

			if (user == null) return;

			users.Remove(user.Name);

			var gamesToRemove = games.Values.Where(x => x.HomeUser == user || x.AwayUser == user).ToArray();

			for (int i = 0; i < gamesToRemove.Count(); i++) {
				AbortGame(gamesToRemove[i]);
			}
		}

		# region Debugging

		public void FakeIt() {
			var homeuser = new GameUser("HomeUser", GetMD5Hash("HomeUser")) {
				ClientId = Context.ClientId
			};
			var awayuser = new GameUser("AwayUser", GetMD5Hash("AwayUser")) {
				ClientId = Context.ClientId
			};

			Caller.Name = homeuser.Name;
			Caller.Id = homeuser.Id;
			Caller.Hash = homeuser.Hash;

			users["Homeuser"] = homeuser;
			users["AwayUser"] = awayuser;

			Game game = new Game {
				HomeUser = homeuser,
				AwayUser = awayuser
			};
			games[game.Id] = game;

			game.Start();

			Caller.startGame(game);
			Caller.addActionMessage("Fake match started");
			Caller.DebugMode = true;
		}

		# endregion
	}
}