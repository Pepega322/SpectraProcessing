﻿using Domain;

namespace WinformsApp.Controllers;

internal static class WinformsTreeViewHelpers
{
	public static TData GetClickData<TData>(object? sender) where TData : class
	{
		var node = GetClickTreeNode(sender);
		return node as TData ?? throw new Exception();
	}

	public static DataSet<TData> GetClickSet<TData>(object? sender)
	{
		var node = GetClickTreeNode(sender);
		return node.Tag as DataSet<TData> ?? throw new Exception();
	}

	public static TreeNode GetClickTreeNode(object? sender)
	{
		var treeView = sender as TreeView ?? throw new Exception();
		return treeView.SelectedNode ?? throw new Exception();
	}

	public static TData GetContextData<TData>(object? sender) where TData : class
	{
		var node = GetContextTreeNode(sender);
		return node.Tag as TData ?? throw new Exception();
	}

	public static DataSet<TData> GetContextParentSet<TData>(object? sender)
	{
		var node = GetContextTreeNode(sender);
		return node.Parent.Tag as DataSet<TData> ?? throw new Exception();
	}

	public static DataSet<TData> GetContextSet<TData>(object? sender)
	{
		var node = GetContextTreeNode(sender);
		return node.Tag as DataSet<TData> ?? throw new Exception();
	}

	private static TreeNode GetContextTreeNode(object? sender)
	{
		var item = sender as ToolStripDropDownItem ?? throw new Exception();
		var contextMenu = item.Owner as ContextMenuStrip;
		while (contextMenu == null)
		{
			var t = item.Owner as ToolStripDropDownMenu ?? throw new Exception();
			contextMenu = t.OwnerItem?.Owner as ContextMenuStrip ?? throw new Exception();
			item = t.OwnerItem as ToolStripDropDownItem ?? throw new Exception();
		}

		var treeView = contextMenu.Tag as TreeView ?? throw new Exception();
		return treeView.SelectedNode;
	}
}