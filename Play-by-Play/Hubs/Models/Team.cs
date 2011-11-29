using System.Collections.Generic;

namespace Play_by_Play.Hubs {
	public class Team {
		public string Name { get; set; }
		public string Color { get; set; }
		public List<Player> Line1 { get; set; }
		public List<Player> Line2 { get; set; }
		public List<Player> Goalies { get; set; }
	}
}