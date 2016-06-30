Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects

Namespace EngineSystem.Services.DataAcquisition.DataSerializer

    ''' <summary>
    ''' 数据采集服务与数据存储服务之间的中间层
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class DataSerializer : Inherits RuntimeObject

        Protected Friend _DataFlows As List(Of DataFlowF) = New List(Of DataFlowF)
        Protected Friend _Url As String
        Protected Friend _SuppressPeriodicMessage As Boolean = False

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Url">文件名或者数据库连接参数</param>
        ''' <remarks></remarks>
        Public Sub New(Url As String)
            _Url = Url
        End Sub

        Public Overridable Sub Initialize(arg As String)
        End Sub

        Protected Friend _ErrMessage As String

        Public Overridable Function GetErrMessage() As String
            Return _ErrMessage
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="arg">提交数据的时候所需要的参数</param>
        ''' <remarks></remarks>
        Public MustOverride Function CommitData(Optional arg As String = "") As Integer
        Public MustOverride Sub CreateHandle(List As HandleF(), Table As String)

        Public Overrides Function ToString() As String
            Return _Url
        End Function

        ''' <summary>
        ''' 关闭与数据存储服务之间的连接
        ''' </summary>
        ''' <remarks></remarks>
        Public MustOverride Sub Close(arg As String)

        ''' <summary>
        ''' 数据采集服务向数据存储服务写入数据包
        ''' </summary>
        ''' <param name="DataPackage"></param>
        ''' <remarks></remarks>
        Public Overridable Sub Append(DataPackage As Generic.IEnumerable(Of DataFlowF))
            Call _DataFlows.AddRange(DataPackage)
        End Sub
    End Class
End Namespace