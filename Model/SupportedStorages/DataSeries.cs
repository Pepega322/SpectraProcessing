using Model.SupportedStorages.Base;
using Model.SupportedDataFormats.Base;

namespace Model.SupportedStorages;
internal class DataSeries : Series
{
    public DataSeries(string name)
    {
        Name = name;
    }

    public override bool ContainsID(string id) => _series.ContainsKey(id);
    public override void Add(string id, Data data) => _series.Add(id, data);
    public override Data Get(string id) => _series[id];
    public override void Remove(string id) => _series.Remove(id);
}
