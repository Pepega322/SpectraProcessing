using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Application.Controllers;

internal static class TreeViewHelpers
{
    public static async Task BuildTreeAsync(this TreeView tree, Func<IEnumerable<TreeNode>> nodeSource)
    {
        var nodes = await Task.Run(() => nodeSource().ToArray());
        tree.Nodes.Clear();
        tree.BeginUpdate();
        tree.Nodes.AddRange(nodes);
        tree.EndUpdate();
    }

    public static IReadOnlyCollection<TreeNode> GetDataNodes<TData>(
        this IDataStorageController<TData> dataStorageController)
    {
        var nodes = new List<TreeNode>();

        foreach (var set in dataStorageController.StorageData)
        {
            var node = new TreeNode
            {
                Text = set.Name,
                Tag = set,
            };

            ConnectDataSubnodes(node);

            nodes.Add(node);
        }

        return nodes;

        static void ConnectDataSubnodes(TreeNode node)
        {
            if (node.Tag is not DataSet<SpectraData> set)
            {
                throw new Exception(nameof(ConnectDataSubnodes));
            }

            foreach (var child in set.Subsets
                         .OrderByDescending(child => child.Name))
            {
                var subnode = new TreeNode
                {
                    Text = child.Name,
                    Tag = child,
                };

                ConnectDataSubnodes(subnode);

                node.Nodes.Add(subnode);
            }

            var subnodes = set.Data
                .OrderByDescending(data => data.Name)
                .Select(
                    data => new TreeNode
                    {
                        Text = data.Name,
                        Tag = data,
                    })
                .ToArray();

            node.Nodes.AddRange(subnodes);
        }
    }

    public static IReadOnlyCollection<TreeNode> GetPlotNodes(this IPlotController plotController)
    {
        return plotController.Plots
            .Where(s => s.Data.Any())
            .Select(ToTreeNode)
            .ToArray();

        TreeNode ToTreeNode(DataSet<SpectraDataPlot> set)
        {
            var subNodes = set.Data
                .OrderByDescending(p => p.Name)
                .Select(
                    plot => new TreeNode
                    {
                        Text = plot.Name,
                        Tag = plot,
                        Checked = plotController.IsPlotVisible(plot).Result,
                    })
                .ToArray();

            var setNode = new TreeNode
            {
                Text = set.Name,
                Tag = set,
                Checked = subNodes.Any(x => x.Checked),
            };

            setNode.Nodes.AddRange(subNodes);

            return setNode;
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
