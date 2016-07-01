Imports System.Web.Script.Serialization
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.AnalysisTools.CellPhenotype.SSystem.Kernel.ObjectModels
Imports LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver.LDM
Imports Microsoft.VisualBasic.Extensions

Namespace Script

    ''' <summary>
    ''' 可以被保存至文件的脚本模型对象
    ''' </summary>
    Public Class Model : Inherits ModelBaseType

        ''' <summary>
        ''' The user define function.
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property UserFunc As [Function]()
        <XmlElement> Public Property [Constant] As Constant()

        Dim __varHash As Dictionary(Of Var)

        ''' <summary>
        ''' A collection of the system variables.
        ''' (系统中的运行变量的集合)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlArray> Public Property Vars As Var()
            Get
                If __varHash Is Nothing Then
                    Return New Var() {}
                Else
                    Return __varHash.Values.ToArray
                End If
            End Get
            Set(value As Var())
                If value Is Nothing Then
                    __varHash = New Dictionary(Of Var)
                Else
                    __varHash = value.ToDictionary
                End If
            End Set
        End Property

        ''' <summary>
        ''' The data channel in this system kernel.
        ''' (系统中的反应过程数据通道)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlArray> Public Property sEquations As SEquation()
        ''' <summary>
        ''' The disturbing factors in this system.
        ''' (系统中的干扰因素的集合)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlArray> Public Property Experiments As Experiment()

        Public Property Title As String
        Public Property Comment As String

        ''' <summary>
        ''' The ticks count value of the time that exit this simulation.
        ''' (整个内核运行的退出时间) 
        ''' </summary>
        ''' <remarks></remarks>
        <XmlElement> Public Property FinalTime As Integer

        <ScriptIgnore> Public ReadOnly Property Summary As String
            Get
                Return $"{Vars.Length} Substrates and {sEquations.Length} reaction channels."
            End Get
        End Property

        Public Sub Add(x As Var)
            Call __varHash.Add(x)
        End Sub

        Public Function FindObject(x As String) As Var
            Return __varHash(x)
        End Function

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(Comment) Then
                Return Title
            Else
                Return String.Format("{0}; //{1}", Title, Comment)
            End If
        End Function

        ''' <summary>
        ''' Load a model from a compiled xml model file.
        ''' (从一个已经编译好的XML文件加载)
        ''' </summary>
        ''' <param name="Path">The target compiled xml model file.(目标已经编译好的XML模型文件)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Load(Path As String) As Model
            Return Path.LoadXml(Of Model)()
        End Function

        ''' <summary>
        ''' Load from a script file.
        ''' (从一个脚本源文件中获取模型数据)
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Widening Operator CType(Path As String) As Model
            Return ScriptCompiler.Compile(Path)
        End Operator
    End Class
End Namespace