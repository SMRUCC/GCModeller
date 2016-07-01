Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    Public Class CultivationMediumsMetabolites : Inherits DataAdapter(Of EngineSystem.ObjectModels.SubSystem.CultivationMediums)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "CultivationMediumsMetabolites"
            End Get
        End Property

        Sub New(DataSource As EngineSystem.ObjectModels.SubSystem.CultivationMediums)
            Call MyBase.New(DataSource)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = (From item In System.CultivationMediums Let value = item.DataSource Select value).ToArray
            Return LQuery
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Dim LQuery = (From item In System.CultivationMediums Select New HandleF With {.Handle = item.Handle, .Identifier = item.Identifier}).ToArray
            Return LQuery
        End Function
    End Class

    Public Class CultivationMediumsReactor : Inherits DataAdapter(Of EngineSystem.ObjectModels.SubSystem.CultivationMediums)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "CultivationMediumsReactor"
            End Get
        End Property

        Sub New(DataSource As EngineSystem.ObjectModels.SubSystem.CultivationMediums)
            Call MyBase.New(DataSource)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return Me.System.DataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return Me.System.get_DataSerializerHandles
        End Function
    End Class
End Namespace