using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UnrealLoads
{
	public interface IUnrealLoadsSettings
	{
		bool AutoReset { get; set; }
		bool AutoSplitOncePerMap { get; set; }
		bool AutoSplitOnMapChange { get; set; }
		bool AutoStart { get; set; }
		bool DbgShowMap { get; set; }
		IList<Map> Maps { get; }

		XmlNode GetSettings(XmlDocument doc);
		void SetSettings(XmlNode settings);

		Control UserControl { get; }
	}
}
