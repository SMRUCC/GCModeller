Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Runtime.CompilerServices

Imports RDotNET.REngineExtension
Imports RDotNET.SymbolicExpressionExtension
Imports RDotNET

''' <summary>
''' Wrapper for R engine script invoke.
''' </summary>
Public Module RExtensionInvoke

    <Extension>
    Public Function AsBoolean(sym As SymbolicExpression) As Boolean
        Return sym.AsLogical.First
    End Function

    ''' <summary>
    ''' This function equals to the function &lt;library> in R system.
    ''' </summary>
    ''' <param name="packageName"></param>
    ''' <returns></returns>
    <Extension> Public Function Library(REngine As REngine, packageName As String) As Boolean
        Dim Command As String = $"library(""{packageName}"");"
        Try
            Dim Result As String() = REngine.Evaluate(Command).AsCharacter().ToArray()
            Return True
        Catch ex As Exception
            Call App.LogException(ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 获取来自于R服务器的输出，而不将结果打印于终端之上
    ''' </summary>
    ''' <param name="script"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function WriteLine(REngine As REngine, script As String) As String()
        Dim Result As SymbolicExpression = REngine.Evaluate(script)

        If Result Is Nothing Then
            Return {}
        Else
            If Result.Type = Internals.SymbolicExpressionType.Closure OrElse
                Result.Type = Internals.SymbolicExpressionType.Null Then
                Return New String() {}
            End If

            Dim array As String() = Result.AsCharacter().ToArray
            Return array
        End If
    End Function

    ''' <summary>
    ''' 获取来自于R服务器的输出，而不将结果打印于终端之上
    ''' </summary>
    ''' <param name="script"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function WriteLine(REngine As REngine, script As IRProvider) As String()
        Return REngine.WriteLine(script.RScript)
    End Function

#If DEBUG Then
    ReadOnly __logs As StreamWriter = App.GetAppSysTempFile(".log").OpenWriter
#End If

    ''' <summary>
    ''' Evaluates a R statement in the given string.
    ''' </summary>
    ''' <param name="R"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ζ(R As String) As SymbolicExpression
#If DEBUG Then
        Call __logs.WriteLine(R)
        Call __logs.Flush()
#End If
        Return RServer.Evaluate(R)
    End Function

    ''' <summary>
    ''' Quite the R system.
    ''' </summary>
    <Extension> Public Sub q(REngine As REngine)
        Dim result = REngine.Evaluate("q()")
    End Sub
End Module
