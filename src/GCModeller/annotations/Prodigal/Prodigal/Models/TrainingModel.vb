''' <summary>
''' 训练模型 - 包含编码区模型、RBS模型、起始密码子模型
''' </summary>
Public Class TrainingModel
    ''' <summary>编码区六聚体频率（4096个值，索引=HexamerToIndex）</summary>
    Public Property CodingHexamerCount As Double()

    ''' <summary>非编码区六聚体频率</summary>
    Public Property NoncodingHexamerCount As Double()

    ''' <summary>六聚体对数似然比得分</summary>
    Public Property HexamerScores As Double()

    ''' <summary>起始密码子频率</summary>
    Public Property StartCodonFreq As Dictionary(Of String, Double)

    ''' <summary>RBS位置权重矩阵（行=位置，列=A/C/G/T）</summary>
    Public Property RbsPwm As Double(,)

    ''' <summary>RBS模体列表</summary>
    Public Property RbsMotifs As List(Of String)

    ''' <summary>RBS模体得分表</summary>
    Public Property RbsMotifScores As Dictionary(Of String, Double)

    ''' <summary>GC含量</summary>
    Public Property GcContent As Double

    ''' <summary>平均基因长度</summary>
    Public Property AvgGeneLength As Double

    ''' <summary>模型是否已训练</summary>
    Public Property Trained As Boolean

    ''' <summary>训练迭代次数</summary>
    Public Property IterationCount As Integer

    ''' <summary>训练使用的基因数量</summary>
    Public Property TrainingGeneCount As Integer

    ''' <summary>编码区六聚体总数</summary>
    Public Property CodingHexamerTotal As Double

    ''' <summary>非编码区六聚体总数</summary>
    Public Property NoncodingHexamerTotal As Double

    ''' <summary>模型版本号</summary>
    Public Property Version As String = "1.0"

    Public Sub New()
        ReDim CodingHexamerCount(4095)
        ReDim NoncodingHexamerCount(4095)
        ReDim HexamerScores(4095)
        StartCodonFreq = New Dictionary(Of String, Double) From {
            {"ATG", 0.75}, {"GTG", 0.15}, {"TTG", 0.1}
        }
        RbsPwm = New Double(5, 3) {}
        RbsMotifs = New List(Of String) From {
            "AGGAGG", "AGGAG", "GGAGG", "AGGA", "GAGG", "GGAG", "GAG", "AGG", "GGG"
        }
        RbsMotifScores = New Dictionary(Of String, Double)
        GcContent = 0.5
        AvgGeneLength = 900
        Trained = False
        IterationCount = 0
        TrainingGeneCount = 0
        CodingHexamerTotal = 0
        NoncodingHexamerTotal = 0
    End Sub
End Class