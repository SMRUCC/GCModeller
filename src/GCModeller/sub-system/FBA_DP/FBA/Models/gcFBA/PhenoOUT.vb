Imports LANS.SystemsBiology.GCModeller.AnalysisTools.ModelSolvers.FBA.FBA_OUTPUT
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace Models.rFBA

    Public Class PhenoModel : Inherits Simpheny.RXN
        Implements IDynamicMeta(Of Double)

        Dim _props As Dictionary(Of String, Double)

        Sub New()
        End Sub

        Sub New(base As Simpheny.RXN)
            Me.Abbreviation = base.Abbreviation
            Me.Enzyme = base.Enzyme
            Me.Objective = base.Objective
            Me.OfficialName = base.OfficialName
            Me.ReactionId = base.ReactionId
            Me.Reversible = base.Reversible
        End Sub

        <Meta(GetType(Double))>
        Public Property Properties As Dictionary(Of String, Double) Implements IDynamicMeta(Of Double).Properties
            Get
                If _props Is Nothing Then
                    _props = New Dictionary(Of String, Double)
                End If
                Return _props
            End Get
            Set(value As Dictionary(Of String, Double))
                _props = value
            End Set
        End Property

        Public Const tag_UPPER_BOUND As String = "[UPPER_BOUND]"
        Public Const tag_LOWER_BOUND As String = "[LOWER_BOUND]"

        Public Sub AddLowerBound(sample As String, value As Double)
            Call Properties.Add(tag_LOWER_BOUND & sample, value)
        End Sub

        Public Sub AddUpperBound(sample As String, value As Double)
            Call Properties.Add(tag_UPPER_BOUND & sample, value)
        End Sub
    End Class

    Public Class RPKMStat : Inherits DynamicPropertyBase(Of Double)
        Implements sIdEnumerable
        Implements IPhenoOUT

        Public Property Locus As String Implements sIdEnumerable.Identifier
        <Meta(GetType(Double))> Public Overrides Property Properties As Dictionary(Of String, Double)
            Get
                Return MyBase.Properties
            End Get
            Set(value As Dictionary(Of String, Double))
                MyBase.Properties = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return Locus
        End Function
    End Class
End Namespace