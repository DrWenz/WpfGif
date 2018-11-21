using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfGif.MetaData
{
    internal enum GifFrameDisposalMethod
    {
        /// <summary>
        /// Do nothing
        /// </summary>
        None = 0,

        /// <summary>
        /// Do not dispose
        /// </summary>
        DoNotDispose = 1,

        /// <summary>
        /// Restore the background
        /// </summary>
        RestoreBackground = 2,

        /// <summary>
        /// Restore the previus image
        /// </summary>
        RestorePrevious = 3
    }
}
