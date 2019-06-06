Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace DataStructure

    ''' <summary>
    ''' The gene expression data samples file.(基因的表达数据样本)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DataFrameRow

        Public Property geneID As String
        ''' <summary>
        ''' This gene's expression value in the different experiment condition.(同一个基因在不同实验之下的表达值)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property experiments As Double()

        ''' <summary>
        ''' Gets the sample counts of current gene expression data.(获取基因表达数据样本数目)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property samples As Integer
            Get
                Return experiments.TryCount
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}   ==> {1}", geneID, String.Join(", ", experiments))
        End Function

        ''' <summary>
        ''' Load the PfsNET file1 and file2 data into the memory.(加载PfsNET计算数据之中的文件1和文件2至计算机内存之中)
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadData(path As String) As DataFrameRow()
            Dim LQuery As DataFrameRow() =
                LinqAPI.Exec(Of DataFrameRow) <= From line As String
                                                 In IO.File.ReadAllLines(path)
                                                 Let Tokens As String() = Strings.Split(line, vbTab)
                                                 Select New DataFrameRow With {
                                                     .geneID = Tokens.First,
                                                     .experiments = (From s As String
                                                                          In Tokens
                                                                     Select Val(s)).ToArray
                                                 }
            Return LQuery
        End Function

        Public Shared Function TakeSamples(data As DataFrameRow(), sampleVector As Integer(), reversed As Boolean) As DataFrameRow()
            Dim LQuery As DataFrameRow() =
                LinqAPI.Exec(Of DataFrameRow) <= From x As DataFrameRow
                                                 In data
                                                 Let samples As Double() = x.experiments.Takes(sampleVector, reversed:=reversed)
                                                 Select New DataFrameRow With {
                                                     .geneID = x.geneID,
                                                     .experiments = samples
                                                 }
            Return LQuery
        End Function

        ''' <summary>
        ''' 以列为单位
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateApplyFunctionCache(data As DataFrameRow()) As KeyValuePair(Of String(), Double()())
            Dim LQuery = (From i As Integer In data.First.experiments.Sequence
                          Let rows = (From row In data Select row.experiments(i)).ToArray
                          Select rows).ToArray
            Return New KeyValuePair(Of String(), Double()())((From row In data Select row.geneID).ToArray, LQuery)
        End Function

        Public Shared Function CreateDataFrameFromCache(names As String(), cols As Double()()) As DataFrameRow()
            Dim LQuery As DataFrameRow() =
                LinqAPI.Exec(Of DataFrameRow) <= From i As SeqValue(Of String)
                                                 In names.SeqIterator
                                                 Let samples As Double() = (From col As Double()
                                                                            In cols
                                                                            Select col(i.i)).ToArray
                                                 Select New DataFrameRow With {
                                                     .geneID = i.value,
                                                     .experiments = samples
                                                 }
            Return LQuery
        End Function
    End Class
End Namespace