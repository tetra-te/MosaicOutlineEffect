using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace MosaicOutlineEffect
{
    [VideoEffect("モザイク縁取り", ["装飾"], ["mosaic outline"], isAviUtlSupported: false, isEffectItemSupported: false)]
    internal class MosaicOutlineEffect : VideoEffectBase
    {
        public override string Label => "モザイク縁取り";

        [Display(GroupName = "モザイク縁取り", Name = "太さ", Description = "縁の太さ")]
        [AnimationSlider("F1", "px", 0, 10)]
        public Animation StrokeThickness { get; } = new Animation(3, 0, 500);

        [Display(GroupName = "モザイク縁取り", Name = "サイズ", Description = "モザイクのブロックの大きさ")]
        [AnimationSlider("F1", "px", 1, 20)]
        public Animation MosaicSize { get; } = new Animation(10, 1, 2000);

        [Display(GroupName = "モザイク縁取り", Name = "不透明度", Description = "不透明度")]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation Opacity { get; } = new Animation(100, 0, 100);

        [Display(GroupName = "モザイク縁取り", Name = "精度", Description = "縁取りの正確さ\r\n小さいほど軽い")]
        [AnimationSlider("F1", "%", 0.1, 100)]
        public Animation Accuracy { get; } = new Animation(50, 0.1, 100);

        [Display(GroupName = "モザイク縁取り", Name = "色", Description = "縁の色")]
        [ColorPicker]
        public Color Color { get => color; set => Set(ref color, value); }
        Color color = Colors.White;

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new MosaicOutlineEffectProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [StrokeThickness, MosaicSize, Opacity, Accuracy];
    }
}
