Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Public Module BicorMatrix

    <Extension>
    Public Sub CreateMatrixStore(x As Matrix, repo As String, Optional type As CorrelationEncodings = CorrelationEncodings.Float32)
        Dim matrix = $"{repo}/bicor.dat".Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
        Dim index = $"{repo}/index.dat".Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
        Dim geneIds As String() = x.rownames
        Dim nSample As Integer = x.sample_count
        Dim t0 = Now

        ' 20261004 zip archive is not working as expected due to the reason
        ' of zip stream can not be random access(can not be seek)

        ' ② 逐行 bicor 计算（模拟耗时的一次性矩阵计算），边算边写 corstore
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
                                   row(j) = CSng(Bicor.BiweightMidcorrelation(vi, x(j).experiments))
                               End If
                           End Sub)

                Call bar.SetLabel(geneIds(i))
                Call writer.WriteRow(geneIds(i), row)
            Next

            Call writer.Complete()
        End Using

        Call $"correlation store written: {StringFormats.Lanudry(matrix.Length)} in {StringFormats.ReadableElapsedTime(Now - t0)}".info

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
