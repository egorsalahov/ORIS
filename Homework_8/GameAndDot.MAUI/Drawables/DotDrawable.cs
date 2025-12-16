using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using GameAndDot.MAUI;

namespace GameAndDot.MAUI.Drawables
{
    // умеет рисовать на GraphicsView
    public class DotDrawable : IDrawable
    {
        public ObservableCollection<Dot> Dots { get; set; } = new();

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float dotRadius = 5f; // Диаметр 10px, радиус 5px

            foreach (var dot in Dots)
            {
                canvas.FillColor = dot.Color;
                // Рисуем круг 
                canvas.FillCircle(dot.X, dot.Y, dotRadius);
            }
        }
    }
}
