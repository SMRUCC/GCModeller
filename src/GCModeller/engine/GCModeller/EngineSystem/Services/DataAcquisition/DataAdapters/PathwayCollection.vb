Imports System.Text
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    Public Class PathwayCollection : Inherits DataAdapter(Of EngineSystem.ObjectModels.SubSystem.PathwayCollection)
        Implements IDataAdapter

        Sub New(Source As EngineSystem.ObjectModels.SubSystem.PathwayCollection)
            Call MyBase.New(Source)
        End Sub

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "pathways"
            End Get
        End Property

        Public Overrides Function DataSource() As DataSource()
            Return MyBase.System.DataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return System.get_DataSerializerHandles
        End Function
    End Class
End Namespace