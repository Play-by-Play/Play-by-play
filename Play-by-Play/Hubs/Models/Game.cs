using System;
using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Models;

namespace Play_by_Play.Hubs.Models {
	public class Game {
		public string Id { get; set; }
		public GameBoard Board { get; set; }
		public GameUser HomeUser { get; set; }
		public GameUser AwayUser { get; set; }
		public bool IsHomeTurn { get; private set; }
		private List<TacticCard> AvailableCards { get; set; }

		public Game() {
			Id = Guid.NewGuid().ToString("d");
			Board = new GameBoard();
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

		public void Start() {
			# region Team Init
			HomeUser.Team = new Team {
				Name = "DET",
				Color = "c00",
				Players = new List<Player> {
					new Player{
						Name = "Zetterberg",
						Offence = 5,
						Defence = 3,
						Position = Position.LW,
						Formation = Formation.Line1,
						Id = 1
					},
					new Player{
						Name = "Datsyuk",
						Offence = 4,
						Defence = 4,
						Position = Position.C,
						Formation = Formation.Line1,
						Id = 2
					},
					new Player{
						Name = "Holmstrom",
						Offence = 3,
						Defence = 3,
						Position = Position.RW,
						Formation = Formation.Line1,
						Id = 3
					},
					new Player{
						Name = "Lidstrom",
						Offence = 3,
						Defence = 4,
						Position = Position.LD,
						Formation = Formation.Line1,
						Id = 4
					},
					new Player{
						Name = "Rafalski",
						Offence = 2,
						Defence = 4,
						Position = Position.RD,
						Formation = Formation.Line1,
						Id = 5
					},
					new Player{
						Name = "Cleary",
						Offence = 4,
						Defence = 2,
						Position = Position.LW,
						Formation = Formation.Line2,
						Id = 6
					},
					new Player{
						Name = "Filppula",
						Offence = 4,
						Defence = 2,
						Position = Position.C,
						Formation = Formation.Line2,
						Id = 7
					},
					new Player{
						Name = "Bertuzzi",
						Offence = 4,
						Defence = 3,
						Position = Position.RW,
						Formation = Formation.Line2,
						Id = 8
					},
					new Player{
						Name = "Kronwall",
						Offence = 3,
						Defence = 3,
						Position = Position.LD,
						Formation = Formation.Line2,
						Id = 9
					},
					new Player{
						Name = "Stuart",
						Offence = 2,
						Defence = 4,
						Position = Position.RD,
						Formation = Formation.Line2,
						Id = 10
					},
					new Player{
						Name = "Howard",
						Offence = 3,
						Defence = 5,
						Position = Position.G,
						Formation = Formation.Goalies,
						Id = 11
					},
					new Player{
						Name = "Osgood",
						Offence = 3,
						Defence = 4,
						Position = Position.G,
						Formation = Formation.Goalies,
						Id = 12
					}
				}
			};

			AwayUser.Team = new Team {
				Name = "NYR",
				Color = "00c",
				Players = new List<Player> {
					new Player{
						Name = "Dubinsky",
						Offence = 5,
						Defence = 2,
						Position = Position.LW,
						Formation = Formation.Line1,
						Id = 13
					},
					new Player{
						Name = "Drury",
						Offence = 5,
						Defence = 3,
						Position = Position.C,
						Formation = Formation.Line1,
						Id = 14
					},
					new Player{
						Name = "Gaborik",
						Offence = 6,
						Defence = 1,
						Position = Position.RW,
						Formation = Formation.Line1,
						Id = 15
					},
					new Player{
						Name = "Girardi",
						Offence = 1,
						Defence = 4,
						Position = Position.LD,
						Formation = Formation.Line1,
						Id = 16
					},
					new Player{
						Name = "Staal",
						Offence = 3,
						Defence = 4,
						Position = Position.RD,
						Formation = Formation.Line1,
						Id = 17
					},
					new Player{
						Name = "Zuccarello",
						Offence = 4,
						Defence = 2,
						Position = Position.LW,
						Formation = Formation.Line2,
						Id = 18
					},
					new Player{
						Name = "Anisimov",
						Offence = 4,
						Defence = 2,
						Position = Position.C,
						Formation = Formation.Line2,
						Id = 19
					},
					new Player{
						Name = "Callahan",
						Offence = 4,
						Defence = 3,
						Position = Position.RW,
						Formation = Formation.Line2,
						Id = 20
					},
					new Player{
						Name = "McCabe",
						Offence = 2,
						Defence = 4,
						Position = Position.LD,
						Formation = Formation.Line2,
						Id = 21
					},
					new Player{
						Name = "Del Zotto",
						Offence = 2,
						Defence = 3,
						Position = Position.RD,
						Formation = Formation.Line2,
						Id = 22
					},
					new Player{
						Name = "Lundqvist",
						Offence = 4,
						Defence = 4,
						Position = Position.G,
						Formation = Formation.Goalies,
						Id = 23
					},
					new Player{
						Name = "Biron",
						Offence = 2,
						Defence = 3,
						Position = Position.G,
						Formation = Formation.Goalies,
						Id = 24
					}
				}
			};
			# endregion


		}

		public BattleResult ExecuteFaceOff() {
			var battleResult = new BattleResult(new List<Player> {Board.HomeFaceoff}, new List<Player> {Board.AwayFaceoff});

			IsHomeTurn = battleResult.HomeTotal > battleResult.AwayTotal;

			return battleResult;
		}
	}
}