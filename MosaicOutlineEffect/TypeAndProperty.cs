using System.Reflection;

namespace MosaicOutlineEffect
{
    internal static class TypeAndProperty
    {
        static Assembly assembly = Assembly.Load("YukkuriMovieMaker");
        
        public static Type? TypeofMosaic = assembly.GetType("YukkuriMovieMaker.Player.Video.Effects.CustomEffects.Mosaic");

        public static PropertyInfo? SizeProp = TypeofMosaic?.GetProperty("Size");
    }
}