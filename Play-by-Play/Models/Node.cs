using System.Collections.Generic;
using Play_by_Play.Hubs.Models;

namespace Play_by_Play.Models
{
	public class Node
	{
		public int Id { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public virtual List<TacticCard> Cards { get; set; }

		public override string ToString()
		{
			return "{" + string.Format("X: {0}, Y: {1}", X, Y) + "}";
		}

		public Node Reverse()
		{
			return new Node
			{
				X = 1 - X,
				Y = 3 - Y
			};
		}
	}
}