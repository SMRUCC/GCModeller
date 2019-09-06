#Region "Microsoft.VisualBasic::d6dd47e89bbd21a8c1567032c6900473, RDotNET.Extensions.VisualBasic\ScriptBuilder\Helpers\RTypes.vb"

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

    '     Structure RBoolean
    ' 
    '         Properties: [FALSE], [TRUE]
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: RScript
    ' 
    '     Class RExpression
    ' 
    '         Properties: value
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: RScript, ToString
    '         Operators: -, <=, >=
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract

Namespace SymbolBuilder.Rtypes

    ''' <summary>
    ''' TRUE, FALSE, T, F
    ''' </summary>
    Public Structure RBoolean : Implements IScriptProvider

        ''' <summary>
        ''' T
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property [TRUE] As New RBoolean(RScripts.TRUE)
        ''' <summary>
        ''' F
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property [FALSE] As New RBoolean(RScripts.FALSE)

        ReadOnly __value As String

        Private Sub New(value As String)
            __value = value
        End Sub

        Sub New(value As Boolean)
            Call Me.New(If(value, RScripts.TRUE, RScripts.FALSE))
        End Sub

        ''' <summary>
        ''' <see cref="[TRUE]"/> or <see cref="[FALSE]"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function RScript() As String Implements IScriptProvider.RScript
            Return __value
        End Function
    End Structure

    ''' <summary>
    ''' The R Expression object, is a basic parts of the R statement.
    ''' (R表达式，是脚本单词元<see cref="IRToken"/>的一部分)
    ''' </summary>
    Public Class RExpression : Inherits IRToken
        Implements IScriptProvider

        Default Public ReadOnly Property AccInd(expression As String) As RExpression
            Get
                Return $"{RScript()}[{expression}]"
            End Get
        End Property

        <XmlText> Public Property value As String

        Sub New(R As String)
            _value = R
        End Sub

        Sub New(R As IRToken)
            _value = R.RScript
        End Sub

        Sub New()
        End Sub

        ''' <summary>
        ''' Directly returns the <see cref="value"/> string data.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function RScript() As String Implements IScriptProvider.RScript
            Return _value
        End Function

        Public Overrides Function ToString() As String
            Return _value
        End Function

        Public Shared Widening Operator CType(R As String) As RExpression
            Return New RExpression(R)
        End Operator

        Public Overloads Shared Narrowing Operator CType(R As RExpression) As String
            If R Is Nothing Then
                Return Nothing
            End If
            Return R._value
        End Operator

        Public Shared Operator -(R As RExpression) As RExpression
            Return New RExpression("-" & R.RScript)
        End Operator

        ''' <summary>
        ''' variable value assignment
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="token"></param>
        ''' <returns></returns>
        Public Overloads Shared Operator <=(s As String, token As RExpression) As RExpression
            Return New RExpression(s & $" <- {token.RScript}")
        End Operator

        Public Overloads Shared Operator >=(sb As String, token As RExpression) As RExpression
            Throw New InvalidOperationException("NOT_SUPPORT_THIS_OPERATOR")
        End Operator
    End Class
End Namespace
