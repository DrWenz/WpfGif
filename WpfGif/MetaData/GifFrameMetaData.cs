using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfGif.MetaData
{
    /// <summary>
    /// Metadata about a frame in a GIF Image
    /// </summary>
    internal class GifFrameMetaData
    {
        #region Public Deklaration

        /// <summary>
        /// Left position of the frame
        /// </summary>
        public int Left { get; }

        /// <summary>
        /// Top position of the frame
        /// </summary>
        public int Top { get; }

        /// <summary>
        /// Width of the Frame
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Height of the Frame
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// time span until next frame
        /// </summary>
        public TimeSpan Delay { get; }

        /// <summary>
        /// Disposal mode for the frame.
        /// Tell us how to draw the next picture.
        /// </summary>
        public GifFrameDisposalMethod DisposalMethod { get; }

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="delay"></param>
        /// <param name="disposalMethod"></param>
        public GifFrameMetaData(int left, int top, int width, int height, TimeSpan delay, GifFrameDisposalMethod disposalMethod)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            Delay = delay;
            DisposalMethod = disposalMethod;
        }
        #endregion
    }
}
