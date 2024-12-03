namespace Bot.Discord.Common;

public interface ITest
{
    int GetNumber();
}

public class Test : ITest
{
    private int _num = 0;

    public int GetNumber()
    {
        return ++_num;
    }
}