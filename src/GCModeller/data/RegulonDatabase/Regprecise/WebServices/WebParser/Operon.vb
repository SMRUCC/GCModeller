Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Regprecise

    ''' <summary>
    ''' Operon that regulated in a regulon
    ''' </summary>
    Public Class Operon

        <XmlAttribute> Public Property sId As String
        <XmlElement> Public Property Members As RegulatedGene()

        Public Overrides Function ToString() As String
            Dim lstName As String = Members.ToArray(Function(g) g.Name).JoinBy(", ")
            If String.IsNullOrEmpty(lstName) Then
                lstName = Members.ToArray(Function(g) g.LocusId).JoinBy(", ")
            End If

            Return lstName
        End Function

        Friend Shared Function OperonParser(page As String) As Operon()
            page = Regex.Match(page, "<table id=""operontbl"">.+?</table>", RegexOptions.IgnoreCase Or RegexOptions.Singleline).Value
            Dim Tokens As String() = Regex.Matches(page, "<tr>.+?</tr>", RegexOptions.IgnoreCase Or RegexOptions.Singleline).ToArray
            Dim locus = __locusParser(page)
            Tokens = (From row As String In Tokens Where InStr(row, "<div class=""operon"">") > 0 Select row).ToArray
            Dim lstOperons As Operon() = Tokens.ToArray(Function(value) __operonParser(value, locus))
            Return lstOperons
        End Function

        Private Shared Function __operonParser(value As String, locus As Dictionary(Of String, String)) As Operon
            Dim genes As String() = Regex.Matches(value, "<span>.+?</span>", RegexOptions.IgnoreCase Or RegexOptions.Singleline).ToArray
            genes = (From s As String In genes Where InStr(s, "Locus", CompareMethod.Text) > 0 Select s).ToArray

            Try
                Dim lstGenes As RegulatedGene() = genes.ToArray(Function(s) __geneParser(s, locus))
                Return New Operon With {
                    .Members = lstGenes
                }
            Catch ex As Exception
                ex = New Exception(genes.GetJson, ex)
                Throw ex
            End Try
        End Function

        '<span> Locus tag: AB57_3864<br>Name: yciC<br>Funciton: Putative metal chaperone, GTPase Of COG0523 family
        '</span>

        Private Shared Function __geneParser(value As String, locus As Dictionary(Of String, String)) As RegulatedGene
            value = Mid(value, 8)
            value = Mid(value, 1, Len(value) - 8)
            value = value.TrimVBCrLf
            value = value.Replace(vbTab, "").Trim

            Dim Tokens As String() = Regex.Split(value, "<\s*br\s*/>", RegexOptions.IgnoreCase)
            Dim locusId As String = Tokens(Scan0)
            Dim Name As String = Tokens(1)
            Dim Func As String = Tokens(2)
            locusId = Mid(locusId, 11).Trim
            Name = Mid(Name, 6).Trim
            Func = Mid(Func, 10).Trim
            Dim vmssid As String = locus.TryGetValue(locusId)
            Dim gene As New RegulatedGene With {
                .Function = Func,
                .LocusId = locusId,
                .Name = Name,
                .vimssId = vmssid
            }
            Return gene
        End Function

        Private Shared Function __locusParser(page As String) As Dictionary(Of String, String)
            Dim locus As String() = Regex.Matches(page, "<a href="".+?"">.+?</a>", RegexOptions.IgnoreCase Or RegexOptions.Singleline).ToArray
            Dim dict = (From s As String In locus
                        Let id As String = s.GetValue, url As String = s.Get_href
                        Let vimssid As String = MicrobesOnline.locusId(url)
                        Where Not String.IsNullOrEmpty(vimssid)
                        Select id, vimssid
                        Group By id Into Group) _
                            .ToDictionary(Function(x) x.id, Function(x) x.Group.First.vimssid)
            Return dict
        End Function

        Public Shared Function PageParser(url As String) As Operon()
            Dim page As String = url.GET
            Return OperonParser(page)
        End Function
    End Class
End Namespace