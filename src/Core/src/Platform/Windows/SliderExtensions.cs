﻿#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WImageSource = Microsoft.UI.Xaml.Media.ImageSource;

namespace Microsoft.Maui.Platform
{
	public static class SliderExtensions
	{
		static void UpdateIncrement(this Slider nativeSlider, ISlider slider)
		{
			double stepping = Math.Min((slider.Maximum - slider.Minimum) / 1000, 1);
			nativeSlider.StepFrequency = stepping;
			nativeSlider.SmallChange = stepping;
		}

		public static void UpdateMinimum(this Slider nativeSlider, ISlider slider)
		{
			nativeSlider.Minimum = slider.Minimum;
			nativeSlider.UpdateIncrement(slider);
		}

		public static void UpdateMaximum(this Slider nativeSlider, ISlider slider)
		{
			nativeSlider.Maximum = slider.Maximum;
			nativeSlider.UpdateIncrement(slider);
		}

		public static void UpdateValue(this Slider nativeSlider, ISlider slider)
		{
			if (nativeSlider.Value != slider.Value)
				nativeSlider.Value = slider.Value;
		}

		public static void UpdateMinimumTrackColor(this Slider platformSlider, ISlider slider)
		{
			var brush = slider.MinimumTrackColor?.ToPlatform();

			if (brush == null)
			{
				platformSlider.Resources.RemoveKeys(_minimumTrackColorResourceKeys);
			}
			else
			{
				platformSlider.Resources.SetValueForAllKey(_minimumTrackColorResourceKeys, brush);
			}
		}

		static readonly string[] _minimumTrackColorResourceKeys =
		{
			"SliderTrackValueFill",
			"SliderTrackValueFilllPointerOver",
			"SliderTrackValueFillPressed",
			"SliderTrackValueFillDisabled",
		};

		public static void UpdateMaximumTrackColor(this Slider platformSlider, ISlider slider)
		{
			var brush = slider.MaximumTrackColor?.ToPlatform();

			if (brush == null)
			{
				platformSlider.Resources.RemoveKeys(_maximumTrackColorResourceKeys);
			}
			else
			{
				platformSlider.Resources.SetValueForAllKey(_maximumTrackColorResourceKeys, brush);
			}
		}

		static readonly string[] _maximumTrackColorResourceKeys =
		{
			"SliderTrackFill",
			"SliderTrackFillPointerOver",
			"SliderTrackFillPressed",
			"SliderTrackFillDisabled",
		};

		public static void UpdateThumbColor(this Slider platformSlider, ISlider slider)
		{
			var brush = slider.ThumbColor?.ToPlatform();

			if (brush == null)
			{
				platformSlider.Resources.RemoveKeys(_thumbColorResourceKeys);
			}
			else
			{
				platformSlider.Resources.SetValueForAllKey(_thumbColorResourceKeys, brush);
			}
		}

		static readonly string[] _thumbColorResourceKeys =
		{
			"SliderThumbBackground",
			"SliderThumbBackgroundPointerOver",
			"SliderThumbBackgroundPressed",
			"SliderThumbBackgroundDisabled",
		};

		internal static async Task UpdateThumbImageSourceAsync(this MauiSlider nativeSlider, ISlider slider, IImageSourceServiceProvider? provider)
		{
			var thumbImageSource = slider.ThumbImageSource;

			if (thumbImageSource == null)
			{
				nativeSlider.ThumbImageSource = null;
				return;
			}

			if (provider != null && thumbImageSource != null)
			{
				var service = provider.GetRequiredImageSourceService(thumbImageSource);
				var nativeThumbImageSource = await service.GetImageSourceAsync(thumbImageSource);

				nativeSlider.ThumbImageSource = nativeThumbImageSource?.Value;
			}
		}
	}
}