using DataSource.Exceptions;
using Domain.InputOutput;
using Domain.SpectraData;
using Domain.SpectraData.Formats;
using Domain.SpectraData.Parser;

namespace DataSource.InputOutput;

public class SpectraFileReader(ISpectraParser parser) : IDataReader<Spectra>
{
	public Spectra Get(string fullName)
	{
		var file = new FileInfo(fullName);
		
		if (!file.Exists)
			throw new FileNotFoundException(fullName);

		if (!Enum.TryParse(file.Extension.TrimStart('.'), true, out SpectraFormat format))
			throw new UndefinedFileException(fullName);

		Spectra data;
		
		try
		{
			data = parser.Parse(format, file.Name, File.ReadAllLines(file.FullName));
		}
		catch (Exception e)
		{
			throw new CorruptedFileException($"{fullName}{Environment.NewLine}{e.Message}");
		}

		return data;
	}
}