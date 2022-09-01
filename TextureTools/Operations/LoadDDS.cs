namespace TextureTools.Operations;

public class LoadDDS : IOperation
{
    private DDS_HEADER _header;
    
    public static async Task<LoadDDS> Create(Stream input)
    {
        var load = new LoadDDS();
        DDS_HEADER.Read(new BinaryReader(input), ref load._header);
        load.OutputSize = new Size
        {
            Width = (int)load._header.dwWidth,
            Height = (int)load._header.dwHeight
        };
        load.IsUncompressed = load._header.PixelFormat.dwFourCC == 0;
        return load;
    }
    
    public Size OutputSize { get; set; }
    
    public bool IsUncompressed { get; set; }
}