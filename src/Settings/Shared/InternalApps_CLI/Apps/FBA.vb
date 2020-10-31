Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\FBA.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   3.3277.7609.23646
'  // ASSEMBLY:  Settings, Version=3.3277.7609.23646, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright (c) SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/31/2020 1:08:12 PM
'  // 
' 
' 
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /Analysis.Phenotype:         
'  /disruptions:                
'  /Flux.Coefficient:           
'  /Flux.KEGG.Filter:           
'  /Func.Coefficient:           
'  /gcFBA.Batch:                
'  /heatmap:                    Draw heatmap from the correlations between the genes and the metabolism
'                               flux.
'  /heatmap.scale:              
'  /Imports:                    
'  /phenos.MAT:                 Merges the objective function result as a Matrix. For calculation the
'                               coefficient of the genes with the phenotype objective function.
'  /phenos.out.Coefficient:     2. Coefficient of the genes with the metabolism fluxs from the batch
'                               analysis result.
'  /phenos.out.MAT:             1. Merge flux.csv result as a Matrix, for the calculation of the coefficient
'                               of the genes with the metabolism flux.
'  /solve.gcmarkup:             
'  /Solver.KEGG:                
'  /Solver.rFBA:                
'  /visual.kegg.pathways:       
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
'''
''' </summary>
'''
Public Class FBA : Inherits InteropService

    Public Const App$ = "FBA.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As FBA
          Return New FBA(App:=directory & "/" & FBA.App)
     End Function

''' <summary>
''' ```bash
''' /Analysis.Phenotype /in &lt;MetaCyc.Sbml&gt; /reg &lt;footprints.csv&gt; /obj &lt;list/path/module-xml&gt; [/obj-type &lt;lst/pathway/module&gt; /params &lt;rfba.parameters.xml&gt; /stat &lt;stat.Csv&gt; /sample &lt;sampleTable.csv&gt; /modify &lt;locus_modify.csv&gt; /out &lt;outDIR&gt;]
''' ```
''' </summary>
'''

Public Function rFBABatch([in] As String, 
                             reg As String, 
                             obj As String, 
                             Optional obj_type As String = "", 
                             Optional params As String = "", 
                             Optional stat As String = "", 
                             Optional sample As String = "", 
                             Optional modify As String = "", 
                             Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Analysis.Phenotype")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/reg " & """" & reg & """ ")
    Call CLI.Append("/obj " & """" & obj & """ ")
    If Not obj_type.StringEmpty Then
            Call CLI.Append("/obj-type " & """" & obj_type & """ ")
    End If
    If Not params.StringEmpty Then
            Call CLI.Append("/params " & """" & params & """ ")
    End If
    If Not stat.StringEmpty Then
            Call CLI.Append("/stat " & """" & stat & """ ")
    End If
    If Not sample.StringEmpty Then
            Call CLI.Append("/sample " & """" & sample & """ ")
    End If
    If Not modify.StringEmpty Then
            Call CLI.Append("/modify " & """" & modify & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /disruptions /model &lt;virtualcell.gcmarkup&gt; [/parallel &lt;num_threads, default=1&gt; /out &lt;output.directory&gt;]
''' ```
''' </summary>
'''

Public Function SingleGeneDisruptions(model As String, Optional parallel As String = "1", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/disruptions")
    Call CLI.Append(" ")
    Call CLI.Append("/model " & """" & model & """ ")
    If Not parallel.StringEmpty Then
            Call CLI.Append("/parallel " & """" & parallel & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Flux.Coefficient /in &lt;rFBA.result_dumpDIR&gt; [/footprints &lt;footprints.csv&gt; /out &lt;outCsv&gt; /spcc /KEGG]
''' ```
''' </summary>
'''

Public Function FluxCoefficient([in] As String, 
                                   Optional footprints As String = "", 
                                   Optional out As String = "", 
                                   Optional spcc As Boolean = False, 
                                   Optional kegg As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Flux.Coefficient")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not footprints.StringEmpty Then
            Call CLI.Append("/footprints " & """" & footprints & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If spcc Then
        Call CLI.Append("/spcc ")
    End If
    If kegg Then
        Call CLI.Append("/kegg ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Flux.KEGG.Filter /in &lt;flux.csv&gt; /model &lt;MetaCyc.sbml&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function KEGGFilter([in] As String, model As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Flux.KEGG.Filter")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/model " & """" & model & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Func.Coefficient /func &lt;objfunc_matrix.csv&gt; /in &lt;rFBA.result_dumpDIR&gt; [/footprints &lt;footprints.csv&gt; /out &lt;outCsv&gt; /spcc]
''' ```
''' </summary>
'''

Public Function FuncCoefficient(func As String, 
                                   [in] As String, 
                                   Optional footprints As String = "", 
                                   Optional out As String = "", 
                                   Optional spcc As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Func.Coefficient")
    Call CLI.Append(" ")
    Call CLI.Append("/func " & """" & func & """ ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not footprints.StringEmpty Then
            Call CLI.Append("/footprints " & """" & footprints & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If spcc Then
        Call CLI.Append("/spcc ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /gcFBA.Batch /model &lt;model.sbml&gt; /phenotypes &lt;KEGG_modules/pathways.DIR&gt; /footprints &lt;footprints.csv&gt; [/obj-type &lt;pathway/module&gt; /params &lt;rfba.parameters.xml&gt; /stat &lt;RPKM-stat.Csv&gt; /sample &lt;sampleTable.csv&gt; /modify &lt;locus_modify.csv&gt; /out &lt;outDIR&gt; /parallel &lt;2&gt;]
''' ```
''' </summary>
'''

Public Function PhenotypeAnalysisBatch(model As String, 
                                          phenotypes As String, 
                                          footprints As String, 
                                          Optional obj_type As String = "", 
                                          Optional params As String = "", 
                                          Optional stat As String = "", 
                                          Optional sample As String = "", 
                                          Optional modify As String = "", 
                                          Optional out As String = "", 
                                          Optional parallel As String = "") As Integer
    Dim CLI As New StringBuilder("/gcFBA.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/model " & """" & model & """ ")
    Call CLI.Append("/phenotypes " & """" & phenotypes & """ ")
    Call CLI.Append("/footprints " & """" & footprints & """ ")
    If Not obj_type.StringEmpty Then
            Call CLI.Append("/obj-type " & """" & obj_type & """ ")
    End If
    If Not params.StringEmpty Then
            Call CLI.Append("/params " & """" & params & """ ")
    End If
    If Not stat.StringEmpty Then
            Call CLI.Append("/stat " & """" & stat & """ ")
    End If
    If Not sample.StringEmpty Then
            Call CLI.Append("/sample " & """" & sample & """ ")
    End If
    If Not modify.StringEmpty Then
            Call CLI.Append("/modify " & """" & modify & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not parallel.StringEmpty Then
            Call CLI.Append("/parallel " & """" & parallel & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /heatmap /x &lt;matrix.csv&gt; [/out &lt;out.tiff&gt; /name &lt;Name&gt; /width &lt;8000&gt; /height &lt;6000&gt;]
''' ```
''' Draw heatmap from the correlations between the genes and the metabolism flux.
''' </summary>
'''

Public Function Heatmap(x As String, 
                           Optional out As String = "", 
                           Optional name As String = "", 
                           Optional width As String = "", 
                           Optional height As String = "") As Integer
    Dim CLI As New StringBuilder("/heatmap")
    Call CLI.Append(" ")
    Call CLI.Append("/x " & """" & x & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not name.StringEmpty Then
            Call CLI.Append("/name " & """" & name & """ ")
    End If
    If Not width.StringEmpty Then
            Call CLI.Append("/width " & """" & width & """ ")
    End If
    If Not height.StringEmpty Then
            Call CLI.Append("/height " & """" & height & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /heatmap.scale /x &lt;matrix.csv&gt; [/factor 30 /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function ScaleHeatmap(x As String, Optional factor As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/heatmap.scale")
    Call CLI.Append(" ")
    Call CLI.Append("/x " & """" & x & """ ")
    If Not factor.StringEmpty Then
            Call CLI.Append("/factor " & """" & factor & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Imports /in &lt;sbml.xml&gt;
''' ```
''' </summary>
'''

Public Function ImportsRxns([in] As String) As Integer
    Dim CLI As New StringBuilder("/Imports")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /phenos.MAT /in &lt;inDIR&gt; [/out &lt;outcsv&gt;]
''' ```
''' Merges the objective function result as a Matrix. For calculation the coefficient of the genes with the phenotype objective function.
''' </summary>
'''

Public Function ObjMAT([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/phenos.MAT")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /phenos.out.Coefficient /gene &lt;samplesCopy.RPKM.csv&gt; /pheno &lt;samples.phenos_out.csv&gt; [/footprints &lt;footprints.csv&gt; /out &lt;out.csv&gt; /spcc]
''' ```
''' 2. Coefficient of the genes with the metabolism fluxs from the batch analysis result.
''' </summary>
'''

Public Function PhenosOUTCoefficient(gene As String, 
                                        pheno As String, 
                                        Optional footprints As String = "", 
                                        Optional out As String = "", 
                                        Optional spcc As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/phenos.out.Coefficient")
    Call CLI.Append(" ")
    Call CLI.Append("/gene " & """" & gene & """ ")
    Call CLI.Append("/pheno " & """" & pheno & """ ")
    If Not footprints.StringEmpty Then
            Call CLI.Append("/footprints " & """" & footprints & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If spcc Then
        Call CLI.Append("/spcc ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /phenos.out.MAT /in &lt;inDIR&gt; /samples &lt;sampleTable.csv&gt; [/out &lt;outcsv&gt; /model &lt;MetaCyc.sbml&gt;]
''' ```
''' 1. Merge flux.csv result as a Matrix, for the calculation of the coefficient of the genes with the metabolism flux.
''' </summary>
'''

Public Function PhenoOUT_MAT([in] As String, samples As String, Optional out As String = "", Optional model As String = "") As Integer
    Dim CLI As New StringBuilder("/phenos.out.MAT")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/samples " & """" & samples & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not model.StringEmpty Then
            Call CLI.Append("/model " & """" & model & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /solve.gcmarkup /model &lt;model.GCMarkup&gt; [/mute &lt;locus_tags.txt/list&gt; /trim /objective &lt;flux_names.txt&gt; /out &lt;out.txt&gt;]
''' ```
''' </summary>
'''
''' <param name="objective"> A name list of the target reaction names, which this file format should be in one line one ID. 
'''               If this argument is ignored, then a entire list of reactions that defined in the input virtual cell model will be used.
''' </param>
''' <param name="trim"> Removes all of the enzymatic reaction which could not found their corresponding enzyme in current 
'''               virtual cell model? By default is retain these reactions.
''' </param>
''' <param name="mute"> + If this parameter is a file path, then locus_tag should be one tag per line in the text file;
'''               + And this parameter is also can be a id list, which the id should seperated by comma symbol, format like: ``id1,id2,id3``.
''' </param>
Public Function SolveGCMarkup(model As String, 
                                 Optional mute As String = "", 
                                 Optional objective As String = "", 
                                 Optional out As String = "", 
                                 Optional trim As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/solve.gcmarkup")
    Call CLI.Append(" ")
    Call CLI.Append("/model " & """" & model & """ ")
    If Not mute.StringEmpty Then
            Call CLI.Append("/mute " & """" & mute & """ ")
    End If
    If Not objective.StringEmpty Then
            Call CLI.Append("/objective " & """" & objective & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If trim Then
        Call CLI.Append("/trim ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Solver.KEGG /in &lt;model.xml&gt; /objs &lt;locus.txt&gt; [/out &lt;outDIR&gt;]
''' ```
''' </summary>
'''
''' <param name="objs"> This parameter defines the objective function in the FBA solver, is a text file which contains a list of genes locus, 
'''                    and these genes locus is associated to a enzyme reaction in the FBA model.
''' </param>
Public Function KEGGSolver([in] As String, objs As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Solver.KEGG")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/objs " & """" & objs & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Solver.rFBA /in &lt;MetaCyc.Sbml&gt; /reg &lt;footprints.csv&gt; /obj &lt;object_function.txt/xml&gt; [/obj-type &lt;lst/pathway/module&gt; /params &lt;rfba.parameters.xml&gt; /stat &lt;stat.Csv&gt; /sample &lt;sampleName&gt; /modify &lt;locus_modify.csv&gt; /out &lt;outDIR&gt;]
''' ```
''' </summary>
'''
''' <param name="obj_type"> The input document type of the objective function, default is a gene_locus list in a text file, alternative format can be KEGG pathway xml and KEGG module xml.
''' </param>
Public Function AnalysisPhenotype([in] As String, 
                                     reg As String, 
                                     obj As String, 
                                     Optional obj_type As String = "", 
                                     Optional params As String = "", 
                                     Optional stat As String = "", 
                                     Optional sample As String = "", 
                                     Optional modify As String = "", 
                                     Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Solver.rFBA")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/reg " & """" & reg & """ ")
    Call CLI.Append("/obj " & """" & obj & """ ")
    If Not obj_type.StringEmpty Then
            Call CLI.Append("/obj-type " & """" & obj_type & """ ")
    End If
    If Not params.StringEmpty Then
            Call CLI.Append("/params " & """" & params & """ ")
    End If
    If Not stat.StringEmpty Then
            Call CLI.Append("/stat " & """" & stat & """ ")
    End If
    If Not sample.StringEmpty Then
            Call CLI.Append("/sample " & """" & sample & """ ")
    End If
    If Not modify.StringEmpty Then
            Call CLI.Append("/modify " & """" & modify & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /visual.kegg.pathways /model &lt;virtualCell.GCMarkup&gt; /maps &lt;kegg_maps.repo.directory&gt; [/gene &lt;default=red&gt; /plasmid.highlight &lt;default=blue&gt; /out &lt;directory&gt;]
''' ```
''' </summary>
'''
''' <param name="gene"> The color of the gene object, if this parameter is a color value. There is a special term: ``exclude``, means do not render gene color.
''' </param>
Public Function VisualKEGGPathways(model As String, 
                                      maps As String, 
                                      Optional gene As String = "red", 
                                      Optional plasmid_highlight As String = "blue", 
                                      Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/visual.kegg.pathways")
    Call CLI.Append(" ")
    Call CLI.Append("/model " & """" & model & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    If Not gene.StringEmpty Then
            Call CLI.Append("/gene " & """" & gene & """ ")
    End If
    If Not plasmid_highlight.StringEmpty Then
            Call CLI.Append("/plasmid.highlight " & """" & plasmid_highlight & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
