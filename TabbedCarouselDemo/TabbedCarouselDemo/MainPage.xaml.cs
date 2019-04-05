using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Xamarin.Forms;

namespace TabbedCarouselDemo
{
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            TabbedCarousel.TabNames =
                new List<string>
                {
                    "Motorcycle", "Car", "Plane", "Boat", "Bike", "Jeep", "Train", "Truck"
                };

            var vehicles = new List<Vehicle>
            {
                new Vehicle {Name = "Motorcycle", Color = Color.Yellow},
                new Vehicle {Name = "Car", Color = Color.Salmon},
                new Vehicle {Name = "Plane", Color = Color.DarkSeaGreen},
                new Vehicle {Name = "Boat", Color = Color.PaleVioletRed},
                new Vehicle {Name = "Bike", Color = Color.Orange},
                new Vehicle {Name = "Jeep", Color = Color.CornflowerBlue},
                new Vehicle {Name = "Train", Color = Color.LightSlateGray},
                new Vehicle {Name = "Truck", Color = Color.MediumPurple}
            };

            TabbedCarousel.TabData =
                new List<object>(vehicles);
        }
    }
}
