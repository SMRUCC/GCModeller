Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

''' <summary>
''' R之中的单步函数调用
''' </summary>
Public Class IRToken : Inherits IRProvider
    Implements IScriptProvider

    ''' <summary>
    ''' ... the additional parameters
    ''' </summary>
    ''' <returns></returns>
    Public Property additional As [Property](Of RExpression)

    ''' <summary>
    '''
    ''' </summary>
    ''' <returns>由于这个对象只是对一个表达式的抽象，最常用的是对一个函数调用的抽象，所以library在这里不可以自动添加，需要自己在后面手工添加</returns>
    Public Overrides Function RScript() As String
        Return Me.GetScript(Me.GetType)
    End Function

    Public Overloads Shared Narrowing Operator CType(token As IRToken) As String
        Return token.RScript
    End Operator

    Public Shared Operator &(token As IRToken, script As String) As String
        Return token.RScript & script
    End Operator

    Public Shared Operator &(script As String, token As IRToken) As String
        Return script & token.RScript
    End Operator

    ''' <summary>
    ''' A part of previous token
    ''' </summary>
    ''' <param name="token"></param>
    ''' <param name="t"></param>
    ''' <returns></returns>
    Public Shared Operator Like(token As String, t As IRToken) As RExpression
        Return $""
    End Operator

    ''' <summary>
    ''' AppendLine
    ''' </summary>
    ''' <param name="sb"></param>
    ''' <param name="token"></param>
    ''' <returns></returns>
    Public Shared Operator +(sb As StringBuilder, token As IRToken) As StringBuilder
        Call sb.AppendLine(token.RScript)
        Return sb
    End Operator

    Public Shared Operator <=(sb As StringBuilder, token As IRToken) As StringBuilder
        Call sb.Append(" <- ")
        Call sb.Append(token.RScript)
        Return sb
    End Operator

    Public Shared Operator >=(sb As StringBuilder, token As IRToken) As StringBuilder
        Throw New InvalidOperationException("NOT_SUPPORT_THIS_OPERATOR")
    End Operator

    ''' <summary>
    ''' variable value assignment
    ''' </summary>
    ''' <param name="s"></param>
    ''' <param name="token"></param>
    ''' <returns></returns>
    Public Shared Operator <=(s As String, token As IRToken) As RExpression
        Return New RExpression(s & $" <- {token.RScript}")
    End Operator

    Public Shared Operator >=(sb As String, token As IRToken) As RExpression
        Throw New InvalidOperationException("NOT_SUPPORT_THIS_OPERATOR")
    End Operator

    ''' <summary>
    ''' variable value assignment
    ''' </summary>
    ''' <param name="s"></param>
    ''' <param name="token"></param>
    ''' <returns></returns>
    Public Shared Operator <=(token As IRToken, s As String) As RExpression
        Return New RExpression(token.RScript & $" <- {s}")
    End Operator

    Public Shared Operator >=(token As IRToken, sb As String) As RExpression
        Throw New InvalidOperationException("NOT_SUPPORT_THIS_OPERATOR")
    End Operator

    ''' <summary>
    '''
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="test">输入</param>
    ''' <param name="[default]">默认值</param>
    ''' <param name="[set]">源属性</param>
    ''' <returns></returns>
    Private Shared Function __assertion(Of T)(test As T, [default] As T, [set] As T) As T
        If test.Equals([default]) Then  '  为default值，说明没有输入，则返回当前对象的值，即set参数
            Return [set]
        Else
            Return test   ' 和default的值不同，说明有输入，则返回test
        End If
    End Function

    Protected Function __assertion(test As String, [default] As String, [set] As String) As String
        Return __assertion(Of String)(test, [default], [set])
    End Function

    Protected Shared Function __assertion(test As Double, [default] As Double, [set] As Double) As Double
        Return __assertion(Of Double)(test, [default], [set])
    End Function

    Protected Shared Function __assertion(test As Boolean, [default] As Boolean, [set] As Boolean) As Boolean
        Return __assertion(Of Boolean)(test, [default], [set])
    End Function

    Protected Shared Function __assertion(test As Integer, [default] As Integer, [set] As Integer) As Integer
        Return __assertion(Of Integer)(test, [default], [set])
    End Function

    Protected Shared Function __assertion(test As Long, [default] As Long, [set] As Long) As Long
        Return __assertion(Of Long)(test, [default], [set])
    End Function
End Class
