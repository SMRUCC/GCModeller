#Region "Microsoft.VisualBasic::79bc7976ed5310c0339968e392c3b94c, CLI_tools\RNA-seq\CLI\Expressions.vb"

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
    '     Function: Log2, Log2Selects, MergeRPKMs, StatChanges
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2

Partial Module CLI

    <ExportAPI("/Stat.Changes", Usage:="/Stat.Changes /deseq <deseq.result.csv> /sample <sampletable.csv> [/out <out.csv> /levels <1000> /diff <0.5>]")>
    Public Function StatChanges(args As CommandLine) As Integer
        Dim inFile As String = args("/deseq")
        Dim sample As String = args("/sample")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".stat_changes.Csv")
        Dim levels As Integer = args.GetValue("/levels", 1000)
        Dim diff As Double = args.GetValue("/diff", 0.5)
        Dim result = SleepIdentified.IdentifyChanges(inFile.LoadCsv(Of ResultData), sample.LoadCsv(Of SampleTable), diff, levels)
        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/RPKM.Log2",
               Usage:="/RPKM.Log2 /in <RPKM.csv> /cond <conditions> [/out <out.csv>]")>
    <ArgumentAttribute("/cond", False,
                   Description:="Syntax format as:  <experiment1>/<experiment2>|<experiment3>/<experiment4>|.....")>
    Public Function Log2(args As CommandLine) As Integer
        Dim inRPKM As String = args("/in")
        Dim out As String = args.GetValue("/out", inRPKM.TrimSuffix & ".log2.csv")
        Dim samples As Experiment() = Experiment.GetSamples(args("/cond"))
        Dim MAT As MatrixFrame = MatrixFrame.Load(IO.File.Load(inRPKM))
        Dim log2s As IO.File = MAT.Log2(samples)
        Return log2s.Save(out, Encodings.ASCII).CLICode
    End Function

    <ExportAPI("/log2.selects", Usage:="/log2.selects /log2 <rpkm.log2.csv> /data <dataset.csv> [/locus_map <locus> /factor 1 /out <out.dataset.csv>]")>
    Public Function Log2Selects(args As CommandLine) As Integer
        Dim inFile As String = args("/log2")
        Dim data As String = args("/data")
        Dim locus_map As String = args.GetValue("/locus_map", "locus")
        Dim out As i32 = args.OpenHandle("/out", inFile.TrimSuffix & $".selects-{data.BaseName}.out.csv")
        Dim log2 = IO.DataSet.LoadDataSet(inFile, "LocusId")
        Dim factor As Double = args.GetValue("/factor", 1.0R)
        Dim dataSets = (From x As IO.EntityObject
                        In IO.EntityObject.LoadDataSet(data, locus_map)
                        Select x
                        Group x By x.ID Into Group) _
                             .ToDictionary(Function(x) x.ID,
                                           Function(x) x.Group.ToArray)
        Dim LQuery = (From x In log2
                      Where dataSets.ContainsKey(x.ID) AndAlso
                          (From p As Double
                           In x.Properties.Values
                           Where Math.Abs(p) >= factor
                           Select p).FirstOrDefault > 0
                      Select dataSets(x.ID)).IteratesALL
        Return LQuery > out
    End Function

    <ExportAPI("/DataFrame.RPKMs", Info:="Merges the RPKM csv data files.",
               Usage:="/DataFrame.RPKMs /in <in.DIR> [/trim /out <out.csv>]")>
    Public Function MergeRPKMs(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim trim As Boolean = args.GetBoolean("/trim")
        Dim out As String = args.GetValue("/out", [in].ParentPath & "/" & [in].BaseName & $".RPKMs{If(trim, "-TRIM", "")}.Csv")
        Dim dataExpr0 As IO.File = MergeDataMatrix([in], trim)
        Return dataExpr0.Save(out, Encodings.ASCII)
    End Function
End Module
