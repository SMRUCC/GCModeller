#Region "Microsoft.VisualBasic::d37f0b7ba6db36f53f42ce7b28148102, GCModeller\models\ncbi-BioSystems\Utils.vb"

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

    '   Total Lines: 55
    '    Code Lines: 47
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 1.93 KB


    ' Module Utils
    ' 
    '     Function: createMetadata, FromGenBank, populateProteins
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

Public Module Utils

    <Extension>
    Public Function FromGenBank(genbank As GBFF.File) As Project
        Dim meta = genbank.createMetadata
        Dim proteins As New Dictionary(Of String, ProteinAnnotation)

        For Each prot As ProteinAnnotation In genbank.populateProteins
            If prot.sequence.StringEmpty Then
                Continue For
            End If

            Call proteins.Add(prot.geneId, prot)
        Next

        Return New Project With {
            .metadata = meta,
            .proteins = New ProteinSet With {
                .proteins = proteins
            }
        }
    End Function

    <Extension>
    Private Iterator Function populateProteins(genbank As GBFF.File) As IEnumerable(Of ProteinAnnotation)
        For Each feature As Feature In From f As Feature
                                       In genbank.Features
                                       Where f.KeyName.TextEquals("CDS")

            Yield New ProteinAnnotation With {
                .locus_id = feature("locus_tag"),
                .geneId = feature("protein_id"),
                .geneName = feature("gene"),
                .description = feature("product"),
                .sequence = feature("translation")
            }
        Next
    End Function

    <Extension>
    Private Function createMetadata(genbank As GBFF.File) As Sys_set
        Return New Sys_set With {
            .sysid = New sysid With {
                .bsid = genbank.Locus.AccessionID,
                .version = genbank.Locus.AccessionID & "." & genbank.Locus.UpdateTime
            },
            .names = {genbank.Definition.Value}
        }
    End Function
End Module

