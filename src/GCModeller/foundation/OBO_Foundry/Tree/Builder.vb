#Region "Microsoft.VisualBasic::a622fcc4ba58ef9a225fa4733bbb9ba0, GCModeller\foundation\OBO_Foundry\Tree\Builder.vb"

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

    '   Total Lines: 119
    '    Code Lines: 91
    ' Comment Lines: 16
    '   Blank Lines: 12
    '     File Size: 4.78 KB


    '     Module Builder
    ' 
    '         Function: BuildTree, GetTermsByLevel, GetTermsLineage, TermLineages, vertexTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models

Namespace Tree

    Public Module Builder

        ''' <summary>
        ''' 字典的主键为<see cref="GenericTree.ID"/>编号
        ''' </summary>
        ''' <param name="terms"></param>
        ''' <returns></returns>
        <Extension>
        Public Function BuildTree(terms As IEnumerable(Of RawTerm)) As Dictionary(Of String, GenericTree)
            Dim vertex As Dictionary(Of String, GenericTree) = terms.vertexTable

            For Each v As GenericTree In vertex.Values
                If Not v.data.ContainsKey("is_a") Then
                    v.is_a = {}
                Else
                    Dim is_a = v.data!is_a _
                        .Select(Function(value)
                                    Return value.StringSplit("\s*!\s*").First.Trim
                                End Function) _
                        .ToArray

                    v.is_a = is_a _
                        .Select(Function(id) vertex(id)) _
                        .ToArray
                End If
            Next

            Return vertex
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function vertexTable(terms As IEnumerable(Of RawTerm)) As Dictionary(Of String, GenericTree)
            Return terms _
                .Where(Function(t) t.type = "[Term]") _
                .Select(Function(t)
                            Dim data = t.GetData
                            Dim id = (data!id).First
                            Return (id:=id, term:=t, data:=data)
                        End Function) _
                .ToDictionary(Function(t) t.id,
                              Function(k)
                                  Dim name = k.data!name.First

                                  Return New GenericTree With {
                                      .ID = k.id,
                                      .data = k.data,
                                      .name = name
                                  }
                              End Function)
        End Function

        ''' <summary>
        ''' 根据``is_a``关系来获取分类关系
        ''' </summary>
        ''' <param name="node"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function TermLineages(node As GenericTree) As IEnumerable(Of NamedCollection(Of GenericTree))
            If node.is_a.IsNullOrEmpty Then
                ' 这个是根节点, 因为``is_a``父节点是空的
                Yield New NamedCollection(Of GenericTree) With {
                    .name = node.name,
                    .value = {node},
                    .description = node.ID
                }
            Else
                For Each parent As GenericTree In node.is_a
                    For Each chain As List(Of GenericTree) In GetTermsLineage(parent, {node, parent})
                        Yield New NamedCollection(Of GenericTree) With {
                            .name = parent.name,
                            .value = chain _
                                .With(Sub(c) Call c.Reverse()) _
                                .ToArray,
                            .description = parent.ID
                        }
                    Next
                Next
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="node">Tree was created by <see cref="Builder.BuildTree(IEnumerable(Of RawTerm))"/> function.</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetTermsByLevel(node As GenericTree, level%) As GenericTree()
            Return node.TermLineages _
                .Select(Function(chain) chain.ElementAtOrDefault(level)) _
                .Where(Function(lineNode) Not lineNode Is Nothing) _
                .Distinct _
                .ToArray
        End Function

        <Extension>
        Private Iterator Function GetTermsLineage(node As GenericTree, parentChain As IEnumerable(Of GenericTree)) As IEnumerable(Of List(Of GenericTree))
            Dim chainList = parentChain.AsList

            If node.is_a.IsNullOrEmpty Then
                Yield chainList
            Else
                For Each parent As GenericTree In node.is_a
                    For Each chain In GetTermsLineage(parent, New List(Of GenericTree)(chainList).Join(parent))
                        Yield chain
                    Next
                Next
            End If
        End Function
    End Module
End Namespace
