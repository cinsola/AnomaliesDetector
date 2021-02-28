namespace Common.Providers
{
    public interface ILogRowProvider
    {
        LogFileRow Load(string line);

        EnvironmentReference LoadEnvironmentReference(string line);
    }
}
