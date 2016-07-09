#Region "Microsoft.VisualBasic::eead7dba5f9e0641b5695815dd891052, ..\GCModeller\sub-system\PLAS.NET\SSystem\System\Elements\Equation.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.Analysis.SSystem.Script
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel

Namespace Kernel.ObjectModels

    Public Class Equation : Inherits Expression

        Dim sBuilder As StringBuilder = New StringBuilder(1024)

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

        Sub New(s As SEquation)
            Me.Model = s
            Me.Expression = s.Expression
            Me.Identifier = s.x
        End Sub

        Sub New(id As String, expr As String)
            Call Me.New(New SEquation(id, expr))
        End Sub

        ''' <summary>
        ''' Evaluate the expression value of the property <see cref="Equation.Expression"></see>.(计算<see cref="Equation.Expression"></see>属性表达式的值)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Evaluate() As Double
            Call Me.sBuilder.Clear()

            sBuilder.Append(Me._Expression)

            For Each e In Kernel.Vars 'Replace the name using the value
                Call sBuilder.Replace(e.UniqueId, e.Value)
            Next

            Dim rtvl As Double = Microsoft.VisualBasic.Mathematical.Expression.Evaluate(sBuilder.ToString)
            Return rtvl
        End Function

        Public Overrides ReadOnly Property Value As Double
            Get
                Return Me.Var.Value
            End Get
        End Property

        Public Function Elapsed() As Boolean
            Var.Value += (Me.Evaluate * 0.1)
            Call Microsoft.VisualBasic.Mathematical.ScriptEngine.SetVariable(Var.UniqueId, Var.Value)

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
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub [Set](e As Kernel)
            Dim Query As IEnumerable(Of var) = From o As var
                                               In e.Vars
                                               Where String.Equals(o.UniqueId, Identifier)
                                               Select o '
            Kernel = e
            Var = Query.First
        End Sub

        Public ReadOnly Property Model As SEquation

        Public Overrides Function get_ObjectHandle() As ObjectHandle
            Return New ObjectHandle With {
                .Identifier = Identifier,
                .Handle = Handle
            }
        End Function
    End Class
End Namespace
