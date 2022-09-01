using System.Runtime.InteropServices;
// ReSharper disable UnassignedField.Global
// ReSharper disable InconsistentNaming

namespace TextureTools;


public struct DDS_HEADER
{
    // ReSharper disable once InconsistentNaming
    public uint dwSize;
    public uint dwHeaderFlags;
    public uint dwHeight;
    public uint dwWidth;
    public uint dwPitchOrLinearSize;
    public uint dwDepth; // only if DDS_HEADER_FLAGS_VOLUME is set in dwHeaderFlags
    public uint dwMipMapCount;
    public uint dwReserved1; // [11]
    public DDS_PIXELFORMAT PixelFormat; // ddspf
    public uint dwSurfaceFlags;
    public uint dwCubemapFlags;
    public uint dwReserved2; // [3]

    public uint GetSize()
    {
        // 9 uint + DDS_PIXELFORMAT uints + 2 uint arrays with 14 uints total
        // each uint 4 bytes each
        return 9 * 4 + PixelFormat.GetSize() + 14 * 4;
    }

    public void Write(BinaryWriter bw)
    {
        bw.Write(dwSize);
        bw.Write(dwHeaderFlags);
        bw.Write(dwHeight);
        bw.Write(dwWidth);
        bw.Write(dwPitchOrLinearSize);
        bw.Write(dwDepth);
        bw.Write(dwMipMapCount);

        // Just write it multiple times, since it's never assigned a value anyway
        for (var i = 0; i < 11; i++)
            bw.Write(dwReserved1);

        // DDS_PIXELFORMAT
        bw.Write(PixelFormat.dwSize);
        bw.Write(PixelFormat.dwFlags);
        bw.Write(PixelFormat.dwFourCC);
        bw.Write(PixelFormat.dwRGBBitCount);
        bw.Write(PixelFormat.dwRBitMask);
        bw.Write(PixelFormat.dwGBitMask);
        bw.Write(PixelFormat.dwBBitMask);
        bw.Write(PixelFormat.dwABitMask);

        bw.Write(dwSurfaceFlags);
        bw.Write(dwCubemapFlags);

        // Just write it multiple times, since it's never assigned a value anyway
        for (var i = 0; i < 3; i++)
            bw.Write(dwReserved2);
    }

    public static uint Size
    {
        get
        {
            unsafe
            {
                return (uint) (sizeof(DDS_HEADER) + sizeof(int) * 10 + sizeof(int) * 2);
            }
        }
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct DDS_HEADER_DXT10
{
    public uint dxgiFormat;
    public uint resourceDimension;
    public uint miscFlag;
    public uint arraySize;
    public uint miscFlags2;

    public void Write(BinaryWriter bw)
    {
        bw.Write(dxgiFormat);
        bw.Write(resourceDimension);
        bw.Write(miscFlag);
        bw.Write(arraySize);
        bw.Write(miscFlags2);
    }

    public static uint Size
    {
        get
        {
            unsafe
            {
                return (uint) sizeof(DDS_HEADER_DXT10);
            }
        }
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct DDS_PIXELFORMAT
{
    public uint dwSize;
    public uint dwFlags;
    public uint dwFourCC;
    public uint dwRGBBitCount;
    public uint dwRBitMask;
    public uint dwGBitMask;
    public uint dwBBitMask;
    public uint dwABitMask;

    public DDS_PIXELFORMAT(uint size, uint flags, uint fourCC, uint rgbBitCount, uint rBitMask, uint gBitMask,
        uint bBitMask, uint aBitMask)
    {
        dwSize = size;
        dwFlags = flags;
        dwFourCC = fourCC;
        dwRGBBitCount = rgbBitCount;
        dwRBitMask = rBitMask;
        dwGBitMask = gBitMask;
        dwBBitMask = bBitMask;
        dwABitMask = aBitMask;
    }

    public uint GetSize()
    {
        // 8 uints, each 4 bytes each
        return 8 * 4;
    }
}