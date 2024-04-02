using System.Windows;
using System.Windows.Media;

namespace Loop.Revit.FavouriteViews
{
    public sealed partial class ViewControl
    {

        public static readonly DependencyProperty FilterTextProperty = DependencyProperty.Register(
            nameof(FilterText), typeof(string), typeof(ViewControl), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty SearchViewNameProperty = DependencyProperty.Register(
            nameof(SearchViewNameEnabled), typeof(bool), typeof(ViewControl), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty SearchSheetNameProperty = DependencyProperty.Register(
            nameof(SearchSheetNameEnabled), typeof(bool), typeof(ViewControl), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty SearchSheetNumberProperty = DependencyProperty.Register(
            nameof(SearchSheetNumberEnabled), typeof(bool), typeof(ViewControl), new PropertyMetadata(default(bool)));
        
        public static readonly DependencyProperty HighlightColorProperty = DependencyProperty.Register(
            nameof(HighlightColor), typeof(Brush), typeof(ViewControl), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register(
            nameof(TextColor), typeof(Brush), typeof(ViewControl), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty TextAccentColorProperty = DependencyProperty.Register(
            nameof(TextAccentColor), typeof(Brush), typeof(ViewControl), new PropertyMetadata(default(Brush)));

        public Brush TextAccentColor
        {
            get { return (Brush)GetValue(TextAccentColorProperty); }
            set { SetValue(TextAccentColorProperty, value); }
        }

        public Brush TextColor
        {
            get { return (Brush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }
        public Brush HighlightColor
        {
            get { return (Brush)GetValue(HighlightColorProperty); }
            set { SetValue(HighlightColorProperty, value); }
        }
        public bool SearchSheetNameEnabled
        {
            get => (bool)GetValue(SearchSheetNameProperty);
            set => SetValue(SearchSheetNameProperty, value);
        }

        public bool SearchViewNameEnabled
        {
            get => (bool)GetValue(SearchViewNameProperty);
            set => SetValue(SearchViewNameProperty, value);
        }

        public bool SearchSheetNumberEnabled
        {
            get => (bool)GetValue(SearchSheetNumberProperty);
            set => SetValue(SearchSheetNumberProperty, value);
        }

        public string FilterText
        {
            get => (string)GetValue(FilterTextProperty);
            set => SetValue(FilterTextProperty, value);
        }
        public ViewControl()
        {
            this.InitializeComponent();
        }
    }
}
