# DitherDown

**DitherDown** is custom post-processing effect for Unity's [High Definition Render Pipeline][hdrp] (HDRP) that dithers and downsamples the entire screen but does not modify color. Based on [Kino][kino] recolor and [Kino Eight][kinoeight].

[hdrp]: https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@7.4
[kino]: https://github.com/keijiro/Kino/tree/upm
[kinoeight]: https://github.com/keijiro/KinoEight/tree/master/Packages/jp.keijiro.kino.post-processing.eight

## System Requirements

This package was built in:

- Unity 2019.4.19f1
- HDRP 7.4.1

Tested with

- Unity 2020.2.6f1
- HDRP 10.3.1

### Dither

![gif](https://i.imgur.com/UYWkxsf.gif)

### Dither Type

Options include: Bayer2x2, 3x3, 4x4, 8x8

![gif](https://imgur.com/9z8duuA.gif)

### Downsample

Note: Non-integer values result in rectangular pixels.

![gif](https://imgur.com/ARgDtMb.gif)

## How to use

Add the package to your project using the package manager's add package from git url option.

Click Edit -> Project Settings -> HDRP Default Settings and add DitherDown to the After Post Process section of Custom Post Process Orders.

In a post processing volume add the Post Processing -> RTWarner -> DitherDown override and configure to suit your needs.

## License

[Unlicense](https://unlicense.org/)
