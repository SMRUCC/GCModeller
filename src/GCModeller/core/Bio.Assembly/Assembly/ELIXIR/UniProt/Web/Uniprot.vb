#Region "Microsoft.VisualBasic::3a925d0938bcd6137137627a648d2f70, core\Bio.Assembly\Assembly\ELIXIR\UniProt\Web\Uniprot.vb"

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

    '   Total Lines: 54
    '    Code Lines: 23 (42.59%)
    ' Comment Lines: 24 (44.44%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 7 (12.96%)
    '     File Size: 2.14 KB


    '     Module WebServices
    ' 
    '         Function: CreateQuery, DownloadProtein, GetEntries
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.Uniprot.Web

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
        Public Function CreateQuery(q As String) As RestQueryResult()
            Dim url As String = UNIPROT_QUERY & q.UrlEncode(jswhitespace:=True)
            Dim json_str As String = url.GET
            Dim results As RestQueryResult() = json_str.LoadJSON(Of RestQueryResultSet) _
                .AsEnumerable _
                .ToArray

            Return results
        End Function

        Const UNIPROT_FASTA_DOWNLOAD_URL As String = "http://www.uniprot.org/uniprot/{0}.fasta"

        ''' <summary>
        ''' Download a protein sequence fasta data from http://www.uniprot.org/ 
        ''' using a specific <paramref name="UniprotId"></paramref>. 
        ''' （从http://www.uniprot.org/网站之上下载一条蛋白质序列）
        ''' </summary>
        ''' <param name="UniprotId">The uniprot id of a protein sequence.(蛋白质在Uniprot数据库之中的编号)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function DownloadProtein(UniprotId As String) As FASTA.FastaSeq
            Dim url As String = String.Format(UNIPROT_FASTA_DOWNLOAD_URL, UniprotId)
            Dim html As String = url.GET
            Return FASTA.FastaSeq.TryParse(html)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="url">CreateQuery(geneId, taxonomy)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("ListEntries")>
        Public Function GetEntries(url As String) As Entry
            Dim pageContent As String = url.GET
            Return Nothing
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
