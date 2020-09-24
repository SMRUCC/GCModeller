#Region "Microsoft.VisualBasic::97cd0f3af3ff0de9422b98275e074a86, data\ExternalDBSource\DEG\Geptop.vb"

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

    ' Class Geptop
    ' 
    '     Properties: assembly, genes, genome
    ' 
    '     Function: GenericEnumerator, GetEnumerator, GetGenomeList, ParseFromOutput
    ' 
    '     Sub: FetchData
    '     Class Gene
    ' 
    '         Properties: [class], essentialityScore, protein
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.Assembly.NCBI.Entrez

Public Class Geptop : Inherits XmlDataModel
    Implements Enumeration(Of Gene)

    Public Class Gene
        <XmlAttribute> Public Property [class] As Integer
        <XmlAttribute> Public Property essentialityScore As Double
        <XmlAttribute> Public Property protein As String
    End Class

    Public Property genome As String
    Public Property assembly As String()

    <XmlElement("gene")>
    Public Property genes As Gene()

    Public Shared Function ParseFromOutput(output As String) As Geptop
        Dim lines = output.SolveStream _
            .LineTokens _
            .Where(Function(l) Strings.Trim(l).First <> "#"c) _
            .Skip(1) _
            .ToArray

        Return lines _
            .Select(Function(line)
                        Dim tokens = line.Split(ASCII.TAB)

                        Return New Geptop.Gene With {
                            .[class] = Val(tokens(0)),
                            .essentialityScore = Val(tokens(1)),
                            .protein = tokens(3)
                        }
                    End Function) _
            .ToArray
    End Function

    Const listAPI$ = "http://cefg.uestc.cn/geptop/list.html"

    Public Shared Function GetGenomeList() As NamedValue(Of String)()
        Dim html As String = listAPI.GET
        Dim list = html.Matches("<li.+?</li>").ToArray
        Dim genomes = list _
            .Select(Function(li)
                        Dim link = li.href
                        Dim name = li.StripHTMLTags

                        Return New NamedValue(Of String) With {
                            .Name = name,
                            .Value = link
                        }
                    End Function) _
            .ToArray

        Return genomes
    End Function

    Public Shared Sub FetchData(save$)
        Dim web As New WebQuery(Of NamedValue(Of String))(
            Function(li) li.Value,
            Function(li) li.Name,
            Function(s, t) Geptop.ParseFromOutput(s),
 _
            prefix:=Nothing,
            cache:=$"{save}/.geptop"
        )

        For Each genome As NamedValue(Of String) In GetGenomeList()
            Dim result As Geptop = web.Query(Of Geptop)(genome, "*.html")
            Dim accessionID$() = genome.Name _
                .Matches("\(.+?\)") _
                .LastOrDefault _
                .GetStackValue("(", ")") _
                .StringSplit("\s+")

            result.assembly = accessionID
            result.genome = genome.Name
            result.GetXml.SaveTo($"{save}/{genome.Name.NormalizePathString}.Xml")

            For Each id As String In result.assembly
                With $"{save}/assembly/{id}.gb"
                    If Not .FileExists Then
                        Call Genbank.Fetch(id, .ByRef)
                        Call Thread.Sleep(2000)
                    End If
                End With
            Next
        Next
    End Sub

    Public Shared Widening Operator CType(genes As Gene()) As Geptop
        Return New Geptop With {.genes = genes}
    End Operator

    Public Iterator Function GenericEnumerator() As IEnumerator(Of Gene) Implements Enumeration(Of Gene).GenericEnumerator
        For Each gene As Gene In genes
            Yield gene
        Next
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of Gene).GetEnumerator
        Yield GenericEnumerator()
    End Function
End Class
