using System.Windows;
using System.Windows.Controls;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Controls
{

    public partial class MealSelectionControl : UserControl
    {
        public MealSelectionControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Meal", typeof(string), typeof(MealSelectionControl));
        public required string Meal { get; set; }

    }
}



