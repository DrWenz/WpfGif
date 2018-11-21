using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WpfGif.Frames;
using WpfGif.MetaData;

namespace WpfGif.Animation
{
    /// <summary>
    /// Creates the animation for the frames of a GIF image
    /// </summary>
    public static class GifAnimationCreator
    {
        #region CreateAnimationFromGif Constructors
        /// <summary>
        /// Create animation of a GIF Image
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static ObjectAnimationUsingKeyFrames CreateAnimationFromGif(string file)
        {
            if (System.IO.File.Exists(file))
                return CreateAnimationFromGif(System.IO.File.ReadAllBytes(file));
            else
                return null;
        }

        /// <summary>
        /// Create animation of a GIF Image
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static ObjectAnimationUsingKeyFrames CreateAnimationFromGif(byte[] gifRawData)
        {
            if (gifRawData != null)
            {
                using (MemoryStream MS = new MemoryStream(gifRawData))
                {
                    return CreateAnimationFromGif(MS);
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Create animation of a GIF Image
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static ObjectAnimationUsingKeyFrames CreateAnimationFromGif(Stream stream)
        {
            if (stream == null) return null;

            // Get the GifDecoder from the stream
            GifBitmapDecoder decoder =
                (GifBitmapDecoder)BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

            return CreateFrames(decoder);
        }

        /// <summary>
        /// Default Constructor.
        /// Create animation of a GIF Image
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static ObjectAnimationUsingKeyFrames CreateAnimationFromGif(GifBitmapDecoder decoder)
        {
            return CreateFrames(decoder);
        }
        #endregion

        #region Creator
        public static ObjectAnimationUsingKeyFrames CreateFrames(GifBitmapDecoder decoder)
        {
            if (decoder == null) return null;

            ObjectAnimationUsingKeyFrames animation = new ObjectAnimationUsingKeyFrames();

            // Read MetaData from GIF
            // this give us the Width and Height of the GIF
            GifMetaData gifData = GifMetaDataReader.GetGifMetaData(decoder);
            if (gifData == null) return null;

            TimeSpan animationTime = TimeSpan.FromMilliseconds(0);
            BitmapSource baseFrame = null;

            // create a animation for every frame in GIF
            foreach (BitmapFrame curFrame in decoder.Frames)
            {
                // first we need to read the metadata from the frame 
                // to know the position, width, height, delay and disposalmethod
                GifFrameMetaData frameMetaData = GifMetaDataReader.GetGifFrameMetaData(curFrame);
                if (frameMetaData != null)
                {
                    // Create a "image" from the current frame
                    BitmapSource CurCreatedFrame =
                        GifImageFrameCreator.CreateFrame(
                            new System.Drawing.Size(gifData.Width, gifData.Height),
                            curFrame, frameMetaData, baseFrame);

                    // add the frame to the animaton
                    animation.KeyFrames.Add(new DiscreteObjectKeyFrame()
                    {
                        KeyTime = animationTime,
                        Value = CurCreatedFrame
                    });

                    // Check the disposal method 
                    switch (frameMetaData.DisposalMethod)
                    {
                        case GifFrameDisposalMethod.None:
                            break;
                        case GifFrameDisposalMethod.DoNotDispose:
                            baseFrame = CurCreatedFrame;
                            break;
                        case GifFrameDisposalMethod.RestoreBackground:
                            baseFrame = GifImageFrameCreator.RestoreBackground(new System.Drawing.Size(gifData.Width, gifData.Height), curFrame, frameMetaData);
                            break;
                        case GifFrameDisposalMethod.RestorePrevious:
                            break;
                        default:
                            break;
                    }

                    // increse the timeline with the delay of the frame
                    animationTime += frameMetaData.Delay;
                }
            }

            animation.Duration = animationTime;

            return animation;
        }
        #endregion
    }
}
