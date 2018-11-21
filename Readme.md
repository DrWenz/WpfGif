WpfGif
===============

Add GIF support to a WPF Image Control.

How to get it
-------------

- Nuget Package
  > [WpfGif Nuget](https://www.nuget.org/packages/WpfGif/1.0.0.1)
    

- Get Release

  Download the current Release Version and add it to your project


How to use it
-------------
* Add the following header to the XAML file

  ```xml
  xmlns:GIF="clr-namespace:WpfGif;assembly=WpfGif"
  ```
* Set source to image

  ```xaml
  <Image GIF:GifImageBehavior.GIFSource="C:\Users\marku\Pictures\GIF\earth.gif" />
  ```
