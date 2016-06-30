Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports Microsoft.VisualBasic.DataVisualization
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace DataVisualization.DynamicMap

    Public MustInherit Class IMapBuilder : Inherits RuntimeObject

        Protected _cellSystemODM As EngineSystem.ObjectModels.SubSystem.CellSystem

        Sub New(ObjectModel As EngineSystem.ObjectModels.SubSystem.CellSystem)
            _cellSystemODM = ObjectModel
        End Sub
    End Class

    Public Class Component : Inherits Network.FileStream.Node
        Public Property Quantity As Double
    End Class

    Public Class ComponentInteraction : Inherits Network.FileStream.NetworkEdge

        Dim _InteractionType As InteractionTypes

        Public Shadows Property InteractionType As InteractionTypes
            Get
                Return _InteractionType
            End Get
            Set(value As InteractionTypes)
                _InteractionType = value
                MyBase.InteractionType = value.ToString
            End Set
        End Property

        <Column("FluxValue")> Public Overrides Property Confidence As Double
            Get
                Return MyBase.Confidence
            End Get
            Set(value As Double)
                MyBase.Confidence = value
            End Set
        End Property

        Public Function ShadowCopy() As ComponentInteraction
            Return New ComponentInteraction With {
                .FromNode = FromNode,
                .ToNode = ToNode,
                .InteractionType = InteractionType
            }
        End Function
    End Class
End Namespace