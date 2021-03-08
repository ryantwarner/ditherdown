using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using SerializableAttribute = System.SerializableAttribute;

[System.Serializable, VolumeComponentMenu("Post-processing/RTWarner/DitherDown")]
public sealed class DitherDown : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    #region Local enum and wrapper class
        public enum DitherType { Bayer2x2, Bayer3x3, Bayer4x4, Bayer8x8 }
        [Serializable] public sealed class DitherTypeParameter : VolumeParameter<DitherType> {}
    #endregion

    #region Exposed parameters

    public DitherTypeParameter ditherType = new DitherTypeParameter { value = DitherType.Bayer2x2 };
    public ClampedFloatParameter dithering = new ClampedFloatParameter(0f, 0, 0.5f);
    public ClampedFloatParameter downsampling = new ClampedFloatParameter(1.0f, 1.0f, 32.0f);

    #endregion

    #region Private variables

    static class IDs
    {
        internal static readonly int Dithering = Shader.PropertyToID("_Dithering");
        internal static readonly int Downsampling = Shader.PropertyToID("_Downsampling");
        internal static readonly int InputTexture = Shader.PropertyToID("_InputTexture");
        internal static readonly int DitherTexture = Shader.PropertyToID("_DitherTexture");
    }

    Material _material;

    DitherType _ditherType;
    Texture2D _ditherTexture;

    #endregion

    #region Postprocess effect implementation

    public bool IsActive() => _material != null;

    public override CustomPostProcessInjectionPoint injectionPoint =>
        CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        _material = CoreUtils.CreateEngineMaterial("Hidden/Shader/DitherDown");
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle srcRT, RTHandle destRT)
    {
        if (_ditherType != ditherType.value || _ditherTexture == null)
        {
            CoreUtils.Destroy(_ditherTexture);
            _ditherType = ditherType.value;
            _ditherTexture = GenerateDitherTexture(_ditherType);
        }

        if (_material == null) return;
        _material.SetFloat(IDs.Dithering, dithering.value);
        _material.SetFloat(IDs.Downsampling, downsampling.value);

        _material.SetTexture(IDs.InputTexture, srcRT);
        _material.SetTexture(IDs.DitherTexture, _ditherTexture);
        HDUtils.DrawFullScreen(cmd, _material, destRT);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(_material);
    }

    #endregion

    #region Dither texture generator

        static Texture2D GenerateDitherTexture(DitherType type)
        {
            if (type == DitherType.Bayer2x2)
            {
                var tex = new Texture2D(2, 2, TextureFormat.R8, false, true);
                tex.LoadRawTextureData(new byte [] {0, 170, 255, 85});
                tex.Apply();
                return tex;
            }

            if (type == DitherType.Bayer3x3)
            {
                var tex = new Texture2D(3, 3, TextureFormat.R8, false, true);
                tex.LoadRawTextureData(new byte [] {
                    0, 223, 95, 191, 159, 63, 127, 31, 255
                });
                tex.Apply();
                return tex;
            }

            if (type == DitherType.Bayer4x4)
            {
                var tex = new Texture2D(4, 4, TextureFormat.R8, false, true);
                tex.LoadRawTextureData(new byte [] {
                    0, 136, 34, 170, 204, 68, 238, 102,
                    51, 187, 17, 153, 255, 119, 221, 85
                });
                tex.Apply();
                return tex;
            }

            if (type == DitherType.Bayer8x8)
            {
                var tex = new Texture2D(8, 8, TextureFormat.R8, false, true);
                tex.LoadRawTextureData(new byte [] {
                    0, 194, 48, 242, 12, 206, 60, 255,
                    129, 64, 178, 113, 141, 76, 190, 125,
                    32, 226, 16, 210, 44, 238, 28, 222,
                    161, 97, 145, 80, 174, 109, 157, 93,
                    8, 202, 56, 250, 4, 198, 52, 246,
                    137, 72, 186, 121, 133, 68, 182, 117,
                    40, 234, 24, 218, 36, 230, 20, 214,
                    170, 105, 153, 89, 165, 101, 149, 85
                });
                tex.Apply();
                return tex;
            }

            return null;
        }

        #endregion
}
