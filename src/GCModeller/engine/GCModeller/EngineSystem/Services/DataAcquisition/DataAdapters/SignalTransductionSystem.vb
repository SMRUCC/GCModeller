Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    Public Class SignalTransductionSystem : Inherits DelegateSystem
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "SignalTransductionNetwork"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.SignalTransductionNetwork)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return MyBase.System.DataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return MyBase.System.get_DataSerializerHandles
        End Function
    End Class
End Namespace