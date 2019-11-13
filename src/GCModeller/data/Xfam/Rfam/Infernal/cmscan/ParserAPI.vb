#Region "Microsoft.VisualBasic::13c83248d613ddd8e801db6e69cd69c1, data\Xfam\Rfam\Infernal\cmscan\ParserAPI.vb"

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
    '         Function: __hitParser, __querys, LoadCmScan
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser

Namespace Infernal.cmscan

    Public Module ParserAPI

        <Extension>
        Public Function LoadCmScan(path As String) As ScanSites
            Dim lines As String() = path.ReadAllLines
            Dim i As Integer
            Dim head As String() = ReadHead(lines, offset:=i, __isHead:=AddressOf UntilBlank)
            Dim query As String = head(5).Split(":"c).Last.Trim
            Dim db As String = head(6).Split(":"c).Last.Trim

            Return New ScanSites With {
                .QueryHits = __querys(lines, i),
                .version = head(1),
                .CM = db,
                .query = query
            }
        End Function

        Private Function __querys(buf As String(), offset As Integer) As Query
            Dim title As String = buf(offset)
            Dim describ As String = buf(offset + 1).Replace("Description:", "").Trim
            Dim s As New Value(Of String)
            Dim list As New List(Of Hit)

            offset += 4

            Dim fields As Integer() = buf(offset).CrossFields

            offset += 1

            Do While Not (s = buf.Read(offset)).StringEmpty AndAlso InStr(s, uncertain) <= 0
                list += s.value.__hitParser(fields)
            Loop

            Dim ulist As New List(Of Hit)

            Do While Not (s = buf.Read(offset)).StringEmpty
                ulist += s.value.__hitParser(fields)
            Loop

            Dim len As Long = Regex.Match(title, "L=\d+").Value.Split("="c).Last.ParseLong

            Return New Query With {
                .Description = describ,
                .hits = list,
                .Length = len,
                .title = title.Replace("Query:", "").Trim,
                .uncertainHits = ulist
            }
        End Function

        Public Const uncertain As String = "------ inclusion threshold ------"

        <Extension> Private Function __hitParser(s As String, fields As Integer()) As Hit
            Dim array As String() = s.FieldParser(fields)
            Dim rank As String = array(1) & array(2)
            Dim i As Pointer = New Pointer(3) <= 2
            Dim evalue As Double = array(++i).ParseDouble
            Dim score As Double = array(++i).ParseDouble
            Dim bias As String = array(++i)
            Dim rfam As String = array(++i).Trim
            Dim start As Long = array(++i).ParseLong
            Dim [end] As Long = array(++i).ParseLong
            Dim strand As String = array(-1 + (i << -2))

            i = (i + 2) <= 2

            Dim mdl As String = array(++i)
            Dim trunc As String = array(++i)
            Dim gc As Double = array(++i).ParseDouble
            Dim describ As String = array(i)

            Dim hit As New Hit With {
                .bias = bias,
                .description = describ,
                .end = [end],
                .Evalue = evalue,
                .gc = gc,
                .mdl = mdl.Trim,
                .modelname = rfam,
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
