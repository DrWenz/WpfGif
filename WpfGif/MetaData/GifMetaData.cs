using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfGif.MetaData
{
    /// <summary>
    /// Metadata about a GIF Image
    /// </summary>
    internal class GifMetaData
    {
        #region Public Deklaration
        /// <summary>
        /// Width of the GIF Image
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Height of the GIF Image
        /// </summary>
        public int Height { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public GifMetaData(int width, int height)
        {
            Width = width;
            Height = height;
        }
        #endregion
    }
}
