Imports System.IO
Imports std = System.Math

Namespace Imaging.BitmapImage.FileStream
    Public Enum BitsPerPixelEnum As Integer
        Monochrome = 1
        Four = 4
        Eight = 8
        RBG16 = 16
        RGB24 = 24
        RGBA32 = 32
    End Enum

    ''' <summary>
    ''' Number of bytes for specific Pixel format.
    ''' </summary>
    Public Enum BytesPerPixelEnum As Integer
        RBG16 = 2
        RGB24 = 3
        RGBA32 = 4
    End Enum

    Public Enum CompressionMethod As Integer
        BI_RGB = 0 ' none
        BI_RLE8 = 1
        BI_RLE4 = 2
        BI_BITFIELDS = 3
        BI_JPEG = 4
        BI_PNG = 5
        BI_ALPHABITFIELDS = 6
        BI_CMYK = 11
        BI_CMYKRLE8 = 12
        BI_CMYKRLE4 = 13
    End Enum

    Public Class Bitmap
        Public ReadOnly Property Width As Integer = 0
        Public ReadOnly Property Height As Integer = 0
        Public ReadOnly Property BitsPerPixelEnum As BitsPerPixelEnum

        ''' <summary>
        ''' BMP file must be aligned at 4 butes at the end of row
        ''' </summary>
        ''' <paramname="BitsPerPixelEnum"></param>
        ''' <returns></returns>
        Public ReadOnly Property BytesPerRow As Integer
            Get
                Return RequiredBytesPerRow(Width, BitsPerPixelEnum)
            End Get
        End Property

        ''' <summary>
        ''' NOTE: we don't care for images that are less than 24 bits
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property BytesPerPixel As Integer
            Get
                Return CInt(BitsPerPixelEnum) / 8
            End Get
        End Property
        Public ReadOnly Property PixelData As Byte()

        ''' <summary>
        ''' Get reversed order or rows.
        ''' For Bitmap image, pixel rows are stored from bottom to top.
        ''' So first row in bitmap file is lowest row in Image.
        ''' </summary>
        ''' <returns>Pixel data with reversed (fliped) rows</returns>
        Public ReadOnly Property PixelDataFliped As Byte()
            Get
                Dim rowListData = New List(Of Byte())()
                Dim totalRows = Height
                Dim pixelsInRow = Width

                For row = totalRows - 1 To 0 Step -1
                    ' NOTE: this only works on images that are 8/24/32 bits per pixel
                    Dim one_row = PixelData.Skip(row * Width * BytesPerPixel).Take(Width * BytesPerPixel).ToArray()
                    rowListData.Add(one_row)
                Next
                Dim reversedBytes = rowListData.SelectMany(Function(row) row).ToArray()
                Return reversedBytes
            End Get
        End Property

        Public ReadOnly Property FileHeader As BitmapFileHeader
        Public ReadOnly Property InfoHeaderBytes As Byte()

        ''' <summary>
        ''' Create new Bitmap object
        ''' </summary>
        ''' <paramname="width"></param>
        ''' <paramname="height"></param>
        ''' <paramname="pixelData"></param>
        ''' <paramname="bitsPerPixel"></param>
        Public Sub New(width As Integer, height As Integer, pixelData As Byte(), Optional bitsPerPixel As BitsPerPixelEnum = BitsPerPixelEnum.RGB24)
            Me.Width = width
            Me.Height = height
            Me.PixelData = pixelData
            Me.BitsPerPixelEnum = bitsPerPixel

            Dim rawImageSize = BytesPerRow * height

            ' Are we receiving proper byte[] size ?
            If pixelData.Length <> width * height * BytesPerPixel Then Throw New ArgumentOutOfRangeException($"{NameOf(pixelData)} has invalid size.")


            If bitsPerPixel = BitsPerPixelEnum.RGB24 Then InfoHeaderBytes = New BitmapInfoHeader(width, height, bitsPerPixel, rawImageSize).HeaderInfoBytes
            If bitsPerPixel = BitsPerPixelEnum.RGBA32 Then
                InfoHeaderBytes = New BitmapInfoHeaderRGBA(width, height, bitsPerPixel, rawImageSize).HeaderInfoBytes
            End If

            FileHeader = New BitmapFileHeader(width, height, bitsPerPixel, rawImageSize)
        End Sub

        ''' <summary>
        ''' Get bitmap as byte aray for saving to file
        ''' </summary>
        ''' <paramname="flipped">Flip (reverse order of) rows. Bitmap pixel rows are stored from bottom to up as shown in image</param>
        ''' <returns></returns>
        Public Function GetBmpBytes(Optional flipped As Boolean = False) As Byte()
            'var rawImageSize = BytesPerRow * Height;
            'var buffer = new byte[BitmapFileHeader.BitmapFileHeaderSizeInBytes + rawImageSize];
            'Buffer.BlockCopy( this.FileHeader.HeaderBytes, 0, buffer, 0, BitmapFileHeader.BitmapFileHeaderSizeInBytes );

            'if (flipped) {
            '	Buffer.BlockCopy( this.PixelDataFliped, 0, buffer, BitmapFileHeader.BitmapFileHeaderSizeInBytes, PixelData.Length );
            '} else {
            '	Buffer.BlockCopy( this.PixelData, 0, buffer, BitmapFileHeader.BitmapFileHeaderSizeInBytes, PixelData.Length );
            '}
            'return buffer;

            Using stream = GetBmpStream(flipped)
                Return stream.ToArray()
            End Using
        End Function

        ''' <summary>
        ''' Get bitmap as byte stream for saving to file
        ''' </summary>
        ''' <paramname="flipped">Flip (reverse order of) rows. Bitmap pixel rows are stored from bottom to up as shown in image</param>
        ''' <returns></returns>
        Public Function GetBmpStream(Optional fliped As Boolean = False) As MemoryStream
            Dim rawImageSize = BytesPerRow * Height

            'var stream = new System.IO.MemoryStream( BitmapFileHeader.BitmapFileHeaderSizeInBytes + (int) rawImageSize );
            Dim stream = New MemoryStream(rawImageSize)

            'using (var writer = new BinaryWriter( stream )) {
            Dim writer = New BinaryWriter(stream)
            writer.Write(FileHeader.HeaderBytes)
            writer.Write(InfoHeaderBytes)
            writer.Flush()
            stream.Flush()

            Dim paddingRequired = BytesPerRow <> Width * BytesPerPixel
            Dim bytesToCopy = Width * BytesPerPixel
            Dim pixData = If(fliped, PixelDataFliped, PixelData)

            If paddingRequired Then
                For counter = 0 To Height - 1
                    Dim rowBuffer = New Byte(BytesPerRow - 1) {}
                    Buffer.BlockCopy(src:=pixData, srcOffset:=counter * bytesToCopy, dst:=rowBuffer, dstOffset:=0, count:=bytesToCopy)
                    writer.Write(rowBuffer)
                Next
            Else
                writer.Write(pixData)
            End If

            stream.Position = 0

            Return stream
        End Function

        ''' <summary>
        ''' BMP file must be aligned at 4 bytes at the end of row
        ''' </summary>
        ''' <paramname="width">Image Width</param>
        ''' <paramname="bitsPerPixel">Bits per pixel</param>
        ''' <returns>How many bytes BMP requires per row</returns>
        Public Shared Function RequiredBytesPerRow(width As Integer, bitsPerPixel As BitsPerPixelEnum) As Integer
            Return CInt(std.Ceiling(CDec(width * bitsPerPixel) / 32)) * 4
        End Function

        ''' <summary>
        ''' Check if padding is required (extra bytes for a row).
        ''' </summary>
        ''' <paramname="width">Width of image</param>
        ''' <paramname="bitsPerPixel">Bits per pixels to calculate actual byte requirement</param>
        ''' <paramname="bytesPerRow">BMP required bytes per row</param>
        ''' <returns>True/false if we need to allocate extra bytes (for BMP saving) for padding</returns>
        Public Shared Function IsPaddingRequired(width As Integer, bitsPerPixel As BitsPerPixelEnum, bytesPerRow As Integer) As Boolean
            Return bytesPerRow <> width * bitsPerPixel / 8
        End Function
    End Class
End Namespace
