#Region "Microsoft.VisualBasic::7f613f2eb99e77cc527dd3780bae00a1, sub-system\PLAS.NET\SSystem\System\Elements\Equation.vb"

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

'     Class Equation
' 
'         Properties: Expression, Model, Value
' 
'         Constructor: (+2 Overloads) Sub New
' 
'         Function: Elapsed, Evaluate, get_ObjectHandle, ToString
' 
'         Sub: [Set]
' 
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression.Impl
Imports Microsoft.VisualBasic.Math.Scripting.Types
Imports SMRUCC.genomics.Analysis.SSystem.Script
Imports SMRUCC.genomics.GCModeller.Framework
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel

Namespace Kernel.ObjectModels

    Public Class Equation : Inherits Kernel_Driver.Expression

        ''' <summary>
        ''' 使用代谢底物的UniqueID属性值作为数值替代的表达式
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Expression As String

        Friend Kernel As Kernel

        ''' <summary>
        ''' The target that associated with this channel.
        ''' (与本计算通道相关联的目标对象)
        ''' </summary>
        ''' <remarks></remarks>
        Friend Var As var

        Dim dynamics As Expression
        Dim engine As ExpressionEngine

        Sub New(s As SEquation, engine As ExpressionEngine)
            Me.Model = s
            Me.Expression = s.Expression
            Me.engine = engine
            Me.Identifier = s.x
#If DEBUG Then
            Call Expression.__DEBUG_ECHO
#End If
            Me.dynamics = New ExpressionTokenIcer(Expression) _
                .GetTokens _
                .ToArray _
                .DoCall(AddressOf BuildExpression)
        End Sub

        Sub New(id As String, expr As String, engine As ExpressionEngine)
            Call Me.New(New SEquation(id, expr), engine)
        End Sub

        ''' <summary>
        ''' Evaluate the expression value of the property <see cref="Equation.Expression"></see>.
        ''' (计算<see cref="Equation.Expression"></see>属性表达式的值)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Evaluate() As Double
            Dim rtvl As Double = dynamics.Evaluate(engine)
            Return rtvl
        End Function

        ''' <summary>
        ''' The node states in the current network state.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Value As Double
            Get
                Return Me.Var.Value
            End Get
        End Property

        ''' <summary>
        ''' 执行一次数学运算，然后使用当前所更新的变量值更新表达式计算引擎内部的变量值
        ''' </summary>
        ''' <param name="engine"></param>
        ''' <returns></returns>
        Public Function Elapsed(engine As ExpressionEngine) As Boolean
            Var.Value += (Me.Evaluate * Kernel.Precision)
            engine(Var.UniqueId) = Var.Value

            Return True
        End Function

        Public Overrides Function ToString() As String
            If Var Is Nothing Then
                Return String.Format("{0}'={1}", Identifier, Expression)
            Else
                Return String.Format("{0}; //{1}'={2}", Var.ToString, Identifier, Expression)
            End If
        End Function

        ''' <summary>
        ''' Set up the simulation kernel.
        ''' (设置模拟核心)
        ''' </summary>
        ''' <param name="k"></param>
        ''' <remarks></remarks>
        Public Sub [Set](k As Kernel)
            Kernel = k
            Var = LinqAPI.DefaultFirst(Of var) <=
 _
                From o As var
                In k.Vars
                Where String.Equals(o.UniqueId, Identifier)
                Select o '

            If Var Is Nothing Then
                Var = New var With {
                    .UniqueId = Identifier,
                    .Value = 10.0R
                }
                k.symbolTable(Identifier) = Var
            End If
        End Sub

        Public ReadOnly Property Model As SEquation

        Public Overrides Function get_ObjectHandle() As ObjectHandle
            Return New ObjectHandle With {
                .ID = Identifier,
                .Handle = Handle
            }
        End Function
    End Class
End Namespace
