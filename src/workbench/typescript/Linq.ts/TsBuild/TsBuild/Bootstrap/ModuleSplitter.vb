#Region "Microsoft.VisualBasic::88e0e683d0178054989581418f81cc24, typescript\Linq.ts\TsBuild\TsBuild\Bootstrap\ModuleSplitter.vb"

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

    '     Module ModuleSplitter
    ' 
    '         Function: createModuleInternal, getAppName, getModuleReference, hasAppNameProperty, hasClassAnnotation
    '                   hasInitFunction, hasReferBootstrap, isClassAnnotation, isModuleDefinition, PopulateModules
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Namespace Bootstrap

    Module ModuleSplitter

        ''' <summary>
        ''' 将bootstrap应用模块解析出来
        ''' </summary>
        ''' <param name="tokens"></param>
        ''' <param name="sourceJs$"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function PopulateModules(tokens As IEnumerable(Of Token), sourceJs$) As IEnumerable(Of NamedValue(Of String))
            ' app模块在编译出来的js文件中是从最顶层的declare起始的
            Dim modTokens As New List(Of Token)
            Dim stack As New Stack(Of Integer)
            Dim jsModule As NamedValue(Of String)

            sourceJs = sourceJs.LineTokens.JoinBy(ASCII.LF)

            For Each t As Token In tokens
                If t = TypeScriptTokens.declare AndAlso stack.Count = 0 AndAlso modTokens > 0 Then
                    ' 这个可能是最顶层的模块申明
                    If modTokens.isModuleDefinition Then
                        jsModule = modTokens.createModuleInternal(sourceJs)

                        If Not jsModule.IsEmpty Then
                            Yield jsModule
                        End If
                    End If

                    modTokens *= 0
                ElseIf t = TypeScriptTokens.openStack Then
                    stack.Push(t.start)
                ElseIf t = TypeScriptTokens.closeStack Then
                    stack.Pop()
                End If

                modTokens += t
            Next

            If modTokens > 0 AndAlso modTokens.isModuleDefinition Then
                Yield modTokens.createModuleInternal(sourceJs)
            End If
        End Function

        <Extension>
        Private Function createModuleInternal(modTokens As List(Of Token), sourceJs$) As NamedValue(Of String)
            Dim start = modTokens.First.start
            Dim len = modTokens.Last.ends - modTokens.First.start
            Dim jsBlock = sourceJs.Substring(start, len)
            Dim appName = modTokens.getAppName
            Dim ref = modTokens.getModuleReference

            ' 不是最终的app模块
            ' 忽略掉
            If appName.StringEmpty Then
                Return Nothing
            End If

            Return New NamedValue(Of String) With {
                .Description = appName,
                .Name = ref,
                .Value = jsBlock
            }
        End Function

        <Extension>
        Private Function getAppName(modTokens As List(Of Token)) As String
            Dim t As Token

            ' 找到appName之后的往下第一条string
            For i As Integer = 0 To modTokens.Count - 1
                t = modTokens(i)

                If t = TypeScriptTokens.string AndAlso t = """appName""" Then
                    For j As Integer = i + 1 To modTokens.Count - 1
                        t = modTokens(j)

                        If t = TypeScriptTokens.string Then
                            Return t.text.GetStackValue("""", """")
                        End If
                    Next
                End If
            Next

            Return Nothing
        End Function

        <Extension>
        Private Function getModuleReference(modTokens As List(Of Token)) As String
            Dim ref As New List(Of String)
            Dim i As New Pointer(Of Token)(modTokens)
            Dim t As Token

            ' 在遇到class标记之前
            ' 将所有var后面的identifier都拿出来
            Do While Not i.Current.isClassAnnotation
                t = ++i

                If t = TypeScriptTokens.declare Then
                    ref += i.Current.text
                End If
            Loop

            Return ref.JoinBy(".")
        End Function

        <Extension>
        Private Function isModuleDefinition(modTokens As List(Of Token)) As Boolean
            ' 是否具有class注释标记
            If Not modTokens.hasClassAnnotation Then
                Return False
            End If

            ' 因为模块之间可能存在继承关系
            ' 所以在这里只需要至少满足下面的条件即可
            If modTokens.hasAppNameProperty OrElse modTokens.hasInitFunction OrElse modTokens.hasReferBootstrap Then
                Return True
            End If

            Return False
        End Function

        <Extension>
        Private Function hasReferBootstrap(modTokens As List(Of Token)) As Boolean
            For Each t As Token In modTokens
                If t = TypeScriptTokens.identifier AndAlso t = "Bootstrap" Then
                    Return True
                End If
            Next

            Return False
        End Function

        <Extension>
        Private Function hasInitFunction(modTokens As List(Of Token)) As Boolean
            For Each t As Token In modTokens
                If t = TypeScriptTokens.identifier AndAlso t.text.EndsWith(".prototype.init") Then
                    Return True
                End If
            Next

            Return False
        End Function

        <Extension>
        Private Function hasAppNameProperty(modTokens As List(Of Token)) As Boolean
            For Each t As Token In modTokens
                If t = TypeScriptTokens.string AndAlso t = """appName""" Then
                    Return True
                End If
            Next

            Return False
        End Function

        <Extension>
        Private Function isClassAnnotation(t As Token) As Boolean
            Return t = TypeScriptTokens.comment AndAlso t = "/** @class */"
        End Function

        <Extension>
        Private Function hasClassAnnotation(modTokens As List(Of Token)) As Boolean
            For Each t As Token In modTokens
                If t.isClassAnnotation Then
                    Return True
                End If
            Next

            Return False
        End Function
    End Module
End Namespace
