#Region "Microsoft.VisualBasic::2708287673545bce98d244b275544683, GCModeller\sub-system\PLAS.NET\SSystem\System\Elements\Equation.vb"

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


    ' Code Statistics:

    '   Total Lines: 130
    '    Code Lines: 74
    ' Comment Lines: 35
    '   Blank Lines: 21
    '     File Size: 4.36 KB


    '     Class Equation
    ' 
    '         Properties: Expression, Id, Model, precision, Value
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Elapsed, Evaluate, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression.Impl
Imports SMRUCC.genomics.Analysis.SSystem.Script

Namespace Kernel.ObjectModels

    ''' <summary>
    ''' A systems dynamics which is associated with a target symbol
    ''' </summary>
    Public Class Equation : Inherits Expression
        Implements IReadOnlyId

        ''' <summary>
        ''' 使用代谢底物的UniqueID属性值作为数值替代的表达式
        ''' </summary>
        ''' <remarks></remarks>
        Public Property Expression As String

        ''' <summary>
        ''' The target that associated with this channel.
        ''' (与本计算通道相关联的目标对象)
        ''' </summary>
        ''' <remarks></remarks>
        Friend var As var

        Dim dynamics As Expression
        Dim bound As DoubleRange

        Public Property precision As Double

        Public ReadOnly Property Model As SEquation

        ''' <summary>
        ''' the unique identifier
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Id As String Implements IReadOnlyId.Identity
            Get
                Return Model.x
            End Get
        End Property

        ''' <summary>
        ''' The node states in the current network state.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Value As Double
            Get
                Return var.Value
            End Get
        End Property

        Sub New(s As SEquation, bound As DoubleRange)
            Me.bound = bound
            Me.Model = s
            Me.Expression = s.Expression
            Me.dynamics = New ExpressionTokenIcer(Expression) _
                .GetTokens _
                .ToArray _
                .DoCall(AddressOf BuildExpression)
        End Sub

        Sub New(s As SEquation, kernel As Kernel)
            Call Me.New(s, kernel.bounds(s.x))

            Me.precision = kernel.precision
            Me.var = kernel.GetValue(Id)

            If var Is Nothing Then
                var = New var With {
                    .Id = Id,
                    .Value = 10.0R
                }
                kernel.symbolTable(Id) = var
            End If
        End Sub

        ''' <summary>
        ''' Evaluate the expression value of the property <see cref="Equation.Expression"></see>.
        ''' (计算<see cref="Equation.Expression"></see>属性表达式的值)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Overrides Function Evaluate(env As ExpressionEngine) As Double
            Return dynamics.Evaluate(env)
        End Function

        ''' <summary>
        ''' 执行一次数学运算，然后使用当前所更新的变量值更新表达式计算引擎内部的变量值
        ''' </summary>
        ''' <param name="engine"></param>
        ''' <returns></returns>
        Public Function Elapsed(engine As ExpressionEngine) As Boolean
            Dim delta As Double = Evaluate(engine) * precision

            If Double.IsNaN(delta) Then
                delta = 0
                ' delta = Evaluate(engine) * precision
            ElseIf Double.IsPositiveInfinity(delta) OrElse delta > bound.Max Then
                delta = bound.Max
            ElseIf Double.IsNegativeInfinity(delta) OrElse delta < bound.Min Then
                delta = bound.Min
            End If

            var.Value += delta

            If var.Value < 0 Then
                var.Value = 0
            End If

            engine(var.Id) = var.Value

            Return True
        End Function

        Public Overrides Function ToString() As String
            If var Is Nothing Then
                Return String.Format("{0}'={1}", Id, Expression)
            Else
                Return String.Format("{0}; //{1}'={2}", var.ToString, Id, Expression)
            End If
        End Function
    End Class
End Namespace
