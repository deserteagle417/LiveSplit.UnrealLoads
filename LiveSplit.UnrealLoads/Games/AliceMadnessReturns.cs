using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LiveSplit.UnrealLoads.Games
{
	class AliceMadnessReturns : GameSupport
	{
		public override HashSet<string> GameNames { get; } = new HashSet<string>
		{
			"Alice: Madness Returns",
			"Alice Madness Returns",
			"Alice 2"
		};

		public override HashSet<string> ProcessNames { get; } = new HashSet<string>
		{
			"AliceMadnessReturns"
		};

		public override HashSet<string> Maps { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Chapter1_L1_Alley_01",
            "Chapter1_L1_Cin_114",
            "Chapter1_L1_Cin_115",
            "Chapter1_L1_Cin_117",
            "Chapter1_L1_Doctor_Ext",
            "Chapter1_L1_Doctor_Ins",
            "Chapter1_L1_Doctor_Ins2",
            "Chapter1_L1_Doctor_Office",
            "Chapter1_L1_Doctor_S",
            "Chapter1_L1_LSP_Alley",
            "Chapter1_L1_LSP_DocLow",
            "Chapter1_L1_LSP_DocUp",
            "Chapter1_L1_LSP_Market",
            "Chapter1_L1_LSP_Pubs",
            "Chapter1_L1_LSP_Road",
            "Chapter1_L1_Minge_01",
            "Chapter1_L1_Minge_02",
            "Chapter1_L1_Minge_03",
            "Chapter1_L1_Minge_04",
            "Chapter1_L1_Minge_04_S",
            "Chapter1_L1_Minge_05",
            "Chapter1_L1_Minge_World",
            "Chapter1_L1_Music_01",
            "Chapter1_L1_P",
            "Chapter1_L1_Roof_01",
            "Chapter1_L1_Roof_01_S",
            "Chapter1_L1_Roof_02",
            "Chapter1_L1_Sound_01"
		};
	}
}
