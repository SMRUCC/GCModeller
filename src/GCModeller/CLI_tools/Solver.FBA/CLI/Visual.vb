#Region "Microsoft.VisualBasic::6cc83cf600a670256509304223c8a8c4, ..\GCModeller\CLI_tools\Solver.FBA\CLI\Visual.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Serialization.JSON
Imports RDotNet.Extensions.Bioinformatics
Imports RDotNet.Extensions.VisualBasic
Imports RDotNet.Extensions.VisualBasic.gplots
Imports RDotNet.Extensions.VisualBasic.grDevices
Imports RDotNet.Extensions.VisualBasic.utils.read.table
Imports SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA

Partial Module CLI

    <ExportAPI("/heatmap",
               Info:="Draw heatmap from the correlations between the genes and the metabolism flux.",
               Usage:="/heatmap /x <matrix.csv> [/out <out.tiff> /name <Name> /width <8000> /height <6000>]")>
    Public Function Heatmap(args As CommandLine.CommandLine) As Integer
        Dim inX As String = args("/x")
        Dim out As String = args.GetValue("/out", inX.TrimSuffix & ".tiff")
        Dim outDIR As String = out.ParentPath
        Dim nameMap As String = args.GetValue("/name", "Name")
        Dim script As New Heatmap With {
            .dataset = New readcsv(inX),
            .rowNameMaps = nameMap,
            .image = New tiff(out, args.GetValue("/width", 8000), args.GetValue("/height", 6000)),
            .heatmap = heatmap2.Puriney
        }

        Dim scriptText As String = script.RScript
        Dim STD As String() = RServer.WriteLine(scriptText)
        Dim result As heatmap2OUT = heatmap2OUT.RParser(script.output, script.locusId, script.samples)

        Call scriptText.SaveTo(outDIR & "/heatmap.r")
        Call result.GetJson.SaveTo(outDIR & "/heatmap.result.json")
        Return STD.FlushAllLines(outDIR & "/heatmap.STD.txt")
    End Function

    <ExportAPI("/heatmap.scale", Usage:="/heatmap.scale /x <matrix.csv> [/factor 30 /out <out.csv>]")>
    Public Function ScaleHeatmap(args As CommandLine.CommandLine) As Integer
        Dim inX As String = args("/x")
        Dim factor As Double = args.GetValue("/factor", 30)
        Dim out As String = args.GetValue("/out", inX.TrimSuffix & "-" & factor & "__scales.csv")
        Dim MAT As File = File.Load(inX)
        Dim title As RowObject = MAT.First()
        title(Scan0) = NameOf(RPKMStat.Locus)
        Dim data = MAT.AsDataSource(Of RPKMStat)
        data = (From x As RPKMStat In data
                Select New RPKMStat With {
                    .Locus = x.Locus,
                    .Properties = x.Properties _
                        .ToDictionary(Function(key) key.Key,
                                      Function(v) v.Value * factor)}).ToArray
        Return data.SaveTo(out).CLICode
    End Function
End Module

