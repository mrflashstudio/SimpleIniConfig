# SimpleIniConfig [![nuget](https://img.shields.io/nuget/v/SimpleIniConfig.svg)](https://www.nuget.org/packages/SimpleIniConfig)
Just a simple configuration library i use across my projects.

## Usage
```csharp
using SimpleIniConfig

namespace SomeNamespace
{
    public class SomeClass
    {
        private Config config;
        
        public SomeClass()
        {
            config = new Config(@"myConfig.ini"); //initializing config
            
            //getting values
            var someInt = config.GetValue(@"someInt", int.MaxValue);
            var someIntArray = config.GetValue(@"someIntArray", new int[] { int.MaxValue, int.MinValue });
            
            //writing value
            config.SetValue(@"someInt", 0);
            config.SetValue(@"someIntArray", new int[] { 0 });
        }
    }
}
```
