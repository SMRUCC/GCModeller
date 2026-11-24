' ============================================================================
' PreviewBoard.vb - 把训练 / 重构 / 生成的图像拼成 PNG 预览图
'
' 训练是不是真的有效，最终要看图：
'   dataset-samples.png     训练数据长什么样
'   test-originals.png      测试集原始图像
'   test-reconstructed.png  模型的重构结果
'   generated-samples.png   从先验随机生成的图像
'   reconstruct-compare.png 原始（上半）与重构（下半）的直接对照
' ============================================================================

Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.MachineVision.RasterGenerative.Raster

''' <summary>
''' 预览图的落盘工具。
''' </summary>
Public Module PreviewBoard

    ''' <summary>确保目录存在，并返回绝对路径。</summary>
    Public Function EnsureDirectory(directory As String) As String
        Dim full As String = IO.Path.GetFullPath(directory)

        If Not IO.Directory.Exists(full) Then
            Call IO.Directory.CreateDirectory(full)
        End If

        Return full
    End Function

    ''' <summary>把一组位图按网格拼成预览图并保存为 PNG。</summary>
    Public Sub SaveMontage(path As String, images As Bitmap(),
                           Optional columns As Integer = 8, Optional scale As Integer = 4)
        If images Is Nothing OrElse images.Length = 0 Then
            Throw New ArgumentException("没有可用于拼图的图像", NameOf(images))
        End If

        Call EnsureParent(path)

        Using board As Bitmap = RgbImageTensor.Montage(images, columns, scale)
            Call board.Save(path, System.Drawing.Imaging.ImageFormat.Png)
        End Using
    End Sub

    ''' <summary>
    ''' 把原始图像与重构结果拼在同一张图上（上半部分原始、下半部分重构）。
    ''' </summary>
    Public Sub SaveComparison(path As String, originals As Bitmap(), reconstructed As Bitmap(),
                              Optional columns As Integer = 8, Optional scale As Integer = 4)
        If originals Is Nothing OrElse originals.Length = 0 Then
            Throw New ArgumentException("没有可对照的原始图像", NameOf(originals))
        End If

        If reconstructed Is Nothing OrElse reconstructed.Length <> originals.Length Then
            Throw New ArgumentException(
                $"重构结果的数量必须与原始图像一致：{originals.Length} vs {If(reconstructed Is Nothing, 0, reconstructed.Length)}",
                NameOf(reconstructed))
        End If

        Dim stacked As Bitmap() = New Bitmap(originals.Length * 2 - 1) {}

        Call Array.Copy(originals, 0, stacked, 0, originals.Length)
        Call Array.Copy(reconstructed, 0, stacked, originals.Length, reconstructed.Length)

        Call SaveMontage(path, stacked, columns, scale)
    End Sub

    Private Sub EnsureParent(path As String)
        Dim folder As String = IO.Path.GetDirectoryName(IO.Path.GetFullPath(path))

        If Not String.IsNullOrEmpty(folder) AndAlso Not IO.Directory.Exists(folder) Then
            Call IO.Directory.CreateDirectory(folder)
        End If
    End Sub

End Module
