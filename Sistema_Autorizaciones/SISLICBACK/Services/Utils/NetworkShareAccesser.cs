using System.Runtime.InteropServices;

namespace SISLICBACK.Services.Utils {
    public class NetworkShareAccesser : IDisposable {
        private readonly string _networkName;

        public NetworkShareAccesser(string networkName, string userName, string password) {
            _networkName = networkName;

            var netResource = new NETRESOURCE {
                dwType = 1, // RESOURCETYPE_DISK
                lpRemoteName = networkName
            };

            int result = WNetAddConnection2(netResource, password, userName, 0);

            if(result != 0)
                throw new InvalidOperationException("Error al conectar a red: " + result);
        }

        ~NetworkShareAccesser() {
            Dispose();
        }

        public void Dispose() {
            WNetCancelConnection2(_networkName, 0, true);
            GC.SuppressFinalize(this);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NETRESOURCE netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);

        [StructLayout(LayoutKind.Sequential)]
        private class NETRESOURCE {
            public int dwScope = 0;
            public int dwType = 0;
            public int dwDisplayType = 0;
            public int dwUsage = 0;
            public string lpLocalName = null;
            public string lpRemoteName = null;
            public string lpComment = null;
            public string lpProvider = null;
        }
    }
}
