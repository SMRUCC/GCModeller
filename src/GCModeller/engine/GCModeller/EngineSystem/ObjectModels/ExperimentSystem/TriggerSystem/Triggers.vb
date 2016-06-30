Imports System.Reflection
Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports Microsoft.VisualBasic.ComponentModel

Namespace EngineSystem.ObjectModels.ExperimentSystem.Triggers

    Public Class ConditionalTrigger : Inherits TriggerBase
        Implements IAddressHandle

        ''' <summary>
        ''' 对Action的调用方法
        ''' </summary>
        ''' <remarks></remarks>
        Public Invoke As Func(Of Integer)
        Protected Friend _TriggerSystem As TriggerSystem
        ReadOnly TriggedCondition As MethodInfo

        Protected Sub New(TriggerCondition As MethodInfo)
            Me.TriggedCondition = TriggerCondition
        End Sub

        Protected Friend Shared Function CreateObject(TestMethod As MethodInfo, TriggerSystem As TriggerSystem) As ConditionalTrigger
            Dim Trigger As ConditionalTrigger = New ConditionalTrigger(TriggerCondition:=TestMethod)
            Trigger._TriggerSystem = TriggerSystem
            Trigger.Handle = Val(Regex.Match(TestMethod.Name, "\d+").Value)
            Return Trigger
        End Function

        ''' <summary>
        ''' 通过比较查看当前的触发器对象是否满足触发条件
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function TriggerTest() As Boolean
            Dim f As Boolean = DirectCast(TriggedCondition.Invoke(Nothing, _TriggerSystem._ComponentSource), Boolean)
            Return f
        End Function

        Public Property Handle As Integer Implements IAddressHandle.Address

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose( disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class

    Public Class PeriodicTrigger : Inherits TriggerBase

        ''' <summary>
        ''' 两个数字之间的关系比较
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum QuantitativeRelations
            ''' <summary>
            ''' 等于
            ''' </summary>
            ''' <remarks></remarks>
            Equals
            ''' <summary>
            ''' 不等于
            ''' </summary>
            ''' <remarks></remarks>
            NotEquals
            ''' <summary>
            ''' 大于
            ''' </summary>
            ''' <remarks></remarks>
            GreaterThan
            ''' <summary>
            ''' 小于
            ''' </summary>
            ''' <remarks></remarks>
            SmallerThan
            ''' <summary>
            ''' 大于或等于
            ''' </summary>
            ''' <remarks></remarks>
            GreaterThanOrEquals
            ''' <summary>
            ''' 小于或等于
            ''' </summary>
            ''' <remarks></remarks>
            SmallerThanOrEquals
        End Enum

        Public Shared ReadOnly Property ComparedMethods As Dictionary(Of QuantitativeRelations, Func(Of Double, Double, Boolean)) =
            New Dictionary(Of QuantitativeRelations, Func(Of Double, Double, Boolean)) From {
 _
                {QuantitativeRelations.Equals, Function(a As Double, b As Double) a = b},
                {QuantitativeRelations.NotEquals, Function(a As Double, b As Double) a <> b},
                {QuantitativeRelations.GreaterThan, Function(a As Double, b As Double) a > b},
                {QuantitativeRelations.SmallerThan, Function(a As Double, b As Double) a < b},
                {QuantitativeRelations.GreaterThanOrEquals, Function(a As Double, b As Double) a >= b},
                {QuantitativeRelations.SmallerThanOrEquals, Function(a As Double, b As Double) a <= b}
        }

        ''' <summary>
        ''' 获取目标对象的值的方法
        ''' </summary>
        ''' <remarks></remarks>
        Public GetTargetValue As Func(Of Double)
        ''' <summary>
        ''' 将要进行比较的设定值，在构造函数中初始化之后就不能够再进行修改
        ''' </summary>
        ''' <remarks></remarks>
        Protected ReadOnly ComparedValue As Double
        Protected ReadOnly ComparedMethod As Func(Of Double, Double, Boolean)

        Public Overrides Function TriggerTest() As Boolean
            Dim f As Boolean = ComparedMethod(GetTargetValue(), ComparedValue)
            Return f
        End Function
    End Class

    Public MustInherit Class TriggerBase : Inherits RuntimeObject

        Protected Friend _strCondition As String

#Region "Methods"
        Public MustOverride Function TriggerTest() As Boolean

        Public Overrides Function ToString() As String
            Return _strCondition
        End Function
#End Region

    End Class
End Namespace
