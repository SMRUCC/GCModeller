Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComparativeAlignment

    Public Structure ColorLegend : Implements sIdEnumerable

        Public Property color As Color

        Sub New(s As String, color As Color)
            Me.type = s
            Me.color = color
        End Sub

        Public Property type As String Implements sIdEnumerable.Identifier

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function TCSColors() As Dictionary(Of ColorLegend)
            Dim colours As New Dictionary(Of ColorLegend)
            colours += New ColorLegend("Gene encoding HyHK", Color.Red)
            colours += New ColorLegend("Gene encoding orthordox HK", Color.Cyan)
            colours += New ColorLegend("Gene encoding RR", Color.Purple)
            Return colours
        End Function
    End Structure

    Public Class DrawingModel

#Region "PTT regions"
        Public Property Query As ComparativeGenomics.GenomeModel
        Public Property aligns As ComparativeGenomics.GenomeModel()
#End Region

        ''' <summary>
        ''' BBH result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property links As Orthology()

        ''' <summary>
        ''' <see cref="Query"></see>的基因组序列，这个适用于计算GCSkew的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property nt As FASTA.FastaToken
        Public Property COGColors As Dictionary(Of String, Brush)
        Public Property Legends As Dictionary(Of ColorLegend)

        Public Iterator Function EnumerateTitles() As IEnumerable(Of String)
            Yield Query.Title

            For Each align As ComparativeGenomics.GenomeModel In aligns
                Yield Query.Title
            Next
        End Function
    End Class

    Public Class Orthology : Inherits ComparativeGenomics.GeneLink

        ''' <summary>
        ''' query与subject的基因组的组合字符串
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks> 
        Public Property spPairs As String

        Public Overrides Function ToString() As String
            Return String.Format("{0}  <-->  {1}", Me.genome1, Me.genome2)
        End Function
    End Class
End Namespace