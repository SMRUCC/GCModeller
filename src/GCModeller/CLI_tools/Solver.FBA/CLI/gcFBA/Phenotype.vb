#Region "Microsoft.VisualBasic::67fd862782e6a9bf064f720e2431f4d2, CLI_tools\Solver.FBA\CLI\gcFBA\Phenotype.vb"

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
    '     Function: (+2 Overloads) __getObjectives, __out, AnalysisPhenotype, FluxCoefficient, FuncCoefficient
    '               KEGGFilter, rFBABatch
    '     Class rFBADump
    ' 
    '         Properties: FluxKEGG, FluxResult, Models, Modifier, Objectives
    '                     outDIR, Parameters, RPKM, RPKMStat, SampleTable
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.FBA_DP
Imports SMRUCC.genomics.Analysis.FBA_DP.DocumentFormat
Imports SMRUCC.genomics.Analysis.FBA_DP.FBA_OUTPUT
Imports SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Model.SBML.ExportServices.Intersection
Imports SMRUCC.genomics.Model.SBML.Level2

Partial Module CLI

    ''' <summary>
    ''' 对一个指定的性状计算出sampleTable里面的所有的sample条件下的变化
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Analysis.Phenotype",
               Usage:="/Analysis.Phenotype /in <MetaCyc.Sbml> /reg <footprints.csv> /obj <list/path/module-xml> [/obj-type <lst/pathway/module> /params <rfba.parameters.xml> /stat <stat.Csv> /sample <sampleTable.csv> /modify <locus_modify.csv> /out <outDIR>]")>
    Public Function rFBABatch(args As CommandLine) As Integer
        Dim inModel As String = args("/in")
        Dim regs As String = args("/reg")
        Dim obj As String = args("/obj")
        Dim modify As String = args("/modify")
        Dim out As String = args.GetValue("/out", inModel.TrimSuffix & ".rFBA/")
        Dim model As XmlFile = XmlFile.Load(inModel)
        Dim footprints As List(Of RegulatesFootprints) = regs.LoadCsv(Of RegulatesFootprints)
        Dim objectives As FBA_OUTPUT.ObjectiveFunction =
            __getObjectives(obj, args.GetValue("/obj-type", "lst"))
        Dim param As String = args("/params")
        Dim paramValue As rFBA_ARGVS
        If param.FileExists Then
            paramValue = param.LoadXml(Of rFBA_ARGVS)
        Else
            paramValue = New rFBA_ARGVS
        End If

        Dim sampleName As String = args("/sample")
        Dim sampleTable = sampleName.LoadCsv(Of SampleTable)
        Dim stat As String = args("/stat")
        Dim rpkm As List(Of ExprStats) = stat.LoadCsv(Of ExprStats)
        Dim rpkmSets As Dictionary(Of String, RPKMStat) =
            New Dictionary(Of String, RPKMStat)
        Dim phenoOUT As Dictionary(Of String, PhenoOUT) =
            New Dictionary(Of String, PhenoOUT)
        Dim modelDumps As Dictionary(Of String, PhenoModel) =
            New Dictionary(Of String, PhenoModel)
        Dim solver As New FBAlpRSolver(GCModeller.FileSystem.GetR_HOME)

        For Each sample As SampleTable In sampleTable
            Dim script As String = ""
            Dim result As lpOUT = Nothing
            Dim modifier As Dictionary(Of String, Double) =
                modify.LoadCsv(Of Modifier).ToDictionary(Function(x) x.locus,
                                                         Function(x) x.modify)

            Dim lpModel As New rFBAMetabolism(model, footprints, paramValue)
            lpModel.GeneFactors = modifier
            lpModel.SetObjectiveGenes(objectives.Associates)
            lpModel.SetRPKM(rpkm, sample.sampleName)
            result = solver.RSolving(lpModel, script)

            For Each gene In lpModel.GeneFactors
                If Not rpkmSets.ContainsKey(gene.Key) Then
                    Call rpkmSets.Add(gene.Key, New RPKMStat With {.Locus = gene.Key})
                End If
                Call rpkmSets(gene.Key).Properties.Add(sample.sampleName, gene.Value)
            Next

            Call __getObjectives(result.Objective, lpModel, objectives, sample.sampleName)

            Dim csvData As TabularOUT() = result.CreateDataFile(lpModel)
            For Each flux In csvData
                If Not phenoOUT.ContainsKey(flux.Rxn) Then
                    Call phenoOUT.Add(flux.Rxn, New PhenoOUT With {.Rxn = flux.Rxn})
                End If
                Call phenoOUT(flux.Rxn).Properties.Add(sample.sampleName, flux.Flux)
            Next

            Call script.SaveTo(out & $"/rFBA-{sample.sampleName}.R")

            Dim statModel = lpModel.DumpModel
            For Each flux In statModel
                If Not modelDumps.ContainsKey(flux.ReactionId) Then
                    Call modelDumps.Add(flux.ReactionId, New PhenoModel(flux))
                End If

                Dim outFlux = modelDumps(flux.ReactionId)
                outFlux.AddLowerBound(sample.sampleName, flux.LOWER_BOUND)
                outFlux.AddUpperBound(sample.sampleName, flux.UPPER_BOUND)
            Next
        Next

        ' 保存所有的输入以及输出结果，以方便进行下游的表型分析

        Dim DIR As New rFBADump(out)

        Call rpkmSets.Values.SaveTo(DIR.RPKMStat)
        Call objectives.SaveAsXml(DIR.Objectives)
        Call modelDumps.Values.SaveTo(DIR.Models)
        Call phenoOUT.Values.SaveTo(DIR.FluxResult)
        Call sampleTable.SaveTo(DIR.SampleTable)
        Call paramValue.SaveAsXml(DIR.Parameters)
        Call PathExtensions.SafeCopyTo(modify, DIR.Modifier)
        Call rpkm.SaveTo(DIR.RPKM)

        Return 0
    End Function

    Public Class rFBADump

        Public ReadOnly Property outDIR As String

        Sub New(out As String)
            outDIR = out

            Me.RPKMStat = out & "/RPKM.Stat.Csv"
            Me.Objectives = out & "/ObjFunc.Xml"
            Me.Models = out & "/Models.Csv"
            Me.FluxResult = out & "/Flux.Csv"
            Me.FluxKEGG = out & "/Flux.KEGG.Csv"
            Me.SampleTable = out & "/SampleTable.Csv"
            Me.Parameters = out & "/rFBA.Params.Xml"
            Me.Modifier = out & "/Modifier.Csv"
            Me.RPKM = out & "/RPKM.Csv"
        End Sub

        ''' <summary>
        ''' 转录组的输入数据
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RPKMStat As String
        Public ReadOnly Property Objectives As String
        Public ReadOnly Property Models As String
        Public ReadOnly Property FluxResult As String
        Public ReadOnly Property FluxKEGG As String
        Public ReadOnly Property SampleTable As String
        Public ReadOnly Property Parameters As String
        Public ReadOnly Property Modifier As String
        ''' <summary>
        ''' 来自于DeSeq工具的计算出来的归一化的数据，包含有等级映射和状态的枚举信息
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RPKM As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    ''' <summary>
    ''' 计算出突变对表型的相关度，应该计算一次野生型和突变型的数据
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Solver.rFBA",
               Usage:="/Solver.rFBA /in <MetaCyc.Sbml> /reg <footprints.csv> /obj <object_function.txt/xml> [/obj-type <lst/pathway/module> /params <rfba.parameters.xml> /stat <stat.Csv> /sample <sampleName> /modify <locus_modify.csv> /out <outDIR>]")>
    <Argument("/obj-type", True,
                   Description:="The input document type of the objective function, default is a gene_locus list in a text file, alternative format can be KEGG pathway xml and KEGG module xml.")>
    Public Function AnalysisPhenotype(args As CommandLine) As Integer
        Dim inModel As String = args("/in")
        Dim regs As String = args("/reg")
        Dim obj As String = args("/obj")
        Dim modify As String = args("/modify")
        Dim out As String = args.GetValue("/out", inModel.TrimSuffix & ".rFBA/")
        Dim model As XmlFile = XmlFile.Load(inModel)
        Dim footprints = regs.LoadCsv(Of RegulatesFootprints)
        Dim objectives As FBA_OUTPUT.ObjectiveFunction = __getObjectives(obj, args.GetValue("/obj-type", "lst"))
        Dim param As String = args("/params")
        Dim paramValue As rFBA_ARGVS
        If param.FileExists Then
            paramValue = param.LoadXml(Of rFBA_ARGVS)
        Else
            paramValue = New rFBA_ARGVS
        End If
        Dim lpModel As New rFBAMetabolism(model, footprints, paramValue)
        Dim sampleName As String = args("/sample")
        Dim stat As String = args("/stat")
        lpModel.GeneFactors = modify.LoadCsv(Of Modifier).ToDictionary(Function(x) x.locus, Function(x) x.modify)
        lpModel.SetObjectiveGenes(objectives.Associates)

        If stat.FileExists Then
            Dim rpkm = stat.LoadCsv(Of DESeq2.ExprStats)
            Call lpModel.SetRPKM(rpkm, sampleName)

            Dim rpkmSets = lpModel.GeneFactors.Select(
                Function(x) New RPKMStat With {
                    .Locus = x.Key,
                    .Properties = New Dictionary(Of String, Double) From {
                        {sampleName, x.Value}
                    }})
            Call rpkmSets.SaveTo(out & "/RPKM.Stat.Csv")
        End If

        Call lpModel.DumpModel.SaveTo(out & "/Models.Csv")

        Dim solver As New FBAlpRSolver(GCModeller.FileSystem.GetR_HOME)
        Dim script As String = ""
        Dim result = solver.RSolving(lpModel, script)
        Dim csvData As TabularOUT() = result.CreateDataFile(lpModel)

        Call script.SaveTo(out & "/rFBA.R")
        Call csvData.SaveTo(out & "/Flux.Csv")
        Call __getObjectives(result.Objective, lpModel, objectives, sampleName).SaveAsXml(out & "/ObjFunc.Xml")

        Return 0
    End Function

    Private Function __getObjectives(value As String, model As lpSolveRModel, obj As FBA_OUTPUT.ObjectiveFunction, sample As String) As FBA_OUTPUT.ObjectiveFunction
        obj.Add(sample, Val(value))
        obj.Factors = model.Objectives
        Return obj
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="type">lst/pathway/module</param>
    ''' <returns></returns>
    Private Function __getObjectives(file As String, type As String) As FBA_OUTPUT.ObjectiveFunction
        Dim objective As New FBA_OUTPUT.ObjectiveFunction

        Select Case type.ToLower
            Case "lst"
                GoTo PLANT
            Case "pathway", "pwy"
                Dim pathway = file.LoadXml(Of bGetObject.Pathway)
                objective.Comments = "Assigned from KEGG pathway " & pathway.EntryId
                objective.Associates = pathway.GetPathwayGenes
                objective.Name = pathway.Name
                objective.Info = pathway.Description
            Case "module", "mod"
                Dim mods = file.LoadXml(Of bGetObject.Module)
                objective.Comments = "Assigned from KEGG Module " & mods.EntryId
                objective.Associates = mods.GetPathwayGenes
                objective.Name = mods.Name
                objective.Info = mods.Description
            Case Else
                Call $"Unable to determine objective type:  {type}, using list default".__DEBUG_ECHO
PLANT:          objective.Associates = file.ReadAllLines
                objective.Comments = "Plant Assigned"
                objective.Name = file.BaseName
                objective.Info = file
        End Select

        Return objective
    End Function

    ''' <summary>
    ''' 计算调控因子和代谢过程相关性，从单个计算结果之中进行分析
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Flux.Coefficient",
               Usage:="/Flux.Coefficient /in <rFBA.result_dumpDIR> [/footprints <footprints.csv> /out <outCsv> /spcc /KEGG]")>
    Public Function FluxCoefficient(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim spcc As Boolean = args.GetBoolean("/spcc")
        Dim footprintFile As String = args("/footprints")
        Dim out As String = args.GetValue("/out", inDIR & "/" & __out(spcc, args.GetBoolean("/KEGG"), footprintFile))
        Dim DIR As New rFBADump(inDIR)
        Dim footprints = footprintFile.LoadCsv(Of RegulatesFootprints)
        Dim flxus As List(Of PhenoOUT) = If(args.GetBoolean("/KEGG"), DIR.FluxKEGG, DIR.FluxResult).LoadCsv(Of PhenoOUT)
        Dim sampleTable = DIR.SampleTable.LoadCsv(Of SampleTable)
        Dim rpkm As List(Of ExprStats) = DIR.RPKM.LoadCsv(Of ExprStats)

        If Not footprints.IsNullOrEmpty Then
            Dim TF As String() = footprints.Where(Function(x) Not String.IsNullOrEmpty(x.Regulator)).Select(
                Function(x) x.Regulator).Distinct.ToArray
            rpkm = (From x As ExprStats In rpkm
                    Where Array.IndexOf(TF, x.locus) > -1
                    Select x).AsList
        End If

        Dim result As RPKMStat() = PhenoCoefficient.Coefficient(flxus, rpkm, sampleTable, Not spcc)
        Return result.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 目标函数和基因之间的相关性，由于这里的目标函数的结果是从批量计算的结果之中所导出合并的一个矩阵，故而输入的基因表达量和实验用的sampleTable都是一样的，
    ''' 所以这里的/in参数的作用是得到计算相关性的基因的表达的数据，用哪一个文件夹的数据都无所谓
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Func.Coefficient",
               Usage:="/Func.Coefficient /func <objfunc_matrix.csv> /in <rFBA.result_dumpDIR> [/footprints <footprints.csv> /out <outCsv> /spcc]")>
    Public Function FuncCoefficient(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim spcc As Boolean = args.GetBoolean("/spcc")
        Dim footprintFile As String = args("/footprints")
        Dim out As String = args("/out") Or (args("/func").TrimSuffix & "." & __out(spcc, False, footprintFile))
        Dim DIR As New rFBADump(inDIR)
        Dim footprints = footprintFile.LoadCsv(Of RegulatesFootprints)  ' footprints的作用是用来筛选出调控因子
        Dim flxus As List(Of RPKMStat) = args("/func").LoadCsv(Of RPKMStat)
        Dim sampleTable = DIR.SampleTable.LoadCsv(Of SampleTable)
        Dim rpkm As List(Of ExprStats) = DIR.RPKM.LoadCsv(Of ExprStats)

        If Not footprints.IsNullOrEmpty Then
            Dim TF As String() = footprints.Where(Function(x) Not String.IsNullOrEmpty(x.Regulator)).Select(
                Function(x) x.Regulator).Distinct.ToArray
            rpkm = (From x As ExprStats In rpkm
                    Where Array.IndexOf(TF, x.locus) > -1
                    Select x).AsList
        End If

        Dim result As RPKMStat() = PhenoCoefficient.Coefficient(flxus, rpkm, sampleTable, Not spcc)
        Return result.SaveTo(out, Encodings.ASCII).CLICode
    End Function

    Private Function __out(spcc As String, kegg As Boolean, footprints As String) As String
        Dim file As String = "Phenotypes"
        If spcc Then
            file &= ".sPCC"
        Else
            file &= ".PCC"
        End If
        If kegg Then
            file &= ".KEGG"
        End If
        If Not String.IsNullOrEmpty(footprints) Then
            file &= "." & footprints.BaseName
        End If
        file &= ".csv"
        Return file
    End Function

    <ExportAPI("/Flux.KEGG.Filter", Usage:="/Flux.KEGG.Filter /in <flux.csv> /model <MetaCyc.sbml> [/out <out.csv>]")>
    Public Function KEGGFilter(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".KEGG.Csv")
        Dim model As XmlFile = XmlFile.Load(args("/model"))
        Dim fluxs As List(Of PhenoOUT) = inFile.LoadCsv(Of PhenoOUT)
        Dim KEGGs = LoadReactions(GCModeller.FileSystem.KEGG.Directories.GetReactions)
        Dim intSets = model.KEGGReactions(KEGGs)
        Dim fluxHash = fluxs.ToDictionary
        Dim LQuery As PhenoOUT() = (From x In intSets Where fluxHash.ContainsKey(x.id) Select fluxHash(x.id)).ToArray
        Dim maps = intSets.Select(Function(x) New KeyValuePair(x.id, x.Notes.Text))
        Dim outMaps As String = out.TrimSuffix & ".KEGG_Maps.Xml"
        Call maps.SaveAsXml(outMaps)
        Return LQuery.SaveTo(out).CLICode
    End Function

    ' 已经有heatmap来代替这个方法了

    '<ExportAPI("/Draw.Coefficient", Usage:="/Draw.Coefficient /in <coefficient.csv> /model <metacyc.sbml> /mods <mods.DIR> [/out <outImage.png>]")>
    'Public Function CoefficientDraw(args As CommandLine) As Integer
    '    Dim inModel As String = args("/model")
    '    Dim model As XmlFile = XmlFile.Load(inModel)
    '    Dim mods As String = args("/mods")
    '    Dim res As Image = FluxVisual.DrawingModule(args("/in"), model, mods)
    '    Dim out As String = args.GetValue("/out", args("/in").TrimFileExt & "CoefficientDraw.png")
    '    Return res.SaveAs(out, ImageFormats.Png).CLICode
    'End Function
End Module
