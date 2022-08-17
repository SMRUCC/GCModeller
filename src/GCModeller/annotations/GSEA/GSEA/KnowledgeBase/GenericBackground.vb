#Region "Microsoft.VisualBasic::0b6e73a0a827aa2e4a847aaa5b6aae22, annotations\GSEA\GSEA\KnowledgeBase\GenericBackground.vb"

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

' Module GenericBackground
' 
'     Function: CreateGOGeneric, CreateKOGeneric, createTermGenericGene
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module GenericBackground

    ''' <summary>
    ''' Create the KO generic background
    ''' </summary>
    ''' <param name="KO_terms"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function CreateKOGeneric(KO_terms As String(), kegg As IEnumerable(Of Map), nsize As Integer) As Background
        Return GSEA.CreateBackground(
            db:=KO_terms,
            createGene:=AddressOf createTermGenericGene,
            getTerms:=Function(term) {term},
            define:=GSEA.KEGGClusters(kegg),
            genomeName:="generic",
            taxonomy:="generic",
            outputAll:=False
        ).With(Sub(background)
                   background.size = nsize
               End Sub)
    End Function

    Private Function createTermGenericGene(term As String, terms As String()) As BackgroundGene
        Return New BackgroundGene With {
            .accessionID = term,
            .[alias] = terms,
            .locus_tag = New NamedValue With {
                .name = term,
                .text = term
            },
            .name = term,
            .term_id = terms
        }
    End Function

    <Extension>
    Public Function CreateGOGeneric(GO_terms As IEnumerable(Of String), go As GetClusterTerms, nsize As Integer) As Background
        Return GSEA.CreateBackground(
            db:=GO_terms,
            createGene:=AddressOf createTermGenericGene,
            getTerms:=Function(term) {term},
            define:=go,
            genomeName:="generic",
            taxonomy:="generic",
            outputAll:=False
        ).With(Sub(background)
                   background.size = nsize
               End Sub)
    End Function

    <Extension>
    Public Function ImportsTree(tree As Tree(Of String)) As Background
        Dim allTerms = tree.PopulateAllNodes.ToArray
        Dim terms As Cluster() = allTerms _
            .Select(Function(t)
                        Return New Cluster With {
                            .ID = t.label,
                            .description = t.label,
                            .names = t.label,
                            .members = t.enumerateAllTerms.ToArray
                        }
                    End Function) _
            .ToArray

        Return New Background With {
            .build = Now,
            .clusters = terms
        }
    End Function

    <Extension>
    Private Iterator Function enumerateAllTerms(node As Tree(Of String)) As IEnumerable(Of BackgroundGene)
        For Each t In node.PopulateAllNodes
            Yield New BackgroundGene With {
                .accessionID = t.label,
                .[alias] = {t.label},
                .locus_tag = New NamedValue With {
                    .name = t.label,
                    .text = t.label
                },
                .name = t.label,
                .term_id = {t.label}
            }
        Next
    End Function
End Module
