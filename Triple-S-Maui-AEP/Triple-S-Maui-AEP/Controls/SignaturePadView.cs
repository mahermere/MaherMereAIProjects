using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Triple_S_Maui_AEP.Controls
{
    /// <summary>
    /// A custom control for capturing signatures via touch or mouse input
    /// </summary>
    public class SignaturePadView : GraphicsView
    {
        private List<PointF> _currentStroke = new();
        private readonly List<List<PointF>> _strokes = new();

        public static readonly BindableProperty StrokeColorProperty =
            BindableProperty.Create(nameof(StrokeColor), typeof(Color), typeof(SignaturePadView), Colors.Black);

        public static readonly BindableProperty StrokeWidthProperty =
            BindableProperty.Create(nameof(StrokeWidth), typeof(float), typeof(SignaturePadView), 2f);

        public Color StrokeColor
        {
            get => (Color)GetValue(StrokeColorProperty);
            set => SetValue(StrokeColorProperty, value);
        }

        public float StrokeWidth
        {
            get => (float)GetValue(StrokeWidthProperty);
            set => SetValue(StrokeWidthProperty, value);
        }

        public SignaturePadView()
        {
            Drawable = new SignatureDrawable(this);
            BackgroundColor = Colors.White;

            StartInteraction += OnStartInteraction;
            DragInteraction += OnDragInteraction;
            EndInteraction += OnEndInteraction;
            CancelInteraction += OnCancelInteraction;
        }

        public void Clear()
        {
            _strokes.Clear();
            _currentStroke.Clear();
            Invalidate();
        }

        public bool IsEmpty => _strokes.Count == 0;

        public async Task<string?> GetSignatureAsBase64Async()
        {
            try
            {
                var image = await this.CaptureAsync();
                if (image != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        // Save as PNG
                        await image.CopyToAsync(stream);
                        byte[] imageBytes = stream.ToArray();
                        return Convert.ToBase64String(imageBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error capturing signature: {ex.Message}");
            }

            return   null;
        }

        private void OnStartInteraction(object? sender, TouchEventArgs e)
        {
            if (e.Touches is null || !e.Touches.Any())
            {
                return;
            }

            _currentStroke = new List<PointF> { e.Touches.First() };
            _strokes.Add(_currentStroke);
            Invalidate();
        }

        private void OnDragInteraction(object? sender, TouchEventArgs e)
        {
            if (_currentStroke.Count == 0 || e.Touches is null || !e.Touches.Any())
            {
                return;
            }

            _currentStroke.Add(e.Touches.First());
            Invalidate();
        }

        private void OnEndInteraction(object? sender, TouchEventArgs e)
        {
            if (_currentStroke.Count == 1)
            {
                _currentStroke.Add(_currentStroke[0]);
                Invalidate();
            }
        }

        private void OnCancelInteraction(object? sender, EventArgs e)
        {
            _currentStroke = new List<PointF>();
        }

        private class SignatureDrawable : IDrawable
        {
            private readonly SignaturePadView _owner;

            public SignatureDrawable(SignaturePadView owner)
            {
                _owner = owner;
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                canvas.FillColor = _owner.BackgroundColor ?? Colors.White;
                canvas.FillRectangle(dirtyRect);

                canvas.StrokeColor = _owner.StrokeColor ?? Colors.Black;
                canvas.StrokeSize = _owner.StrokeWidth;
                canvas.StrokeLineCap = LineCap.Round;
                canvas.StrokeLineJoin = LineJoin.Round;

                // Draw completed strokes
                foreach (var stroke in _owner._strokes)
                {
                    if (stroke.Count > 1)
                    {
                        for (int i = 0; i < stroke.Count - 1; i++)
                        {
                            canvas.DrawLine(
                                stroke[i].X,
                                stroke[i].Y,
                                stroke[i + 1].X,
                                stroke[i + 1].Y
                            );
                        }
                    }
                    else if (stroke.Count == 1)
                    {
                        canvas.FillColor = _owner.StrokeColor ?? Colors.Black;
                        canvas.FillCircle(stroke[0].X, stroke[0].Y, Math.Max(1f, _owner.StrokeWidth / 2f));
                    }
                }

                // Draw current stroke
                if (_owner._currentStroke.Count > 1)
                {
                    for (int i = 0; i < _owner._currentStroke.Count - 1; i++)
                    {
                        canvas.DrawLine(
                            _owner._currentStroke[i].X,
                            _owner._currentStroke[i].Y,
                            _owner._currentStroke[i + 1].X,
                            _owner._currentStroke[i + 1].Y
                        );
                    }
                }
            }
        }
    }
}
