namespace Domain.DataSource;

public interface IDataSource<TData> where TData : Data {
	TData? Get(string path);
}