Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    Public Class TFRegulators : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
        Implements IDataAdapter

        Dim _Regulators As EngineSystem.ObjectModels.Entity.Compound()

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
            Call MyBase.New(System)

            Dim ChunkBuffer = (From Model In System.NetworkComponents.AsParallel
                               Where Not Model.MotifSites.IsNullOrEmpty
                               Select (From item
                                       In Model.MotifSites
                                       Select (From n In item.Regulators Select n.EntityBaseType).ToArray).ToArray.MatrixToVector).ToArray.MatrixToVector
            Me._Regulators = (From item As ObjectModels.Entity.Compound In ChunkBuffer Select item Distinct).ToArray
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = (From item In _Regulators Let value = item.DataSource Select value).ToArray
            Return LQuery
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Dim LQuery = (From item In _Regulators Select item.SerialsHandle).ToArray
            Return LQuery
        End Function

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "TFRegulatorCompounds"
            End Get
        End Property
    End Class
End Namespace