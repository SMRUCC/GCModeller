Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel

Namespace Regprecise

    ''' <summary>
    ''' 调控关系，这个对象相当于网络之中的边对象
    ''' </summary>
    Public Class RegulationFootprint : Implements IPolymerSequenceModel

        ''' <summary>
        ''' 预测出来的基因组之中的调控因子的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Property regulator As String
        Public Property family As String
        ''' <summary>
        ''' 预测出来的被调控的基因
        ''' </summary>
        ''' <returns></returns>
        Public Property regulated As String

        ''' <summary>
        ''' motif位点的基因组坐标位置
        ''' </summary>
        ''' <returns></returns>
        <Column("motif-context", GetType(NucleotideLocationParser))>
        Public Property motif As NucleotideLocation

        ''' <summary>
        ''' 这个位置上的序列片段数据
        ''' </summary>
        ''' <returns></returns>
        Public Property sequenceData As String Implements IPolymerSequenceModel.SequenceData

        ''' <summary>
        ''' motif位点到被调控的基因之间的最短距离
        ''' </summary>
        ''' <returns></returns>
        Public Property distance As Integer
        ''' <summary>
        ''' 这个位点所属的复制子的编号，这个属性是为了将基因组的染色体DNA和质粒DNA区分开
        ''' </summary>
        ''' <returns></returns>
        Public Property replicon As String
        ''' <summary>
        ''' 受调控的基因的功能
        ''' </summary>
        ''' <returns></returns>
        Public Property product As String
        Public Property regulog As String
        Public Property biological_process As String
        Public Property effector As String
        Public Property mode As String

        ''' <summary>
        ''' 来自于RegPrecise数据库之中的调控因子注释来源
        ''' </summary>
        ''' <returns></returns>
        Public Property regprecise As String
        ''' <summary>
        ''' 相似度
        ''' </summary>
        ''' <returns></returns>
        Public Property identities As Double

        ''' <summary>
        ''' 注释来源的基因组名称
        ''' </summary>
        ''' <returns></returns>
        Public Property species As String

        ''' <summary>
        ''' motif位点的预测来源
        ''' </summary>
        ''' <returns></returns>
        Public Property site As String

        Public Overrides Function ToString() As String
            Return $"[{family}]..{regulog}..{regulator} ({mode}) {regulated}"
        End Function

    End Class

End Namespace