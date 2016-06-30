Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.ObjectModels.Entity

    ''' <summary>
    ''' 对生化反应过程其调控作用的小分子化合物，其调控的对象包括反应过程以及酶分子对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ReactionModifier : Inherits Compound

        Protected Friend RegulatorBaseType As GCML_Documents.XmlElements.SignalTransductions.Regulator

        <DumpNode> Public ReadOnly Property InhibitionType As Boolean
            Get
                Return RegulatorBaseType.Activation
            End Get
        End Property

        <DumpNode> Public Overrides Property Quantity As Double
            Get
                Return MyBase.EntityBaseType.Quantity
            End Get
            Set(value As Double)
                MyBase.EntityBaseType.Quantity = value
            End Set
        End Property

        Public Overrides ReadOnly Property DataSource As DataSource
            Get
                Return New DataSource(Handle, EntityBaseType.DataSource.value)
            End Get
        End Property

        <DumpNode> Public b = 2
        <DumpNode> Public m As Double = 2

        Sub New(Regulator As GCML_Documents.XmlElements.SignalTransductions.Regulator, Metabolite As Entity.Compound)
            Me.RegulatorBaseType = Regulator
            MyBase.EntityBaseType = Metabolite
            MyBase.Identifier = Regulator.Identifier
            Me.ModelBaseElement = Metabolite.ModelBaseElement
        End Sub

        Protected Friend Sub New()
        End Sub

        Public Shared Function GetRegulationFluxValue(Regulator As ReactionModifier) As Double
            Dim v As Double = 1

            If Regulator.InhibitionType Then
                v = 1 / (1 + (Regulator.Quantity / Regulator.b) ^ Regulator.m)
            Else
                Dim n = (Regulator.Quantity / Regulator.b) ^ Regulator.m
                v = n / (1 + n)
            End If

            Return v
        End Function

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.EntityReactionModifier
            End Get
        End Property
    End Class
End Namespace