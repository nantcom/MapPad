using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if WINDOWS_APP

using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

#endif

namespace NantCom.MapPad.Core
{
    public class SimpleProcess

#if WINDOWS_APP
        : NantCom.MapPad.Lib.NotifyPropertyChangedBase
#endif
    {
        /// <summary>
        /// Process Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Windows title of this process
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The icon
        /// </summary>
        public byte[] Icon { get; set; }

#if WINDOWS_APP
        private BitmapImage _Icon;

        public BitmapImage IconSource
        {
            get
            {
                if (_Icon == null)
                {
                    _Icon = new BitmapImage();
                    var sync = SynchronizationContext.Current;
                    Task.Run(async () =>
                    {
                        InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream();
                        await ms.WriteAsync(this.Icon.AsBuffer());
                        ms.Seek(0);

                        sync.Post(async (state) =>
                        {
                            await _Icon.SetSourceAsync(ms);

                            ms.Dispose();

                        }, null);

                    });
                }

                return _Icon;
            }
        }
#endif
    }
}
