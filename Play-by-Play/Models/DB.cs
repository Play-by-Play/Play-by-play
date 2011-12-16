using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using Play_by_Play.Hubs.Models;

namespace Play_by_Play.Models {
	public static class DB {
		private static readonly DataContext Context = new DataContext();
		public static List<TacticCard> GetTacticCards() {
			return Context.TacticCards.ToList();
		}
	}

	public class Node {
		public int Id { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public virtual List<TacticCard> Cards { get; set; } 

		public override string ToString() {
			return "{" + string.Format("X: {0}, Y: {1}", X, Y) + "}";
		}

		public Node Reverse() {
			return new Node {
				X = 1 - X,
				Y = 3 - Y
			};
		}
	}

	public class Movement {
		public int Id { get; set; }
		public virtual Node Start { get; set; }
		public virtual Node End { get; set; }
		public virtual TacticCard Card { get; set; }
		public int Order { get; set; }

		public Movement Reverse() {
			return new Movement {
				Start = Start.Reverse(),
				End = End.Reverse(),
				Order = Order,
				Card = Card
			};
		}
	}

	public class DataContext : DbContext {
		public DbSet<TacticCard> TacticCards { get; set; }
		public DbSet<Node> Nodes { get; set; }
		public DbSet<Movement> Movements { get; set; }
	}
}
			