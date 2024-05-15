namespace Domain.DataSource;

public interface IDataSource<TData>
{
	TData? Get(string path);
}