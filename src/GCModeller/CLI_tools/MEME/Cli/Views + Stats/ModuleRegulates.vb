#Region "Microsoft.VisualBasic::769d9140536557832048b2b395640d05, CLI_tools\MEME\Cli\Views + Stats\ModuleRegulates.vb"

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
    '     Function: __modsVenn, ModuleRegulates, SiteStat
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Math
Imports RDotNET.Extensions.Bioinformatics.VennDiagram.ModelAPI
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat

Partial Module CLI

    <ExportAPI("--site.stat",
             Info:="Statics of the PCC correlation distribution of the regulation",
             Usage:="--site.stat /in <footprints.csv> [/out <out.csv>]")>
    Public Function SiteStat(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim outFile As String = args.GetValue("/out", inFile.TrimSuffix & ".siteStat.csv")
        Dim raw = inFile.LoadCsv(Of PredictedRegulationFootprint)

        Dim Total As Integer = raw.Count
        raw = (From site In raw.AsParallel Where Not String.IsNullOrEmpty(site.Regulator) Select site).AsList
        Dim Regulates As Integer = raw.Count
        Dim dict As SortedDictionary(Of Double, PredictedRegulationFootprint()) =
            New SortedDictionary(Of Double, PredictedRegulationFootprint())

        raw = (From site In raw.AsParallel Where site.Pcc <> 1.0R Select site).AsList

        For p As Double = 0.6 To 1 Step 0.05
            Dim nex = p + 0.05
            Dim pp As Double = p
            Dim n = (From site In raw.AsParallel Where site.Pcc.RangesAt(pp, nex) Select site).ToArray
            Call dict.Add(p, n)
        Next

        For p As Double = -0.6 To -1 Step -0.05
            Dim nex = p - 0.05
            Dim pp As Double = p
            Dim n = (From site In raw.AsParallel Where site.Pcc.RangesAt(nex, pp) Select site).ToArray
            Call dict.Add(p, n)
        Next

        Dim doc As New IO.File

        Call doc.Add("Total", CStr(Total))
        Call doc.Add("Regulates", CStr(Regulates))
        Call doc.Add({})
        Call doc.Add("Levels", "Numbers")

        For Each level In dict
            Call doc.Add(CStr(level.Key), CStr(level.Value.Length))
        Next

        Return doc.Save(outFile, Encoding:=Encoding.ASCII).CLICode
    End Function

    Private Function __modsVenn(Of Tcol As IEnumerable(Of String))(
                                   modRegulators As Dictionary(Of String, Tcol),
                                   types As String()) As VennDiagram

        Dim venn As New VennDiagram
        Dim MAT = VectorMapper(modRegulators, Function(x) x.ToArray)
        Dim serials As New List(Of Partition)

        For Each type As String In types
            Dim x As New Partition(type)

            If MAT.ContainsKey(type) Then
                x.Vector = MAT(type)
                Call serials.Add(x)
            Else
                Call $"{type} is empty!".__DEBUG_ECHO
            End If
        Next

        venn.partitions = serials.ToArray
        venn.RandomColors()

        Return venn
    End Function

    ''' <summary>
    ''' 需要事先已经填上了代谢途径的信息在里面
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--modules.regulates",
               Info:="Exports the Venn diagram model for the module regulations.",
               Usage:="--modules.regulates /in <virtualfootprints.csv> [/out <out.DIR> /mods <KEGG_modules.DIR>]")>
    <ArgumentAttribute("/in", False,
                   Description:="The footprints data required of fill out the pathway Class, category and type information before you call this function.
                   If the fields is blank, then your should specify the /mods parameter.")>
    Public Function ModuleRegulates(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix)
        Dim regulations = inFile.LoadCsv(Of PredictedRegulationFootprint)
        Dim modsDIR As String = args("/mods")

        If modsDIR.DirectoryExists Then
            Dim modsInfo = BriteHEntry.ModuleClassAPI.FromModules(modsDIR)
            regulations = modsInfo.Fill(regulations).AsList
        End If

        Dim Types = (From row As PredictedRegulationFootprint
                     In regulations
                     Where Not String.IsNullOrEmpty(row.Regulator)
                     Select row
                     Group row By row.Type Into Group)
        Dim [Class] = (From type In Types
                       Where Not String.IsNullOrEmpty(type.Type)
                       Select type.Type,
                           classes = (From row As PredictedRegulationFootprint
                                      In type.Group
                                      Select row
                                      Group row By cls = row.Class Into Group).ToArray)
        Dim modRegulators As Dictionary(Of String, List(Of String)) =
            New Dictionary(Of String, List(Of String))

        For Each cls In [Class]
            Dim path As String = $"{out}/{BriteHEntry.Module.TrimPath(cls.Type)}"
            Dim lstRegulators As New List(Of String)

            For Each ccls In cls.classes
                Dim DIR As String = path & $"/{BriteHEntry.Module.TrimPath(ccls.cls)}/"
                Dim groups = ccls.Group.GroupBy(Function(row) row.Category)
                For Each cat As IGrouping(Of String, PredictedRegulationFootprint) In groups
                    Dim file As String = DIR & $"/{BriteHEntry.Module.TrimPath(cat.Key)}.csv"
                    Dim group = cat.ToArray

                    Call group.SaveTo(file)
                    Call lstRegulators.Add(group.Select(Function(x) x.Regulator).ToArray)
                Next
            Next

            Call modRegulators.Add(cls.Type, lstRegulators.Distinct.AsList)
        Next

        Dim removes = (From x In modRegulators Where x.Value.IsNullOrEmpty Select x.Key)
        For Each nameMode As String In removes
            Call modRegulators.Remove(nameMode)
        Next

        Dim venn = __modsVenn(modRegulators, regulations.Select(Function(r) r.Type).Distinct.ToArray)
        venn.Title = "KEGG Modules Regulations Compares"
        venn.saveTiff = out & "/kMod.venn.tiff"
        venn.SaveTo(out & "/kMod.venn.R")
        Call vennSaveCommon(out & "/ModsRegulatorViews.csv", modRegulators)

        Return 0
    End Function
End Module
