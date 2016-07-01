Imports SMRUCC.genomics.Toolkits.RNA_Seq.dataExprMAT
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Net.Protocols.RawStream
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic
Imports System.Text

''' <summary>
''' 对PCC矩阵进行快速的二进制序列化
''' </summary>
''' <remarks>由于是一个二维的矩阵，坐标之间有着一一对应的顺序关系，所以这里不可以使用并行化拓展</remarks>
<PackageNamespace("PCC.Matrix.Serialization",
                  Description:="Tools for fast IO of the gene expression pcc matrix.",
                  Category:=APICategories.UtilityTools,
                  Publisher:="xie.guigang@gcmodeller.org")>
Public Module MatrixSerialization

    ''' <summary>
    ''' &lt;TOTAL_BYTES> + &lt;STRING_LENGTH>
    ''' </summary>
    ''' <param name="sample"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Sample.Serialize")>
    Public Function Serialize(sample As ExprSamples) As Byte()
        Dim locusId As Byte() = Encoding.ASCII.GetBytes(sample.locusId)
        Dim array As Streams.Array.Double =
            New Streams.Array.Double With {
                .Values = sample.Values
        }
        Dim arrayBuffer As Byte() = array.Serialize

        ' totalSize + tag_length + tag_string + array
        Dim chunkBuffer As Byte() = New Byte(INT64 + INT32 + locusId.Length + arrayBuffer.Length) {}
        Dim TotalSize As Long = locusId.Length + arrayBuffer.Length
        Dim header As Byte() = BitConverter.GetBytes(TotalSize)
        Dim idLength As Byte() = BitConverter.GetBytes(locusId.Length)
        Dim p As Long = Scan0

        Call System.Array.ConstrainedCopy(header, Scan0, chunkBuffer, p.Move(INT64), INT64)
        Call System.Array.ConstrainedCopy(idLength, Scan0, chunkBuffer, p.Move(INT32), INT32)
        Call System.Array.ConstrainedCopy(locusId, Scan0, chunkBuffer, p.Move(locusId.Length), locusId.Length)
        Call System.Array.ConstrainedCopy(arrayBuffer, Scan0, chunkBuffer, p, arrayBuffer.Length)

        Return chunkBuffer
    End Function

    <ExportAPI("Save.bin")>
    Public Function SaveBin(MAT As PccMatrix, SaveTo As String) As Boolean
        Dim LQuery As Byte() =
            MAT.ToArray(
                Function(sample) MatrixSerialization.Serialize(sample), Parallel:=False).MatrixToVector
        Return LQuery.FlushStream(SaveTo)
    End Function

    ''' <summary>
    ''' 加载二进制数据库
    ''' </summary>
    ''' <param name="from"></param>
    ''' <returns></returns>
    <ExportAPI("Matrix.Load.bin")>
    Public Function Load(from As String) As PccMatrix
        Dim byts As Byte() = IO.File.ReadAllBytes(from)
        Dim samples As New List(Of ExprSamples)
        Dim p As Long = Scan0
        Dim header As Byte() = New Byte(INT64 - 1) {}
        Dim headOffset As Integer = INT64 + INT32

        ' 偏移量是数据块的总长度+INT64的长度
        Do While p < byts.Length - 1
            Call Array.ConstrainedCopy(byts, p, header, Scan0, INT64)

            Dim totalSize As Long = BitConverter.ToInt64(header, Scan0) + headOffset
            Dim chunkBuffer As Byte() = New Byte(totalSize - 1) {}

            Call Array.ConstrainedCopy(byts, p, chunkBuffer, Scan0, chunkBuffer.Length)
            Call p.Move(totalSize + 1)
            Call samples.Add(MatrixSerialization.CreateObject(chunkBuffer))
        Loop

        Return New PccMatrix(samples)
    End Function

    Private Function CreateObject(byts As Byte()) As ExprSamples
        Dim locusLen As Byte() = New Byte(INT32 - 1) {}
        Call Array.ConstrainedCopy(byts, INT64, locusLen, Scan0, INT32)
        Dim len As Integer = BitConverter.ToInt32(locusLen, Scan0)
        Dim chunkBuffer As Byte() = New Byte(len - 1) {}
        Call Array.ConstrainedCopy(byts, INT64 + INT32, chunkBuffer, Scan0, len)
        Dim ID As String = System.Text.Encoding.ASCII.GetString(chunkBuffer)
        locusLen = New Byte(INT64 - 1) {}
        Call Array.ConstrainedCopy(byts, Scan0, locusLen, Scan0, INT64)
        Dim arrayLength As Integer = BitConverter.ToInt64(locusLen, Scan0) - len
        chunkBuffer = New Byte(arrayLength - 1) {}
        Call Array.ConstrainedCopy(byts, INT64 + INT32 + len, chunkBuffer, Scan0, arrayLength)
        Dim samples As New Streams.Array.Double(chunkBuffer)
        Dim sampleValue As New ExprSamples With {
            .locusId = ID,
            .Values = samples.Values
        }
        Return sampleValue
    End Function
End Module
