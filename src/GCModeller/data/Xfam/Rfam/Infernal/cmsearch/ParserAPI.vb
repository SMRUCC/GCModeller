#Region "Microsoft.VisualBasic::900c7e4a3528f5a4994c0c97ba659865, data\Xfam\Rfam\Infernal\cmsearch\ParserAPI.vb"

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

    '     Module ParserAPI
    ' 
    '         Function: __hitParser, __queryParser, __querys, LoadCMSearch
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser

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

            Dim s As New Value(Of String)
            Dim list As New List(Of Hit)
            Dim fields As Integer() = buf(offset - 1).CrossFields

            Do While Not (s = buf.Read(offset)).StringEmpty AndAlso InStr(s, cmscan.uncertain) <= 0
                list += s.value.__hitParser(fields)
            Loop

            Dim ulist As New List(Of Hit)

            Do While Not (s = buf.Read(offset)).StringEmpty
                If InStr(s, "[No hits detected that satisfy reporting thresholds]") > 0 Then
                    Exit Do
                Else
                    ulist += s.value.__hitParser(fields)
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
