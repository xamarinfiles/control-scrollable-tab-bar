using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ScrolledEventArgs = Xamarin.Forms.ScrolledEventArgs;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TabbedCarousel
{
    public partial class TabbedCarousel : ContentView
    {
        #region Enums

        private enum XDirection
        {
            Unknown,
            Left,
            Right
        }

        #endregion

        #region Fields

        private static double PageWidth => Device.Info.ScaledScreenSize.Width;

        private XDirection SwipeDirection { get; set; }

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

        #region TabButtonIndex

        public int TabButtonIndex
        {
            get => (int) GetValue(TabButtonIndexProperty);
            set => SetValue(TabButtonIndexProperty, value);
        }

        [SuppressMessage("ReSharper", "RedundantArgumentName")]
        private static readonly BindableProperty TabButtonIndexProperty =
            BindableProperty.Create(
                propertyName: nameof(TabButtonIndex),
                returnType: typeof(int),
                declaringType: typeof(TabbedCarousel),
                defaultValue: default(int),
                propertyChanged: TabButtonIndexChanged);

        #endregion

        #region TabViewIndex

        public int TabViewIndex
        {
            get => (int) GetValue(TabViewIndexProperty);
            set => SetValue(TabViewIndexProperty, value);
        }

        [SuppressMessage("ReSharper", "RedundantArgumentName")]
        private static readonly BindableProperty TabViewIndexProperty =
            BindableProperty.Create(
                propertyName: nameof(TabViewIndex),
                returnType: typeof(int),
                declaringType: typeof(TabbedCarousel),
                defaultValue: default(int),
                propertyChanged: TabViewIndexChanged);

        #endregion

        #region TabNames

        public IList<string> TabNames
        {
            get => (IList<string>) GetValue(TabNamesProperty);
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

        #region TabViews

        public ObservableCollection<View> TabViews
        {
            get => (ObservableCollection<View>) GetValue(TabViewsProperty);
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

        private void PanGestureRecognizer_OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                case GestureStatus.Running:
                    XDirection newDirection = XDirection.Unknown;

                    if (e.TotalX < 0)
                        newDirection = XDirection.Right;

                    if (e.TotalX > 0)
                        newDirection = XDirection.Left;

                    if (SwipeDirection == newDirection)
                    {
                        Debug.WriteLine($"*** {e.TotalX}");

                        return;
                    }

                    SwipeDirection = newDirection;

                    Debug.WriteLine($"*** {e.TotalX} => {SwipeDirection}");

                    break;

                case GestureStatus.Completed:
                    if (SwipeDirection == XDirection.Left && TabViewIndex > 0)
                    {
                        var tab = Tabs.Children[TabViewIndex -= 1];
                        TabViewsCarousel.ScrollToAsync(tab, ScrollToPosition.MakeVisible,
                            true);

                        var tabName = TabButtons.Children[TabButtonIndex -= 1];
                        TabButtonsBar.ScrollToAsync(tabName, ScrollToPosition.MakeVisible,
                            true);
                    }

                    if (SwipeDirection == XDirection.Right &&
                        TabViewIndex < Tabs.Children.Count - 1)
                    {
                        var tab = Tabs.Children[TabViewIndex += 1];
                        TabViewsCarousel.ScrollToAsync(tab, ScrollToPosition.MakeVisible,
                            true);

                        var tabName = TabButtons.Children[TabButtonIndex += 1];
                        TabButtonsBar.ScrollToAsync(tabName, ScrollToPosition.MakeVisible,
                            true);
                    }

                    break;
            }
        }

        private static void TabButtonIndexChanged(BindableObject bindable,
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

            tabbedCarousel.TabViewIndex = newTabIndex;

            void SetTabUnderlineColor(int tabIndex, Color underlineColor)
            {
                if (tabIndex < 0)
                    return;

                var tabButton = (StackLayout) tabButtons.Children[tabIndex];
                var boxView = (BoxView) tabButton.LogicalChildren[1];

                boxView.BackgroundColor = underlineColor;
            }
        }

        private static void TabViewIndexChanged(BindableObject bindable, object oldvalue,
            object newvalue)
        {
            if (!(bindable is TabbedCarousel tabbedCarousel) ||
                !(newvalue is int newTabIndex) ||
                !(oldvalue is int oldTabIndex))
                return;

            if (newTabIndex == oldTabIndex)
                return;

            if (tabbedCarousel.Tabs.Children.Count < 1)
                return;

            var tab = tabbedCarousel.Tabs.Children[newTabIndex];
            tabbedCarousel.TabViewsCarousel.ScrollToAsync(tab,
                ScrollToPosition.MakeVisible,
                false);
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
                    // TODO
                    if (int.TryParse(((StackLayout) sender).ClassId, out var tabIndex))
                        tabbedCarousel.TabButtonIndex = tabIndex;
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
            tabbedCarousel.TabViewIndex = 0;
        }

        private static void TabViewsChanged(BindableObject bindable, object oldValue,
            object newValue)
        {
            if (!(bindable is TabbedCarousel tabbedCarousel) ||
                tabbedCarousel.TabViews == null ||
                tabbedCarousel.TabViews.Count <= 0)
                return;

            tabbedCarousel.Tabs.Children.Clear();

            foreach (var tabView in tabbedCarousel.TabViews)
            {
                tabView.WidthRequest = PageWidth;
                tabbedCarousel.Tabs.Children.Add(tabView);
            }
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