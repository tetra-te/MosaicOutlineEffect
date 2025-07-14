using System.Numerics;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project.Effects;

namespace MosaicOutlineEffect
{
    internal class MosaicOutlineEffectProcessor : IVideoEffectProcessor
    {
        MosaicOutlineEffect item;

        DisposeCollector disposer = new();

        AffineTransform2D scaleEffect1;
        AffineTransform2D scaleEffect2;

        OutlineEffect outlineEffect;
        IVideoEffectProcessor outlineEffectProcessor;

        D2D1CustomShaderEffectBase mosaicEffect;

        AlphaFillCustomEffect alphaFillEffect;

        Opacity opacityEffect;

        Composite compositeEffect;

        bool isFirst = true;
        float r, g, b;
        double opacity, accuracy;

        public ID2D1Image Output { get; }

        public MosaicOutlineEffectProcessor(IGraphicsDevicesAndContext devices, MosaicOutlineEffect item)
        {
            this.item = item;
         
            outlineEffect = new OutlineEffect();
            outlineEffectProcessor = outlineEffect.CreateVideoEffect(devices);
            disposer.Collect(outlineEffectProcessor);
            scaleEffect1 = new AffineTransform2D(devices.DeviceContext);
            disposer.Collect(scaleEffect1);
            scaleEffect2 = new AffineTransform2D(devices.DeviceContext);
            disposer.Collect(scaleEffect2);
            mosaicEffect = (D2D1CustomShaderEffectBase)Activator.CreateInstance(TypeAndProperty.TypeofMosaic, [devices]);
            disposer.Collect(mosaicEffect);
            alphaFillEffect = new AlphaFillCustomEffect(devices);
            disposer.Collect(alphaFillEffect);
            opacityEffect = new Opacity(devices.DeviceContext);
            disposer.Collect(opacityEffect);
            compositeEffect = new Composite(devices.DeviceContext);
            disposer.Collect(compositeEffect);

            using (var image = scaleEffect1.Output)
            {
                outlineEffectProcessor.SetInput(image);
            }
            using (var image = outlineEffectProcessor.Output)
            {
                scaleEffect2.InterPolationMode = AffineTransform2DInterpolationMode.NearestNeighbor;
                scaleEffect2.SetInput(0, image, true);
            }
            using (var image = scaleEffect2.Output)
            {
                mosaicEffect.SetInput(0, image, true);
            }
            using (var image = mosaicEffect.Output)
            {
                alphaFillEffect.Threshold = 1 / 255f;
                alphaFillEffect.SetInput(0, image, true);
            }
            using (var image = alphaFillEffect.Output)
            {
                opacityEffect.SetInput(0, image, true);
            }
            using (var image = opacityEffect.Output)
            {
                compositeEffect.SetInput(0, image, true);
            }

            Output = compositeEffect.Output;
            disposer.Collect(Output);
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemPosition.Frame;
            var fps = effectDescription.FPS;

            var accuracy = item.Accuracy.GetValue(frame, length, fps) / 100;

            var strokeThickness = item.StrokeThickness.GetValue(frame, length, fps) * accuracy;
            SetAnimationValue(outlineEffect.StrokeThickness, strokeThickness);

            var mosaicSize = item.MosaicSize.GetValue(frame, length, fps);
            TypeAndProperty.SizeProp.SetValue(mosaicEffect, (float)mosaicSize);

            var r = item.Color.R / 255f;
            var g = item.Color.G / 255f;
            var b = item.Color.B / 255f;

            var opacity = item.Opacity.GetValue(frame, length, fps) / 100;

            if (isFirst || this.r != r)
            {
                alphaFillEffect.R = r;
                this.r = r;
            }
            if (isFirst || this.g != g)
            {
                alphaFillEffect.G = g;
                this.g = g;
            }
            if (isFirst || this.b != b)
            {
                alphaFillEffect.B = b;
                this.b = b;
            }
            if (isFirst || this.opacity != opacity)
            {
                opacityEffect.Value = (float)opacity;
                this.opacity = opacity;
            }
            if (isFirst || this.accuracy != accuracy)
            {
                scaleEffect1.TransformMatrix = Matrix3x2.CreateScale((float)accuracy);
                scaleEffect2.TransformMatrix = Matrix3x2.CreateScale(1 / (float)accuracy);
                this.accuracy = accuracy;
            }
            
            isFirst = false;

            outlineEffectProcessor.Update(effectDescription);
            
            return effectDescription.DrawDescription;
        }

        static void SetAnimationValue(Animation animation, double value)
        {
            var current = animation.GetValue(0, 1, 30);
            animation.AddToEachValues(value - current);
        }

        public void SetInput(ID2D1Image? input)
        {
            compositeEffect.SetInput(1, input, true);
            scaleEffect1.InterPolationMode = AffineTransform2DInterpolationMode.NearestNeighbor;
            scaleEffect1.SetInput(0, input, true);
        }

        public void ClearInput()
        {
            compositeEffect.SetInput(0, null, true);
            compositeEffect.SetInput(1, null, true);
            scaleEffect1.SetInput(0, null, true);
            scaleEffect2.SetInput(0, null, true);
            outlineEffectProcessor.ClearInput();
            mosaicEffect.SetInput(0, null, true);
            alphaFillEffect.SetInput(0, null, true);
            opacityEffect.SetInput(0, null, true);
        }

        public void Dispose()
        {
            disposer.Dispose();
        }    
    }
}