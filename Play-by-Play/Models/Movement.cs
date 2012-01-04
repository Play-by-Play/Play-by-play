using Play_by_Play.Hubs.Models;

namespace Play_by_Play.Models
{
	public class Movement
	{
		public int Id { get; set; }
		public virtual Node Start { get; set; }
		public virtual Node End { get; set; }
		public virtual TacticCard Card { get; set; }
		public int Order { get; set; }

		public Movement Reverse()
		{
			return new Movement
			{
				Start = Start.Reverse(),
				End = End.Reverse(),
				Order = Order,
				Card = Card
			};
		}
	}
}