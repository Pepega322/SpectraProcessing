using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Application.Extensions;

internal static class TreeViewExtensions
{
    public static async Task BuildTreeAsync(this TreeView tree, Func<IEnumerable<TreeNode>> nodeSource)
    {
        var nodes = await Task.Run(() => nodeSource().ToArray());
        tree.BeginUpdate();
        tree.Nodes.Clear();
        tree.Nodes.AddRange(nodes);
        tree.EndUpdate();
    }

    public static IReadOnlyCollection<TreeNode> GetDataNodes<TData>(this IEnumerable<DataSet<TData>> dataSets)
    {
        var nodes = new List<TreeNode>();

        foreach (var set in dataSets)
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

    public static IReadOnlyCollection<TreeNode> GetPlotNodes(this ISpectraController spectraController)
    {
        return spectraController.Plots
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
                        Checked = spectraController.IsPlotVisible(plot).Result,
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

    public static TData? GetClickData<TData>(object? sender)
        where TData : class
    {
        var node = GetClickTreeNode(sender);
        return node?.Tag as TData;
    }

    public static DataSet<TData>? GetClickSet<TData>(object? sender)
    {
        var node = GetClickTreeNode(sender);
        return node?.Tag as DataSet<TData>;
    }

    public static TreeNode? GetClickTreeNode(object? sender)
    {
        var treeView = sender as TreeView;
        return treeView?.SelectedNode;
    }

    public static TData? GetContextData<TData>(object? sender)
        where TData : class
    {
        var node = GetContextTreeNode(sender);
        return node.Tag as TData;
    }

    public static TData? GetContextData<TData>(object? sender, out TreeNode node)
        where TData : class
    {
        node = GetContextTreeNode(sender);
        return node.Tag as TData;
    }

    public static DataSet<TData>? GetContextParentSet<TData>(object? sender)
    {
        var node = GetContextTreeNode(sender);
        return node.Parent?.Tag as DataSet<TData>;
    }

    public static DataSet<TData>? GetContextSet<TData>(object? sender)
    {
        var node = GetContextTreeNode(sender);
        return node.Tag as DataSet<TData>;
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
