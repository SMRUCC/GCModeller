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
'  // VERSION:   1.0.0.0
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
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
'  /Solve:                      solve a FBA model from a specific (SBML) model file.
'  /solve.gcmarkup:             
'  /Solver.KEGG:                
'  /Solver.rFBA:                
'  /visual.kegg.pathways:       
'  compile:                     Compile data source into a model file so that the fba program can using
'                               the data to performing the simulation calculation.
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

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As FBA
          Return New FBA(App:=directory & "/" & FBA.App)
     End Function

''' <summary>
''' ```
''' /Analysis.Phenotype /in &lt;MetaCyc.Sbml> /reg &lt;footprints.csv> /obj &lt;list/path/module-xml> [/obj-type &lt;lst/pathway/module> /params &lt;rfba.parameters.xml> /stat &lt;stat.Csv> /sample &lt;sampleTable.csv> /modify &lt;locus_modify.csv> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function rFBABatch([in] As String, reg As String, obj As String, Optional obj_type As String = "", Optional params As String = "", Optional stat As String = "", Optional sample As String = "", Optional modify As String = "", Optional out As String = "") As Integer
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
''' ```
''' /disruptions /model &lt;virtualcell.gcmarkup> [/parallel &lt;num_threads, default=1> /out &lt;output.directory>]
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
''' ```
''' /Flux.Coefficient /in &lt;rFBA.result_dumpDIR> [/footprints &lt;footprints.csv> /out &lt;outCsv> /spcc /KEGG]
''' ```
''' </summary>
'''
Public Function FluxCoefficient([in] As String, Optional footprints As String = "", Optional out As String = "", Optional spcc As Boolean = False, Optional kegg As Boolean = False) As Integer
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
''' ```
''' /Flux.KEGG.Filter /in &lt;flux.csv> /model &lt;MetaCyc.sbml> [/out &lt;out.csv>]
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
''' ```
''' /Func.Coefficient /func &lt;objfunc_matrix.csv> /in &lt;rFBA.result_dumpDIR> [/footprints &lt;footprints.csv> /out &lt;outCsv> /spcc]
''' ```
''' </summary>
'''
Public Function FuncCoefficient(func As String, [in] As String, Optional footprints As String = "", Optional out As String = "", Optional spcc As Boolean = False) As Integer
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
''' ```
''' /gcFBA.Batch /model &lt;model.sbml> /phenotypes &lt;KEGG_modules/pathways.DIR> /footprints &lt;footprints.csv> [/obj-type &lt;pathway/module> /params &lt;rfba.parameters.xml> /stat &lt;RPKM-stat.Csv> /sample &lt;sampleTable.csv> /modify &lt;locus_modify.csv> /out &lt;outDIR> /parallel &lt;2>]
''' ```
''' </summary>
'''
Public Function PhenotypeAnalysisBatch(model As String, phenotypes As String, footprints As String, Optional obj_type As String = "", Optional params As String = "", Optional stat As String = "", Optional sample As String = "", Optional modify As String = "", Optional out As String = "", Optional parallel As String = "") As Integer
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
''' ```
''' /heatmap /x &lt;matrix.csv> [/out &lt;out.tiff> /name &lt;Name> /width &lt;8000> /height &lt;6000>]
''' ```
''' Draw heatmap from the correlations between the genes and the metabolism flux.
''' </summary>
'''
Public Function Heatmap(x As String, Optional out As String = "", Optional name As String = "", Optional width As String = "", Optional height As String = "") As Integer
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
''' ```
''' /heatmap.scale /x &lt;matrix.csv> [/factor 30 /out &lt;out.csv>]
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
''' ```
''' /Imports /in &lt;sbml.xml>
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
''' ```
''' /phenos.MAT /in &lt;inDIR> [/out &lt;outcsv>]
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
''' ```
''' /phenos.out.Coefficient /gene &lt;samplesCopy.RPKM.csv> /pheno &lt;samples.phenos_out.csv> [/footprints &lt;footprints.csv> /out &lt;out.csv> /spcc]
''' ```
''' 2. Coefficient of the genes with the metabolism fluxs from the batch analysis result.
''' </summary>
'''
Public Function PhenosOUTCoefficient(gene As String, pheno As String, Optional footprints As String = "", Optional out As String = "", Optional spcc As Boolean = False) As Integer
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
''' ```
''' /phenos.out.MAT /in &lt;inDIR> /samples &lt;sampleTable.csv> [/out &lt;outcsv> /model &lt;MetaCyc.sbml>]
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
''' ```
''' /solve -i &lt;sbml_file> -o &lt;output_result_dir> -d &lt;max/min> [-m &lt;sbml/model> -f &lt;object_function> -knock_out &lt;gene_id_list>]
''' ```
''' solve a FBA model from a specific (SBML) model file.
''' </summary>
'''
Public Function Solve(i As String, o As String, d As String, Optional m As String = "", Optional f As String = "", Optional knock_out As String = "") As Integer
    Dim CLI As New StringBuilder("/solve")
    Call CLI.Append(" ")
    Call CLI.Append("-i " & """" & i & """ ")
    Call CLI.Append("-o " & """" & o & """ ")
    Call CLI.Append("-d " & """" & d & """ ")
    If Not m.StringEmpty Then
            Call CLI.Append("-m " & """" & m & """ ")
    End If
    If Not f.StringEmpty Then
            Call CLI.Append("-f " & """" & f & """ ")
    End If
    If Not knock_out.StringEmpty Then
            Call CLI.Append("-knock_out " & """" & knock_out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /solve.gcmarkup /model &lt;model.GCMarkup> [/mute &lt;locus_tags.txt/list> /trim /objective &lt;flux_names.txt> /out &lt;out.txt>]
''' ```
''' </summary>
'''
Public Function SolveGCMarkup(model As String, Optional mute As String = "", Optional objective As String = "", Optional out As String = "", Optional trim As Boolean = False) As Integer
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
''' ```
''' /Solver.KEGG /in &lt;model.xml> /objs &lt;locus.txt> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
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
''' ```
''' /Solver.rFBA /in &lt;MetaCyc.Sbml> /reg &lt;footprints.csv> /obj &lt;object_function.txt/xml> [/obj-type &lt;lst/pathway/module> /params &lt;rfba.parameters.xml> /stat &lt;stat.Csv> /sample &lt;sampleName> /modify &lt;locus_modify.csv> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function AnalysisPhenotype([in] As String, reg As String, obj As String, Optional obj_type As String = "", Optional params As String = "", Optional stat As String = "", Optional sample As String = "", Optional modify As String = "", Optional out As String = "") As Integer
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
''' ```
''' /visual.kegg.pathways /model &lt;virtualCell.GCMarkup> /maps &lt;kegg_maps.repo.directory> [/gene &lt;default=red> /plasmid.highlight &lt;default=blue> /out &lt;directory>]
''' ```
''' </summary>
'''
Public Function VisualKEGGPathways(model As String, maps As String, Optional gene As String = "red", Optional plasmid_highlight As String = "blue", Optional out As String = "") As Integer
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

''' <summary>
''' ```
''' compile -i &lt;input_file> -o &lt;output_file> [-if &lt;sbml/metacyc> -of &lt;fba/fba2> -f &lt;objective_function> -d &lt;max/min>]
''' ```
''' Compile data source into a model file so that the fba program can using the data to performing the simulation calculation.
''' </summary>
'''
Public Function Compile(i As String, o As String, Optional [if] As String = "", Optional [of] As String = "", Optional f As String = "", Optional d As String = "") As Integer
    Dim CLI As New StringBuilder("compile")
    Call CLI.Append(" ")
    Call CLI.Append("-i " & """" & i & """ ")
    Call CLI.Append("-o " & """" & o & """ ")
    If Not [if].StringEmpty Then
            Call CLI.Append("-if " & """" & [if] & """ ")
    End If
    If Not [of].StringEmpty Then
            Call CLI.Append("-of " & """" & [of] & """ ")
    End If
    If Not f.StringEmpty Then
            Call CLI.Append("-f " & """" & f & """ ")
    End If
    If Not d.StringEmpty Then
            Call CLI.Append("-d " & """" & d & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
