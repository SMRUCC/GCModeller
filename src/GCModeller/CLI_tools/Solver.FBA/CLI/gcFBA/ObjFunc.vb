#Region "Microsoft.VisualBasic::4fee03fe9ceae94a9e283fa0339f3aaf, CLI_tools\Solver.FBA\CLI\gcFBA\ObjFunc.vb"

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
    '     Function: ObjMAT, PhenoOUT_MAT, PhenosOUTCoefficient
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.FBA_DP
Imports SMRUCC.genomics.Analysis.FBA_DP.DocumentFormat
Imports SMRUCC.genomics.Analysis.FBA_DP.FBA_OUTPUT
Imports SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2
Imports SMRUCC.genomics.Model.SBML.ExportServices.Intersection
Imports SMRUCC.genomics.Model.SBML.Level2

Partial Module CLI

    ''' <summary>
    ''' 将Object Function的结果合并为一个矩阵
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/phenos.MAT",
               Info:="Merges the objective function result as a Matrix. For calculation the coefficient of the genes with the phenotype objective function.",
               Usage:="/phenos.MAT /in <inDIR> [/out <outcsv>]")>
    Public Function ObjMAT(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & "/" & inDIR.BaseName & "." & NameOf(ObjMAT) & ".csv")
        Dim files = (From path As String
                     In FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchAllSubDirectories, "ObjFunc.Xml")
                     Select func = path.LoadXml(Of ObjectiveFunction),
                         path.ParentDirName)
        Dim MAT = (From x In files
                   Select New RPKMStat With {
                       .Locus = x.ParentDirName,
                       .Properties = x.func.Result.ToDictionary(
                            Function(key) key.Key,
                            Function(val) val.Value)}).ToArray
        Return MAT.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 将Object Function的结果合并为一个矩阵
    ''' </summary>
    ''' <param name="args">假设所有的实验都是相同的数据的基础上面做出来的，唯一的区别是目标函数是不同的表型</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/phenos.out.MAT",
               Info:="1. Merge flux.csv result as a Matrix, for the calculation of the coefficient of the genes with the metabolism flux.",
               Usage:="/phenos.out.MAT /in <inDIR> /samples <sampleTable.csv> [/out <outcsv> /model <MetaCyc.sbml>]")>
    Public Function PhenoOUT_MAT(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & "/" & inDIR.BaseName & "." & NameOf(PhenoOUT_MAT) & ".csv")
        Dim files = (From path As String
                     In FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchAllSubDirectories, "Flux.Csv")
                     Select func = path.LoadCsv(Of PhenoOUT),
                         path.ParentDirName)
        Dim sampleTable = args("/samples").LoadCsv(Of SampleTable)

        Dim MAT As New Dictionary(Of String, RPKMStat)
        Dim samples As New List(Of KeyValuePair)

        For Each obj In files
            For Each sample As SampleTable In sampleTable

                Dim sampleName As String = sample.sampleName & "." & obj.ParentDirName

                For Each func As PhenoOUT In obj.func
                    If Not MAT.ContainsKey(func.Rxn) Then
                        Dim val As New RPKMStat With {
                            .Locus = func.Rxn,
                            .Properties = New Dictionary(Of String, Double)
                        }
                        Call MAT.Add(func.Rxn, val)
                    End If

                    Call MAT(func.Rxn).Properties.Add(sampleName, func.Properties(sample.sampleName))
                Next

                Call samples.Add(New KeyValuePair(sample.sampleName, sampleName))

            Next
        Next

        '  复制基因的表达以方便后面计算相关性
        Dim RPKMFile = (From path As String
                        In FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchAllSubDirectories, "RPKM.Csv")
                        Select path).FirstOrDefault
        Dim RPKM = RPKMFile.LoadCsv(Of ExprStats)
        Dim genes As New Dictionary(Of String, RPKMStat)

        For Each gene In RPKM

            Dim gExpr As New RPKMStat With {
                .Locus = gene.locus,
                .Properties = New Dictionary(Of String, Double)
            }

            Call genes.Add(gene.locus, gExpr)

            For Each sample In samples

                Call gExpr.Properties.Add(sample.Value, gene.GetLevel(sample.Key))

            Next
        Next

        Call genes.Values.SaveTo(out.TrimSuffix & ".genes_RPKM.csv")

        Dim model As String = args("/model")

        If model.FileExists Then
            Dim sbml As XmlFile = XmlFile.Load(model)
            Dim KEGGs = LoadReactions(GCModeller.FileSystem.KEGG.Directories.GetReactions)
            Dim intSets = sbml.KEGGReactions(KEGGs)
            Dim LQuery As RPKMStat() = (From x As Elements.Reaction In intSets Where MAT.ContainsKey(x.id) Select MAT(x.id)).ToArray
            Dim maps = intSets.Select(Function(x) New KeyValuePair(x.id, x.Notes.Text))
            Dim outMaps As String = out.TrimSuffix & ".KEGG_Maps.Xml"
            Call maps.SaveAsXml(outMaps)
            Return LQuery.SaveTo(out).CLICode
        Else
            Return MAT.Values.SaveTo(out).CLICode
        End If
    End Function

    <ExportAPI("/phenos.out.Coefficient",
               Info:="2. Coefficient of the genes with the metabolism fluxs from the batch analysis result.",
               Usage:="/phenos.out.Coefficient /gene <samplesCopy.RPKM.csv> /pheno <samples.phenos_out.csv> [/footprints <footprints.csv> /out <out.csv> /spcc]")>
    Public Function PhenosOUTCoefficient(args As CommandLine) As Integer
        Dim genes As String = args("/gene")
        Dim pheno As String = args("/pheno")
        Dim footprint As String = args("/footprints")
        Dim spcc As Boolean = args.GetBoolean("/spcc")
        Dim out As String = args.GetValue("/out", pheno.TrimSuffix & "." & __out(spcc, False, footprint))

        Dim RPKMs = genes.LoadCsv(Of RPKMStat)
        Dim phenos = pheno.LoadCsv(Of RPKMStat)
        Dim footprints = footprint.LoadCsv(Of RegulatesFootprints)

        If Not footprints.IsNullOrEmpty Then
            Dim TF As String() = footprints.Where(Function(x) Not String.IsNullOrEmpty(x.Regulator)).Select(
                Function(x) x.Regulator).Distinct.ToArray
            RPKMs = (From x As RPKMStat In RPKMs
                     Where Array.IndexOf(TF, x.Locus) > -1
                     Select x).AsList
        End If

        Return phenos.Coefficient(RPKMs, Not spcc).SaveTo(out, Encodings.ASCII).CLICode
    End Function
End Module
