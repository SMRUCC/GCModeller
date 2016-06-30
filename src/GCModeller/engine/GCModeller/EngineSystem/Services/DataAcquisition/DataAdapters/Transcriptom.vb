Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    ''' <summary>
    ''' 获取转录产物的实时浓度的数据转接器
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Transcriptome : Inherits DataAdapter(Of EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "Transcriptome"
            End Get
        End Property

        Sub New(ExpressionRegulationNetwork As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
            Call MyBase.New(ExpressionRegulationNetwork)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = From Transcript As ObjectModels.Entity.Transcript
                         In MyBase.System._InternalTranscriptsPool
                         Select New DataSource(Transcript.Handle, Transcript.Quantity)  '
            Return LQuery.ToArray
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Dim retVal = (From item In MyBase.System._InternalTranscriptsPool Select New HandleF With {.Handle = item.Handle, .Identifier = item.Identifier.Replace("-transcript", "")}).ToArray
            Return retVal
        End Function
    End Class

    Public Class TranscriptionFlux : Inherits Transcriptome
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "transcription_flux"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return MyBase.System.DataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return (From item In MyBase.System.NetworkComponents Select item.SerialsHandle).ToArray
        End Function
    End Class

    Public Class BasalTranscription : Inherits DataAdapter(Of EngineSystem.ObjectModels.SubSystem.ExpressionSystem.BasalExpressionKeeper)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "basal_transcription_flux"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.BasalExpressionKeeper)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return (From item In System.BasalExpressionFluxes Let Value = item.DataSource Select Value).ToArray
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return (From item In System.BasalExpressionFluxes Select item.SerialsHandle).ToArray
        End Function
    End Class

    Public Class BasalTranslationFlux : Inherits LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.ExpressionSystem.BasalExpressionKeeper)
        Implements IDataAdapter

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.BasalExpressionKeeper)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = (From Translation In System.BasalTranslationFluxs Let value = Translation.DataSource Select value).ToArray
            Return LQuery
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Dim LQuery = (From Translation In System.BasalTranslationFluxs Select Translation.SerialsHandle).ToArray
            Return LQuery
        End Function

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "BasalTranslationFlux"
            End Get
        End Property
    End Class
End Namespace