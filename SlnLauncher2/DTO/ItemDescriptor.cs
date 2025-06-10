using System;

namespace SlnLauncher2.DTO;

internal abstract class ItemDescriptor : IEquatable<ItemDescriptor>
{
    public string Path { get; }
    public string Name { get; }

    protected ItemDescriptor(string path, string name)
    {
        Path = path;
        Name = name;
    }

    public bool Equals(ItemDescriptor other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Path == other.Path && Name == other.Name;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((ItemDescriptor) obj);
    }

    public override int GetHashCode() => HashCode.Combine(Path, Name);
}