using System.Data.Entity;
using Play_by_Play.Hubs.Models;

namespace Play_by_Play.Models
{
	public class DataContext : DbContext
	{
		public DbSet<TacticCard> TacticCards { get; set; }
		public DbSet<Node> Nodes { get; set; }
		public DbSet<Movement> Movements { get; set; }
	}
}