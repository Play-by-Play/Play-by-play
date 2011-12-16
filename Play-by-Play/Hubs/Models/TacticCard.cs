using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Play_by_Play.Models;

namespace Play_by_Play.Hubs.Models {
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

		public TacticCard Reverse() {
			return new TacticCard {
				Id = Id,
				Name = Name,
				Difficulty = Difficulty,
				StartNode = StartNode.Reverse(),
				Nodes = Nodes.Select(x => x.Reverse()).ToList(),
				Movements = Movements.Select(x => x.Reverse()).ToList(),
				Passes = Passes.Select(x => x.Reverse()).ToList(),
				Shot = Shot.Reverse()
			};
		}
	}
}