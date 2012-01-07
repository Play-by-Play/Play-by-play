using System;

namespace Play_by_Play.Hubs.Models
{
	public class RandomGenerator
	{
		private static readonly Random Rnd = new Random();
		public virtual int Next(int min, int max)
		{
			return Rnd.Next(min, max);
		}
	}
}