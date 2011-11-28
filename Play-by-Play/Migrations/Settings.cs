using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Providers;
using System.Data.SqlClient;
using System.Linq;
using Play_by_Play.Models;

namespace Play_by_Play.Migrations {
	public class Settings : DbMigrationContext<DataContext> {
		public Settings() {
			AutomaticMigrationsEnabled = true;
			SetCodeGenerator<CSharpMigrationCodeGenerator>();
			AddSqlGenerator<SqlConnection, SqlServerMigrationSqlGenerator>();

			// Uncomment the following line if you are using SQL Server Compact 
			// SQL Server Compact is available as the SqlServerCompact NuGet package
			// AddSqlGenerator<System.Data.SqlServerCe.SqlCeConnection, SqlCeMigrationSqlGenerator>();

			// Seed data: 
			//   Override the Seed method in this class to add seed data.
			//    - The Seed method will be called after migrating to the latest version.
			//    - The method should be written defensively in order that duplicate data is not created. E.g:
			//
			var context = new DataContext();
			if (!context.Nodes.Any()) {
				context.Nodes.Add(new Node { X = 0, Y = 0 });
				context.Nodes.Add(new Node { X = 0, Y = 1 });
				context.Nodes.Add(new Node { X = 0, Y = 2 });
				context.Nodes.Add(new Node { X = 0, Y = 3 });
				context.Nodes.Add(new Node { X = 1, Y = 0 });
				context.Nodes.Add(new Node { X = 1, Y = 1 });
				context.Nodes.Add(new Node { X = 1, Y = 2 });
				context.Nodes.Add(new Node { X = 1, Y = 3 });
				context.SaveChanges();
			}
			if (context.TacticCards.Any()) return;
			context.TacticCards.Add(new TacticCard {
				Name = "Give 'n Take",
				Difficulty = 4,
				Nodes = new List<Node>() {
					context.Nodes.Where(x => x.X == 0 && x.Y == 2).First(),
					context.Nodes.Where(x => x.X == 1 && x.Y == 1).First(),
					context.Nodes.Where(x => x.X == 0 && x.Y == 0).First(),
					context.Nodes.Where(x => x.X == 1 && x.Y == 0).First()
				},
				StartNode = context.Nodes.Where(x => x.X == 0 && x.Y == 2).First(),
				Movements = new List<Movement> {
					new Movement {
						Start = context.Nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						End = context.Nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 1
					}
				},
				Passes = new List<Movement> {
					new Movement {
						Start = context.Nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						End = context.Nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						Order = 1
					},
					new Movement {
						Start = context.Nodes.Where(x => x.X == 1 && x.Y == 1).First(),
						End = context.Nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 2
					},
					new Movement {
						Start = context.Nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						End = context.Nodes.Where(x => x.X == 1 && x.Y == 0).First(),
						Order = 3
					}
				},
				Shot = context.Nodes.Where(x => x.X == 1 && x.Y == 0).First()
			});

			context.TacticCards.Add(new TacticCard {
				Name = "Left On",
				Difficulty = 4,
				Nodes = new List<Node>() {
					context.Nodes.Where(x => x.X == 0 && x.Y == 3).First(),
					context.Nodes.Where(x => x.X == 0 && x.Y == 0).First()
				},
				StartNode = context.Nodes.Where(x => x.X == 0 && x.Y == 3).First(),
				Movements = new List<Movement>(),
				Passes = new List<Movement> {
					new Movement {
						Start = context.Nodes.Where(x => x.X == 0 && x.Y == 3).First(),
						End = context.Nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 1
					}
				},
				Shot = context.Nodes.Where(x => x.X == 0 && x.Y == 0).First()
			});

			context.TacticCards.Add(new TacticCard {
				Name = "Longshot",
				Difficulty = 4,
				Nodes = new List<Node>() {
					context.Nodes.Where(x => x.X == 0 && x.Y == 3).First()
				},
				StartNode = context.Nodes.Where(x => x.X == 0 && x.Y == 3).First(),
				Movements = new List<Movement>(),
				Passes = new List<Movement>(),
				Shot = context.Nodes.Where(x => x.X == 0 && x.Y == 3).First()
			});

			context.TacticCards.Add(new TacticCard {
				Name = "Straight",
				Difficulty = 4,
				Nodes = new List<Node>() {
					context.Nodes.Where(x => x.X == 0 && x.Y == 3).First(),
					context.Nodes.Where(x => x.X == 0 && x.Y == 2).First(),
					context.Nodes.Where(x => x.X == 0 && x.Y == 0).First()
				},
				StartNode = context.Nodes.Where(x => x.X == 0 && x.Y == 3).First(),
				Movements = new List<Movement>(),
				Passes = new List<Movement> {
					new Movement {
						Start = context.Nodes.Where(x => x.X == 0 && x.Y == 3).First(),
						End = context.Nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						Order = 1
					},
					new Movement {
						Start = context.Nodes.Where(x => x.X == 0 && x.Y == 2).First(),
						End = context.Nodes.Where(x => x.X == 0 && x.Y == 0).First(),
						Order = 2
					}
				},
				Shot = context.Nodes.Where(x => x.X == 0 && x.Y == 0).First()
			});

			context.TacticCards.Add(new TacticCard {
				Name = "Nailed",
				Difficulty = 3,
				Nodes = new List<Node>() {
					context.Nodes.Where(x => x.X == 0 && x.Y == 2).First()
				},
				StartNode = context.Nodes.Where(x => x.X == 0 && x.Y == 2).First(),
				Movements = new List<Movement>(),
				Passes = new List<Movement>(),
				Shot = context.Nodes.Where(x => x.X == 0 && x.Y == 2).First()
			});
			context.SaveChanges();
		}
	}
}
