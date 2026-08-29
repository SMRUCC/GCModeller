#Region "Microsoft.VisualBasic::f90cec9e8e004a9a70ca1df9736eec3f, data\STRING\Tsv.vb"

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

'   Total Lines: 256
'    Code Lines: 192 (75.00%)
' Comment Lines: 34 (13.28%)
'    - Xml Docs: 94.12%
' 
'   Blank Lines: 30 (11.72%)
'     File Size: 13.09 KB


'     Class LinkAction
' 
'         Properties: a_is_acting, action, item_id_a, item_id_b, mode
'                     score
' 
'         Function: LoadText
' 
'     Class linksDetail
' 
'         Properties: coexpression, coexpression_transferred, combined_score, cooccurence, database
'                     database_transferred, experimental, experiments, experiments_transferred, fusion
'                     homology, neighborhood, neighborhood_transferred, protein1, protein2
'                     textmining, textmining_transferred
' 
'         Function: IteratesLinks, LoadFile, Selects, ToString
' 
'     Class entrez_gene_id_vs_string
' 
'         Properties: Entrez_Gene_ID, STRING_Locus_ID
' 
'         Function: BuildMaps, BuildMapsFromFile, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection

Namespace Tabular.Tsv

    ''' <summary>
    ''' separate identifier mapping files, for several frequently used name_spaces...
    ''' </summary>
    Public Class entrez_gene_id_vs_string

        <Column("#Entrez_Gene_ID")> Public Property Entrez_Gene_ID As String
        Public Property STRING_Locus_ID As String

        Public Overrides Function ToString() As String
            Return $"{Entrez_Gene_ID} <--> {STRING_Locus_ID}"
        End Function

        Public Shared Function BuildMapsFromFile(path As String, Optional tsv As Boolean = True) As Dictionary(Of String, String)
            If tsv Then
                Return BuildMaps(path.Imports(Of entrez_gene_id_vs_string)(vbTab))
            Else
                Return BuildMaps(path.LoadCsv(Of entrez_gene_id_vs_string))
            End If
        End Function

        Public Shared Function BuildMaps(source As IEnumerable(Of entrez_gene_id_vs_string)) As Dictionary(Of String, String)
            Return source.ToDictionary(Function(x) x.Entrez_Gene_ID, Function(x) x.STRING_Locus_ID)
        End Function
    End Class

    Public Class protein_aliases

        <Column("#string_protein_id")>
        Public Property string_protein_id As String
        Public Property [alias] As String
        Public Property source As String

    End Class

    Public Class protein_info

        <Column("#string_protein_id")>
        Public Property string_protein_id As String
        Public Property preferred_name As String
        Public Property protein_size As String
        Public Property annotation As String

    End Class
End Namespace
