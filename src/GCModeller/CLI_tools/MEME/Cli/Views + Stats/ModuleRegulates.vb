Imports System.Text
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic
Imports RDotNET.Extensions.Bioinformatics.VennDiagram.ModelAPI

Partial Module CLI

    <ExportAPI("--site.stat",
             Info:="Statics of the PCC correlation distribution of the regulation",
             Usage:="--site.stat /in <footprints.csv> [/out <out.csv>]")>
    Public Function SiteStat(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim outFile As String = args.GetValue("/out", inFile.TrimFileExt & ".siteStat.csv")
        Dim raw = inFile.LoadCsv(Of PredictedRegulationFootprint)

        Dim Total As Integer = raw.Count
        raw = (From site In raw.AsParallel Where Not String.IsNullOrEmpty(site.Regulator) Select site).ToList
        Dim Regulates As Integer = raw.Count
        Dim dict As SortedDictionary(Of Double, PredictedRegulationFootprint()) =
            New SortedDictionary(Of Double, PredictedRegulationFootprint())

        raw = (From site In raw.AsParallel Where site.Pcc <> 1.0R Select site).ToList

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

        Dim doc As New DocumentStream.File

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
    <ParameterInfo("/in", False,
                   Description:="The footprints data required of fill out the pathway Class, category and type information before you call this function.
                   If the fields is blank, then your should specify the /mods parameter.")>
    Public Function ModuleRegulates(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimFileExt)
        Dim regulations = inFile.LoadCsv(Of PredictedRegulationFootprint)
        Dim modsDIR As String = args("/mods")

        If modsDIR.DirectoryExists Then
            Dim modsInfo = BriteHEntry.ModuleClassAPI.FromModules(modsDIR)
            regulations = modsInfo.Fill(regulations).ToList
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
                For Each cat In (From row In ccls.Group Select row Group row By row.Category Into Group)
                    Dim file As String = DIR & $"/{BriteHEntry.Module.TrimPath(cat.Category)}.csv"
                    Call cat.Group.SaveTo(file)
                    Call lstRegulators.Add(cat.Group.ToArray(Function(x) x.Regulator))
                Next
            Next

            Call modRegulators.Add(cls.Type, lstRegulators.Distinct.ToList)
        Next

        Dim removes = (From x In modRegulators Where StringHelpers.IsNullOrEmpty(x.Value) Select x.Key)
        For Each nameMode As String In removes
            Call modRegulators.Remove(nameMode)
        Next

        Dim venn = __modsVenn(modRegulators, regulations.ToArray(Function(r) r.Type).Distinct.ToArray)
        venn.Title = "KEGG Modules Regulations Compares"
        venn.saveTiff = out & "/kMod.venn.tiff"
        venn.SaveTo(out & "/kMod.venn.R")
        Call vennSaveCommon(out & "/ModsRegulatorViews.csv", modRegulators)

        Return 0
    End Function
End Module