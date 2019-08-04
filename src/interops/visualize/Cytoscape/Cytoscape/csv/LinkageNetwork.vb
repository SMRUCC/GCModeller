#Region "Microsoft.VisualBasic::5d0c1bb829ebe748bce7816d1aa115db, visualize\Cytoscape\Cytoscape\csv\LinkageNetwork.vb"

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

    '     Module LinkageNetwork
    ' 
    '         Function: __addPrefix, BuildNetwork
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Tables

    ''' <summary>
    ''' 一般应用这个模块进行知识网络的构建与可视化模型生成
    ''' </summary>
    Public Module LinkageNetwork

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="schema$">The color schema name</param>
        ''' <returns></returns>
        Public Function BuildNetwork(source As File, Optional typePrefix As Boolean = True, Optional schema$ = "material") As NetworkTables
            source = source.Trim

            Dim types$() = source.Headers.ToArray ' 表头作为节点类型
            Dim nodes As New Dictionary(Of FileStream.Node)
            Dim edges As New List(Of NetworkEdge)
            Dim colors As Dictionary(Of String, String) =
                Designer _
                .GetColors(term:=schema, n:=types.Length) _
                .Select(AddressOf ToHtmlColor) _
                .SeqIterator _
                .ToDictionary(Function(name) types.ElementAtOrDefault(name, App.NextTempName),
                              Function(color) +color)
            Dim linkages = source.Columns.SlideWindows(2).ToArray
            Dim parents As NamedValue(Of String)() = New NamedValue(Of String)(source.RowNumbers - 2) {} ' 

            For Each linkage In linkages.SeqIterator
                Dim a$() = (+linkage)(0)  ' 第一列的所有数据
                Dim b$() = (+linkage)(1)  ' 第二列的所有数据
                Dim type1 = a.First   ' 这里获得的是表头
                Dim type2 = b.First

                ' 跳过表头
                a = a.Skip(1).ToArray
                b = b.Skip(1).ToArray

                ' 按行遍历当前的这两列的所有数据
                For i As Integer = 0 To a.Length - 1
                    Dim list_a$() = a(i).Trim.StringSplit(";\s*")
                    Dim list_b$() = b(i).Trim.StringSplit(";\s*")
                    Dim typeA = type1  ' 应该将A的类型单独拿出来，要不然后面遇见空缺的时候会被覆盖掉的，导致后面的类型全部错位

                    If list_a.Length = 0 AndAlso list_b.Length = 0 Then
                        Continue For
                    End If

                    If Not list_a.IsNullOrEmpty Then
                        parents(i) = New NamedValue(Of String) With {
                            .Name = typeA,
                            .Value = a(i)
                        }
                    Else
                        For pIndex = i To 0 Step -1
                            list_a = parents(pIndex).Value.StringSplit(";\s*")
                            If Not list_a.IsNullOrEmpty Then
                                typeA = parents(pIndex).Name
                                Exit For
                            End If
                        Next
                    End If

                    For Each n As String In list_a
                        n = If(typePrefix, n.__addPrefix(typeA), n)

                        If Not nodes.ContainsKey(n) Then
                            nodes(n) = New FileStream.Node With {
                                .ID = n,
                                .NodeType = typeA
                            }
                        End If
                    Next
                    For Each n As String In list_b
                        n = If(typePrefix, n.__addPrefix(type2), n)

                        If Not nodes.ContainsKey(n) Then
                            nodes(n) = New FileStream.Node With {
                                .ID = n,
                                .NodeType = type2
                            }
                        End If
                    Next

                    For Each aID In list_a
                        For Each bID In list_b
                            edges += New NetworkEdge With {
                                .FromNode = If(typePrefix, aID.__addPrefix(typeA), aID),
                                .ToNode = If(typePrefix, bID.__addPrefix(type2), bID),
                                .Interaction = $"{typeA} --- {type2}"
                            }
                        Next
                    Next
                Next
            Next

            For Each node As FileStream.Node In nodes.Values
                node.Properties.Add("color", colors(node.NodeType))
            Next

            Return New NetworkTables With {
                .Edges = edges,
                .Nodes = nodes
            }
        End Function

        <Extension>
        Private Function __addPrefix(node$, prefix$) As String
            Return $"[{prefix}] {node}"
        End Function
    End Module
End Namespace
