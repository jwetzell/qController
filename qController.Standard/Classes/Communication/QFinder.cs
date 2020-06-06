//Currently unused would like to reimplement eventually
//Currently being done in qConnectionPage.xaml.cs Scan() method
using Serilog;
using System.Collections.Generic;
using Zeroconf;

namespace qController
{
    public class QFinder
    {
        public QFinder()
        {
        }

		public async void SearchForWorkspaces()
		{
            Log.Debug("QFINDER - Scanning Started");
            IReadOnlyList<IZeroconfHost> results = await ZeroconfResolver.ResolveAsync("_qlab._udp.local.");
            Log.Debug("QFINDER - Scanning Done");
		}
    }
}
