﻿using System;
using Microsoft.Maui.Controls.Platform;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using WScrollMode = Microsoft.UI.Xaml.Controls.ScrollMode;

namespace Microsoft.Maui.Controls.Handlers
{
	public partial class ShellHandler : ViewHandler<Shell, ShellView>
	{
		ScrollViewer _scrollViewer;
		double? _topAreaHeight = null;
		double? _headerHeight = null;
		double? _headerOffset = null;

		protected override ShellView CreatePlatformView()
		{
			var shellView = new ShellView();
			shellView.SetElement(VirtualView);
			return shellView;
		}

		protected override void ConnectHandler(ShellView platformView)
		{
			base.ConnectHandler(platformView);

			if (platformView is MauiNavigationView mauiNavigationView)
				mauiNavigationView.OnApplyTemplateFinished += OnApplyTemplateFinished;

			platformView.PaneOpened += OnPaneOpened;
			platformView.PaneOpening += OnPaneOpening;
			platformView.PaneClosing += OnPaneClosing;
			platformView.ItemInvoked += OnMenuItemInvoked;
		}

		protected override void DisconnectHandler(ShellView platformView)
		{
			base.DisconnectHandler(platformView);

			if (platformView is MauiNavigationView mauiNavigationView)
				mauiNavigationView.OnApplyTemplateFinished -= OnApplyTemplateFinished;

			platformView.PaneOpened -= OnPaneOpened;
			platformView.PaneOpening -= OnPaneOpening;
			platformView.PaneClosing -= OnPaneClosing;
			platformView.ItemInvoked -= OnMenuItemInvoked;
		}

		void OnMenuItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
		{
			var item = args.InvokedItemContainer?.DataContext as Element;
			if (item != null)
				(VirtualView as IShellController)?.OnFlyoutItemSelected(item);
		}

		void OnApplyTemplateFinished(object sender, System.EventArgs e)
		{
			if (PlatformView == null)
				return;

			_scrollViewer = PlatformView.MenuItemsScrollViewer;

			UpdateValue(nameof(Shell.FlyoutHeaderBehavior));
		}

		void OnPaneOpened(UI.Xaml.Controls.NavigationView sender, object args)
		{
			PlatformView.UpdateFlyoutBackdrop();
		}

		void OnPaneClosing(UI.Xaml.Controls.NavigationView sender, UI.Xaml.Controls.NavigationViewPaneClosingEventArgs args)
		{
			args.Cancel = true;
			VirtualView.FlyoutIsPresented = false;
		}

		void OnPaneOpening(UI.Xaml.Controls.NavigationView sender, object args)
		{
			UpdateValue(nameof(Shell.FlyoutBackground));
			UpdateValue(nameof(Shell.FlyoutVerticalScrollMode));
			PlatformView.UpdateFlyoutBackdrop();
			PlatformView.UpdateFlyoutPosition();
			VirtualView.FlyoutIsPresented = true;
		}

		public override void SetVirtualView(IView view)
		{
			base.SetVirtualView(view);

			if (PlatformView.Element != view)
				PlatformView.SetElement((Shell)view);
		}

		public static void MapFlyoutBackdrop(ShellHandler handler, Shell view)
		{
			if (Brush.IsNullOrEmpty(view.FlyoutBackdrop))
				handler.PlatformView.FlyoutBackdrop = null;
			else
				handler.PlatformView.FlyoutBackdrop = view.FlyoutBackdrop;
		}

		public static void MapCurrentItem(ShellHandler handler, Shell view)
		{
			handler.PlatformView.SwitchShellItem(view.CurrentItem, true);
		}

		public static void MapFlyoutBackground(ShellHandler handler, Shell view)
		{
			handler.PlatformView.UpdatePaneBackground(
				!Brush.IsNullOrEmpty(view.FlyoutBackground) ?
					view.FlyoutBackground :
					view.FlyoutBackgroundColor?.AsPaint());
		}

		public static void MapFlyoutVerticalScrollMode(ShellHandler handler, Shell view)
		{
			handler.PlatformView.UpdateFlyoutVerticalScrollMode((WScrollMode)(int)view.FlyoutVerticalScrollMode);
		}

		public static void MapFlyout(ShellHandler handler, IFlyoutView flyoutView)
		{
			if (handler.PlatformView is RootNavigationView rnv)
				rnv.FlyoutView = flyoutView.Flyout;

			handler.PlatformView.FlyoutCustomContent = flyoutView.Flyout?.ToPlatform(handler.MauiContext);
			
		}

		public static void MapIsPresented(ShellHandler handler, IFlyoutView flyoutView)
		{
			// WinUI Will close the pane inside of the apply template code
			// so we wait until the control is loaded before applying IsPresented
			if (handler.PlatformView.IsLoaded)
				handler.PlatformView.IsPaneOpen = flyoutView.IsPresented;
		}

		public static void MapFlyoutWidth(ShellHandler handler, IFlyoutView flyoutView)
		{
			handler.PlatformView.UpdateFlyoutWidth(flyoutView);
		}

		public static void MapFlyoutBehavior(ShellHandler handler, IFlyoutView flyoutView)
		{
			handler.PlatformView.UpdateFlyoutBehavior(flyoutView);
		}

		public static void MapFlyoutFooter(ShellHandler handler, Shell view)
		{
			if (handler.PlatformView.PaneFooter == null)
				handler.PlatformView.PaneFooter = new ShellFooterView(view);
		}

		public static void MapFlyoutHeader(ShellHandler handler, Shell view)
		{
			if (handler.PlatformView.PaneHeader == null)
				handler.PlatformView.PaneHeader = new ShellHeaderView(view);
		}

		public static void MapFlyoutHeaderBehavior(ShellHandler handler, Shell view)
		{
			handler.UpdateFlyoutHeaderBehavior(view);
		}

		public static void MapItems(ShellHandler handler, Shell view)
		{
			handler.PlatformView.UpdateMenuItemSource();
		}

		public static void MapFlyoutItems(ShellHandler handler, Shell view)
		{
			handler.PlatformView.UpdateMenuItemSource();
		}

		void UpdateFlyoutHeaderBehavior(Shell view)
		{
			var flyoutHeader = (ShellHeaderView)PlatformView.PaneHeader;

			if (view.FlyoutHeaderBehavior == FlyoutHeaderBehavior.Default ||
				view.FlyoutHeaderBehavior == FlyoutHeaderBehavior.Fixed)
			{
				var defaultHeight = _headerHeight;
				var defaultTranslateY = _headerOffset;

				UpdateFlyoutHeaderTransformation(flyoutHeader, defaultHeight, defaultTranslateY);
				return;
			}

			if (_scrollViewer != null)
			{
				_scrollViewer.ViewChanged -= OnScrollViewerViewChanged;
				_scrollViewer.ViewChanged += OnScrollViewerViewChanged;
			}
		}

		void UpdateFlyoutHeaderTransformation(ShellHeaderView flyoutHeader, double? height, double? translationY)
		{
			if (translationY.HasValue)
			{
				flyoutHeader.RenderTransform = new CompositeTransform
				{
					TranslateY = translationY.Value
				};
			}

			if (height.HasValue)
			{
				flyoutHeader.Height = height.Value;
			}
		}

		void OnScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			if (_scrollViewer == null)
				return;

			var flyoutHeader = PlatformView?.PaneHeader as ShellHeaderView;

			if (flyoutHeader == null)
				return;

			if (_headerHeight == null)
				_headerHeight = flyoutHeader.ActualHeight;

			if (_headerOffset == null)
			{
				if (flyoutHeader.RenderTransform is CompositeTransform compositeTransform)
					_headerOffset = compositeTransform.TranslateY;
				else
					_headerOffset = 0;
			}

			switch (VirtualView?.FlyoutHeaderBehavior)
			{
				case FlyoutHeaderBehavior.Scroll:
					var scrollHeight = Math.Max(_headerHeight.Value - _scrollViewer.VerticalOffset, 0);
					var scrollTranslateY = -_scrollViewer.VerticalOffset;

					UpdateFlyoutHeaderTransformation(flyoutHeader, scrollHeight, scrollTranslateY);
					break;
				case FlyoutHeaderBehavior.CollapseOnScroll:
					var topNavArea = (StackPanel)PlatformView.TopNavArea;
					if (_topAreaHeight == null)
						_topAreaHeight = Math.Max(topNavArea.ActualHeight, 50.0f);

					var calculatedHeight = _headerHeight.Value - _scrollViewer.VerticalOffset;
					var collapseOnScrollHeight = calculatedHeight < _topAreaHeight.Value ? _topAreaHeight.Value : calculatedHeight;

					var offsetY = -_scrollViewer.VerticalOffset;
					var maxOffsetY = -_topAreaHeight.Value;
					var collapseOnScrollTranslateY = offsetY < maxOffsetY ? maxOffsetY : offsetY;

					UpdateFlyoutHeaderTransformation(flyoutHeader, collapseOnScrollHeight, collapseOnScrollTranslateY);
					break;
			}
		}
	}
}
