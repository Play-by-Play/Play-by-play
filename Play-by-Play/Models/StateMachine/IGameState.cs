using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Play_by_Play.Models.StateMachine {
	public interface IGameState {
		IGameState Execute(object o);
	}
}
