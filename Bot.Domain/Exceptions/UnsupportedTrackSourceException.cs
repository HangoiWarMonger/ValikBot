namespace Bot.Domain.Exceptions;

public class UnsupportedTrackSourceException : Exception
{
    public UnsupportedTrackSourceException() : base(ExceptionMessage.InvalidUrl())
    {
    }
}