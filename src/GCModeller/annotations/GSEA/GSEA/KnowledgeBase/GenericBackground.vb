#Region "Microsoft.VisualBasic::5ef23fddc221f41c58076e6c0f8a0e1e, annotations\GSEA\GSEA\KnowledgeBase\GenericBackground.vb"

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

    '   Total Lines: 56
    '    Code Lines: 46 (82.14%)
    ' Comment Lines: 6 (10.71%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 4 (7.14%)
    '     File Size: 1.97 KB


    ' Module GenericBackground
    ' 
    '     Function: CreateGOGeneric, CreateKOGeneric, createTermGenericGene
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML

Public Module GenericBackground

    ''' <summary>
    ''' Create the KO generic background
    ''' </summary>
    ''' <param name="KO_terms"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function CreateKOGeneric(KO_terms As String(), kegg As IEnumerable(Of Map), nsize As Integer) As Background
        Dim clusterInfo As New KOMapCluster(kegg)

        Return GSEA.CreateBackground(
            db:=KO_terms,
            createGene:=AddressOf createTermGenericGene,
            getTerms:=Function(term) {term},
            define:=AddressOf clusterInfo.KOIDMap,
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
            .term_id = BackgroundGene.UnknownTerms(terms).ToArray
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
End Module
