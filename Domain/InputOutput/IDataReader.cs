﻿namespace Domain.InputOutput;

public interface IDataReader<out TData>
{
	TData Get(string path);
}