Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace Infernal.cmsearch

    Public Module ParserAPI

        <Extension>
        Public Function LoadCMSearch(path As String) As SearchSites
            Dim lines As String() = path.ReadAllLines
            Dim i As Integer
            Dim head As String() = ReadHead(lines, offset:=i, __isHead:=AddressOf UntilBlank)
            Dim query As String = head(5).Split(":"c).Last.Trim
            Dim db As String = head(6).Split(":"c).Last.Trim

            Return New SearchSites With {
                .Queries = __querys(lines, i).ToArray,
                .version = head(1),
                .CM = db,
                .query = query
            }
        End Function

        Private Iterator Function __querys(buf As String(), offset As Integer) As IEnumerable(Of Query)
            Dim parts As IEnumerable(Of String()) = buf.Skip(offset).Split("//")

            For Each part As String() In parts
                Yield part.__queryParser
                Call Console.Write("."c)
            Next
        End Function

        <Extension>
        Private Function __queryParser(buf As String()) As Query
            Dim title As String = buf(Scan0)
            Dim acc As String = buf(1).Replace("Accession:", "").Trim
            Dim describ As String = buf(2)
            Dim offset As Integer = 5

            If InStr(describ, "Description:") > 0 Then
                describ = describ.Replace("Description:", "").Trim
                offset += 1
            Else
                describ = ""
            End If

            Dim s As String = Nothing
            Dim list As New List(Of Hit)
            Dim fields As Integer() = buf(offset - 1).CrossFields

            Do While Not buf.Read(offset).ShadowCopy(s).IsBlank AndAlso InStr(s, cmscan.uncertain) <= 0
                list += s.__hitParser(fields)
            Loop

            Dim ulist As New List(Of Hit)

            Do While Not buf.Read(offset).ShadowCopy(s).IsBlank
                If InStr(s, "[No hits detected that satisfy reporting thresholds]") > 0 Then
                    Exit Do
                Else
                    ulist += s.__hitParser(fields)
                End If
            Loop

            Dim len As Long = Regex.Match(title, "CLEN=\d+").Value.Split("="c).Last.ParseLong

            Return New Query With {
                .Accession = acc,
                .hits = list,
                .Clen = len,
                .Query = title.Replace("Query:", "").Trim,
                .Uncertain = ulist,
                .describ = describ
            }
        End Function

        <Extension> Private Function __hitParser(s As String, fields As Integer()) As Hit
            Dim array As String() = s.FieldParser(fields)
            Dim rank As String = array(1) & array(2)
            Dim evalue As Double = array(3).ParseDouble
            Dim score As Double = array(5).ParseDouble
            Dim bias As String = array(7)
            Dim rfam As String = array(9).Trim
            Dim start As Long = array(11).ParseLong
            Dim [end] As Long = array(13).ParseLong
            Dim strand As String = array(14)
            Dim mdl As String = array(15)
            Dim trunc As String = array(17)
            Dim gc As Double = array(19).ParseDouble
            Dim describ As String = array(21)

            Dim hit As New Hit With {
                .bias = bias,
                .description = describ,
                .end = [end],
                .Evalue = evalue,
                .gc = gc,
                .mdl = mdl.Trim,
                .sequence = rfam,
                .rank = rank.Trim,
                .score = score,
                .start = start,
                .strand = strand.Trim,
                .trunc = trunc.Trim
            }

            Return hit
        End Function
    End Module
End Namespace