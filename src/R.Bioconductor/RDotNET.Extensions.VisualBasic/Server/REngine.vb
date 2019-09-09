#Region "Microsoft.VisualBasic::00cdb4702aa904861963717968023b3f, RDotNET.Extensions.VisualBasic\Server\REngine.vb"

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

    ' Module RExtensionInvoke
    ' 
    '     Function: [function], (+2 Overloads) __call, AsBoolean, copy, StringNULL
    '               (+2 Overloads) WriteLine
    ' 
    '     Sub: q
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports RDotNET
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.SymbolicExpressionExtension

''' <summary>
''' Wrapper for R engine script invoke.
''' </summary>
Public Module RExtensionInvoke

    ''' <summary>
    ''' Declaring a function object in the R language
    ''' </summary>
    ''' <param name="args"></param>
    ''' <param name="def"></param>
    ''' <returns></returns>
    Public Function [function](args As IEnumerable(Of String), def As String) As String
        Dim tmp As String = RDotNetGC.Allocate

        SyncLock R
            With R
                .call = $"{tmp} <- function({String.Join(", ", args.ToArray)}) {"{
" & def & "
}"}"
            End With
        End SyncLock

        Return tmp
    End Function

    <Extension>
    Public Function AsBoolean(sym As SymbolicExpression) As Boolean
        Return sym.AsLogical.First
    End Function

    ''' <summary>
    ''' Is string <paramref name="s"/> R NA null?.(字符串是否为空字符串或者等于R之中的NA值？)
    ''' </summary>
    ''' <param name="s$"></param>
    ''' <returns></returns>
    <Extension> Public Function StringNULL(s$) As Boolean
        Return s.StringEmpty OrElse s = "NA"
    End Function

    ''' <summary>
    ''' R variable copy
    ''' </summary>
    ''' <param name="var$"></param>
    ''' <returns></returns>
    <Extension> Public Function copy(var$) As String
        Dim x$ = RDotNetGC.Allocate

        SyncLock R
            With R
                .call = $"{x} <- {var};"
            End With
        End SyncLock

        Return x
    End Function

    '''' <summary>
    '''' This function equals to the function &lt;library> in R system.
    '''' </summary>
    '''' <param name="packageName"></param>
    '''' <returns></returns>
    '<Extension> Public Function Library(REngine As REngine, packageName As String) As Boolean
    '    Dim Command As String = $"library(""{packageName}"");"
    '    Try
    '        Dim Result As String() = REngine.Evaluate(Command).AsCharacter().ToArray()
    '        Return True
    '    Catch ex As Exception
    '        Call App.LogException(ex)
    '        Return False
    '    End Try
    'End Function

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

    ''' <summary>
    ''' Evaluates a R statement in the given string.
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <returns></returns>
    <Extension>
    Public Function __call(expr As IScriptProvider) As SymbolicExpression
        SyncLock R
            With R
#If DEBUG Then
                Call .__logs.WriteLine(expr.RScript)
                Call .__logs.Flush()
#End If
                Return .Evaluate(expr.RScript)
            End With
        End SyncLock
    End Function

    ''' <summary>
    ''' Evaluates a R statement in the given string.
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <returns></returns>
    <Extension>
    Public Function __call(expr As String) As SymbolicExpression
        SyncLock R
            With R
#If DEBUG Then
                Call .__logs.WriteLine(expr)
                Call .__logs.Flush()
#End If
                Return .Evaluate(expr)
            End With
        End SyncLock
    End Function

    ''' <summary>
    ''' Quite the R system.
    ''' </summary>
    <Extension> Public Sub q(REngine As REngine)
        Dim result = REngine.Evaluate("q()")
    End Sub
End Module
