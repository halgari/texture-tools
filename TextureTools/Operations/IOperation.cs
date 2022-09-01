using System.Numerics;

namespace TextureTools.Operations;

public interface IOperation
{
    public Size OutputSize { get; set; }
}