using System;
public class PlacementCustomParametersException : Exception
{
    public PlacementCustomParametersException()
    {
    }

    public PlacementCustomParametersException(string message)
        : base(message)
    {
    }
}
