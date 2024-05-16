namespace DataSource.Exceptions;

public class CorruptedFileException(string message = "") : Exception(message);