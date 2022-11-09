using System;
#nullable enable

namespace FSM
{
	public class Transition
	{
		public State To { get; }
		public Func<bool>? Condition { get; }
		
		public EventObject? TriggerEvent { get; }

		public Transition(State to, Func<bool> condition)
		{
			To = to;
			Condition = condition;
		}

		public Transition(State to, EventObject triggerEvent)
		{
			To = to;
			TriggerEvent = triggerEvent;
		}
	}
}
