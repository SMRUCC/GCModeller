Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Map(Of String, String)
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

''' <summary>
''' 代表 DIAMOND BLASTP 结果文件 (.m8) 中的一行记录
''' </summary>
Public Class DiamondAnnotation : Implements IBlastHit, IMap

    ''' <summary>
    ''' 1. 查询序列ID
    ''' </summary>
    ''' <returns></returns>
    Public Property QseqId As String Implements IBlastHit.queryName, IMap.Key

    ''' <summary>
    ''' 2. 目标序列ID
    ''' </summary>
    ''' <returns></returns>
    Public Property SseqId As String Implements IBlastHit.hitName, IBlastHit.description, IMap.Maps

    ''' <summary>
    ''' 3. 比对一致性百分比
    ''' </summary>
    ''' <returns></returns>
    Public Property Pident As Double

    ''' <summary>
    ''' 4. 比对长度
    ''' </summary>
    ''' <returns></returns>
    Public Property Length As Integer

    ''' <summary>
    ''' 5. 错配数
    ''' </summary>
    ''' <returns></returns>
    Public Property Mismatch As Integer

    ''' <summary>
    ''' 6. Gap 打开次数
    ''' </summary>
    ''' <returns></returns>
    Public Property GapOpen As Integer

    ''' <summary>
    ''' 7. 查询序列起始位置
    ''' </summary>
    ''' <returns></returns>
    Public Property QStart As Integer

    ''' <summary>
    ''' 8. 查询序列结束位置
    ''' </summary>
    ''' <returns></returns>
    Public Property QEnd As Integer

    ''' <summary>
    ''' 9. 目标序列起始位置
    ''' </summary>
    ''' <returns></returns>
    Public Property SStart As Integer

    ''' <summary>
    ''' 10. 目标序列结束位置
    ''' </summary>
    ''' <returns></returns>
    Public Property SEnd As Integer

    ''' <summary>
    ''' 11. E-value (期望值)
    ''' </summary>
    ''' <returns></returns>
    Public Property EValue As Double

    ''' <summary>
    ''' 12. Bit Score (比特得分)
    ''' </summary>
    ''' <returns></returns>
    Public Property BitScore As Double

    Public Overrides Function ToString() As String
        Return $"{QseqId} vs {SseqId} | Identity: {Pident}% | E-value: {EValue}"
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetSingleHit() As BestHit
        Return New BestHit With {
            .QueryName = Me.QseqId,
            .HitName = SseqId,
            .identities = Pident,
            .length_hsp = Length,
            .evalue = EValue,
            .score = BitScore,
            .length_query = QEnd - QStart,
            .length_hit = SEnd - SStart
        }
    End Function
End Class
