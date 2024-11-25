namespace DTNL.UmbracoCms.SourceGenerators.ImageStylesHelperGenerator;

[AttributeUsage(AttributeTargets.Class)]
public class ImageStylesHelperGeneratorAttribute : Attribute
{
    public string Directory { get; }

    public ImageStylesHelperGeneratorAttribute(string directory)
    {
        Directory = directory;
    }
}
