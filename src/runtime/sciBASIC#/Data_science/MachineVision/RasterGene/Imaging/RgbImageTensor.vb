' ============================================================================
' RgbImageTensor.vb - GDI+ 位图与 NHWC 图像张量之间的适配层
'
' 模型内部统一使用 channel-last 的 4 维张量 [N, H, W, C]，像素值归一化到 [0, 1]，
' 这与 ConvVAE 的伯努利似然假设一致（输出经 sigmoid 落在同一区间）。
'
' 像素读写走 LockBits + Marshal.Copy：32x32 的小图逐像素 GetPixel 会慢上一个数量级，
' 而训练集动辄数百张、每轮都要整批读入。
' ============================================================================

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace Raster

    ''' <summary>
    ''' 位图 ⇄ RGB 图像张量的转换与拼版预览。
    ''' </summary>
    Public Module RgbImageTensor

        ''' <summary>RGB 图像的通道数。</summary>
        Public Const ColorChannels As Integer = 3

        ''' <summary>把单张位图转换为 <c>[1, H, W, 3]</c> 的张量。</summary>
        ''' <param name="image">源位图；任意像素格式。</param>
        ''' <returns>归一化到 [0, 1] 的 channel-last 张量。</returns>
        Public Function FromBitmap(image As Bitmap) As Tensor
            If image Is Nothing Then
                Throw New ArgumentNullException(NameOf(image))
            End If

            Dim width As Integer = image.Width
            Dim height As Integer = image.Height
            Dim result As New Tensor(New Integer() {1, height, width, ColorChannels})
            Dim buffer As Double() = result.Data
            Dim owned As Bitmap = Nothing
            Dim source As Bitmap = image

            If image.PixelFormat <> PixelFormat.Format32bppArgb Then
                ' 统一到 32bppArgb，LockBits 的字节布局才是确定的 BGRA 顺序
                owned = New Bitmap(width, height, PixelFormat.Format32bppArgb)

                Using g As Graphics = Graphics.FromImage(owned)
                    Call g.DrawImageUnscaled(image, 0, 0)
                End Using

                source = owned
            End If

            Try
                Dim data As BitmapData = source.LockBits(
                    New Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

                Try
                    Dim stride As Integer = data.Stride
                    Dim row As Byte() = New Byte(stride - 1) {}

                    For y As Integer = 0 To height - 1
                        Call Marshal.Copy(IntPtr.Add(data.Scan0, y * stride), row, 0, stride)

                        For x As Integer = 0 To width - 1
                            Dim offset As Integer = ((y * width) + x) * ColorChannels
                            Dim pixel As Integer = x * 4

                            buffer(offset) = row(pixel + 2) / 255.0         ' R
                            buffer(offset + 1) = row(pixel + 1) / 255.0     ' G
                            buffer(offset + 2) = row(pixel) / 255.0         ' B
                        Next
                    Next
                Finally
                    Call source.UnlockBits(data)
                End Try
            Finally
                If owned IsNot Nothing Then
                    Call owned.Dispose()
                End If
            End Try

            Return result
        End Function

        ''' <summary>把多张等尺寸位图堆叠为 <c>[N, H, W, 3]</c> 的张量。</summary>
        Public Function FromBitmaps(images As IEnumerable(Of Bitmap)) As Tensor
            If images Is Nothing Then
                Throw New ArgumentNullException(NameOf(images))
            End If

            Dim buffer As New List(Of Bitmap)

            For Each image As Bitmap In images
                If image Is Nothing Then
                    Throw New ArgumentException("图像集合中存在空引用", NameOf(images))
                End If

                Call buffer.Add(image)
            Next

            If buffer.Count = 0 Then
                Throw New ArgumentException("图像集合不能为空", NameOf(images))
            End If

            Dim first As Tensor = FromBitmap(buffer(0))
            Dim count As Integer = buffer.Count
            Dim height As Integer = first.Shape(1)
            Dim width As Integer = first.Shape(2)
            Dim rowSize As Integer = height * width * ColorChannels
            Dim result As New Tensor(New Integer() {count, height, width, ColorChannels})
            Dim target As Double() = result.Data

            Call Array.Copy(first.Data, target, rowSize)

            For i As Integer = 1 To count - 1
                Dim tile As Tensor = FromBitmap(buffer(i))

                If tile.Shape(1) <> height OrElse tile.Shape(2) <> width Then
                    Throw New ArgumentException(
                        $"图像尺寸不一致：第 0 张为 {height}x{width}，第 {i} 张为 {tile.Shape(1)}x{tile.Shape(2)}",
                        NameOf(images))
                End If

                Call Array.Copy(tile.Data, 0, target, i * rowSize, rowSize)
            Next

            Return result
        End Function

        ''' <summary>把批张量中的第 <paramref name="index"/> 张图像还原为位图。</summary>
        ''' <param name="images">形状为 <c>[N, H, W, 3]</c> 的张量。</param>
        ''' <param name="index">批内下标。</param>
        ''' <returns>24bpp 位图；数值会被裁剪到 [0, 1] 再量化到 0-255。</returns>
        Public Function ToBitmap(images As Tensor, Optional index As Integer = 0) As Bitmap
            Dim shape As Integer() = AssertImageBatch(images)

            If index < 0 OrElse index >= shape(0) Then
                Throw New ArgumentOutOfRangeException(NameOf(index), $"批内下标越界：{index} / {shape(0)}")
            End If

            Dim height As Integer = shape(1)
            Dim width As Integer = shape(2)
            Dim channels As Integer = shape(3)
            Dim rowSize As Integer = height * width * channels
            Dim source As Double() = images.Data
            Dim result As New Bitmap(width, height, PixelFormat.Format32bppArgb)
            Dim data As BitmapData = result.LockBits(
                New Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)

            Try
                Dim stride As Integer = data.Stride
                Dim row As Byte() = New Byte(stride - 1) {}

                For y As Integer = 0 To height - 1
                    Call Array.Clear(row, 0, row.Length)

                    For x As Integer = 0 To width - 1
                        Dim offset As Integer = index * rowSize + ((y * width) + x) * channels
                        Dim pixel As Integer = x * 4

                        row(pixel + 2) = ToByte(source(offset))                 ' R
                        row(pixel + 1) = ToByte(source(offset + 1))             ' G
                        row(pixel) = ToByte(source(offset + 2))                 ' B
                        row(pixel + 3) = 255                                    ' A
                    Next

                    Call Marshal.Copy(row, 0, IntPtr.Add(data.Scan0, y * stride), stride)
                Next
            Finally
                Call result.UnlockBits(data)
            End Try

            Return result
        End Function

        ''' <summary>把整个批张量还原为位图数组。</summary>
        Public Function ToBitmaps(images As Tensor) As Bitmap()
            Dim shape As Integer() = AssertImageBatch(images)
            Dim result As Bitmap() = New Bitmap(shape(0) - 1) {}

            For i As Integer = 0 To result.Length - 1
                result(i) = ToBitmap(images, i)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 把一批图像按网格拼成一张预览图（每张图按 <paramref name="scale"/> 放大，
        ''' 使用最近邻插值以保留像素边界）。
        ''' </summary>
        ''' <param name="images">形状为 <c>[N, H, W, 3]</c> 的张量。</param>
        ''' <param name="columns">每行放几张图。</param>
        ''' <param name="scale">每张图的放大倍数。</param>
        ''' <param name="gap">图与图之间的间隔像素。</param>
        Public Function Montage(images As Tensor, Optional columns As Integer = 8,
                                Optional scale As Integer = 4, Optional gap As Integer = 2) As Bitmap
            Return Montage(ToBitmaps(images), columns, scale, gap)
        End Function

        ''' <summary>把一组位图按网格拼成一张预览图。</summary>
        Public Function Montage(images As Bitmap(), Optional columns As Integer = 8,
                                Optional scale As Integer = 4, Optional gap As Integer = 2) As Bitmap
            If images Is Nothing OrElse images.Length = 0 Then
                Throw New ArgumentException("图像集合不能为空", NameOf(images))
            End If

            columns = If(columns <= 0, 1, columns)
            scale = If(scale <= 0, 1, scale)
            gap = If(gap < 0, 0, gap)

            Dim tileWidth As Integer = images(0).Width * scale
            Dim tileHeight As Integer = images(0).Height * scale
            Dim rows As Integer = (images.Length + columns - 1) \ columns
            Dim width As Integer = columns * tileWidth + (columns + 1) * gap
            Dim height As Integer = rows * tileHeight + (rows + 1) * gap
            Dim result As New Bitmap(width, height, PixelFormat.Format32bppArgb)

            Using g As Graphics = Graphics.FromImage(result)
                g.Clear(Color.FromArgb(38, 38, 38))
                g.InterpolationMode = InterpolationMode.NearestNeighbor
                g.PixelOffsetMode = PixelOffsetMode.Half
                g.SmoothingMode = SmoothingMode.None

                For i As Integer = 0 To images.Length - 1
                    Dim row As Integer = i \ columns
                    Dim column As Integer = i Mod columns
                    Dim left As Integer = gap + column * (tileWidth + gap)
                    Dim top As Integer = gap + row * (tileHeight + gap)

                    Call g.DrawImage(images(i), New Rectangle(left, top, tileWidth, tileHeight))
                Next
            End Using

            Return result
        End Function

        ''' <summary>
        ''' 校验张量是否为 <c>[N, H, W, 3]</c>，并返回其形状。
        ''' </summary>
        Public Function AssertImageBatch(images As Tensor) As Integer()
            If images Is Nothing Then
                Throw New ArgumentNullException(NameOf(images))
            End If

            If images.Rank <> 4 Then
                Throw New ArgumentException(
                    $"图像批张量必须是 4 维 [N, H, W, C]，实际秩为 {images.Rank}")
            End If

            If images.Shape(3) <> ColorChannels Then
                Throw New ArgumentException(
                    $"图像批张量的通道数必须为 {ColorChannels}（RGB），实际为 {images.Shape(3)}")
            End If

            Return images.Shape
        End Function

        ''' <summary>把 [0, 1] 的浮点像素量化到字节，越界值直接裁剪。</summary>
        Private Function ToByte(value As Double) As Byte
            If Double.IsNaN(value) OrElse value <= 0 Then
                Return 0
            End If

            If value >= 1 Then
                Return 255
            End If

            Return CByte(std.Round(value * 255.0))
        End Function

    End Module

End Namespace
