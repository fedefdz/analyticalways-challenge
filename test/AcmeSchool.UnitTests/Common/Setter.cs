namespace AcmeSchool.UnitTests.Common
{
    internal class Setter
    {
        internal static void SetProperty<T>(object instance, string propertyName, T value)
        {
            var property = instance.GetType().GetProperty(propertyName);
            property!.SetValue(instance, value);
        }
    }
}
