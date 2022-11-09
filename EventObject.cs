using System;
#nullable enable

namespace FSM
{
	public class EventObject
	{
		public event Action? Event;

		public void Invoke()
		{
			Event?.Invoke();
		}
	}
}
