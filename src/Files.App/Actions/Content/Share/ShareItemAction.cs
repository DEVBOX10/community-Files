﻿// Copyright (c) Files Community
// Licensed under the MIT License.

using Windows.ApplicationModel.DataTransfer;

namespace Files.App.Actions
{
	internal sealed partial class ShareItemAction : ObservableObject, IAction
	{
		private readonly IContentPageContext context;

		public string Label
			=> Strings.Share.GetLocalizedResource();

		public string Description
			=> Strings.ShareItemDescription.GetLocalizedFormatResource(context.SelectedItems.Count);

		public RichGlyph Glyph
			=> new(themedIconStyle: "App.ThemedIcons.Share");

		public bool IsExecutable =>
			IsContextPageTypeAdaptedToCommand() &&
			DataTransferManager.IsSupported() &&
			context.SelectedItems.Any() &&
			context.SelectedItems.All(ShareItemHelpers.IsItemShareable);

		public ShareItemAction()
		{
			context = Ioc.Default.GetRequiredService<IContentPageContext>();

			context.PropertyChanged += Context_PropertyChanged;
		}

		public Task ExecuteAsync(object? parameter = null)
		{
			return ShareItemHelpers.ShareItemsAsync(context.SelectedItems);
		}

		private bool IsContextPageTypeAdaptedToCommand()
		{
			return
				context.PageType != ContentPageTypes.RecycleBin &&
				context.PageType != ContentPageTypes.Home &&
				context.PageType != ContentPageTypes.Ftp &&
				context.PageType != ContentPageTypes.ZipFolder &&
				context.PageType != ContentPageTypes.None;
		}

		private void Context_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(IContentPageContext.SelectedItems):
				case nameof(IContentPageContext.PageType):
					OnPropertyChanged(nameof(IsExecutable));
					break;
			}
		}
	}
}
