using System.Windows;
using System.Windows.Controls;

namespace PredprofInterpreter
{
    class PlayerBorder : Border
    {
        public static readonly DependencyProperty XPositionProperty;
        public static readonly DependencyProperty YPositionProperty;

        public int XPosition
        {
            get => (int)GetValue(XPositionProperty);
            set => SetValue(XPositionProperty, value);
        }
        public int YPosition
        {
            get => (int)GetValue(YPositionProperty);
            set => SetValue(YPositionProperty, value);
        }

        static PlayerBorder()
        {
            XPositionProperty = DependencyProperty.Register(
                "XPosition",
                typeof(int),
                typeof(PlayerBorder),
                new FrameworkPropertyMetadata(
                    0,
                    (d, e) => { },
                    (d, baseValue) =>
                    {
                        if (baseValue is int value)
                        {
                            if (value < 0)
                                value = 0;

                            if (value > 20)
                                value = 20;

                            Grid.SetColumn((UIElement)d, value * 2 + 2);
                            return value;
                        }
                        return 0;
                    }
                    )
                );
            YPositionProperty = DependencyProperty.Register(
                "YPosition",
                typeof(int),
                typeof(PlayerBorder),
                new FrameworkPropertyMetadata(
                    0,
                    (d, e) => { },
                    (d, baseValue) =>
                    {
                        if (baseValue is int value)
                        {
                            if (value < 0)
                                value = 0;

                            if (value > 20)
                                value = 20;

                            Grid.SetRow((UIElement)d, 40 - value * 2);
                            return value;
                        }
                        return 0;
                    }
                    )
                );
        }
    }
}
