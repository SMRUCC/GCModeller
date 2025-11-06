#Region "Microsoft.VisualBasic::720723439615de530b752cb92bdd4b07, data\GO_gene-ontology\GO_Annotation\Annotations.vb"

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

    '   Total Lines: 59
    '    Code Lines: 49 (83.05%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (16.95%)
    '     File Size: 2.27 KB


    ' Module Annotations
    ' 
    '     Function: createGoTerms, (+2 Overloads) PfamAssign
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data.GeneOntology.Annotation.xref2go
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports SMRUCC.genomics.Data.Xfam.Pfam.Pipeline.LocalBlast

Public Module Annotations

    <Extension>
    Public Function PfamAssign(pfamhits As IEnumerable(Of PfamHit), pfam2GO As Dictionary(Of String, toGO())) As AnnotationClusters
        Dim mapstoGO As New Dictionary(Of String, (desc$, go As List(Of toGO)))
        Dim go As toGO()

        For Each hit As PfamHit In pfamhits
            If Not mapstoGO.ContainsKey(hit.QueryName) Then
                mapstoGO(hit.QueryName) = (hit.description, New List(Of toGO))

                Call $"{hit.QueryName}: {hit.description}".debug
            End If

            go = pfam2GO.TryGetValue(hit.pfamID)

            If Not go Is Nothing Then
                Call mapstoGO(hit.QueryName).go.AddRange(go)
            End If
        Next

        Dim annotations As ProteinAnnotation() = mapstoGO _
            .Select(Function(prot)
                        Return New ProteinAnnotation With {
                            .proteinID = prot.Key,
                            .description = prot.Value.desc,
                            .GO_terms = prot.Value.go.createGoTerms
                        }
                    End Function) _
            .ToArray

        Return New AnnotationClusters With {
            .proteins = annotations
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function createGoTerms(toGo As IEnumerable(Of toGO)) As NamedValue()
        Return toGo.GroupBy(Function(t) t.map2GO_id) _
            .Select(Function(g) g.First) _
            .Select(Function(g)
                        Return New NamedValue(g.map2GO_id, g.map2GO_term)
                    End Function) _
            .ToArray
    End Function

    <Extension>
    Public Function PfamAssign(annotations As IEnumerable(Of PfamString), pfam2GO As Dictionary(Of String, toGO())) As AnnotationClusters
        Throw New NotImplementedException
    End Function
End Module
