using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Demo.Services
{
    [Service(
        ServiceType = typeof(IFieldDemoService),
        Lifetime = DependencyLifetime.Singleton)]
    public interface IFieldDemoService
    {
        void AddDecimalField(string fieldName, bool baseProductfield, string fieldGroup, string templateName, bool multiCulture);
        void AddIntField(string fieldName, bool baseProductfield, string fieldGroup, string templateName, bool multiCulture);
        void AddTextField(string fieldName, bool baseProductfield, string fieldGroup, string templateName, bool multiCulture);
        void AddTextOptionField(string fieldName, bool baseProductfield, string fieldGroup, string templateName, string options);
    }
}