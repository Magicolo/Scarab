using System;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public sealed class SeparatorAttribute : CustomAttributeBase {
}
