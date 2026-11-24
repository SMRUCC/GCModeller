Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

''' <summary>
''' 相关矩阵存储中保存的相关系数类型。
''' </summary>
''' <remarks>
''' 类型不写入存储格式（索引中没有该字段），而是由数据文件名判别：
''' <c>bicor.dat</c> → <see cref="CorrelationKind.Bicor"/>，<c>pearson.dat</c> → <see cref="CorrelationKind.Pearson"/>，
''' 索引文件统一为 <c>index.dat</c>（见 <see cref="CorrelationStoreFiles"/>）。
''' </remarks>
Public Enum CorrelationKind

    ''' <summary>Biweight midcorrelation（WGCNA 鲁棒相关，默认）</summary>
    Bicor

    ''' <summary>经典 Pearson 相关系数</summary>
    Pearson
End Enum

''' <summary>
''' 相关矩阵存储目录下的文件名约定（写入端与读取端共用的单一事实源）
''' </summary>
Public Module CorrelationStoreFiles

    ''' <summary>bicor 数据文件名（不含目录）</summary>
    Public Const BicorDataFile As String = "bicor.dat"
    ''' <summary>pearson 数据文件名（不含目录）</summary>
    Public Const PearsonDataFile As String = "pearson.dat"
    ''' <summary>索引文件名（不含目录，两种相关类型共用）</summary>
    Public Const IndexFile As String = "index.dat"

    ''' <summary>相关类型 → 数据文件名</summary>
    Public Function DataFileOf(kind As CorrelationKind) As String
        Return If(kind = CorrelationKind.Pearson, PearsonDataFile, BicorDataFile)
    End Function

    ''' <summary>数据文件名 → 相关类型；不是约定的数据文件名时返回 Nothing</summary>
    Public Function KindOf(dataFileName As String) As CorrelationKind?
        If String.IsNullOrEmpty(dataFileName) Then
            Return Nothing
        End If

        Select Case dataFileName.ToLowerInvariant()
            Case PearsonDataFile : Return CorrelationKind.Pearson
            Case BicorDataFile : Return CorrelationKind.Bicor
            Case Else : Return Nothing
        End Select
    End Function
End Module

Public Module BicorMatrix

    ''' <summary>
    ''' 计算基因 x 基因的相关矩阵并写入磁盘存储（边算边写，O(N) 内存流式）
    ''' </summary>
    ''' <param name="x">基因 x 样本的表达矩阵</param>
    ''' <param name="repo">存储目录；数据文件按 <paramref name="corKind"/> 命名为 bicor.dat / pearson.dat，索引为 index.dat</param>
    ''' <param name="type">磁盘编码方式，默认 <see cref="CorrelationEncodings.Float32"/></param>
    ''' <param name="corKind">相关系数类型，默认 <see cref="CorrelationKind.Bicor"/>（与既有行为一致）</param>
    <Extension>
    Public Sub CreateMatrixStore(x As Matrix, repo As String,
                                 Optional type As CorrelationEncodings = CorrelationEncodings.Float32,
                                 Optional corKind As CorrelationKind = CorrelationKind.Bicor)
        Dim matrix = $"{repo}/{CorrelationStoreFiles.DataFileOf(corKind)}".Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
        Dim index = $"{repo}/{CorrelationStoreFiles.IndexFile}".Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
        Dim geneIds As String() = x.rownames
        Dim nSample As Integer = x.sample_count
        Dim t0 = Now

        ' 相关系数计算函数：默认 bicor（鲁棒），也可切换为经典 pearson；
        ' Pearson 必须绑两参数重载——含 ByRef 可选参数的重载不能用于委托转换
        Dim correlate As Func(Of Double(), Double(), Double)

        If corKind = CorrelationKind.Pearson Then
            correlate = AddressOf Correlations.GetPearson
        Else
            correlate = AddressOf Bicor.BiweightMidcorrelation
        End If

        ' 20261004 zip archive is not working as expected due to the reason
        ' of zip stream can not be random access(can not be seek)

        ' ② 逐行相关计算（一次性矩阵计算），边算边写 corstore
        Using writer As New CorrelationMatrixWriter(matrix, index, geneIds, nSample, type, CompressionLevel.NoCompression)
            Dim bar As ProgressBar = Nothing

            For Each i As Integer In TqdmWrapper.Range(0, geneIds.Length, bar:=bar)
                Dim row(geneIds.Length - 1) As Single
                Dim vi As Double() = x(i).experiments

                Call Parallel.For(0, geneIds.Length,
                     body:=Sub(j)
                               If j = i Then
                                   row(j) = 1.0F
                               Else
                                   row(j) = CSng(correlate(vi, x(j).experiments))
                               End If
                           End Sub)

                Call bar.SetLabel(geneIds(i))
                Call writer.WriteRow(geneIds(i), row)
            Next

            Call writer.Complete()
        End Using

        Call $"{corKind} correlation store written: {StringFormats.Lanudry(matrix.Length)} in {StringFormats.ReadableElapsedTime(Now - t0)}".info

        Try
            Call matrix.Flush()
            Call index.Flush()
            Call matrix.Dispose()
            Call index.Dispose()
        Catch ex As Exception
            Call ex.Message.warning
            Call App.LogException(ex)
        End Try
    End Sub
End Module
