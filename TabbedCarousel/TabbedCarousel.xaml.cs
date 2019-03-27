using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CarouselView.FormsPlugin.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TabbedCarousel
{
    public partial class TabbedCarousel : ContentView
    {
        #region Enums

        #endregion

        #region Constructors

        public TabbedCarousel()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception exception)
            {
                SaveExceptionLocation(exception);

                throw;
            }
        }

        #endregion

        #region Protected Overrides

        #endregion

        #region Bindable Properties

        //#region AutoScroll

        //public bool AutoScroll
        //{
        //    get => (bool) GetValue(AutoScrollProperty);
        //    set => SetValue(AutoScrollProperty, value);
        //}

        //[SuppressMessage("ReSharper", "ArgumentsStyleLiteral")]
        //[SuppressMessage("ReSharper", "RedundantArgumentName")]
        //public static readonly BindableProperty AutoScrollProperty =
        //    BindableProperty.Create(
        //        propertyName: nameof(AutoScroll),
        //        returnType: typeof(bool),
        //        declaringType: typeof(TabbedCarousel),
        //        defaultValue: true);

        //#endregion

        #region SelectedTabIndex

        public int SelectedTabIndex
        {
            get => (int) GetValue(SelectedTabIndexProperty);
            set => SetValue(SelectedTabIndexProperty, value);
        }

        [SuppressMessage("ReSharper", "ArgumentsStyleLiteral")]
        [SuppressMessage("ReSharper", "RedundantArgumentName")]
        private static readonly BindableProperty SelectedTabIndexProperty =
            BindableProperty.Create(
                propertyName: nameof(SelectedTabIndex),
                returnType: typeof(int),
                declaringType: typeof(TabbedCarousel),
                // Start with an invalid index position to keep same selection routine
                defaultValue: -1,
                propertyChanged: SelectedTabIndexChanged);

        #endregion

        #region TabNames

        public IList<string> TabNames
        {
            get => (IList<string>)GetValue(TabNamesProperty);
            set => SetValue(TabNamesProperty, value);
        }

        [SuppressMessage("ReSharper", "RedundantArgumentName")]
        private static readonly BindableProperty TabNamesProperty =
            BindableProperty.Create(
                propertyName: nameof(TabNames),
                returnType: typeof(IList<string>),
                declaringType: typeof(TabbedCarousel),
                defaultValue: default(IList<string>),
                propertyChanged: TabNamesChanged);

        #endregion

        //#region TabView

        //public DataTemplate TabView
        //{
        //    get => (DataTemplate)GetValue(TabViewProperty);
        //    set => SetValue(TabViewProperty, value);
        //}

        //[SuppressMessage("ReSharper", "RedundantArgumentName")]
        //public static readonly BindableProperty TabViewProperty =
        //    BindableProperty.Create(
        //        propertyName: nameof(TabView),
        //        returnType: typeof(DataTemplate),
        //        declaringType: typeof(TabbedCarousel),
        //        defaultValue: default(DataTemplate));

        //#endregion

        #region TabViews

        public ObservableCollection<View> TabViews
        {
            get => (ObservableCollection<View>)GetValue(TabViewsProperty);
            set => SetValue(TabViewsProperty, value);
        }

        [SuppressMessage("ReSharper", "RedundantArgumentName")]
        private static readonly BindableProperty TabViewsProperty =
            BindableProperty.Create(
                propertyName: nameof(TabViews),
                returnType: typeof(ObservableCollection<View>),
                declaringType: typeof(TabbedCarousel),
                defaultValue: default(ObservableCollection<View>),
                propertyChanged: TabViewsChanged);

        #endregion

        #endregion

        #region Properties

        #endregion

        #region Events

        #endregion

        #region Delegates

        #endregion

        #region Private

        private void CarouselPositionSelected(object sender, PositionSelectedEventArgs e)
        {
            var carouselPosition = e.NewValue;
            var tabButton = TabButtons.Children[carouselPosition];

            SelectedTabIndex = carouselPosition;
            TabBar.ScrollToAsync(tabButton, ScrollToPosition.Start, animated: false);
        }

        private static void SelectedTabIndexChanged(BindableObject bindable,
            object oldvalue, object newvalue)
        {
            if (!(bindable is TabbedCarousel tabbedCarousel) ||
                !(newvalue is int newTabIndex) ||
                !(oldvalue is int oldTabIndex))
                return;

            if (newTabIndex == oldTabIndex)
                return;

            var tabButtons = tabbedCarousel.TabButtons;
            SetTabUnderlineColor(oldTabIndex, Color.Transparent);
            SetTabUnderlineColor(newTabIndex, Color.Blue);

            tabbedCarousel.Carousel.Position = newTabIndex;

            void SetTabUnderlineColor(int tabIndex, Color underlineColor)
            {
                if (tabIndex < 0)
                    return;

                var tabButton = (StackLayout) tabButtons.Children[tabIndex];
                var boxView = (BoxView) tabButton.LogicalChildren[1];

                boxView.BackgroundColor = underlineColor;
            }
        }

        private static void TabNamesChanged(BindableObject bindable, object oldvalue,
            object newvalue)
        {
            if (!(bindable is TabbedCarousel tabbedCarousel) ||
                tabbedCarousel.TabNames == null ||
                tabbedCarousel.TabNames.Count <= 0)
                return;

            tabbedCarousel.TabButtons.Children.Clear();

            for (var index = 0; index < tabbedCarousel.TabNames.Count; index++)
            {
                var tabName = tabbedCarousel.TabNames[index];

                var tabLabel = new Label
                {
                    Text = tabName,
                    //Style = Styles.Labels.TabBarButtonUnselectedText
                    // TEMP
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.Black,
                    Margin = new Thickness(10, 0),
                    HorizontalOptions = LayoutOptions.CenterAndExpand
                };

                var tabUnderline = new BoxView
                {
                    //Style = Styles.BoxViews.TabBarButtonUnselectedUnderline
                    // TEMP
                    BackgroundColor = Color.Transparent,
                    HeightRequest = 2,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (sender, eventArgs) =>
                {
                    if (int.TryParse(((StackLayout) sender).ClassId, out var tabIndex))
                        tabbedCarousel.SelectedTabIndex = tabIndex;
                };

                var tabButton = new StackLayout
                {
                    ClassId = index.ToString(),
                    Margin = new Thickness(4, 0),
                    // TEMP
                    //Spacing = Dimensions.TabButtonSpacingInside,
                    BackgroundColor = Color.Silver,
                    Spacing = 8,
                    Children =
                    {
                        tabLabel,
                        tabUnderline
                    }
                };
                tabButton.GestureRecognizers.Add(tapGesture);

                tabbedCarousel.TabButtons.Children.Add(tabButton);
            }

            // TODO Default to first or set based on stored value?
            tabbedCarousel.SelectedTabIndex = 0;
        }

        private static void TabViewsChanged(BindableObject bindable, object oldValue,
            object newValue)
        {
            if (!(bindable is TabbedCarousel tabbedCarousel) ||
                tabbedCarousel.TabViews == null ||
                tabbedCarousel.TabViews.Count <= 0)
                return;

            // TODO
        }

        #endregion

        #region Exceptions

        private void SaveExceptionLocation(Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            exception.Data.Add("Member Name", memberName);
            exception.Data.Add("Source File Path", sourceFilePath);
            exception.Data.Add("Source Line Number", sourceLineNumber);
        }

        #endregion
    }
}
