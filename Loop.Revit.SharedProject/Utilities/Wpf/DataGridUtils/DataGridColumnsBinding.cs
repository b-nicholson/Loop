using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using DataGridTextColumn = System.Windows.Controls.DataGridTextColumn;

namespace Loop.Revit.Utilities.Wpf.DataGridUtils
{
    public static class DataGridColumnsBinding
    {
        public static readonly DependencyProperty BindableColumnsProperty =
            DependencyProperty.RegisterAttached(
                "BindableColumns",
                typeof(ObservableCollection<DataGridColumnModel>),
                typeof(DataGridColumnsBinding),
                new UIPropertyMetadata(null, BindableColumnsPropertyChanged));

        public static void SetBindableColumns(DependencyObject element, ObservableCollection<DataGridColumnModel> value)
        {
            element.SetValue(BindableColumnsProperty, value);
        }

        public static ObservableCollection<DataGridColumnModel> GetBindableColumns(DependencyObject element)
        {
            return (ObservableCollection<DataGridColumnModel>)element.GetValue(BindableColumnsProperty);
        }

        private static void BindableColumnsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = source as DataGrid;
            ObservableCollection<DataGridColumnModel> columns = e.NewValue as ObservableCollection<DataGridColumnModel>;

            dataGrid.Columns.Clear();
            if (columns == null)
            {
                return;
            }

            foreach (DataGridColumnModel columnModel in columns)
            {
                if (columnModel is DataGridButtonColumnModel buttonColumnModel)
                {
                    var templateColumn = new DataGridTemplateColumn
                    {
                        Header = buttonColumnModel.Header,
                        Width = buttonColumnModel.Width
                    };


                    var template = new DataTemplate();
                    var button = new FrameworkElementFactory(typeof(Button));

                    if (buttonColumnModel.ContentIsPackIcon)
                    {
                        // Create a ContentPresenter for the content of the button
                        FrameworkElementFactory contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                        contentPresenterFactory.SetValue(ContentPresenter.ContentProperty, new Binding()); // Binding to the current data item

                        // Set the ContentPresenter as the content of the Button
                        button.AppendChild(contentPresenterFactory);
                        // Set the visual tree of the DataTemplate
                        template.VisualTree = button;
                        // Set a Template for the ContentPresenter to define how the data is displayed
                        DataTemplate iconTemplate = new DataTemplate();
                        FrameworkElementFactory iconFactory = new FrameworkElementFactory(typeof(PackIcon));

                        iconFactory.SetValue(PackIcon.KindProperty,
                            buttonColumnModel.ContentIsBinding
                                ? new Binding(buttonColumnModel.Content as string)
                                : buttonColumnModel.Content); // Set the icon you want 
                        iconTemplate.VisualTree = iconFactory;

                        // Set the ContentTemplate of the ContentPresenter to the iconTemplate
                        contentPresenterFactory.SetValue(ContentPresenter.ContentTemplateProperty, iconTemplate);
                    }
                    else
                    {
                       
                        if (buttonColumnModel.ContentIsBinding)
                        {
                            button.SetBinding(Button.ContentProperty, new Binding(buttonColumnModel.Content as string));
                        }
                        else
                        {
                            button.SetValue(Button.ContentProperty, buttonColumnModel.Content);
                        }

                       
                        template.VisualTree = button;

                    }
                    button.SetBinding(Button.CommandProperty, new Binding(nameof(DataGridButtonColumnModel.Command)) { Source = buttonColumnModel });
                    button.SetBinding(Button.CommandParameterProperty, new Binding(buttonColumnModel.CommandParameterPath));
                    templateColumn.CellTemplate = template;
                    dataGrid.Columns.Add(templateColumn);

                }

                else if (columnModel is DataGridCheckBoxColumnModel checkBoxColumnModel)
                {
                    var checkBoxColumn = new DataGridCheckBoxColumn
                    {
                        Header = checkBoxColumnModel.Header,
                        Width = checkBoxColumnModel.Width
                    };

                    // Use the inherited BindingPath and BindingMode properties
                    var binding = new Binding(checkBoxColumnModel.BindingPath)
                    {
                        Mode = checkBoxColumnModel.BindingMode
                    };
                    checkBoxColumn.Binding = binding;

                    //TODO Optional: Setting command for checkbox change
                    //if (checkBoxColumnModel.CheckedChangedCommand != null)
                    //{
                    //    //TODO Add logic for CheckedChangedCommand
                    //}

                    // Optional: Apply custom style if any
                    //if (checkBoxColumnModel.CheckBoxStyle != null)
                    //{
                    //    checkBoxColumn.ElementStyle = checkBoxColumnModel.CheckBoxStyle;
                    //    checkBoxColumn.EditingElementStyle = checkBoxColumnModel.CheckBoxStyle;
                    //}

                    dataGrid.Columns.Add(checkBoxColumn);
                }

                else
                {
                    // Handle other column types
                    var binding = new Binding(columnModel.BindingPath);
                    binding.Mode = columnModel.BindingMode;
                    
                    FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                    textBlockFactory.SetValue(TextBlock.TextAlignmentProperty, columnModel.HeaderTextAlignment);
                    textBlockFactory.SetValue(TextBlock.TextWrappingProperty, TextWrapping.WrapWithOverflow);
                    textBlockFactory.SetValue(TextBlock.TextProperty, columnModel.Header);

                    DataTemplate headerTemplate = new DataTemplate();
                    headerTemplate.VisualTree = textBlockFactory;



                    var textColumn = new DataGridTextColumn
                    {
                        Binding = binding,
                        Width = columnModel.Width,
                        HeaderTemplate = headerTemplate
                    };
                    dataGrid.Columns.Add(textColumn);
                }
            }
        }
    }
}
