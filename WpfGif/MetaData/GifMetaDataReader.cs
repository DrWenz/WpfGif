using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfGif.MetaData
{
    /// <summary>
    /// Metadata reader for GIF Image and it's frames
    /// </summary>
    internal static class GifMetaDataReader
    {
        /// <summary>
        /// Reads the <see cref="GifMetaData"/> for a GIF Image from a <see cref="BitmapDecoder"/>
        /// </summary>
        /// <param name="decoder"></param>
        /// <returns>
        /// Returns NULL if <see cref="BitmapDecoder.Metadata"/> is not from type <see cref="BitmapMetadata"/> or querys "/logscrdesc" not exist.
        /// </returns>
        public static GifMetaData GetGifMetaData(BitmapDecoder decoder)
        {
            if (decoder.Metadata is BitmapMetadata decoderMetaData)
            {
                BitmapMetadata logsrcdesc = (BitmapMetadata)decoderMetaData.GetQuery("/logscrdesc");
                if (logsrcdesc != null)
                {
                    return new GifMetaData(
                        logsrcdesc.GetQueryValue("/Width", 0),
                        logsrcdesc.GetQueryValue("/Height", 0));
                }
                return
                    null;
            }
            else
                return null;
        }

        /// <summary>
        /// Reads the <see cref="GifFrameMetaData"/> from a Frame in a GIF File
        /// </summary>
        /// <param name="frame"></param>
        /// <returns>
        /// Returns NULL if <see cref="BitmapFrame.InternalMetadata"/> is not from type <see cref="BitmapMetadata"/> or querys "/grctlext" & "/imgdesc" not exist.
        /// </returns>
        public static GifFrameMetaData GetGifFrameMetaData(BitmapFrame frame)
        {
            if (frame.Metadata is BitmapMetadata frameMetaData)
            {
                BitmapMetadata grctlext = (BitmapMetadata)frameMetaData.GetQuery("/grctlext");
                BitmapMetadata imgdesc = (BitmapMetadata)frameMetaData.GetQuery("/imgdesc");

                if (grctlext != null && imgdesc != null)
                {
                    var test = frameMetaData.GetQuery("/grctlext/Disposal");

                    return new GifFrameMetaData(
                        imgdesc.GetQueryValue("/Left", 0),
                        imgdesc.GetQueryValue("/Top", 0),
                        imgdesc.GetQueryValue("/Width", 0),
                        imgdesc.GetQueryValue("/Height", 0),
                        TimeSpan.FromMilliseconds(grctlext.GetQueryValue("/Delay", 10) * 10),
                        (GifFrameDisposalMethod)grctlext.GetQueryValue("/Disposal", 0));
                }
                else
                    return null;
            }
            else
                return null;
        }
    }
}
