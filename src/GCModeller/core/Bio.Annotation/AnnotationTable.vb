#Region "Microsoft.VisualBasic::1051645c61916e8741e6d24ba8a9eb64, GCModeller\core\Bio.Annotation\AnnotationTable.vb"

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

    '   Total Lines: 53
    '    Code Lines: 48
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 1.84 KB


    ' Class AnnotationTable
    ' 
    '     Properties: EC, Entrez, fullName, geneName, GO
    '                 ID, KO, ORF, organism, pfam
    '                 uniprot
    ' 
    '     Function: FromUnifyPtf, NA
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.Annotation.Ptf

Public Class AnnotationTable : Inherits DynamicPropertyBase(Of String)
    Implements INamedValue

    Public Property ID As String Implements IKeyedEntity(Of String).Key
    Public Property geneName As String
    Public Property ORF As String
    Public Property Entrez As String()
    Public Property fullName As String()
    Public Property uniprot As String()
    Public Property GO As String()
    Public Property EC As String()
    Public Property KO As String()
    Public Property pfam As String
    Public Property organism As String

    Public Shared Function FromUnifyPtf(protein As ProteinAnnotation) As AnnotationTable
        Return New AnnotationTable With {
            .ID = protein.geneId,
            .geneName = protein.geneName,
            .EC = protein.get("ec"),
            .Entrez = protein.get("entrez"),
            .fullName = {protein.description},
            .GO = protein.get("go"),
            .KO = protein.get("ko"),
            .ORF = protein.locus_id,
            .pfam = protein("pfamstring"),
            .uniprot = protein.get("synonym"),
            .organism = protein("scientific_name")
        }
    End Function

    Public Shared Function NA(geneId As String) As AnnotationTable
        Return New AnnotationTable With {
            .EC = {},
            .Entrez = {},
            .fullName = {"unknown"},
            .geneName = geneId,
            .GO = {},
            .ID = geneId,
            .KO = {},
            .ORF = geneId,
            .organism = "n/a",
            .pfam = "",
            .uniprot = {}
        }
    End Function

End Class
