namespace ClassLibrary
{
    public interface IJsonParser
    {
        void SetJsonToParse(string responseText);
        T GetPropertyValue<T>(params object[] propertyPath);
    }
}