//Currently unused would like to reimplement eventually
//Currently being done in qConnectionPage.xaml.cs Scan() method
//TODO: Implement this along with a way of getting active Instances and # of workspaces for an instance
using Serilog;
using System.Collections.Generic;
using Zeroconf;

namespace qController.Communication
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
