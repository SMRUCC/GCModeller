#Region "Microsoft.VisualBasic::8812d2023617eeca7213a4103ccebf34, core\Bio.Assembly\Assembly\ELIXIR\UniProt\Web\Uniprot.vb"

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

    '   Total Lines: 132
    '    Code Lines: 85 (64.39%)
    ' Comment Lines: 21 (15.91%)
    '    - Xml Docs: 90.48%
    ' 
    '   Blank Lines: 26 (19.70%)
    '     File Size: 5.00 KB


    '     Module WebServices
    ' 
    '         Function: CreateQuery, DownloadProtein, DownloadProteinData
    ' 
    '     Class RestQueryResultSet
    ' 
    '         Properties: results
    ' 
    '         Function: GenericEnumerator
    ' 
    '     Class RestQueryResult
    ' 
    '         Properties: annotationScore, entryType, organism, primaryAccession, proteinDescription
    '                     proteinExistence, secondaryAccessions, sequence, uniProtkbId
    ' 
    '         Function: GetFasta, ToString
    ' 
    '     Class proteinDescription
    ' 
    '         Properties: alternativeNames, recommendedName
    ' 
    '     Class sequence
    ' 
    '         Properties: length, molWeight, value
    ' 
    '     Class organism
    ' 
    '         Properties: commonName, lineage, scientificName, taxonId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.Uniprot.Web

    ''' <summary>
    ''' https://www.uniprot.org/help/api_queries
    ''' https://www.uniprot.org/help/text-search
    ''' https://www.uniprot.org/help/query-fields
    ''' </summary>
    <Package("Uniprot.WebServices")>
    Public Module WebServices

        Const UNIPROT_QUERY As String = "https://rest.uniprot.org/uniprotkb/search?query="

        ''' <summary>
        ''' Create a protein query url. 
        ''' </summary>
        ''' <param name="q"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function CreateQuery(q As String, Optional tax_id As UInteger? = Nothing) As RestQueryResult()
            Dim url As String = UNIPROT_QUERY & q.UrlEncode(jswhitespace:=True)

            If Not tax_id Is Nothing Then
                url = url & $"%20AND%20(taxonomy_id:{tax_id})"
            End If

            Dim json_str As String = url.GET
            Dim results As RestQueryResult() = json_str.LoadJSON(Of RestQueryResultSet) _
                .AsEnumerable _
                .ToArray

            Return results
        End Function

        Const UNIPROT_FASTA_DOWNLOAD_URL As String = "http://www.uniprot.org/uniprot/{0}.fasta"
        Const UNIPROT_ENTRY_DOWNLOAD_URL As String = "https://rest.uniprot.org/uniprotkb/{0}.xml"

        Public Function DownloadProteinData(UniprotId As String) As XML.entry
            Dim url As String = String.Format(UNIPROT_ENTRY_DOWNLOAD_URL, UniprotId)
            Dim xml As String = url.GET
            Return xml.LoadFromXml(Of XML.entry)
        End Function

        ''' <summary>
        ''' Download a protein sequence fasta data from http://www.uniprot.org/ 
        ''' using a specific <paramref name="UniprotId"></paramref>. 
        ''' （从http://www.uniprot.org/网站之上下载一条蛋白质序列）
        ''' </summary>
        ''' <param name="UniprotId">The uniprot id of a protein sequence.(蛋白质在Uniprot数据库之中的编号)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function DownloadProtein(UniprotId As String) As FastaSeq
            Dim url As String = String.Format(UNIPROT_FASTA_DOWNLOAD_URL, UniprotId)
            Dim html As String = url.GET
            Return FastaSeq.TryParse(html)
        End Function
    End Module

    Public Class RestQueryResultSet : Implements Enumeration(Of RestQueryResult)

        Public Property results As RestQueryResult()

        Public Iterator Function GenericEnumerator() As IEnumerator(Of RestQueryResult) Implements Enumeration(Of RestQueryResult).GenericEnumerator
            If results Is Nothing Then
                Return
            End If

            For Each result As RestQueryResult In results
                If result IsNot Nothing Then
                    Yield result
                End If
            Next
        End Function
    End Class

    Public Class RestQueryResult

        Public Property entryType As String
        Public Property primaryAccession As String
        Public Property secondaryAccessions As String()
        Public Property uniProtkbId As String
        Public Property proteinDescription As proteinDescription
        Public Property annotationScore As Double
        Public Property organism As organism
        Public Property proteinExistence As String
        Public Property sequence As sequence

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetFasta() As FastaSeq
            Return New FastaSeq(New String() {primaryAccession, uniProtkbId, organism.scientificName, ToString()}, sequence.value)
        End Function

        Public Overrides Function ToString() As String
            If proteinDescription IsNot Nothing AndAlso proteinDescription.recommendedName IsNot Nothing Then
                Return proteinDescription.recommendedName.fullName.value
            Else
                Return ""
            End If
        End Function

    End Class

    Public Class proteinDescription
        Public Property recommendedName As recommendedName
        Public Property alternativeNames As recommendedName()
    End Class

    Public Class sequence

        Public Property value As String
        Public Property length As Integer
        Public Property molWeight As Double

    End Class

    Public Class organism

        Public Property scientificName As String
        Public Property commonName As String
        Public Property taxonId As String
        Public Property lineage As String()

    End Class
End Namespace
