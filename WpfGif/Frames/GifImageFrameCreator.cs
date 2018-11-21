using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfGif.MetaData;

namespace WpfGif.Frames
{
    /// <summary>
    /// Creates the frames to display from a GIF Image
    /// </summary>
    internal static class GifImageFrameCreator
    {
        #region Create Frame
        /// <summary>
        /// Create a <see cref="BitmapSource"/> that presents a frame from a GIF File
        /// </summary>
        /// <param name="size">Full Size of the GIF Image. See: <see cref="GifMetaData"/></param>
        /// <param name="decoderFrame">Actuel frame from a <see cref="GifBitmapDecoder"/></param>
        /// <param name="frameMetadata"><see cref="GifFrameMetaData from the <paramref name="decoderFrame"/>"/>"/></param>
        /// <param name="viewingFrame">Last viewing Frame</param>
        /// <returns></returns>
        public static BitmapSource CreateFrame(Size size, BitmapSource decoderFrame, GifFrameMetaData frameMetadata, BitmapSource viewingFrame)
        {
            // if vieweingFrame is empty and the frame is full size, we don't need to overdraw anything
            // return the raw frame from the decoder
            if (viewingFrame == null && IsFullFrame(frameMetadata, size))
                return decoderFrame;

            DrawingVisual drawVis = new DrawingVisual();
            using (DrawingContext context = drawVis.RenderOpen())
            {
                // if there is a frame from last frame draw it as "background"
                if (viewingFrame != null)
                {
                    System.Windows.Rect fullRect = new System.Windows.Rect(0, 0, size.Width, size.Height);
                    context.DrawImage(viewingFrame, fullRect);
                }

                // now draw the current frame from decoder 
                System.Windows.Rect rect =
                    new System.Windows.Rect(
                        frameMetadata.Left, frameMetadata.Top,
                        frameMetadata.Width, frameMetadata.Height);
                context.DrawImage(decoderFrame, rect);
            }

            // create a Bitmap from the drawingvisual
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                size.Width, size.Height, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(drawVis);

            return CreateWriteableBitmap(renderBitmap);
        }
        #endregion

        #region Clear Frame
        /// <summary>
        /// Restores the background of the viewing frame for the next frame
        /// </summary>
        /// <param name="size">Full Size of the GIF Image. See: <see cref="GifMetaData"/></param>
        /// <param name="frame">Actuel frame from a <see cref="GifBitmapDecoder"/></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static BitmapSource RestoreBackground(Size size, BitmapSource frame, GifFrameMetaData metadata)
        {
            // if the new frame is full size of the GIF Image we don't need to clear a area
            // the new image will draw over the full size
            if (IsFullFrame(metadata, size))
                return null;
            else
                return ClearFrameArea(frame, metadata);
        }

        /// <summary>
        /// Clears a Area in a frame
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private static BitmapSource ClearFrameArea(BitmapSource frame, GifFrameMetaData metadata)
        {
            DrawingVisual visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                // Clear the place where the new image is placed
                System.Windows.Rect fullRect = new System.Windows.Rect(0, 0, frame.PixelWidth, frame.PixelHeight);
                System.Windows.Rect clearRect = new System.Windows.Rect(metadata.Left, metadata.Top, metadata.Width, metadata.Height);
                PathGeometry clip = Geometry.Combine( new RectangleGeometry(fullRect), new RectangleGeometry(clearRect), GeometryCombineMode.Exclude, null);
                context.PushClip(clip);
                context.DrawImage(frame, fullRect);
            }

            var bitmap = new RenderTargetBitmap(
                    frame.PixelWidth, frame.PixelHeight,
                    frame.DpiX, frame.DpiY,
                    PixelFormats.Pbgra32);
            bitmap.Render(visual);

            var result = new WriteableBitmap(bitmap);

            if (result.CanFreeze && !result.IsFrozen)
                result.Freeze();
            return result;
        }
        #endregion

        #region Full Size
        /// <summary>
        /// Checks if a frame is full size of the GIF Image
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="fullSize"></param>
        /// <returns></returns>
        private static bool IsFullFrame(GifFrameMetaData metadata, Size fullSize)
        {
            return metadata.Left == 0
                   && metadata.Top == 0
                   && metadata.Width == fullSize.Width
                   && metadata.Height == fullSize.Height;
        }
        #endregion

        #region WriteableBitmap
        /// <summary>
        /// Creates a <see cref="WriteableBitmap"/> from a <see cref="RenderTargetBitmap"/>.
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns>
        /// Returns a freezed <see cref="WriteableBitmap"/> if possible
        /// </returns>
        private static WriteableBitmap CreateWriteableBitmap(RenderTargetBitmap bmp)
        {
            WriteableBitmap frame = new WriteableBitmap(bmp);

            // Freeze the bitmap for better performance
            if (frame.CanFreeze && frame.IsFrozen)
                frame.Freeze();

            return frame;
        }
        #endregion
    }
}
