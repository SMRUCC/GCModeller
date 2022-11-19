#Region "Microsoft.VisualBasic::adf94c1558cea14f4e9cc7bc6e600b1f, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\UniProt\Web\Uniprot.vb"

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
    '    Code Lines: 23
    ' Comment Lines: 24
    '   Blank Lines: 7
    '     File Size: 2.08 KB


    '     Module WebServices
    ' 
    '         Function: CreateQuery, DownloadProtein, GetEntries
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.Uniprot.Web

    <Package("Uniprot.WebServices")>
    Public Module WebServices

        Const UNIPROT_QUERY As String = "http://www.uniprot.org/uniprot/?query=name%3A{0}+AND+taxonomy%3A{1}&sort=score"

        ''' <summary>
        ''' Create a protein query url. 
        ''' </summary>
        ''' <param name="geneId"></param>
        ''' <param name="taxonomy"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function CreateQuery(<Parameter("Gene.ID")> geneId As String, taxonomy As String) As String
            Return String.Format(UNIPROT_QUERY, geneId, taxonomy)
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
End Namespace
