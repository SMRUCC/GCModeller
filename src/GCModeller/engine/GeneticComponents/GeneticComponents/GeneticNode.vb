
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Polypeptides

''' <summary>
''' A node entity in the genetic component network
''' </summary>
Public Class GeneticNode

    ''' <summary>
    ''' 一般是Uniprot蛋白编号
    ''' </summary>
    ''' <returns></returns>
    Public Property ID As String
    ''' <summary>
    ''' 一般是Nt库之中的核酸序列编号
    ''' </summary>
    ''' <returns></returns>
    Public Property Accession As String
    Public Property GO As String()
    Public Property KO As String
    ''' <summary>
    ''' 蛋白序列
    ''' </summary>
    ''' <returns></returns>
    Public Property Sequence As AminoAcid()
    ''' <summary>
    ''' 核酸序列
    ''' </summary>
    ''' <returns></returns>
    Public Property Nt As DNA()
    ''' <summary>
    ''' 简单的功能描述
    ''' </summary>
    ''' <returns></returns>
    Public Property [Function] As String
    ''' <summary>
    ''' 这个节点的数据源之中的原始编号
    ''' </summary>
    ''' <returns></returns>
    Public Property Xref As String

    Public Overrides Function ToString() As String
        Return [Function]
    End Function

End Class

