using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Application.Controllers;

internal static class TreeViewHelpers
{
    public static async Task BuildTreeAsync(this TreeView tree, Func<IEnumerable<TreeNode>> nodeSource)
    {
        tree.Nodes.Clear();
        tree.BeginUpdate();
        var nodes = await Task.Run(() => nodeSource().ToArray());
        tree.Nodes.AddRange(nodes);
        tree.EndUpdate();
    }

    public static IEnumerable<TreeNode> GetDataNodes<TData>(this IDataStorageController<TData> dataStorageController)
    {
        foreach (var set in dataStorageController.StorageData)
        {
            var node = new TreeNode { Text = set.Name, Tag = set };
            ConnectDataSubnodes(node);
            yield return node;
        }

        yield break;

        static void ConnectDataSubnodes(TreeNode node)
        {
            if (node.Tag is not DataSet<SpectraData> set)
            {
                throw new Exception(nameof(ConnectDataSubnodes));
            }

            foreach (var child in set.Subsets.OrderByDescending(child => child.Name))
            {
                var subnode = new TreeNode
                {
                    Text = child.Name,
                    Tag = child,
                };

                ConnectDataSubnodes(subnode);

                node.Nodes.Add(subnode);
            }

            foreach (var data in set.Data.OrderByDescending(data => data.Name))
            {
                var subnode = new TreeNode()
                {
                    Text = data.Name,
                    Tag = data,
                };

                node.Nodes.Add(subnode);
            }
        }
    }

    public static IEnumerable<TreeNode> GetPlotNodes(this IPlotController plotController)
    {
        foreach (var set in plotController.Plots)
        {
            var setNode = new TreeNode
            {
                Text = set.Name,
                Tag = set,
                Checked = false,
            };

            foreach (var plot in set.Data.OrderByDescending(p => p.Name))
            {
                var subnode = new TreeNode
                {
                    Text = plot.Name,
                    Tag = plot,
                    Checked = plot.Plottable.IsVisible,
                };

                if (subnode.Checked)
                {
                    setNode.Checked = true;
                }

                setNode.Nodes.Add(subnode);
            }

            if (setNode.Nodes.Count > 0)
            {
                yield return setNode;
            }
        }
    }

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
        var treeView = sender as TreeView ?? throw new InvalidCastException();
        return treeView.SelectedNode ?? throw new Exception();
    }

    public static TData GetContextData<TData>(object? sender) where TData : class
    {
        var node = GetContextTreeNode(sender);
        return node.Tag as TData ?? throw new InvalidCastException();
    }

    public static DataSet<TData> GetContextParentSet<TData>(object? sender)
    {
        var node = GetContextTreeNode(sender);
        return node.Parent?.Tag as DataSet<TData> ?? throw new InvalidCastException();
    }

    public static DataSet<TData> GetContextSet<TData>(object? sender)
    {
        var node = GetContextTreeNode(sender);
        return node.Tag as DataSet<TData> ?? throw new InvalidCastException();
    }

    private static TreeNode GetContextTreeNode(object? sender)
    {
        var item = sender as ToolStripDropDownItem ?? throw new InvalidCastException();

        var contextMenu = item.Owner as ContextMenuStrip;

        while (contextMenu == null)
        {
            var t = item.Owner as ToolStripDropDownMenu ?? throw new InvalidCastException();

            contextMenu = t.OwnerItem?.Owner as ContextMenuStrip ?? throw new InvalidCastException();

            item = t.OwnerItem as ToolStripDropDownItem ?? throw new InvalidCastException();
        }

        var treeView = contextMenu.Tag as TreeView ?? throw new InvalidCastException();

        return treeView.SelectedNode!;
    }
}
