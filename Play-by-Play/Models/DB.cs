using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Hubs.Models;

namespace Play_by_Play.Models
{
	public static class DB {
		private static readonly DataContext Context = new DataContext();
		public static List<TacticCard> GetTacticCards() {
			return Context.TacticCards.ToList();
		}
	}
}
			