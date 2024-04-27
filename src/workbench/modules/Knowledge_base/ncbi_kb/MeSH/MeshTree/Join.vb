#Region "Microsoft.VisualBasic::acdb36ed6124b0bfe7af8a4dfc661b45, G:/GCModeller/src/workbench/modules/Knowledge_base/ncbi_kb//MeSH/MeshTree/Join.vb"

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

    '   Total Lines: 63
    '    Code Lines: 56
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 2.48 KB


    '     Module Join
    ' 
    '         Function: JoinData
    ' 
    '         Sub: JoinData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace MeSH.Tree

    Public Module Join

        <Extension>
        Public Iterator Function JoinData(tree As IEnumerable(Of Term), terms As Dictionary(Of String, DescriptorRecord)) As IEnumerable(Of Term)
            Dim unionTree As Term() = tree _
                .GroupBy(Function(t) t.term) _
                .Select(Function(t)
                            If t.Count = 1 Then
                                Return New Term With {
                                    .term = t.Key,
                                    .tree = {t.First.tree.JoinBy(".")}
                                }
                            Else
                                Return New Term With {
                                    .term = t.Key,
                                    .tree = t _
                                        .Select(Function(ti)
                                                    Return ti.tree.JoinBy(".")
                                                End Function) _
                                        .ToArray
                                }
                            End If
                        End Function) _
                .OrderByDescending(Function(t) t.tree.Length) _
                .ToArray

            For Each term As Term In unionTree
                term.JoinData(terms)
                Yield term
            Next
        End Function

        <Extension>
        Private Sub JoinData(ByRef node As Term, terms As Dictionary(Of String, DescriptorRecord))
            Dim data As DescriptorRecord = terms.TryGetValue(node.term)

            If data Is Nothing Then
                Call $"missing metadata for mesh term: {node.term}".Warning
                Return
            End If

            node.accessionID = data.DescriptorUI
            node.description = data.ConceptList _
                .SafeQuery _
                .Select(Function(c) c.ScopeNote) _
                .JoinBy(vbCrLf) _
                .Trim(" "c, ASCII.CR, ASCII.LF, ASCII.TAB)
            node.alias = data.ConceptList _
                .SafeQuery _
                .Select(Function(c) c.TermList) _
                .IteratesALL _
                .Select(Function(t) t.String) _
                .Distinct _
                .ToArray
        End Sub
    End Module
End Namespace
