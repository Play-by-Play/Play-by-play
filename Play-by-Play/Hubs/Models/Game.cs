using System;
using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Models;
using SignalR.Hubs;

namespace Play_by_Play.Hubs.Models {
	public class Game {
		public string Id { get; set; }
		public GameBoard Board { get; set; }
		public GameScore Score { get; private set; }
		public GameUser HomeUser { get; set; }
		public GameUser AwayUser { get; set; }
		public bool IsHomeTurn { get; /*private*/ set; }
		public bool IsFaceOff { get; private set; }
		public int Period { get; private set; }
		public int Turn { get; private set; }
		public TacticCard CurrentTactic { get; set; }
		private List<TacticCard> AvailableCards { get; set; }

		public bool IsFinished { get; set; }

		public Game() {
			IsFinished = false;
			Id = Guid.NewGuid().ToString("d");
			Board = new GameBoard();
			Score = new GameScore();
			Period = 0;
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
				Id = 1,
				Name = "Give 'n Take",
				Difficulty = 4,
				Nodes = new List<Node>{
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
				Id = 2,
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
				Id = 3,
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
				Id = 4,
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
			});

			cards.Add(new TacticCard{
				Id = 6,
				Name = "Forward",
				Difficulty = 3,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 2).First(),
					nodes.Where(x => x.X == 1 && x.Y == 0).First(),
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
				Movements = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						Order = 1
					},
				},
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 2
					},
				},
				Shot = nodes.Where(x => x.X == 1 && x.Y == 0).First()
			});
			cards.Add(new TacticCard{
				Id = 7,
				Name = "2-on-1",
				Difficulty = 6,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 3).First(),
					nodes.Where(x => x.X == 1 && x.Y == 2).First(),
					nodes.Where(x => x.X == 1 && x.Y == 1).First(),
					nodes.Where(x => x.X == 0 && x.Y == 1).First(),
					nodes.Where(x => x.X == 1 && x.Y == 0).First(),
					nodes.Where(x => x.X == 0 && x.Y == 0).First(),
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
				Movements = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 2
					},
				},
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 2).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						Order = 2
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 1
					},
				},
				Shot = nodes.Where(x => x.X == 0 && x.Y == 0).First()
			});
			cards.Add(new TacticCard{
				Id = 8,
				Name = "Right on",
				Difficulty = 4,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 1 && x.Y == 3).First(),
					nodes.Where(x => x.X == 1 && x.Y == 0).First(),
				},
				StartNode = nodes.Where(x => x.X == 1 && x.Y == 3).First(),
				Movements = new List<Movement> (),
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 1
					},
				},
				Shot = nodes.Where(x => x.X == 1 && x.Y == 0).First()
			});
			cards.Add(new TacticCard{
				Id = 9,
				Name = "Zig Zag",
				Difficulty = 4,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 3).First(),
					nodes.Where(x => x.X == 1 && x.Y == 2).First(),
					nodes.Where(x => x.X == 0 && x.Y == 1).First(),
					nodes.Where(x => x.X == 1 && x.Y == 0).First(),
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
				Movements = new List<Movement> (),
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 2).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						Order = 2
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 3
					},
				},
				Shot = nodes.Where(x => x.X == 1 && x.Y == 0).First()
			});
			cards.Add(new TacticCard{
				Id = 10,
				Name = "Give 'n go",
				Difficulty = 6,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 1 && x.Y == 2).First(),
					nodes.Where(x => x.X == 0 && x.Y == 3).First(),
					nodes.Where(x => x.X == 0 && x.Y == 2).First(),
					nodes.Where(x => x.X == 1 && x.Y == 1).First(),
					nodes.Where(x => x.X == 0 && x.Y == 0).First(),
					nodes.Where(x => x.X == 1 && x.Y == 0).First(),
				},
				StartNode = nodes.Where(x => x.X == 1 && x.Y == 2).First(),
				Movements = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 2
					},
				},
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						Order = 2
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						Order = 3
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 4
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 5
					},
				},
				Shot = nodes.Where(x => x.X == 1 && x.Y == 0).First()
			});
			cards.Add(new TacticCard{
				Id = 11,
				Name = "Forsberg",
				Difficulty = 4,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 3).First(),
					nodes.Where(x => x.X == 0 && x.Y == 2).First(),
					nodes.Where(x => x.X == 0 && x.Y == 0).First(),
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
				Movements = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 2
					},
				},
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
					},
				},
				Shot = nodes.Where(x => x.X == 0 && x.Y == 0).First()
			});
			cards.Add(new TacticCard{
				Id = 12,
				Name = "Mixture",
				Difficulty = 7,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 3).First(),
					nodes.Where(x => x.X == 1 && x.Y == 2).First(),
					nodes.Where(x => x.X == 1 && x.Y == 3).First(),
					nodes.Where(x => x.X == 0 && x.Y == 2).First(),
					nodes.Where(x => x.X == 1 && x.Y == 1).First(),
					nodes.Where(x => x.X == 0 && x.Y == 1).First(),
					nodes.Where(x => x.X == 0 && x.Y == 0).First(),
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
				Movements = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						Order = 2
					},
				},
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 2).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 3).First(),
						Order = 2
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						Order = 3
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						Order = 4
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						Order = 5
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 6
					},
				},
				Shot = nodes.Where(x => x.X == 0 && x.Y == 0).First()
			});
			cards.Add(new TacticCard{
				Id = 13,
				Name = "In the zone",
				Difficulty = 6,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 1 && x.Y == 3).First(),
					nodes.Where(x => x.X == 1 && x.Y == 0).First(),
					nodes.Where(x => x.X == 0 && x.Y == 0).First(),
					nodes.Where(x => x.X == 0 && x.Y == 1).First(),
				},
				StartNode = nodes.Where(x => x.X == 1 && x.Y == 3).First(),
				Movements = new List<Movement> (),
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 2
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						Order = 3
					},
				},
				Shot = nodes.Where(x => x.X == 0 && x.Y == 1).First()
			});
			cards.Add(new TacticCard{
				Id = 14,
				Name = "The n'd",
				Difficulty = 5,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 2).First(),
					nodes.Where(x => x.X == 0 && x.Y == 1).First(),
					nodes.Where(x => x.X == 0 && x.Y == 0).First(),
					nodes.Where(x => x.X == 1 && x.Y == 1).First(),
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
				Movements = new List<Movement> (),
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 2
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						Order = 3
					},
				},
				Shot = nodes.Where(x => x.X == 1 && x.Y == 1).First()
			});
			cards.Add(new TacticCard{
				Id = 15,
				Name = "Nylander",
				Difficulty = 7,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 1 && x.Y == 3).First(),
					nodes.Where(x => x.X == 1 && x.Y == 1).First(),
					nodes.Where(x => x.X == 0 && x.Y == 2).First(),
					nodes.Where(x => x.X == 0 && x.Y == 1).First(),
					nodes.Where(x => x.X == 1 && x.Y == 0).First(),
					nodes.Where(x => x.X == 0 && x.Y == 0).First(),
				},
				StartNode = nodes.Where(x => x.X == 1 && x.Y == 3).First(),
				Movements = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						Order = 2
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						Order = 3
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 4
					},
				},
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						Order = 2
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						Order = 3
					},
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 4
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 5
					},
				},
				Shot = nodes.Where(x => x.X == 0 && x.Y == 0).First()
			});
			cards.Add(new TacticCard{
				Id = 16,
				Name = "Sidechange",
				Difficulty = 5,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 3).First(),
					nodes.Where(x => x.X == 1 && x.Y == 3).First(),
					nodes.Where(x => x.X == 1 && x.Y == 0).First(),
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
				Movements = new List<Movement> (),
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 3).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 2
					},
				},
				Shot = nodes.Where(x => x.X == 1 && x.Y == 0).First()
			});
			cards.Add(new TacticCard{
				Id = 17,
				Name = "Dropzone",
				Difficulty = 4,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 1 && x.Y == 1).First(),
					nodes.Where(x => x.X == 1 && x.Y == 2).First(),
					nodes.Where(x => x.X == 0 && x.Y == 1).First(),
				},
				StartNode = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
				Movements = new List<Movement> (),
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						End = nodes.Where(x => x.X ==1  && x.Y == 2).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 2).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						Order = 2
					},
				},
				Shot = nodes.Where(x => x.X == 0 && x.Y == 1).First()
			});
			cards.Add(new TacticCard{
				Id = 18,
				Name = "Blitzkrieg",
				Difficulty = 5,
				Nodes = new List<Node>() {
					nodes.Where(x => x.X == 0 && x.Y == 3).First(),
					nodes.Where(x => x.X == 1 && x.Y == 1).First(),
					nodes.Where(x => x.X == 0 && x.Y == 1).First(),
				},
				StartNode = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
				Movements = new List<Movement> (),
				Passes = new List<Movement> {
					new Movement {
						Start = nodes.Where(x => x.X == 0 && x.Y == 3).First(),
						End = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						Order = 1
					},
					new Movement {
						Start = nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						End = nodes.Where(x => x.X == 0 && x.Y == 1).First(),
						Order = 2
					},
				},
				Shot = nodes.Where(x => x.X == 0 && x.Y == 1).First()
			});


			return cards;
		}

		public List<TacticCard> GenerateTactics(int amount) {
			var list = new List<TacticCard>(amount);
			var available = AvailableCards.ToList();

			var rnd = new Random();

			for (var i = 0; i < amount; i++) {
				var nrAvailable = available.Count;
				var choosen = rnd.Next(nrAvailable);
				var card = available.ElementAt(choosen);
				list.Add(card);
				available.Remove(card);
			}

			//return list;
			return list;
		}

		public void Start() {
			# region Team Init
			HomeUser.Team = new Team {
				Name = "DET",
				Color = "c00",
				Players = new List<Player> {
					new Player{
						Name = "Zetterberg",
						Offense = 5,
						Defense = 3,
						Position = Position.LW,
						Formation = Formation.Line1,
						Id = 1
					},
					new Player{
						Name = "Datsyuk",
						Offense = 4,
						Defense = 4,
						Position = Position.C,
						Formation = Formation.Line1,
						Id = 2
					},
					new Player{
						Name = "Holmstrom",
						Offense = 3,
						Defense = 3,
						Position = Position.RW,
						Formation = Formation.Line1,
						Id = 3
					},
					new Player{
						Name = "Lidstrom",
						Offense = 3,
						Defense = 4,
						Position = Position.LD,
						Formation = Formation.Line1,
						Id = 4
					},
					new Player{
						Name = "Rafalski",
						Offense = 2,
						Defense = 4,
						Position = Position.RD,
						Formation = Formation.Line1,
						Id = 5
					},
					new Player{
						Name = "Cleary",
						Offense = 4,
						Defense = 2,
						Position = Position.LW,
						Formation = Formation.Line2,
						Id = 6
					},
					new Player{
						Name = "Filppula",
						Offense = 4,
						Defense = 2,
						Position = Position.C,
						Formation = Formation.Line2,
						Id = 7
					},
					new Player{
						Name = "Bertuzzi",
						Offense = 4,
						Defense = 3,
						Position = Position.RW,
						Formation = Formation.Line2,
						Id = 8
					},
					new Player{
						Name = "Kronwall",
						Offense = 3,
						Defense = 3,
						Position = Position.LD,
						Formation = Formation.Line2,
						Id = 9
					},
					new Player{
						Name = "Stuart",
						Offense = 2,
						Defense = 4,
						Position = Position.RD,
						Formation = Formation.Line2,
						Id = 10
					},
					new Player{
						Name = "Howard",
						Offense = 3,
						Defense = 5,
						Position = Position.G,
						Formation = Formation.Goalies,
						Id = 11
					},
					new Player{
						Name = "Osgood",
						Offense = 3,
						Defense = 4,
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
						Offense = 5,
						Defense = 2,
						Position = Position.LW,
						Formation = Formation.Line1,
						Id = 13
					},
					new Player{
						Name = "Drury",
						Offense = 5,
						Defense = 3,
						Position = Position.C,
						Formation = Formation.Line1,
						Id = 14
					},
					new Player{
						Name = "Gaborik",
						Offense = 6,
						Defense = 1,
						Position = Position.RW,
						Formation = Formation.Line1,
						Id = 15
					},
					new Player{
						Name = "Girardi",
						Offense = 1,
						Defense = 4,
						Position = Position.LD,
						Formation = Formation.Line1,
						Id = 16
					},
					new Player{
						Name = "Staal",
						Offense = 3,
						Defense = 4,
						Position = Position.RD,
						Formation = Formation.Line1,
						Id = 17
					},
					new Player{
						Name = "Zuccarello",
						Offense = 4,
						Defense = 2,
						Position = Position.LW,
						Formation = Formation.Line2,
						Id = 18
					},
					new Player{
						Name = "Anisimov",
						Offense = 4,
						Defense = 2,
						Position = Position.C,
						Formation = Formation.Line2,
						Id = 19
					},
					new Player{
						Name = "Callahan",
						Offense = 4,
						Defense = 3,
						Position = Position.RW,
						Formation = Formation.Line2,
						Id = 20
					},
					new Player{
						Name = "McCabe",
						Offense = 2,
						Defense = 4,
						Position = Position.LD,
						Formation = Formation.Line2,
						Id = 21
					},
					new Player{
						Name = "Del Zotto",
						Offense = 2,
						Defense = 3,
						Position = Position.RD,
						Formation = Formation.Line2,
						Id = 22
					},
					new Player{
						Name = "Lundqvist",
						Offense = 4,
						Defense = 4,
						Position = Position.G,
						Formation = Formation.Goalies,
						Id = 23
					},
					new Player{
						Name = "Biron",
						Offense = 2,
						Defense = 3,
						Position = Position.G,
						Formation = Formation.Goalies,
						Id = 24
					}
				}
			};
			# endregion

			NewPeriod();
		}

		public BattleResult ExecuteFaceOff() {
			var battleResult = new BattleResult(new List<Player> { Board.HomeFaceoff }, new List<Player> { Board.AwayFaceoff }, BattleType.FaceOff, false);

			IsHomeTurn = battleResult.IsHomeWinner;

			IsFaceOff = false;
			Board.HomeFaceoff = null;
			Board.AwayFaceoff = null;

			HomeUser.SetTurn(IsHomeTurn);
			AwayUser.SetTurn(!IsHomeTurn);

			return battleResult;
		}

		public bool IsReadyForFaceoff() {
			return Board.IsReadyForFaceoff();
		}

		public bool IsReadyForTactic() {
			return Board.HomePlayers.Count() == 5 &&
						 Board.AwayPlayers.Count() == 5 &&
						 Board.HomeGoalie != null &&
						 Board.AwayGoalie != null &&
						 CurrentTactic != null;
		}

		public TacticResult ExecuteTactic() {
			var tacticResult = new TacticResult {
				Card = CurrentTactic,
				IsHomeAttacking = IsHomeTurn,
				Battles = Board.ExecuteTactic(CurrentTactic, IsHomeTurn)
			};

			var user = IsHomeTurn
									? HomeUser
									: AwayUser;
			user.UseTactic(CurrentTactic);
			CurrentTactic = null;

			ChangeTurn();

			return tacticResult;
		}

		public void SendHomeActionMessage(string message, string type) {
			Hub.GetClients<GameConnection>()[HomeUser.ClientId].addActionMessage(message, type);
		}

		public void SendAwayActionMessage(string message, string type) {
			Hub.GetClients<GameConnection>()[AwayUser.ClientId].addActionMessage(message, type);
		}

		/*private*/
		public void ChangeTurn() {
			if (Turn % 4 == 0) {
				NewPeriod();
				if (Period == 4)
					IsFinished = true;
				return;
			}

			Turn++;

			if (Turn % 2 == 0)
				IsHomeTurn = !IsHomeTurn;
			else {
				Board.ClearBoard();
				Hub.GetClients<GameConnection>()[HomeUser.ClientId].substitution();
				Hub.GetClients<GameConnection>()[AwayUser.ClientId].substitution();
			}

			HomeUser.SetTurn(IsHomeTurn);
			AwayUser.SetTurn(!IsHomeTurn);
		}

		private void NewPeriod() {
			IsFaceOff = true;
			Period++;
			Turn = 1;

			// Send new cards
			HomeUser.AddTactics(GenerateTactics(5 - HomeUser.CurrentCards.Count));
			AwayUser.AddTactics(GenerateTactics(5 - AwayUser.CurrentCards.Count));

			// Call clients
			Hub.GetClients<GameConnection>()[HomeUser.ClientId].newPeriod();
			Hub.GetClients<GameConnection>()[AwayUser.ClientId].newPeriod();

			// Initialize events if start of game
			if(Period == 1) {
				Hub.GetClients<GameConnection>()[HomeUser.ClientId].nextTurn();
				Hub.GetClients<GameConnection>()[AwayUser.ClientId].nextTurn();
			}

		}
	}
}