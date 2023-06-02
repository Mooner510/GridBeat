using System;

namespace Utils {
    public static class NumberUtils {
        public static int Between(this int value, int max, int min) => Math.Min(Math.Max(value, min), max);
        
        public static long Between(this long value, long max, long min) => Math.Min(Math.Max(value, min), max);
        
        public static double Between(this double value, double max, double min) => Math.Min(Math.Max(value, min), max);
        
        public static float Between(this float value, float max, float min) => Math.Min(Math.Max(value, min), max);
    }
}