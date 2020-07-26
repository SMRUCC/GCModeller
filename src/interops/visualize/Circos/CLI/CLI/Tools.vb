#Region "Microsoft.VisualBasic::cfd403c04543d05a821f673c74f9c967, visualize\Circos\CLI\CLI\Tools.vb"

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

    ' Module CLI
    ' 
    '     Function: Compare, DOOR_COGs, DumpNames
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG
Imports SMRUCC.genomics.Visualize.Circos

Partial Module CLI

    <ExportAPI("/DOOR.COGs", Usage:="/DOOR.COGs /DOOR <genome.opr> [/out <out.COGs.Csv>]")>
    Public Function DOOR_COGs(args As CommandLine) As Integer
        Dim inFile As String = args("/DOOR")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".COGs.Csv")
        Dim DOOR As DOOR = DOOR_API.Load(inFile)
        Dim COGs As MyvaCOG() = DOOR.Genes _
            .Select(Function(x)
                        Return New MyvaCOG With {
                .COG = x.COG_number,
                .Description = x.Product,
                .Length = x.Length,
                .Category = Regex.Split(x.COG_number, "\d+").Last,
                .MyvaCOG = x.COG_number,
                .QueryName = x.Synonym}
                    End Function).OrderBy(Function(x) x.QueryName).ToArray
        Return COGs.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Regulons.Dumps", Usage:="/Regulons.Dumps /in <genomes.bbh.DIR> /ptt <genome.ptt> [/out <out.Csv>]")>
    Public Function DumpNames(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim ptt As String = args("/ptt")
        Dim out As String = args.GetValue("/out", inDIR & ".Names.Csv")
        Dim gbPTT = SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.Load(ptt)
        Dim names = NameExtensions.DumpNames(inDIR, gbPTT)
        Return names.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Compare",
               Usage:="/Compare /locis <list.txt> /vectors <vectors.txt.DIR> [/winSize 100 /steps 1 /out <out.csv>]")>
    Public Function Compare(args As CommandLine) As Integer
        Dim locis$ = args("/locis")
        Dim vectorDIR$ = args("/vectors")
        Dim winSize% = args.GetValue("/winSize", 100)
        Dim steps = args.GetValue("/steps", 1)
        Dim out As String = args.GetValue(
            "/out",
            locis.TrimSuffix & "-" & vectorDIR.BaseName & $"-winSize={winSize}-steps={steps}.csv")
        Dim sites As Location() = LinqAPI.Exec(Of Location) <=
            From line As String
            In locis.IterateAllLines
            Let t As String() = line.Split(","c)
            Let st = CInt(Val(t(0)))
            Let sp = CInt(Val(t(1)))
            Select New Location(st, sp)

        Dim csv As New IO.File

        csv.Add({"Item", "genome.avg", "genome.max", "genome.min"}.Join(
                sites.Select(Function(p)
                                 Dim tag$ = $"{p.Left}..{p.Right}"
                                 Return {tag & ".lt", tag & ".gt", tag & ".avg"}
                             End Function).IteratesALL))

        For Each file$ In ls - l - r - "*.txt" <= vectorDIR
            Dim vector#() = file.LoadAsNumericVector
            Dim sliWins = vector.SlideWindows(winSize, steps).ToArray
            Dim avg#() = sliWins _
                .Select(Function(block) block.Average) _
                .ToArray

            Dim row As New RowObject

            Call row.Add(file.BaseName)
            Call row.AddRange({CStr(avg.Average), CStr(avg.Max), CStr(avg.Min)})

            For Each loci As Location In sites
                Dim left = loci.Left / steps + 1
                Dim right = left + loci.Length / steps
                Dim list As New List(Of Double)

                For i As Integer = left To right
                    list += avg#(i)
                Next

                Dim gt = Function(x#) list.Where(Function(d) d > x).Count > 0
                Dim lt = Function(x#) list.Where(Function(d) d < x).Count > 0

                Call row.AddRange({
                     CStr(avg.Where(lt).Count),
                     CStr(avg.Where(gt).Count),
                     CStr(list.Average)
                })
            Next

            Call csv.Add(row)
        Next

        Call csv.AppendLine()
        Call csv.Add("gt=great than genome")
        Call csv.Add("lt=less than genome")

        Return csv.Save(out, Encodings.ASCII).CLICode
    End Function
End Module
