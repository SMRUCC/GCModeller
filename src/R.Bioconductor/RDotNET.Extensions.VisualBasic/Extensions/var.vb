#Region "Microsoft.VisualBasic::2e3b65691a5bf8206d6aa8195e9b0d16, RDotNET.Extensions.VisualBasic\Extensions\var.vb"

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
    '     Properties: Expression, name, RValue, type
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: [As], Enumerates, EvaluateAs, Rvariable, ToString
    ' 
    '     Sub: (+2 Overloads) Dispose, setValInternal
    ' 
    '     Operators: <=, >=
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Web.Script.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports Rbase = RDotNET.Extensions.VisualBasic.API.base

''' <summary>
''' The R runtime variable.(当隐式转换为字符串的时候，返回的是变量名)
''' </summary>
''' 
Public Class var : Implements IDisposable

    ''' <summary>
    ''' The variable name in R runtime environment.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property name As String

    Public ReadOnly Property type As String
        Get
            Return $"typeof({name})".__call _
                .AsCharacter _
                .ToArray _
                .FirstOrDefault
        End Get
    End Property

    <ScriptIgnore>
    Public ReadOnly Property RValue As SymbolicExpression
        Get
            Return R.Evaluate(name)
        End Get
    End Property

    Default Public Property Item(attrName As String) As String
        Get
            With RDotNetGC.Allocate
                Call $"{ .ByRef} <- {name}[['{attrName}']];".__call
                Return .ByRef
            End With
        End Get
        Set(value As String)
            Call $"{name}[['{attrName}']] <- {value};".__call
        End Set
    End Property

    Dim expr As String

    Public Property Expression As String
        Get
            Return expr
        End Get
        Set(value As String)
            expr = value

            Call setValInternal()
        End Set
    End Property

    Private Sub setValInternal()
        SyncLock R
            Call R.Evaluate($"{name} <- {expr}")
        End SyncLock
    End Sub

    Sub New()
        name = RDotNetGC.Allocate
    End Sub

    ''' <summary>
    ''' When this variable object narrowing convert to <see cref="String"/> type, 
    ''' then the ctype function will returns the <see cref="name"/> property 
    ''' value.
    ''' </summary>
    ''' <param name="expr">A value expression for initialize current R variable</param>
    Sub New(expr As String)
        Call Me.New
        Me.expr = expr

        Call setValInternal()
    End Sub

    Sub New(name As String, expr As String)
        Me.name = name
        Me.expr = expr

        Call setValInternal()
    End Sub

    ''' <summary>
    ''' 这个函数会枚举出list对象之中的所有的成员
    ''' </summary>
    ''' <returns></returns>
    Public Iterator Function Enumerates() As IEnumerable(Of NamedValue(Of String))
        For Each name As String In Rbase.names(Me.name)
            Yield New NamedValue(Of String)(name, $"{Me.name}$'{name}'")
        Next
    End Function

    Public Shared Function Rvariable(var$) As var
        Return New var With {
            ._name = var,
            .expr = var
        }
    End Function

    Public Shared Function EvaluateAs(Of T)(var As String) As T
        Dim ref As SymbolicExpression = R.Evaluate(var)
        Dim value As Object

        Select Case GetType(T)
            Case GetType(Integer)
                value = ref.AsInteger.First
            Case GetType(Integer())
                value = ref.AsInteger.ToArray
            Case GetType(Double)
                value = ref.AsNumeric.First
            Case GetType(Double())
                value = ref.AsNumeric.ToArray
            Case Else
                Throw New NotImplementedException(GetType(T).FullName)
        End Select

        Return DirectCast(value, T)
    End Function

    ''' <summary>
    ''' 可以尝试使用这个函数将<see cref="name"/>在R语言环境之中的变量引用结果解析
    ''' 为``.NET``环境之中的对象结果值
    ''' 
    ''' + 如果是初级类型, 则这个函数可以直接进行转换
    ''' + 但是对于复杂的数据类型, 则需要在类型定义申明上面添加<see cref="out"/>自定义解释器的修饰
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function [As](Of T)() As T
        Return EvaluateAs(Of T)(var:=name)
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
        Return name
    End Function

    ''' <summary>
    ''' 返回R环境之中的变量名
    ''' </summary>
    ''' <param name="var"></param>
    ''' <returns></returns>
    Public Shared Narrowing Operator CType(var As var) As String
        Return var.name
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
        Return New var(SymbolBuilder.c(expr))
    End Operator

    Public Shared Widening Operator CType(expr As Double()) As var
        Return New var(SymbolBuilder.c(expr))
    End Operator

    Public Shared Widening Operator CType(expr As Boolean()) As var
        Return New var(SymbolBuilder.c(expr))
    End Operator

    Public Shared Widening Operator CType(expr As Long()) As var
        Return New var(SymbolBuilder.c(expr))
    End Operator

    Public Shared Widening Operator CType(expr As Single()) As var
        Return New var(SymbolBuilder.c(expr))
    End Operator

    Public Shared Widening Operator CType(expr As var()) As var
        Return New var($"c({expr.Select(Function(x) x.name).JoinBy(", ")})")
    End Operator

    Public Shared Widening Operator CType(expr As Microsoft.VisualBasic.Language.Value) As var
        Return New var(Scripting.ToString(expr.Value, NULL))
    End Operator

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    ''' <summary>
    ''' Call gc() in R environment.
    ''' </summary>
    ''' <param name="disposing"></param>
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                Rbase.rm(list:=name)
                Rbase.gc()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
