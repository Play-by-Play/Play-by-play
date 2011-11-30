using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Play_by_Play.Models {
	public static class DB {
		private static readonly DataContext Context = new DataContext();
		public static List<TacticCard> GetTacticCards() {
			return Context.TacticCards.ToList();
		}
	}

	public class TacticCard {
		public int Id { get; set; }
		public string Name { get; set; }
		public int Difficulty { get; set; }
		public virtual Node StartNode { get; set; }
		public virtual List<Node> Nodes { get; set; }
		public virtual List<Movement> Movements { get; set; }
		public virtual List<Movement> Passes { get; set; }
		public virtual Node Shot { get; set; }

		public IEnumerable<Node> MovementNodes{
			get {
				return Movements.Select(movement => movement.End);
			}
		}
		public dynamic ToDynamic() {
			dynamic card = new ExpandoObject();
			card.name = Name;
			card.diff = Difficulty;

			dynamic moves = new ExpandoObject();
			moves.startNode = StartNode;
			moves.nodes = Nodes;
			moves.movementNodes = Movements;
			moves.passes = Passes;
			moves.movingPass = MovementNodes;
			moves.shot = Shot;

			card.moves = moves;

			return card;
		}

		public override string ToString() {
			var builder = new StringBuilder();

			builder.Append("{");
			builder.Append(string.Format("name: {0},", Name));
			builder.Append(string.Format("diff: {0},", Difficulty));
			builder.Append("moves: {");
			builder.Append(string.Format("startNode: {0},", StartNode));
			builder.Append(string.Format("nodes: {0},", Difficulty));
			

			return builder.ToString();
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
	}

	public class Movement {
		public int Id { get; set; }
		public virtual Node Start { get; set; }
		public virtual Node End { get; set; }
		public virtual TacticCard Card { get; set; }
		public int Order { get; set; }
	}

	public class DataContext : DbContext {
		public DbSet<TacticCard> TacticCards { get; set; }
		public DbSet<Node> Nodes { get; set; }
		public DbSet<Movement> Movements { get; set; }
	}
}
			