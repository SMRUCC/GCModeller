#Region "Microsoft.VisualBasic::cf42a09661a9b4d09ad095dbfd004f41, R#\kb\pubmed.vb"

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

    '   Total Lines: 202
    '    Code Lines: 130 (64.36%)
    ' Comment Lines: 49 (24.26%)
    '    - Xml Docs: 89.80%
    ' 
    '   Blank Lines: 23 (11.39%)
    '     File Size: 8.23 KB


    ' Module pubmed_tools
    ' 
    '     Function: citation_list, createArticleTable, get_article_info, Parse, ParseArticleSetXml
    '               ParsePubmed, QueryKeyword, read_articlejson, read_bits_book
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.IO.Compression
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.BITS
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.PubMed
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' PubMed is a free resource supporting the search and retrieval of biomedical and life sciences 
''' literature with the aim of improving health–both globally and personally.
''' 
''' The PubMed database contains more than 36 million citations And abstracts Of biomedical 
''' literature. It does Not include full text journal articles; however, links To the full text 
''' are often present When available from other sources, such As the publisher's website or 
''' PubMed Central (PMC).
''' 
''' Available to the public online since 1996, PubMed was developed And Is maintained by the
''' National Center for Biotechnology Information (NCBI), at the U.S. National Library of 
''' Medicine (NLM), located at the National Institutes of Health (NIH).
''' </summary>
<Package("pubmed")>
<RTypeExport("article", GetType(PubmedArticle))>
<RTypeExport("cite", GetType(MedlineCitation))>
Module pubmed_tools

    Sub Main()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(PubmedArticle()), AddressOf createArticleTable)
        Call Internal.generic.add("summary", GetType(PubmedArticle), AddressOf get_article_info)
    End Sub

    <RGenericOverloads("summary")>
    Private Function get_article_info(article As PubmedArticle, args As list, env As Environment) As Object
        Dim summary As New list(
            slot("PMID") = article.PMID,
            slot("title") = article.GetTitle,
            slot("journal") = article.GetJournal,
            slot("authors") = article.GetAuthors.ToArray,
            slot("doi") = article.GetArticleDoi,
            slot("year") = article.GetPublishYear,
            slot("abstract") = If(article.GetAbstractText, "-")
        )
        Dim mesh As list = list.empty

        For Each term As NamedValue(Of String) In article.GetMeshTerms
            If Not mesh.hasName(term.Name) Then
                Call mesh.add(term.Name, term.Value)
            End If
        Next

        Call summary.add("mesh", mesh)
        Call summary.setAttribute("summary", article.ToString)

        Return summary
    End Function

    <ExportAPI("query")>
    Public Function QueryKeyword(keyword As String,
                                 Optional page As Integer = 1,
                                 Optional size As Integer = 2000) As String

        Return PubMed.QueryPubmedRaw(term:=keyword, page:=page, size:=size)
    End Function

    <ExportAPI("read.bits_book")>
    <RApiReturn(GetType(BookPartWrapper))>
    Public Function read_bits_book(file As String) As Object
        Return file.LoadXml(Of BookPartWrapper)(preprocess:=AddressOf BookPartWrapper.PreprocessingXml)
    End Function

    <ExportAPI("read.article_json")>
    <RApiReturn(GetType(PubMedTextTable))>
    Public Function read_articlejson(file As String) As Object
        Return PubMedTextTable.ParseJSON(file)
    End Function

    ''' <summary>
    ''' get the citation list about current article object
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a collection of the citation object
    ''' </returns>
    <ExportAPI("citation_list")>
    <RApiReturn(GetType(Citation))>
    Public Function citation_list(x As Object, Optional env As Environment = Nothing) As Object
        If x Is Nothing Then
            Return Nothing
        End If

        If TypeOf x Is BookPartWrapper Then
            Return DirectCast(x, BookPartWrapper).GetCitations.ToArray
        Else
            Return Message.InCompatibleType(GetType(PubmedArticle), x.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' Parse the document text as a set of article object
    ''' </summary>
    ''' <param name="text">
    ''' the pubmed database in flat file format, or the xml document content of 
    ''' the pubmed article metadata.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("article")>
    <RApiReturn(GetType(PubmedArticle))>
    Public Function Parse(text As String, Optional xml As Boolean = False) As Object
        If text Is Nothing Then
            Return Nothing
        End If
        If xml Then
            ' apply for debug test
            Return PubmedArticleSet.ParseArticleXml(text)
        End If

        Return PubMed.ParseArticles(text).ToArray
    End Function

    <RGenericOverloads("as.data.frame")>
    Public Function createArticleTable(list As PubmedArticle(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }
        Dim articles = list.Select(Function(a) a.MedlineCitation).ToArray
        Call df.add("pubmed", articles.Select(Function(a) a.PMID?.ID))
        Call df.add("doi", articles.Select(Function(a) a.Article.ELocationID.SafeQuery.Where(Function(d) d.EIdType = "DOI").FirstOrDefault?.Value))
        Call df.add("title", articles.Select(Function(a) a.Article.ArticleTitle))
        Call df.add("abstract", articles.Select(Function(a) a.Article.Abstract?.AbstractText.SafeQuery.Select(Function(d) d.Text).JoinBy(vbCrLf)))
        Return df
    End Function

    ''' <summary>
    ''' parse the text data as the article information
    ''' </summary>
    ''' <param name="text">text data in pubmed flat file format</param>
    ''' <returns></returns>
    <ExportAPI("parse")>
    <RApiReturn(GetType(PubmedArticle))>
    Public Function ParsePubmed(<RRawVectorArgument> text As Object) As Object
        Return CLRVector.asCharacter(text) _
            .Select(Function(si) PubMedServicesExtensions.ParseArticles(si)) _
            .IteratesALL _
            .ToArray
    End Function

    ''' <summary>
    ''' Parse the pubmed article set xml stream data
    ''' </summary>
    ''' <param name="file">
    ''' a single file that contains the pubmed article set data, data could be download from the pubmed ftp server in batch.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' batch download of the pubmed data from ncbi ftp server:
    ''' 
    ''' > ftp://ftp.ncbi.nlm.nih.gov/pubmed/baseline/
    ''' </remarks>
    <ExportAPI("parse.article_set")>
    Public Function ParseArticleSetXml(<RRawVectorArgument> file As Object,
                                       Optional tqdm As Boolean = True,
                                       Optional env As Environment = Nothing) As Object

        Dim buf = SMRUCC.Rsharp.GetFileStream(file, IO.FileAccess.Read, env)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If

        ' test gz or ascii text
        Dim s As MemoryStream

        If buf Like GetType(MemoryStream) Then
            s = buf.TryCast(Of MemoryStream)
        Else
            s = New MemoryStream
            buf.TryCast(Of Stream).CopyTo(s)
            s.Flush()
            s.Seek(Scan0, SeekOrigin.Begin)
        End If

        If s.CheckGZipMagic Then
            Using gzipStream As New GZipStream(s, CompressionMode.Decompress)
                s = New MemoryStream
                gzipStream.CopyTo(s)
                s.Seek(Scan0, SeekOrigin.Begin)
            End Using
        End If

        Dim articles As PubmedArticle() = PubmedArticleSet.LoadStream(s, tqdm:=tqdm).ToArray
        Return articles
    End Function
End Module
