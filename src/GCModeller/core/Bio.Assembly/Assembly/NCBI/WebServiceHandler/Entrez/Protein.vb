#Region "Microsoft.VisualBasic::c562caa567e371850ee26a33ee0c2198, GCModeller\core\Bio.Assembly\Assembly\NCBI\WebServiceHandler\Entrez\Protein.vb"

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

    '   Total Lines: 107
    '    Code Lines: 82
    ' Comment Lines: 4
    '   Blank Lines: 21
    '     File Size: 4.89 KB


    '     Class Protein
    ' 
    '         Function: CreateQuery, GetEntry
    ' 
    '     Class Entry
    ' 
    '         Properties: FASTAUrl, GetBacterial, LocusTag
    ' 
    '         Function: (+2 Overloads) FetchSeq, GetLocusTag, Parse, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions

Namespace Assembly.NCBI.Entrez

    Public Class Protein

        Public Const PROTEIN_SEARCH As String = "http://www.ncbi.nlm.nih.gov/protein/?term={0}+AND+%22{1}%22%5BOrganism%5D"

        Public Shared Function CreateQuery(Keyword As String, Organism As String) As String
            Return String.Format(PROTEIN_SEARCH, Keyword, Organism.Replace(" ", "+").Replace(vbTab, "+"))
        End Function

        Const REPORT As String = "<div><div class=""rprt"">.+?"

        Public Function GetEntry(Keyword As String, Organism As String, Optional MaxLimited As UInteger = 5) As Entry()
            Dim url As String = CreateQuery(Keyword, Organism)
            Dim pageContent As String = url.GET

            Call Debug.WriteLine(url)

            pageContent = Mid(pageContent, InStr(pageContent, "<div class=""title_and_pager"">", CompareMethod.Text) + 20)
            pageContent = Mid(pageContent, 1, InStr(pageContent, "<div class=""title_and_pager bottom"">", CompareMethod.Text))

            Dim Tokens As String() = Strings.Split(pageContent, "<div class=""rprt"">").Skip(1).ToArray
            Dim EntryList As Entry() = New Entry(System.Math.Min(Tokens.Count, MaxLimited) - 1) {}
            For i As Integer = 0 To EntryList.Count - 1
                EntryList(i) = Entry.Parse(Tokens(i))
            Next

            Return EntryList
        End Function
    End Class

    ''' <summary>
    ''' 查询某一个蛋白质或者基因对象所返回的结果数据下载入口点
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Entry : Inherits Entrez.ComponentModels.I_QueryEntry

        Public Property FASTAUrl As String
        Public Property LocusTag As String

        Public ReadOnly Property GetBacterial() As String
            Get
                Dim result = Regex.Match(Title, "\[[^][]+?\]$", RegexOptions.Singleline).Value
                result = Mid(result, 2, Len(result) - 2)
                Return result
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Title
        End Function

        Const REGX_TITLE As String = "<p class=""title"">.+?</p>"
        Const REGX_FASTA As String = "<a ref=""[^<]+?"" href=""[^<]+?"">FASTA</a>"

        Public Shared Function Parse(strText As String) As Entry
            Dim Entry As Entry = New Entry
            Entry.Title = Regex.Match(strText, REGX_TITLE, RegexOptions.Singleline).Value
            Entry.FASTAUrl = Regex.Match(strText, REGX_FASTA, RegexOptions.Singleline).Value
            Entry.Url = Regex.Match(Entry.Title, "href="".+?""", RegexOptions.Singleline).Value
            Entry.Title = Mid(Entry.Title, InStr(Entry.Title, "ref="))
            Entry.Title = Mid(Entry.Title, InStr(Entry.Title, ">") + 1)
            Entry.Title = Mid(Entry.Title, 1, Len(Entry.Title) - 8)
            Entry.FASTAUrl = Regex.Match(Entry.FASTAUrl, "href="".+?""", RegexOptions.Singleline).Value
            Entry.FASTAUrl = "http://www.ncbi.nlm.nih.gov" & Mid(Entry.FASTAUrl, 7, Len(Entry.FASTAUrl) - 7)
            Entry.Url = "http://www.ncbi.nlm.nih.gov" & Mid(Entry.Url, 7, Len(Entry.Url) - 7)
            Entry.LocusTag = GetLocusTag(Entry.Url)

            Return Entry
        End Function

        Public Shared Function FetchSeq(entry As Entry) As SequenceModel.FASTA.FastaSeq
            If entry.LocusTag = "" Then
                Return Nothing
            Else
                Dim KEGG_Entry = KEGG.WebServices.WebRequest.HandleQuery(entry.LocusTag).First
                Return KEGG.WebServices.WebRequest.FetchSeq(KEGG_Entry)
            End If
        End Function

        Public Shared Function FetchSeq(entries As Entry()) As SequenceModel.FASTA.FastaFile
            Dim LQuery = (From entry As NCBI.Entrez.Entry
                          In entries
                          Where Not String.IsNullOrEmpty(entry.LocusTag)
                          Let KEGG_Entry As KEGG.WebServices.QueryEntry() = KEGG.WebServices.WebRequest.HandleQuery(entry.LocusTag)
                          Where Not KEGG_Entry.IsNullOrEmpty
                          Select KEGG.WebServices.WebRequest.FetchSeq(KEGG_Entry.First)).ToArray
            Return CType(LQuery, SequenceModel.FASTA.FastaFile)
        End Function

        Private Shared Function GetLocusTag(url As String) As String
            Dim pageContent As String = url.GET
            Dim p = InStr(pageContent, "Also Known As")
            If p = 0 Then
                Return ""
            Else
                Dim LocusTag As String = Mid(pageContent, p, 100)
                LocusTag = Regex.Match(LocusTag, "[:]\s+[a-zA-Z0-9_]+", RegexOptions.Singleline).Value
                LocusTag = Trim(LocusTag.Split(CChar(":")).Last)

                Return LocusTag
            End If
        End Function
    End Class
End Namespace
