#Region "Microsoft.VisualBasic::8ae14680417b0006ace3a6166b28d82b, GCModeller\annotations\GSEA\FisherCore\OntologyBackground.vb"

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

'   Total Lines: 66
'    Code Lines: 50
' Comment Lines: 10
'   Blank Lines: 6
'     File Size: 2.40 KB


' Module OntologyBackground
' 
'     Function: enumerateAllTerms, (+2 Overloads) ImportsTree, simpleTerm
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Module OntologyBackground

    ''' <summary>
    ''' Create a enrichment background based on a ontology tree model
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="tree">
    ''' the ontology tree
    ''' </param>
    ''' <param name="createTerm"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ImportsTree(Of T)(tree As Tree(Of T), createTerm As Func(Of T, BackgroundGene)) As Background
        ' skip of the first root node
        Dim allTerms = tree.PopulateAllNodes.Skip(1).ToArray
        Dim terms As Cluster() = allTerms _
            .Select(Function(term)
                        Return New Cluster With {
                            .ID = term.label,
                            .description = term.label,
                            .names = term.label,
                            .members = term _
                                .enumerateAllTerms(createTerm) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        Return New Background With {
            .build = Now,
            .clusters = terms
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ImportsTree(tree As Tree(Of String)) As Background
        Return tree.ImportsTree(simpleTerm)
    End Function

    Private Function simpleTerm() As Func(Of String, BackgroundGene)
        Return Function(label)
                   Return New BackgroundGene With {
                       .accessionID = label,
                       .[alias] = {label},
                       .locus_tag = New NamedValue With {
                           .name = label,
                           .text = label
                       },
                       .name = label,
                       .term_id = BackgroundGene.UnknownTerms(label).ToArray
                   }
               End Function
    End Function

    <Extension>
    Private Iterator Function enumerateAllTerms(Of T)(node As Tree(Of T), gene As Func(Of T, BackgroundGene)) As IEnumerable(Of BackgroundGene)
        For Each t In node.PopulateAllNodes
            Yield gene(t.Data)
        Next
    End Function
End Module
