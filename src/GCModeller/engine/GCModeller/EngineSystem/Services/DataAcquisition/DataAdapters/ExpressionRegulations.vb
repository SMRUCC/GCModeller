Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.ExpressionSystem
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    Public Class TranscriptionRegulation : Inherits DataAdapter(Of ExpressionRegulationNetwork)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "Regulation_Of_Transcription"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = (From [Event] In MyBase.System.NetworkComponents Select New DataSource([Event].Handle, [Event].FluxValue)).ToArray
            Return LQuery
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Dim LQuery = (From [Event] In MyBase.System.NetworkComponents Select [Event].SerialsHandle).ToArray
            Return LQuery
        End Function
    End Class

    Public Class TranslationRegulation : Inherits DataAdapter(Of ExpressionRegulationNetwork)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "Regulation_Of_Translation"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = (From [Event] In MyBase.System._InternalEvent_Translations__ Select New DataSource([Event].Handle, [Event].RegulationValue)).ToArray
            Return LQuery
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Dim LQuery = (From [Event] In MyBase.System._InternalEvent_Translations__ Select [Event].SerialsHandle).ToArray
            Return LQuery
        End Function
    End Class
End Namespace