Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
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
        Public Function BuildNetwork(source As File, Optional typePrefix As Boolean = True, Optional schema$ = "material") As Network
            Dim types$() = source.Headers.ToArray ' 表头作为节点类型
            Dim nodes As New Dictionary(Of FileStream.Node)
            Dim edges As New List(Of NetworkEdge)
            Dim colors As Dictionary(Of String, String) =
                Designer _
                .GetColors(term:=schema, n:=types.Length) _
                .Select(Function(c) c.RGB2Hexadecimal) _
                .SeqIterator _
                .ToDictionary(Function(name) types.Get(name, Rnd),
                              Function(color) +color)
            Dim linkages = source.Columns.SlideWindows(2).ToArray
            Dim parent$() = {}

            For Each linkage In linkages
                Dim a = linkage(0)
                Dim b = linkage(1)
                Dim type1 = a.First
                Dim type2 = b.First

                ' 跳过表头
                a = a.Skip(1).ToArray
                b = b.Skip(1).ToArray

                For i As Integer = 0 To a.Length - 1
                    Dim list_a$() = a(i).Trim.StringSplit(";\s*")
                    Dim list_b$() = b(i).Trim.StringSplit(";\s*")

                    If Not list_a.IsNullOrEmpty Then
                        parent = list_a
                    Else
                        list_a = parent
                    End If

                    For Each n As String In list_a
                        n = If(typePrefix, n.__addPrefix(type1), n)

                        If Not nodes.ContainsKey(n) Then
                            nodes(n) = New FileStream.Node With {
                                .ID = n,
                                .NodeType = type1
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
                                .FromNode = If(typePrefix, aID.__addPrefix(type1), aID),
                                .ToNode = If(typePrefix, bID.__addPrefix(type2), bID),
                                .InteractionType = $"{type1} --- {type2}"
                            }
                        Next
                    Next
                Next
            Next

            For Each node As FileStream.Node In nodes.Values
                node.Properties.Add("color", colors(node.NodeType))
            Next

            Return New Network With {
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