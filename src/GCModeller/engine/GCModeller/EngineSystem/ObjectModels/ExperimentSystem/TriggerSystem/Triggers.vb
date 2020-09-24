#Region "Microsoft.VisualBasic::138344d1c48993483c5f4687d7c9c87e, engine\GCModeller\EngineSystem\ObjectModels\ExperimentSystem\TriggerSystem\Triggers.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class ConditionalTrigger
    ' 
    '         Properties: Handle
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: CreateObject, TriggerTest
    ' 
    '         Sub: Assign
    ' 
    '     Class PeriodicTrigger
    ' 
    ' 
    '         Enum QuantitativeRelations
    ' 
    '             Equals, GreaterThan, GreaterThanOrEquals, NotEquals, SmallerThan
    '             SmallerThanOrEquals
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: ComparedMethods
    ' 
    '     Function: TriggerTest
    ' 
    '     Class TriggerBase
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports Microsoft.VisualBasic.ComponentModel

Namespace EngineSystem.ObjectModels.ExperimentSystem.Triggers

    Public Class ConditionalTrigger : Inherits TriggerBase
        Implements IAddressOf

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

        Public Property Handle As Integer Implements IAddressOf.Address

        Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Me.Handle = address
        End Sub
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
