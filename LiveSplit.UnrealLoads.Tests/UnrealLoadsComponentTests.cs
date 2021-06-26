using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using LiveSplit.Model;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace LiveSplit.UnrealLoads.Tests
{
	public class UnrealLoadsComponentTests
    {
		private readonly IFixture _fixture;
		private readonly IUnrealLoadsSettings _settings;
		private readonly IGameMemory _gameMemory;
		private readonly ITimerModel _timer;
		private readonly LiveSplitState _state;
		private readonly UnrealLoadsComponent _sut;

		public UnrealLoadsComponentTests()
		{
			_fixture = new Fixture().Customize(new AutoNSubstituteCustomization() { ConfigureMembers = true });

			_settings = _fixture.Create<IUnrealLoadsSettings>();
			_settings.DbgShowMap = false;
			_settings.AutoReset
				= _settings.AutoStart
				= _settings.AutoSplitOncePerMap
				= _settings.AutoSplitOnMapChange = true;

			_state = new LiveSplitState(null, null, null, null, null);
			_gameMemory = Substitute.For<IGameMemory>();
			_timer = Substitute.For<ITimerModel>();

			_sut = new UnrealLoadsComponent(_state, _timer, _gameMemory, _settings);
		}

		[Theory, CombinatorialData]
		public void WhenMapChangeTwice_EmptyMapList_ShouldSplitTwiceWithAnySplitHistorySetting(bool splitOnlyOnce)
		{
			// Arrange
			_settings.AutoSplitOncePerMap = splitOnlyOnce;

			_settings.Maps.Clear();
			var prevMap = _fixture.Create<string>();
			var nextMap = _fixture.Create<string>();
			_gameMemory.OnMapChange += Raise.Event<MapChangeEventHandler>(_gameMemory, prevMap, nextMap);

			// Act
			_gameMemory.OnMapChange += Raise.Event<MapChangeEventHandler>(_gameMemory, prevMap, nextMap);

			// Assert
			_timer.ReceivedWithAnyArgs(2).Split();
		}

		[Theory]
		[MemberData(nameof(GetCasesThatShouldSplit), true)]
		[MemberData(nameof(GetCasesThatShouldSplit), false)]
		public void WhenMapChangeTwice_ShouldSplitOnlyOnceIfOptionEnabled(Map exitMap, Map enterMap, bool splitOnlyOnce)
		{
			// Arrange
			_settings.AutoSplitOncePerMap = splitOnlyOnce;

			if (exitMap != null) _settings.Maps.Add(exitMap);
			if (enterMap != null) _settings.Maps.Add(enterMap);
			var prevMap = exitMap?.Name.ToUpperInvariant() ?? _fixture.Create<string>();
			var nextMap = enterMap?.Name.ToUpperInvariant() ?? _fixture.Create<string>();
			_gameMemory.OnMapChange += Raise.Event<MapChangeEventHandler>(_gameMemory, prevMap, nextMap);

			// Act
			_gameMemory.OnMapChange += Raise.Event<MapChangeEventHandler>(_gameMemory, prevMap, nextMap);

			// Assert
			_timer.ReceivedWithAnyArgs(splitOnlyOnce ? 1 : 2).Split();
		}

		public static IEnumerable<object[]> GetCasesThatShouldSplit(bool splitonlyonce)
		{
			var exitMapName = Guid.NewGuid().ToString().ToLowerInvariant();
			var enterMapName = Guid.NewGuid().ToString().ToLowerInvariant();

			// 1. leaving an unlisted map, entering an enabled map
			yield return new object[]
			{
				null,
				new Map(enterMapName) { SplitOnEnter = true },
				splitonlyonce
			};
			// 2. leaving an enabled map, entering an unlisted map
			yield return new object[]
			{
				new Map(exitMapName) { SplitOnLeave = true },
				null,
				splitonlyonce
			};
			// 3. leaving a listed map, entering an enabled map
			yield return new object[]
			{
				new Map(exitMapName),
				new Map(enterMapName) { SplitOnEnter = true },
				splitonlyonce
			};
			// 4. leaving an enabled map, entering a listed map
			yield return new object[]
			{
				new Map(exitMapName) { SplitOnLeave = true },
				new Map(enterMapName),
				splitonlyonce
			};
			// 5. leaving and entering enabled maps
			yield return new object[]
			{
				new Map(exitMapName) { SplitOnLeave = true },
				new Map(enterMapName) { SplitOnEnter = true },
				splitonlyonce
			};
		}
    }
}
