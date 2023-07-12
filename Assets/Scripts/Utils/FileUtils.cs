using System;
using System.IO;

namespace Utils {
    public static class FileUtils {
        public static string[] GetDirectories(string path) {
            try {
                return Directory.GetDirectories(path);
            } catch {
                return Array.Empty<string>();
            }
        }
    }
}