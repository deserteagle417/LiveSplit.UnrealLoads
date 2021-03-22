using System;

namespace LiveSplit.UnrealLoads
{
	public interface IGameMemory
	{
		event EventHandler OnLoadEnded;
		event EventHandler OnLoadStarted;
		event MapChangeEventHandler OnMapChange;
		event EventHandler OnReset;
		event EventHandler OnSplit;
		event EventHandler OnStart;

		void StartMonitoring();
		void Stop();
	}

	public delegate void MapChangeEventHandler(object sender, string prevMap, string nextMap);
}
