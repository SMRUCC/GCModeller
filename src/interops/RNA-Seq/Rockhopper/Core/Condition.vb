' /********************************************************************************/
'
'  Rockhopper —— Condition 数据模型
'
'  复刻自原始 Rockhopper（Brian Tjaden, 2013）Java 源码中的 Condition.java。
'  一个 Condition 代表一个实验条件下的全部重复（Replicate）。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Text

Namespace Core

    ''' <summary>
    ''' A Condition keeps track of the RNA-seq data in each
    ''' replicate experiment for a given condition.
    ''' </summary>
    Public Class Condition

        Private ReadOnly replicates As List(Of Replicate)

        ''' <summary>Avg upper quartile over all replicates in all conditions.</summary>
        Private Shared _avgUpperQuartile As Long

        ''' <summary>
        ''' Constructs an empty Condition.
        ''' </summary>
        Public Sub New()
            Me.replicates = New List(Of Replicate)()
            Me.Partner = -1
        End Sub

        ''' <summary>
        ''' 条件名称 / 标签。
        ''' </summary>
        Public Property Name As String

        ''' <summary>
        ''' If there are no replicates, index of most similar other condition.
        ''' </summary>
        Public Property Partner As Integer

        ''' <summary>
        ''' Used for differentially expressed p-value correction.
        ''' </summary>
        Public ReadOnly Property MinDiffExpressionLevel As Integer
            Get
                Return _minDiffExpressionLevel
            End Get
        End Property
        Private _minDiffExpressionLevel As Integer

        ''' <summary>
        ''' Return the Replicate at the specified index.
        ''' </summary>
        Public Function GetReplicate(i As Integer) As Replicate
            If i >= 0 AndAlso i < replicates.Count Then
                Return replicates(i)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Adds a replicate to this Condition.
        ''' </summary>
        Public Sub AddReplicate(r As Replicate)
            Me.replicates.Add(r)
        End Sub

        ''' <summary>
        ''' Return the number of Replicates performed for this Condition.
        ''' </summary>
        Public Function NumReplicates() As Integer
            Return replicates.Count
        End Function

        ''' <summary>
        ''' 全部重复（只读）。
        ''' </summary>
        Public ReadOnly Property Replicates As IReadOnlyList(Of Replicate)
            Get
                Return replicates
            End Get
        End Property

        ''' <summary>
        ''' The background probability decreases as the number of reads increases.
        ''' We use this fact to estimate a minimum level of expression necessary
        ''' for us to be able to compute a p-value of differential expression.
        ''' Currently, the expression threshold is set (based on anecdote and
        ''' experience) to 0.005.
        ''' </summary>
        Public Sub SetMinDiffExpressionLevel()
            Const THRESHOLD As Double = 0.005
            _minDiffExpressionLevel = 0
            While True
                Dim foundThreshold As Boolean = True
                For i As Integer = 0 To replicates.Count - 1
                    If replicates(i).GetBackgroundProb(_minDiffExpressionLevel) >= THRESHOLD Then
                        foundThreshold = False
                    End If
                Next
                If foundThreshold Then Return
                _minDiffExpressionLevel += 1
            End While
        End Sub

        ''' <summary>
        ''' Returns a String representation of this object.
        ''' </summary>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            For i As Integer = 0 To replicates.Count - 1
                sb.Append(replicates(i).ToString() & vbLf)
            Next
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Set the average upper quartile gene expression over
        ''' all replicates in all conditions.
        ''' </summary>
        Public Shared Property AvgUpperQuartile As Long
            Get
                Return _avgUpperQuartile
            End Get
            Set(value As Long)
                _avgUpperQuartile = value
            End Set
        End Property

    End Class

End Namespace
