Imports System
Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.ComponentModel
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.ComponentModel
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.Language

Namespace ComparativeGenomics

    ''' <summary>
    ''' 比较两个基因组的差异的模型
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DrawingModel
        Public Property Genome1 As GenomeModel
        Public Property Genome2 As GenomeModel
        Public Property Links As GeneLink()
    End Class

    Public Class GenomeModel : Implements IEnumerable(Of GeneObject)

        Public Property genes As GeneObject()
        Public Property Length As Integer
        Public Property Title As String

        ''' <summary>
        ''' 假若是一个完整的基因组，则这个属性为0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SegmentOffset As Integer

        Public Overrides Function ToString() As String
            Return Title
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of GeneObject) Implements IEnumerable(Of GeneObject).GetEnumerator
            For Each item In genes
                Yield item
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class

    ''' <summary>
    ''' 两个基因组之间的相互共同的基因
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneLink

        Public Property genome1 As String
        Public Property genome2 As String
        Public Property Color As Color
        Public Property annotation As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} === {1};   //{2}", genome1, genome2, annotation)
        End Function

        Public Overloads Function Equals(id1 As String, id2 As String) As Boolean
            Return (String.Equals(id1, genome1, StringComparison.OrdinalIgnoreCase) AndAlso String.Equals(id2, genome2, StringComparison.OrdinalIgnoreCase)) OrElse
                (String.Equals(id2, genome1, StringComparison.OrdinalIgnoreCase) AndAlso String.Equals(id1, genome2, StringComparison.OrdinalIgnoreCase))
        End Function
    End Class
End Namespace