using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace T2Tools
{
    public static class Extensions
    {
        public static void DoubleBuffered(this Control control, bool enable)
        {
            var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static void DrawCircle(this Graphics g, Pen pen,
                                        float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                            radius + radius, radius + radius);
        }

        public static void FillCircle(this Graphics g, Brush brush,
                                        float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                            radius + radius, radius + radius);
        }

    }
}
