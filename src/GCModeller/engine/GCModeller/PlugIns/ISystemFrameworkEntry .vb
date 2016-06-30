Imports System.Reflection
Imports System.Text
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.MySQL

Namespace PlugIns

    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)>
    Public Class ISystemFrameworkEntry : Inherits Attribute

        Public Interface ISystemFramework
            ReadOnly Property DataSource As DataSource()
            Function Initialzie() As Integer
            'Function Tick() As Integer

            ''' <summary>
            ''' 从本系统模块之中创建一个数据采集服务对象实例
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Function CreateServiceInstanceSerials() As IDataAcquisitionService()
        End Interface

        Public Enum Types As Integer
            ''' <summary>
            ''' 外部系统模块加载失败
            ''' </summary>
            ''' <remarks></remarks>
            BROKEN_ASSEMBLY = -1

            CellSystem
            DelegateSystem
            ExpressionRegulationNetwork
            Metabolism
        End Enum

        ''' <summary>
        ''' Name of this computational model.(这个计算模型模块的名称)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ModelName As String
        ''' <summary>
        ''' 这个计算模型的代码编写作者列表，建议使用分号分隔
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Authors As String
        ''' <summary>
        ''' 这个计算模型的引用文献列表，建议使用分号分隔
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Articles As String
        Public Property Type As ISystemFrameworkEntry.Types
        ''' <summary>
        ''' ISystemFramework interface type information of the external model assembly.(外部对象模型代码的类型信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ISystem As TypeInfo

        Public Property Description As String

        Public Function GetDescription() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Call sBuilder.AppendFormat("Model Name:        [{0}]  {1}" & vbCrLf, Type.ToString, ModelName)
            Call sBuilder.AppendFormat("EntryPoint:         {1}" & vbCrLf, Type.ToString, ISystem.FullName)
            Call sBuilder.AppendFormat("Authors:            {0}" & vbCrLf, Authors)
            Call sBuilder.AppendFormat("Reference Articles: {0}" & vbCrLf, Articles)
            Call sBuilder.AppendFormat("Description:        {0}" & vbCrLf, Description)

            Return sBuilder.ToString
        End Function

        Public Function Initialize(System As TypeInfo) As ISystemFrameworkEntry
            Me.ISystem = System
            Return Me
        End Function

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(ModelName) Then
                Return Type.ToString
            Else
                Return String.Format("[{0}] {1}", ModelName, Type.ToString)
            End If
        End Function
    End Class
End Namespace