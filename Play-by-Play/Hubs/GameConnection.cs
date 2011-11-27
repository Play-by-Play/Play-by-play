using Play_by_Play.Models;
using Play_by_Play.Models.StateMachine;
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
			string name = Caller.Name;
			if (!users.ContainsKey(Caller.Name)) return;
			if (games.Values.Where(x => x.HomeUser.Name == Caller.Name || (x.AwayUser != null && x.AwayUser.Name == Caller.Name)).Count() > 0) return;
			var user = users[Caller.Name];
			var game = new Game {
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

			Caller.gameId = game.Id;
			Caller.startGame(game);
			Caller.addActionMessage("Game against " + game.HomeUser.Name + " has started");

			Clients[game.HomeUser.ClientId].startGame(game);
			Clients[game.HomeUser.ClientId].addActionMessage("Game against " + game.AwayUser.Name + " has started");
			Clients.removeGame(game.Id);
		}

		public void GetTacticCards(int amount) {
			var game = games.Values.Single(x => x.AwayUser.ClientId.Equals(Context.ClientId) || x.HomeUser.ClientId.Equals(Context.ClientId));
			var tactics = game.GenerateTactics(amount);

			var writer = new JavaScriptSerializer();
			var result = writer.Serialize(tactics);
			Caller.createTacticCards(result);
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
	}
	public class Game {
		public string Id { get; set; }
		public GameStateMachine GameState { get; private set; }
		public GameUser HomeUser { get; set; }
		public GameUser AwayUser { get; set; }
		private List<TacticCard> AvailableCards { get; set; }

		public Game() {
			Id = Guid.NewGuid().ToString("d");
			GameState = new GameStateMachine();
			AvailableCards = GenerateTacticCards();
		}

		private List<TacticCard> GenerateTacticCards() {
			var cards = new List<TacticCard>();
			var nodes = new List<Node>();

			nodes.Add(new Node { X = 0, Y = 0 });
			nodes.Add(new Node { X = 0, Y = 1 });
			nodes.Add(new Node { X = 0, Y = 2 });
			nodes.Add(new Node { X = 0, Y = 3 });
			nodes.Add(new Node { X = 1, Y = 0 });
			nodes.Add(new Node { X = 1, Y = 1 });
			nodes.Add(new Node { X = 1, Y = 2 });
			nodes.Add(new Node { X = 1, Y = 3 });
			
			cards.Add(new TacticCard {
				Name = "Give 'n Take",
				Difficulty = 4,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 2).First(),
					nodes.Where(x => x.X == 1 && x.Y == 1).First(),
					nodes.Where(x => x.X == 0 && x.Y == 0).First(),
					nodes.Where(x => x.X == 1 && x.Y == 0).First()
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
				Movements = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 1
					}
				},
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 2
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 3
					}
				},
				Shot = nodes.Where(x => x.X == 1 && x.Y == 0).First()
			});

			cards.Add(new TacticCard {
				Name = "Left On",
				Difficulty = 4,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 3).First(),
					nodes.Where(x => x.X == 0 && x.Y == 0).First()
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
				Movements = new List<Movement>(),
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 1
					}
				},
				Shot = nodes.Where(x => x.X == 0 && x.Y == 0).First()
			});

			cards.Add(new TacticCard {
				Name = "Longshot",
				Difficulty = 4,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 3).First()
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
				Movements = new List<Movement>(),
				Passes = new List<Movement>(),
				Shot = nodes.Where(x => x.X == 0 && x.Y == 3).First()
			});

			cards.Add(new TacticCard {
				Name = "Straight",
				Difficulty = 4,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 3).First(),
					nodes.Where(x => x.X == 0 && x.Y == 2).First(),
					nodes.Where(x => x.X == 0 && x.Y == 0).First()
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
				Movements = new List<Movement>(),
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 2
					}
				},
				Shot = nodes.Where(x => x.X == 0 && x.Y == 0).First()
			});

			cards.Add(new TacticCard {
				Name = "Nailed",
				Difficulty = 3,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 2).First()
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
				Movements = new List<Movement>(),
				Passes = new List<Movement>(),
				Shot = nodes.Where(x => x.X == 0 && x.Y == 2).First()
			});

			return cards;
		}

		public List<TacticCard> GenerateTactics(int amount) {
			var list = new List<TacticCard>(amount);

			for (var i = 0; i < amount; i++) {
				var nrAvailable = AvailableCards.Count;
				var choosen = (int)Math.Floor(new Random(nrAvailable).NextDouble());
				var card = AvailableCards.ElementAt(choosen);
				HomeUser.AddTactic(card);

				nrAvailable = AvailableCards.Count;
				choosen = (int)Math.Floor(new Random(nrAvailable).NextDouble());
				card = AvailableCards.ElementAt(choosen);
				AwayUser.AddTactic(card);
			}

			//return list;
			return AvailableCards;
		}
	}

	public class GameEvent {

	}

	[Serializable]
	public class GameUser {
		public string ClientId { get; set; }
		public string Id { get; set; }
		public string Name { get; set; }
		public string Hash { get; set; }
		private List<TacticCard> CurrentCards { get; set; }
		private List<TacticCard> UsedCards { get; set; }
		public Team Team { get; set; }

		public GameUser() {

		}

		public GameUser(string name, string hash) {
			Name = name;
			Hash = hash;
			Id = Guid.NewGuid().ToString("d");
			CurrentCards = new List<TacticCard>();
			UsedCards = new List<TacticCard>();
		}

		public void AddTactic(TacticCard card) {
			CurrentCards.Add(card);
		}
	}

	public class Team {
		public List<Player> Players { get; set; }
	}

	public class Player {
		public string Name { get; private set; }
		public Position Position { get; set; }

		public Player(string name) {
			Name = name;
		}
	}

	public enum Position {
		C,
		LW,
		RW,
		LD,
		RD,
		G
	}
}