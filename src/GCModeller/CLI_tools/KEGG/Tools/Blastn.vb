#Region "Microsoft.VisualBasic::b33388bf5bf5f7438e7297c41fc94e8e, CLI_tools\KEGG\Tools\Blastn.vb"

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

    ' Module Blastn
    ' 
    '     Function: __parser, __submit, PageParser, Parser, (+2 Overloads) Submit
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Public Module Blastn

    Public Function Submit(seq As String) As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.BlastnHit()
        Return __submit(FileIO.FileSystem.ReadAllText(seq))
    End Function

    Private Function __submit(seq As String) As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.BlastnHit()
        Dim param As New Collections.Specialized.NameValueCollection

        Call param.Add("sequence", seq)
        Call param.Add("prog", "blastn")
        Call param.Add("dbname", "genes-nt")
        Call param.Add("V_value", 500)
        Call param.Add("B_value", 250)
        Call param.Add("submit", "Compute")

        Dim result = PostRequest("http://www.genome.jp/tools-bin/blastplus", param).html
        Dim hits = Parser(page:=result)
        Return hits
    End Function

    Public Function Submit(seq As SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.BlastnHit()
        Dim doc As String = seq.GenerateDocument(60)
        Return __submit(doc)
    End Function

    Const hit As String = "<input type=""checkbox"".+?<a href=""[/]tmp[/]blast[/].+?\d+<[/]a>   \d+\.\d+"

    Public Function PageParser(url As String) As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.BlastnHit()
        Return Parser(url.GET)
    End Function

    Public Function Parser(page As String) As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.BlastnHit()
        Dim hits As String() = Strings.Split(page, """checkbox""").Skip(1).ToArray
        Dim parsing = hits.Select(Function(x) __parser(x))
        Return parsing
    End Function

    Private Function __parser(line As String) As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.BlastnHit
        Dim entry As String = Regex.Match(line, "value="".+?""", RegexOptions.IgnoreCase).Value
        Dim def As String = Regex.Match(line, "</a>.+?<a", RegexOptions.IgnoreCase).Value
        Dim KO As String = Regex.Match(line, ">K\d+</a>", RegexOptions.IgnoreCase).Value
        Dim params As String() = line.Split(">"c)
        Dim bits As Double = Val(params(params.Length - 2).Trim)
        Dim evalue As Double = Val(params.Last.Trim)
        KO = KO.GetValue
        def = Mid(def, 5).Trim
        def = Mid(def, 1, Len(def) - 2).Trim
        entry = entry.Split(""""c)(1)

        Return New SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.BlastnHit With {
            .Bits = bits,
            .Eval = evalue,
            .Definition = def,
            .KO = KO,
            .LocusId = entry
        }
    End Function

    '     name="ckid" value="ach:Achl_1407"><a href="http://www.genome.jp/dbget-bin/www_bget?ach:Achl_1407">achAchl_1407</a>  3-oxoacid CoA-transferase, B subunit (EC:2.8.3.5)  <a href="/dbget-bin/www_bget?K01029" target="_blank">K01029</a>   <a href="/tmp/blast/1512172111047skpW/result_blast.html#ach:Achl_1407">44.6</a>    0.69
    '<input type =

End Module
