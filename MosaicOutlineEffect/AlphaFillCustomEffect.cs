using System.Runtime.InteropServices;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace MosaicOutlineEffect
{
    internal class AlphaFillCustomEffect : D2D1CustomShaderEffectBase
    {
        public float Threshold
        {
            set => SetValue((int)EffectImpl.Properties.Threshold, value);
            get => GetFloatValue((int)EffectImpl.Properties.Threshold);
        }

        public float R
        {
            set => SetValue((int)EffectImpl.Properties.R, value);
            get => GetFloatValue((int)EffectImpl.Properties.R);
        }

        public float G
        {
            set => SetValue((int)EffectImpl.Properties.G, value);
            get => GetFloatValue((int)EffectImpl.Properties.G);
        }

        public float B
        {
            set => SetValue((int)EffectImpl.Properties.B, value);
            get => GetFloatValue((int)EffectImpl.Properties.B);
        }

        public float Invert
        {
            set => SetValue((int)EffectImpl.Properties.Invert, value);
            get => GetFloatValue((int)EffectImpl.Properties.Invert);
        }

        public float KeepColor
        {
            set => SetValue((int)EffectImpl.Properties.KeepColor, value);
            get => GetFloatValue((int)EffectImpl.Properties.KeepColor);
        }

        public AlphaFillCustomEffect(IGraphicsDevicesAndContext devices) : base(Create<EffectImpl>(devices))
        {
        }


        [CustomEffect(1)]
        class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
        {
            ConstantBuffer constantBuffer;

            [CustomEffectProperty(PropertyType.Float, (int)Properties.Threshold)]
            public float Threshold
            {
                get => constantBuffer.Threshold;
                set
                {
                    constantBuffer.Threshold = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.R)]
            public float R
            {
                get => constantBuffer.R;
                set
                {
                    constantBuffer.R = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.G)]
            public float G
            {
                get => constantBuffer.G;
                set
                {
                    constantBuffer.G = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.B)]
            public float B
            {
                get => constantBuffer.B;
                set
                {
                    constantBuffer.B = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.Invert)]
            public float Invert
            {
                get => constantBuffer.Invert;
                set
                {
                    constantBuffer.Invert = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.KeepColor)]
            public float KeepColor
            {
                get => constantBuffer.KeepColor;
                set
                {
                    constantBuffer.KeepColor = value;
                    UpdateConstants();
                }
            }

            public EffectImpl() : base(ShaderResourceLoader.GetShaderResource("PixelShader.cso"))
            {

            }

            protected override void UpdateConstants()
            {
                drawInformation?.SetPixelShaderConstantBuffer(constantBuffer);
            }

            public override void MapInputRectsToOutputRect(Vortice.RawRect[] inputRects, Vortice.RawRect[] inputOpaqueSubRects, out Vortice.RawRect outputRect, out Vortice.RawRect outputOpaqueSubRect)
            {
                base.MapInputRectsToOutputRect(inputRects, inputOpaqueSubRects, out outputRect, out outputOpaqueSubRect);
            }

            public override void MapOutputRectToInputRects(Vortice.RawRect outputRect, Vortice.RawRect[] inputRects)
            {
                base.MapOutputRectToInputRects(outputRect, inputRects);
            }

            [StructLayout(LayoutKind.Sequential)]
            struct ConstantBuffer
            {
                public float Threshold;
                public float R;
                public float G;
                public float B;
                public float Invert;
                public float KeepColor;
            }
            public enum Properties
            {
                Threshold = 0,
                R = 1,
                G = 2,
                B = 3,
                Invert = 4,
                KeepColor = 5,
            }
        }
    }
}