namespace StartPos.Shared.Interfaces
{
    public interface IPcPosConfigParser
    {
        string GetValue(string path, string section, string parameter);
    }
}