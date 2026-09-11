' /********************************************************************************/
'
'  Rockhopper —— 比对覆盖度数据模型
'
'  原始 Rockhopper 会将每个重复实验的比对结果压缩为 WIG 覆盖度文件（FileOps 负责读写），
'  Replicate 再从中读取 plus/minus 链的逐碱基读段数。为消除对 Oracle.Java 文件层的依赖，
'  这里把"压缩比对文件"的数据内容显式建模为 AlignmentCoverage，
'  由 IO 层负责读写，Core 层只消费数据。
'
' /********************************************************************************/

Namespace Core

    ''' <summary>
    ''' 单个重复实验、单个基因组（replicon）在正负链上的逐碱基覆盖度。
    ''' </summary>
    Public Class AlignmentCoverage

        ''' <summary>
        ''' 正链每个碱基上的读段数（1-indexed，索引 0 舍弃）。
        ''' </summary>
        Public Property PlusReads As Integer()

        ''' <summary>
        ''' 负链每个碱基上的读段数（1-indexed，索引 0 舍弃）。
        ''' </summary>
        Public Property MinusReads As Integer()

        ''' <summary>
        ''' 该重复实验中比对上的读段的平均长度。
        ''' </summary>
        Public Property AvgLengthReads As Long

        ''' <summary>
        ''' 原始读段文件路径（用于推断重复名称）。
        ''' </summary>
        Public Property ReadFileName As String

        ''' <summary>
        ''' 由原始读段文件名推断出的重复名称（去目录与扩展名）。
        ''' </summary>
        Public Property Name As String

    End Class

End Namespace
