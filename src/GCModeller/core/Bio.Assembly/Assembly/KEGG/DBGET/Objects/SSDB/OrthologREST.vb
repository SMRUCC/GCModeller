#Region "Microsoft.VisualBasic::65bce1d4952bce3089ebb36982975b4e, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\SSDB\OrthologREST.vb"

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

    '   Total Lines: 58
    '    Code Lines: 35
    ' Comment Lines: 11
    '   Blank Lines: 12
    '     File Size: 2.70 KB


    '     Class OrthologREST
    ' 
    '         Properties: Definition, KEGG_ID, Orthologs, Sequence
    ' 
    '         Function: Download, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG.DBGET.bGetObject.SSDB

    ''' <summary>
    ''' 蛋白质直系同源比对blastp结果, ``*.xml``
    ''' </summary>
    ''' <remarks>
    ''' 在直系同源的数据被下载下来之后，这个对象会被直接保存为Xml文档
    ''' </remarks>
    <XmlType("Ortholog")>
    Public Class OrthologREST

        Public Const URL As String = "http://www.kegg.jp/ssdb-bin/ssdb_best?org_gene={0}:{1}"

        <XmlAttribute> Public Property KEGG_ID As String
        <XmlAttribute> Public Property Definition As String
        <XmlElement> Public Property Sequence As String
        ''' <summary>
        ''' 直系同源的蛋白质比对结果
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property Orthologs As SShit()

        Public Const SEPRATOR As String = "------------------------------------------------------------------ -------------------------------------------------------------"

        ' <INPUT TYPE="checkbox" NAME="ckid" VALUE="xca:xccb100_1230" CHECKED><A HREF="http://www.kegg.jp/dbget-bin/www_bget?xca:xccb100_1230" TARGET="_blank">xca:xccb100_1230</A> type IV pilus response regulator PilH  <A HREF="http://www.kegg.jp/dbget-bin/www_bget?ko:K02658"  TARGET="_blank">K02658</a>     120      771 (  378)     182    1.000    120     &lt;-&gt; <a href='/ssdb-bin/ssdb_ortholog_view?org_gene=xcb:XC_1184&org=xca&threshold=&type=' target=_ortholog>66</a>
        Public Const REGEX_ORTHO_ITEM As String = "<INPUT TYPE=""checkbox"" .+? target=_ortholog>\d+</a>"

        Public Overrides Function ToString() As String
            Return Definition
        End Function

        Public Shared Function Download(locusTag As QueryEntry) As OrthologREST
            Dim html As String = String.Format(URL, locusTag.speciesID, locusTag.locusID).GET
            Dim tokens = Strings.Split(html, SEPRATOR)
            Dim hits As String() = Regex.Matches(tokens.Last, REGEX_ORTHO_ITEM, RegexICSng).ToArray

            Dim Ortholog As New OrthologREST With {
                .Orthologs = LinqAPI.Exec(Of String, SShit)(hits) <= Function(s) SShit.CreateObject(s),
                .KEGG_ID = locusTag.ToString
            }

            Dim Fa As FASTA.FastaSeq = WebRequest.FetchSeq(locusTag)

            If Not Fa Is Nothing Then
                Ortholog.Definition = Fa.Title
                Ortholog.Sequence = Fa.SequenceData
            End If

            Return Ortholog
        End Function
    End Class
End Namespace
