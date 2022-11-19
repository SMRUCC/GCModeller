#Region "Microsoft.VisualBasic::51c0b4f729304a72bc0f14fee422a677, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\BriteHText\BriteHTextParser.vb"

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

    '   Total Lines: 93
    '    Code Lines: 62
    ' Comment Lines: 13
    '   Blank Lines: 18
    '     File Size: 2.91 KB


    '     Module BriteHTextParser
    ' 
    '         Function: isValid, (+2 Overloads) Load, loadData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Module BriteHTextParser

        Friend ReadOnly classLevels As Index(Of Char) = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToArray

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="text$">文本内容或者文件的路径</param>
        ''' <returns></returns>
        Public Function Load(text As String) As BriteHText
            Dim raw = text.Replace("<b>", "") _
                          .Replace("</b>", "") _
                          .LineTokens
            Dim lines As String() = LinqAPI.Exec(Of String) <=
 _
                From s As String
                In raw
                Where s.isValid
                Select s

            Return Load(lines, text(1))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function isValid(str As String) As Boolean
            Return Not String.IsNullOrEmpty(str) AndAlso (str.First Like classLevels AndAlso Len(str) > 1)
        End Function

        Public Function Load(lines$(), Optional depth$ = "Z"c) As BriteHText
            Dim classes As New List(Of BriteHText)
            Dim p As Integer = 0
            Dim root As New BriteHText With {
                .classLabel = "/",
                .level = -1,
                .degree = depth
            }

            Do While p < lines.Length - 1
                classes += lines.loadData(p, level:=0, parent:=root)
            Loop

            root.categoryItems = classes

            Return root
        End Function

        ''' <summary>
        ''' 递归加载层次数据
        ''' </summary>
        ''' <param name="lines"></param>
        ''' <param name="p"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Private Function loadData(lines$(), ByRef p%, level%, parent As BriteHText) As BriteHText
            Dim category As New BriteHText With {
                .level = level,
                .classLabel = Mid(lines(p), 2).Trim,
                .parent = parent
            }

            p += 1

            If p > lines.Length - 1 Then
                Return category
            End If

            If lines(p).First > category.CategoryLevel Then
                Dim subCategory As New List(Of BriteHText)

                Do While lines(p).First > category.CategoryLevel
                    subCategory += loadData(lines, p, level + 1, parent:=category)

                    If p > lines.Length - 1 Then
                        Exit Do
                    End If
                Loop

                category.categoryItems = subCategory
            End If

            Return category
        End Function
    End Module
End Namespace
