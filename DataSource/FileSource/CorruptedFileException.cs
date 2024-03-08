namespace DataSource.FileSource;
public class CorruptedFileException(string message = "") : Exception(message);