//Currently deprecated would like to resurrect

using System;
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
            
            IReadOnlyList<IZeroconfHost> results = await ZeroconfResolver.ResolveAsync("_qlab._udp.local.");
            foreach (var result in results){
                Console.WriteLine(result.IPAddress);
            }
            Console.WriteLine("Done");
		}
    }
}
