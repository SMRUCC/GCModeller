#Region "Microsoft.VisualBasic::556146eee1abd2c1147f33071a728ef4, CLI_tools\eggHTS\CLI\Samples\LabelFree.vb"

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
    '     Function: ExtractMatrix, LabelFreeMatrix, LabelFreeMatrixSplit, labelFreeTtest, MajorityProteinIDs
    '               matrixByInternal, matrixByUniprot, MatrixColRenames, MatrixNormalize, PerseusStatics
    '               PerseusTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.DataFrame.Impute
Imports Microsoft.VisualBasic.MIME.Office.Excel
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File

Partial Module CLI

    ''' <summary>
    ''' 将perseus软件的输出转换为csv文档并且导出uniprot编号以方便进行注释
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Perseus.Table")>
    <Usage("/Perseus.Table /in <proteinGroups.txt> [/out <out.csv>]")>
    <Group(CLIGroups.Samples_CLI)>
    Public Function PerseusTable(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".csv")
        Dim data As Perseus() = [in].LoadTsv(Of Perseus)
        Dim idlist As String() = data _
            .Select(Function(prot) prot.ProteinIDs) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim uniprotIDs$() = idlist _
            .Select(Function(s) s.Split("|"c, ":"c)(1)) _
            .Distinct _
            .ToArray

        Call idlist.SaveTo(out.TrimSuffix & ".proteinIDs.txt")
        Call uniprotIDs.SaveTo(out.TrimSuffix & ".uniprotIDs.txt")

        Return data.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Perseus.Stat")>
    <Usage("/Perseus.Stat /in <proteinGroups.txt> [/out <out.csv>]")>
    <Group(CLIGroups.Samples_CLI)>
    Public Function PerseusStatics(args As CommandLine) As Integer
        Dim in$ = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".perseus.Stat.csv")
        Dim data As Perseus() = [in].LoadTsv(Of Perseus)
        Dim csv As New csv

        Call csv.AppendLine({"MS/MS", CStr(Perseus.TotalMSDivideMS(data))})
        Call csv.AppendLine({"Peptides", CStr(Perseus.TotalPeptides(data))})
        Call csv.AppendLine({"ProteinGroups", CStr(data.Length)})

        Return csv.Save(out, Encodings.ASCII).CLICode
    End Function

    <ExportAPI("/Perseus.MajorityProteinIDs")>
    <Usage("/Perseus.MajorityProteinIDs /in <table.csv> [/out <out.txt>]")>
    <Description("Export the uniprot ID list from ``Majority Protein IDs`` row and generates a text file for batch search of the uniprot database.")>
    Public Function MajorityProteinIDs(args As CommandLine) As Integer
        With args <= "/in"
            Dim out$ = (args <= "/out") Or (.TrimSuffix & "-uniprotID.txt").AsDefault
            Dim table As Perseus() = .LoadCsv(Of Perseus)
            Dim major$() = table _
                .Select(Function(protein)
                            Return protein.Majority_proteinIDs
                        End Function) _
                .IteratesALL _
                .Distinct _
                .ToArray

            Return major _
                .FlushAllLines(out) _
                .CLICode
        End With
    End Function

    <ExportAPI("/labelFree.matrix")>
    <Usage("/labelFree.matrix /in <*.csv/*.xlsx> [/sheet <default=proteinGroups> /intensity /uniprot <uniprot.Xml> /organism <scientificName> /out <out.csv>]")>
    <Group(CLIGroups.LabelFreeTool)>
    Public Function LabelFreeMatrix(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim isIntensity As Boolean = args("/intensity")
        Dim uniprot$ = args("/uniprot")
        Dim organism$ = args("/organism")
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.{If(isIntensity, "intensity", "iBAQ")}.csv"
        Dim table As EntityObject() = EntityObject _
            .LoadDataSet([in].ReadTableAuto(args("/sheet") Or "proteinGroups")) _
            .ToArray

        If uniprot.FileExists Then
            Return table.matrixByUniprot(uniprot, organism, isIntensity) _
                        .SaveTo(out) _
                        .CLICode
        Else
            ' 没有额外的信息，则尝试使用内部的注释信息来完成
            Return table.matrixByInternal(isIntensity) _
                        .SaveTo(out) _
                        .CLICode
        End If
    End Function

    <Extension>
    Private Function matrixByInternal(table As EntityObject(), isIntensity As Boolean) As DataSet()
        Dim getID As Func(Of EntityObject, String)

        If table(0).Properties.ContainsKey("Fasta headers") Then
            getID = Function(x)
                        Dim headerID = x("Fasta headers").Split("|"c).ElementAtOrDefault(1)

                        If headerID.StringEmpty Then
                            Return x.ID.StringSplit("\s*;\s*")(0)
                        Else
                            Return headerID
                        End If
                    End Function
        Else
            getID = Function(x) x.ID.StringSplit("\s*;\s*")(0)
        End If

        Return table.ExtractMatrix(getID, isIntensity)
    End Function

    <Extension>
    Private Function ExtractMatrix(table As EntityObject(), getID As Func(Of EntityObject, String), isIntensity As Boolean) As DataSet()
        Dim keyPrefix$ = "iBAQ " Or "Intensity ".When(isIntensity)
        Dim projectDataSet = Function(x As EntityObject) As DataSet
                                 Dim id As String = getID(x)
                                 Dim data = x.Properties _
                                     .Where(Function(c)
                                                Return InStr(c.Key, keyPrefix) > 0
                                            End Function) _
                                     .ToDictionary _
                                     .AsNumeric

                                 Return New DataSet With {
                                     .ID = id,
                                     .Properties = data
                                 }
                             End Function

        Return table.Select(projectDataSet).ToArray
    End Function

    <Extension>
    Private Function matrixByUniprot(table As EntityObject(), xml$, organism$, isIntensity As Boolean) As DataSet()
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' 计算差异蛋白
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/labelFree.t.test")>
    <Usage("/labelFree.t.test /in <matrix.csv> /sampleInfo <sampleInfo.csv> /control <groupName> /treatment <groupName> [/significant <t.test/AB, default=t.test> /level <default=1.5> /p.value <default=0.05> /FDR <default=0.05> /out <out.csv>]")>
    <Group(CLIGroups.LabelFreeTool)>
    Public Function labelFreeTtest(args As CommandLine) As Integer
        Dim data As DataSet() = DataSet.LoadDataSet(args <= "/in") _
            .SimulateMissingValues(byRow:=False, infer:=InferMethods.Min) _
            .TotalSumNormalize _
            .ToArray
        Dim level# = args.GetValue("/level", 1.5)
        Dim pvalue# = args.GetValue("/p.value", 0.05)
        Dim FDR# = args.GetValue("/FDR", 0.05)
        Dim out$ = args.GetValue("/out", (args <= "/in").TrimSuffix & ".log2FC.t.test.csv")
        Dim sampleInfo As SampleInfo() = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
        Dim designer As New AnalysisDesigner With {
            .Controls = args <= "/control",
            .Treatment = args <= "/treatment"
        }
        Dim usingSignificantAB As Boolean = (args("/significant") Or "t.test") = "AB"
        Dim DEPs As DEP_iTraq() = data.logFCtest(designer, sampleInfo, level, pvalue, FDR, significantA:=usingSignificantAB)

        Return DEPs _
            .ToArray _
            .SaveDataSet(out) _
            .CLICode
    End Function

    <ExportAPI("/labelFree.matrix.split")>
    <Usage("/labelFree.matrix.split /in <matrix.csv> /sampleInfo <sampleInfo.csv> /designer <analysis_designer.csv> [/out <directory>]")>
    <Group(CLIGroups.LabelFreeTool)>
    Public Function LabelFreeMatrixSplit(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.matrix/"
        Dim matrix As DataSet() = DataSet.LoadDataSet([in]).ToArray
        Dim sampleInfo As SampleInfo() = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
        Dim designer As AnalysisDesigner() = (args <= "/designer").LoadCsv(Of AnalysisDesigner)

        For Each analysis As AnalysisDesigner In designer
            Dim controls = sampleInfo.SampleIDs(analysis.Controls)
            Dim treatments = sampleInfo.SampleIDs(analysis.Treatment)
            Dim subMatrix As DataSet() =
                Iterator Function(projection As String()) As IEnumerable(Of DataSet)
                    For Each protein In matrix
                        Yield protein.SubSet(projection)
                    Next
                End Function(controls + treatments).ToArray

            Dim controlGroup = sampleInfo.TakeGroup(analysis.Controls).AsList
            Dim treatmentGroup As SampleInfo() = sampleInfo.TakeGroup(analysis.Treatment)

            Names(subMatrix) = controlGroup + treatmentGroup

            Call subMatrix.SaveTo($"{out}/{analysis.Title}.csv")
        Next

        Return 0
    End Function

    <ExportAPI("/names")>
    <Usage("/names /in <matrix.csv> /sampleInfo <sampleInfo.csv> [/out <out.csv>]")>
    <Group(CLIGroups.LabelFreeTool)>
    Public Function MatrixColRenames(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.sample_name.csv"
        Dim matrix As DataSet() = DataSet.LoadDataSet([in]).ToArray
        Dim sampleInfo As SampleInfo() = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)

        Names(matrix) = sampleInfo

        Return matrix.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Matrix.Normalization")>
    <Usage("/Matrix.Normalization /in <matrix.csv> /infer <min/avg, default=min> [/out <normalized.csv>]")>
    <Group(CLIGroups.LabelFreeTool)>
    Public Function MatrixNormalize(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim infer$ = args("/infer") Or "min"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.normalized.csv"
        Dim matrix As DataSet() = DataSet.LoadDataSet([in]).ToArray
        Dim method As InferMethods

        If infer.TextEquals("min") Then
            method = InferMethods.Min
        Else
            method = InferMethods.Average
        End If

        matrix = matrix _
            .SimulateMissingValues(byRow:=False, infer:=method) _
            .TotalSumNormalize _
            .ToArray

        Return matrix.SaveTo(out).CLICode
    End Function
End Module
