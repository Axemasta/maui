#if IOS || MACOS || MACCATALYST
using PlatformView = UIKit.UIView;
#elif ANDROID
using AndroidX.ConstraintLayout.Motion.Widget;

using PlatformView = Android.Views.View;
#elif WINDOWS
using PlatformView = Microsoft.UI.Xaml.FrameworkElement;
#elif TIZEN
using PlatformView = Tizen.NUI.BaseComponents.View;
#elif NET6_0_OR_GREATER || (NETSTANDARD || !PLATFORM)
using PlatformView = System.Object;
#endif

using System;
using System.Diagnostics;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Maui.Controls.Sample.Behaviors;

public partial class ColorBehavior : PlatformBehavior<VisualElement>
{
	public static readonly BindableProperty ColorProperty = BindableProperty.Create(
		nameof(Color),
		typeof(Color),
		typeof(ColorBehavior),
		null ,
		propertyChanged: UpdateColor);
    
	public Color? Color
	{
		get => (Color?)GetValue(ColorProperty);
		set => SetValue(ColorProperty, value);
	}
    
	private static void UpdateColor(BindableObject bindable, object oldvalue, object newvalue)
	{
		if (bindable is ColorBehavior colorBehavior)
		{
			colorBehavior.UpdateColor();
		}
	}

	private void UpdateColor()
	{
		if (View is null)
		{
			return;
		}
        
		View.BackgroundColor = Color;

		AnnounceColors();
	}
    
	protected VisualElement? View { get; set; }
    
	protected override void OnAttachedTo(VisualElement bindable, PlatformView platformView)
	{
		base.OnAttachedTo(bindable, platformView);
        
		View = bindable;
		UpdateColor();

		if (Application.Current is not null)
		{
			Application.Current.RequestedThemeChanged += OnAppThemeChanged;
		}
	}

	private void OnAppThemeChanged(object? sender, AppThemeChangedEventArgs e)
	{
		UpdateColor();
	}

	private void AnnounceColors()
	{
		if (Color is null)
		{
			Debug.WriteLine("Color was null");
			return;
		}
		
		var isRed = ColorsMatch(Color, Color.FromArgb("#FF0000")); // Dark
		var isBlue = ColorsMatch(Color, Color.FromArgb("#0000FF")); // Light
		
		var theme = Application.Current?.RequestedTheme ?? AppTheme.Unspecified;

		if (isRed)
		{
			Debug.WriteLine($"The color was red (dark) when app theme is {theme}");
		}
		else if (isBlue)
		{
			Debug.WriteLine($"The color was blue (light) when app theme is {theme}");
		}
		else
		{
			throw new InvalidOperationException("The color was not set to red or blue...");
		}
	}

	private bool ColorsMatch(Color color1, Color color2)
	{
		return color1.Red == color2.Red && 
		       color1.Green == color2.Green && 
		       color1.Blue == color2.Blue && 
		       color1.Alpha == color2.Alpha;
	}
}