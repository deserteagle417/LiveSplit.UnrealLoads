using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UnrealLoads
{
	public class UnrealLoadsComponent : LogicComponent
	{
		public override string ComponentName => "UnrealLoads";

		private readonly IUnrealLoadsSettings Settings;
		private readonly ITimerModel _timer;
		private readonly IGameMemory _gameMemory;
		private readonly LiveSplitState _state;
		private readonly HashSet<string> _splitHistory;

		public UnrealLoadsComponent(LiveSplitState state, ITimerModel timer, IGameMemory gameMemory, IUnrealLoadsSettings settings)
		{
			bool debug = false;
#if DEBUG
			debug = true;
#endif
			Trace.WriteLine("[NoLoads] Using LiveSplit.UnrealLoads component version " + Assembly.GetExecutingAssembly().GetName().Version + " " + ((debug) ? "Debug" : "Release") + " build");

			_state = state;
			_timer = timer;
			_splitHistory = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			Settings = settings;

			_state.OnStart += _state_OnStart;

			_gameMemory = gameMemory;
			_gameMemory.OnReset += gameMemory_OnReset;
			_gameMemory.OnStart += gameMemory_OnStart;
			_gameMemory.OnSplit += _gameMemory_OnSplit;
			_gameMemory.OnLoadStarted += gameMemory_OnLoadStarted;
			_gameMemory.OnLoadEnded += gameMemory_OnLoadEnded;
			_gameMemory.OnMapChange += _gameMemory_OnMapChange;

			_gameMemory.StartMonitoring();
		}

		void _state_OnStart(object sender, EventArgs e)
		{
			_timer.InitializeGameTime();
			_splitHistory.Clear();
		}

		void _gameMemory_OnSplit(object sender, EventArgs e)
		{
			_timer.Split();
		}

		void _gameMemory_OnMapChange(object sender, string prevMap, string nextMap)
		{
#if DEBUG
			if (Settings.DbgShowMap)
				MessageBox.Show(_state.Form, prevMap + " -> " + nextMap, "LiveSplit.UnrealLoads",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif

			if (!Settings.AutoSplitOnMapChange)
				return;

			if (Settings.Maps.Count == 0)
			{
				_timer.Split();
				return;
			}

			var shouldSplit = false;
			var enterMap = Settings.Maps
				.FirstOrDefault(map => string.Equals(map.Name, nextMap, StringComparison.OrdinalIgnoreCase));

			if (enterMap != null && enterMap.SplitOnEnter && ShouldSplitMap(enterMap.Name))
			{
				shouldSplit = true;
				_splitHistory.Add(nextMap);
			}

			var leaveMap = Settings.Maps
				.FirstOrDefault(map => string.Equals(map.Name, prevMap, StringComparison.OrdinalIgnoreCase));
			if (leaveMap != null && leaveMap.SplitOnLeave && ShouldSplitMap(leaveMap.Name))
			{
				shouldSplit = true;
				_splitHistory.Add(prevMap);
			}

			if (shouldSplit)
			{
				_timer.Split();
			}
		}

		bool ShouldSplitMap(string mapName)
		{
			return !Settings.AutoSplitOncePerMap || !_splitHistory.Contains(mapName);
		}

		public override void Dispose()
		{
			_gameMemory?.Stop();
			_state.OnStart -= _state_OnStart;
		}

		void gameMemory_OnReset(object sender, EventArgs e)
		{
			if (Settings.AutoReset)
			{
				_timer.Reset();
			}
		}

		void gameMemory_OnStart(object sender, EventArgs e)
		{
			if (_state.CurrentPhase == TimerPhase.NotRunning && Settings.AutoStart)
			{
				_timer.Start();
			}
		}

		void gameMemory_OnLoadStarted(object sender, EventArgs e)
		{
			_state.IsGameTimePaused = true;
		}

		void gameMemory_OnLoadEnded(object sender, EventArgs e)
		{
			_state.IsGameTimePaused = false;
		}

		public override XmlNode GetSettings(XmlDocument document) => Settings.GetSettings(document);

		public override Control GetSettingsControl(LayoutMode mode) => Settings.UserControl;

		public override void SetSettings(XmlNode settings) => Settings.SetSettings(settings);

		public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }
	}
}
