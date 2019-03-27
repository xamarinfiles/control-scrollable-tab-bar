using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Xamarin.Forms;

namespace TabbedCarouselDemo.Controls
{
    public partial class TabbedView : ContentView
    {
        #region Enums

        #endregion

        #region Constructors

        public TabbedView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        #endregion

        #region Protected Overrides

        #endregion

        #region Bindable Properties

        #region Text

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        [SuppressMessage("ReSharper", "RedundantArgumentName")]
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                propertyName: nameof(Text),
                returnType: typeof(string),
                declaringType: typeof(TabbedView),
                defaultValue: default(string));

        #endregion

        #region TextColor

        public Color TextColor
        {
            get => (Color) GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        [SuppressMessage("ReSharper", "RedundantArgumentName")]
        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(
                propertyName: nameof(TextColor),
                returnType: typeof(Color),
                declaringType: typeof(TabbedView),
                defaultValue: default(Color));

        #endregion

        #endregion

        #region Properties

        #endregion

        #region Events

        #endregion

        #region Delegates

        #endregion

        #region Private

        #endregion
    }
}
