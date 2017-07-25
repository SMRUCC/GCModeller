#Region "Microsoft.VisualBasic::4813a6650fd42d03e48751884146c765, ..\GCModeller\analysis\Microarray\Enrichment\DAVID.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace DAVID

    Public Module DAVID

        Public Function Load(path$) As FunctionCluster()
            Dim lines As IEnumerable(Of String()) = path _
                .ReadAllLines _
                .Split("Annotation Cluster \d+.+", True) _
                .Where(Function(line) line.Length > 0) _
                .ToArray
            Dim tsv As New List(Of String)

            tsv += lines(0)(Scan0)

            For Each line In lines
                tsv += line.Skip(1)
            Next

            Dim out = tsv.ImportsTsv(Of FunctionCluster)
            Return out
        End Function

        <Extension>
        Public Function SelectGoTerms(data As IEnumerable(Of FunctionCluster)) As FunctionCluster()
            Return LinqAPI.Exec(Of FunctionCluster) <= From x As FunctionCluster
                                                       In data
                                                       Where x.Category.StartsWith("GOTERM_")
                                                       Select x
                                                       Order By x.Category Ascending
        End Function

        ''' <summary>
        ''' 在选择了KEGG的term之后，在这个函数之中还会自动生成KEGG的pathwaymap的url，如果<paramref name="uniprot2KEGG"/>参数可以被使用的话
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SelectKEGGPathway(data As IEnumerable(Of FunctionCluster), Optional uniprot2KEGG As Dictionary(Of String, String()) = Nothing) As FunctionCluster()
            Dim KEGG = LinqAPI.Exec(Of FunctionCluster) <=
 _
                From x As FunctionCluster
                In data
                Where x.Category = "KEGG_PATHWAY"
                Select x
                Order By x.Category Ascending

            If Not uniprot2KEGG.IsNullOrEmpty Then
                For Each term As FunctionCluster In KEGG
                    Dim profile As New NamedCollection(Of NamedValue(Of String)) With {
                        .Name = term.Term.GetTagValue(":").Name,
                        .Value = term.ORFs _
                            .Select(AddressOf Trim) _
                            .Where(Function(id) uniprot2KEGG.ContainsKey(id)) _
                            .Select(Function(ID)
                                        Return uniprot2KEGG(ID).Select(Function(kid) New NamedValue(Of String)(kid, "red"))
                                    End Function) _
                            .IteratesALL _
                            .ToArray
                    }

                    term.Link = profile.KEGGURLEncode()
                Next
            End If

            Return KEGG
        End Function
    End Module

    Public Class FunctionCluster : Implements IKEGGTerm

        Public Property Category As String Implements IKEGGTerm.ID
        Public Property Term As String Implements IKEGGTerm.Term
        Public Property Count As Integer
        <Column("%")> Public Property Percent As Double
        Public Property PValue As Double Implements IKEGGTerm.Pvalue
        <Collection("Genes", ",")> Public Property ORFs As String() Implements IKEGGTerm.ORF
        <Column("List Total")> Public Property ListTotal As Integer
        <Column("Pop Hits")> Public Property PopHits As Integer
        <Column("Pop Total")> Public Property PopTotal As Integer
        <Column("Fold Enrichment")> Public Property FoldEnrichment As Double
        Public Property Bonferroni As Double
        Public Property Benjamini As Double
        Public Property FDR As Double
        Public Property Link As String Implements IKEGGTerm.Link

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
