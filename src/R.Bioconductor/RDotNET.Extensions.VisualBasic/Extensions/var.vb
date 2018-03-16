#Region "Microsoft.VisualBasic::feac868bdbb7360cab8ec94e7d64d5f0, RDotNET.Extensions.VisualBasic\Extensions\var.vb"

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

    ' Class var
    ' 
    '     Properties: Expression, Name, RValue, type
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: [As], Rvariable, ToString
    ' 
    '     Sub: __setValue
    ' 
    '     Operators: <=, >=
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Web.Script.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Serialization.JSON
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

''' <summary>
''' The R runtime variable.(当隐式转换为字符串的时候，返回的是变量名)
''' </summary>
''' 
Public Class var

    Public ReadOnly Property Name As String

    Public ReadOnly Property type As String
        Get
            Return $"typeof({Name})".__call _
                .AsCharacter _
                .ToArray _
                .FirstOrDefault
        End Get
    End Property

    <ScriptIgnore>
    Public ReadOnly Property RValue As SymbolicExpression
        Get
            Return R.Evaluate(Name)
        End Get
    End Property

    Dim _expr As String

    Public Property Expression As String
        Get
            Return _expr
        End Get
        Set(value As String)
            _expr = value
            Call __setValue()
        End Set
    End Property

    Private Sub __setValue()
        Call $"{Name} <- {_expr}".__call
    End Sub

    Sub New()
        Name = App.NextTempName
    End Sub

    Sub New(expr As String)
        Call Me.New
        Me._expr = expr
        Call __setValue()
    End Sub

    Sub New(name As String, expr As String)
        Me.Name = name
        Me._expr = expr
        Call __setValue()
    End Sub

    Public Shared Function Rvariable(var$) As var
        Return New var With {
            ._Name = var,
            ._expr = var
        }
    End Function

    ''' <summary>
    ''' <see cref="out"/>
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Public Function [As](Of T)() As T
        Throw New NotImplementedException
    End Function

    Public Overloads Shared Operator <=(x As var, expr As String) As String
        x.Expression = expr
        Return expr
    End Operator

    Public Overloads Shared Operator >=(x As var, expr As String) As String
        Throw New NotImplementedException
    End Operator

    ''' <summary>
    ''' 因为name是表达式的值的引用源，而这个变量又是经常直接用于字符串插值之中的，
    ''' 所以在这里直接返回name以方便自动生成R分析脚本 
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return Name
    End Function

    ''' <summary>
    ''' 返回R环境之中的变量名
    ''' </summary>
    ''' <param name="var"></param>
    ''' <returns></returns>
    Public Shared Narrowing Operator CType(var As var) As String
        Return var.Name
    End Operator

    Public Shared Widening Operator CType(expr As String) As var
        Return New var(expr)
    End Operator

    Public Shared Widening Operator CType(expr As Integer) As var
        Return New var(expr)
    End Operator

    Public Shared Widening Operator CType(expr As Double) As var
        Return New var(expr)
    End Operator

    Public Shared Widening Operator CType(expr As Long) As var
        Return New var(expr)
    End Operator

    Public Shared Widening Operator CType(expr As Single) As var
        Return New var(expr)
    End Operator

    Public Shared Widening Operator CType(expr As Boolean) As var
        Return New var(Rbool(expr))
    End Operator

    Public Shared Widening Operator CType(expr As String()) As var
        Return New var(expr:=$"c({expr.Select(AddressOf Rstring).JoinBy(", ")})")
    End Operator

    Public Shared Widening Operator CType(expr As Integer()) As var
        Return New var(c(expr))
    End Operator

    Public Shared Widening Operator CType(expr As Double()) As var
        Return New var(c(expr))
    End Operator

    Public Shared Widening Operator CType(expr As Boolean()) As var
        Return New var(c(expr))
    End Operator

    Public Shared Widening Operator CType(expr As Long()) As var
        Return New var(c(expr))
    End Operator

    Public Shared Widening Operator CType(expr As Single()) As var
        Return New var(c(expr))
    End Operator

    Public Shared Widening Operator CType(expr As var()) As var
        Return New var($"c({expr.Select(Function(x) x.Name).JoinBy(", ")})")
    End Operator

    Public Shared Widening Operator CType(expr As Microsoft.VisualBasic.Language.Value) As var
        Return New var(Scripting.ToString(expr.value, NULL))
    End Operator
End Class
