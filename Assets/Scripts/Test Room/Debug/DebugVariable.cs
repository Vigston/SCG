using System;

public class DebugVariable
{
	public string Name;
	public Func<object> ValueGetter;

	public DebugVariable(string name, Func<object> getter)
	{
		Name = name;
		ValueGetter = getter;
	}
}