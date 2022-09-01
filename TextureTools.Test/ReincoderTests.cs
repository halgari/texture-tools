using System.Numerics;
using System.Reflection;
using BCnEncoder.Decoder;
using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using BCnEncoder.Shared.ImageFiles;
using SixLabors.ImageSharp.Processing;

namespace TextureTools.Test;

public class UnitTest1
{
    [Theory]
    [MemberData(nameof(TestData))]
    public async Task Test1(DXGI_FORMAT format, Vector2 size, string name)
    {
        await using var istream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        
        var decoder = new BcDecoder();
        var ddsFile = DdsFile.Load(istream);
        var data = await decoder.DecodeToImageRgba32Async(ddsFile);
        data.Mutate(x => x.Resize((int)size.X, (int)size.Y, KnownResamplers.Welch));
        
        var encoder = new BcEncoder
        {
            OutputOptions =
            {
                Quality = CompressionQuality.Balanced,
                GenerateMipMaps = true,
                Format = ToCompressionFormat(format),
                FileFormat = OutputFileFormat.Dds
            }
        };
        var recompressed = await encoder.EncodeToDdsAsync(data, CancellationToken.None);
        var testFile = new MemoryStream();
        recompressed.Write(testFile);
        testFile.Position = 0;

        var plan = await Plan.Load(testFile);

    }


    private static CompressionFormat ToCompressionFormat(DXGI_FORMAT dx)
    {
        return dx switch
        {
            DXGI_FORMAT.BC1_UNORM => CompressionFormat.Bc1,
            DXGI_FORMAT.BC2_UNORM => CompressionFormat.Bc2,
            DXGI_FORMAT.BC3_UNORM => CompressionFormat.Bc3,
            DXGI_FORMAT.BC4_UNORM => CompressionFormat.Bc4,
            DXGI_FORMAT.BC5_UNORM => CompressionFormat.Bc5,
            DXGI_FORMAT.BC7_UNORM => CompressionFormat.Bc7,
            DXGI_FORMAT.B8G8R8A8_UNORM => CompressionFormat.Bgra,
            DXGI_FORMAT.R8G8B8A8_UNORM => CompressionFormat.Rgba,
            _ => throw new Exception($"Cannot re-encode texture with {dx} format, encoding not supported")
        };
    }

    private static IEnumerable<DXGI_FORMAT> Formats =>
        new[]
        {
            DXGI_FORMAT.R8G8B8A8_UNORM
        };

    private static IEnumerable<Vector2> Sizes =>
        new[]
        {
            new Vector2(64, 64)
        };

    private static IEnumerable<string> ResourceImageNames => 
        Assembly.GetExecutingAssembly().GetManifestResourceNames()
            .Where(r => r.EndsWith(".dds", StringComparison.InvariantCultureIgnoreCase));

    public static IEnumerable<object[]> TestData =>
        ResourceImageNames.SelectMany(n => Formats.SelectMany(f => Sizes.Select(size => new object[] {f, size, n})));

}