#nullable enable
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Microsoft.Maui.Platform
{
	public static class ControlExtensions
	{
		public static void UpdateFont(this Control platformButton, ITextStyle textStyle, IFontManager fontManager) =>
			platformButton.UpdateFont(textStyle.Font, fontManager);

		public static void UpdateFont(this Control platformControl, Font font, IFontManager fontManager)
		{
			platformControl.FontSize = fontManager.GetFontSize(font);
			platformControl.FontFamily = fontManager.GetFontFamily(font);
			platformControl.FontStyle = font.ToFontStyle();
			platformControl.FontWeight = font.ToFontWeight();
			platformControl.IsTextScaleFactorEnabled = font.AutoScalingEnabled;
		}

		public static void UpdateIsEnabled(this Control platformControl, bool isEnabled) =>
			platformControl.IsEnabled = isEnabled;

		public static void UpdateBackground(this Control platformControl, IView view, UI.Xaml.Media.Brush? defaultBrush = null) =>
			platformControl.UpdateBackground(view.Background, defaultBrush);

		public static void UpdateBackground(this Control platformControl, Paint? paint, UI.Xaml.Media.Brush? defaultBrush = null) =>
			platformControl.UpdateProperty(Control.BackgroundProperty, paint.IsNullOrEmpty() ? defaultBrush : paint?.ToPlatform());

		public static void UpdateBackground(this Border platformControl, Paint? paint, UI.Xaml.Media.Brush? defaultBrush = null) =>
			platformControl.UpdateProperty(Border.BackgroundProperty, paint.IsNullOrEmpty() ? defaultBrush : paint?.ToPlatform());

		public static void UpdateBackground(this Panel platformControl, Paint? paint, UI.Xaml.Media.Brush? defaultBrush = null) =>
			platformControl.UpdateProperty(Panel.BackgroundProperty, paint.IsNullOrEmpty() ? defaultBrush : paint?.ToPlatform());

		public static async Task UpdateBackgroundImageSourceAsync(this Panel platformView, IViewBackgroundImagePart viewBackgroundImagePart, IImageSourceServiceProvider? provider)	
			=> await platformView.UpdateBackgroundImageAsync(viewBackgroundImagePart, provider);

		public static async Task UpdateBackgroundImageSourceAsync(this Control platformView, IViewBackgroundImagePart viewBackgroundImagePart, IImageSourceServiceProvider? provider)
			=> await platformView.UpdateBackgroundImageAsync(viewBackgroundImagePart, provider);

		public static void UpdateForegroundColor(this Control platformControl, Color color, UI.Xaml.Media.Brush? defaultBrush = null) =>
			platformControl.Foreground = color?.ToPlatform() ?? defaultBrush ?? platformControl.Foreground;

		public static void UpdatePadding(this Control platformControl, IPadding padding, UI.Xaml.Thickness? defaultThickness = null) =>
			platformControl.UpdatePadding(padding.Padding, defaultThickness);

		public static void UpdatePadding(this Control platformControl, Thickness padding, UI.Xaml.Thickness? defaultThickness = null)
		{
			platformControl.Padding = padding.IsNaN
				? defaultThickness ?? new UI.Xaml.Thickness()
				: padding.ToPlatform();
		}

		public static void UpdateTextColor(this Control nativeControl, ITextStyle text) =>
			nativeControl.UpdateProperty(Control.ForegroundProperty, text.TextColor);

		public static void UpdateCharacterSpacing(this Control nativeControl, ITextStyle text) =>
			nativeControl.CharacterSpacing = text.CharacterSpacing.ToEm();
		
		internal static async Task UpdateBackgroundImageAsync(this FrameworkElement platformView, IViewBackgroundImagePart viewBackgroundImagePart, IImageSourceServiceProvider? provider)
		{
			if (platformView == null || provider == null)
				return;

			var backgroundImageSource = viewBackgroundImagePart.Source;

			if (backgroundImageSource == null)
			{
				if (platformView is Panel panel)
					panel.Background = null;

				if (platformView is Control control)
					control.Background = null;
				return;
			}

			if (provider != null && backgroundImageSource != null)
			{
				var service = provider.GetRequiredImageSourceService(backgroundImageSource);
				var nativeBackgroundImageSource = await service.GetImageSourceAsync(backgroundImageSource);
				
				var background = new ImageBrush { ImageSource = nativeBackgroundImageSource?.Value };
			
				if (platformView is Panel panel)
					panel.Background = background;

				if (platformView is Control control)
					control.Background = background;
			}
		}
	}
}