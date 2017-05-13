Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/FBA.exe

Namespace GCModellerApps


''' <summary>
'''
''' </summary>
'''
Public Class FBA : Inherits InteropService


Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
''' ```
''' /Analysis.Phenotype /in &lt;MetaCyc.Sbml> /reg &lt;footprints.csv> /obj &lt;list/path/module-xml> [/obj-type &lt;lst/pathway/module> /params &lt;rfba.parameters.xml> /stat &lt;stat.Csv> /sample &lt;sampleTable.csv> /modify &lt;locus_modify.csv> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function rFBABatch(_in As String, _reg As String, _obj As String, Optional _obj_type As String = "", Optional _params As String = "", Optional _stat As String = "", Optional _sample As String = "", Optional _modify As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Analysis.Phenotype")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/reg " & """" & _reg & """ ")
Call CLI.Append("/obj " & """" & _obj & """ ")
If Not _obj_type.StringEmpty Then
Call CLI.Append("/obj-type " & """" & _obj_type & """ ")
End If
If Not _params.StringEmpty Then
Call CLI.Append("/params " & """" & _params & """ ")
End If
If Not _stat.StringEmpty Then
Call CLI.Append("/stat " & """" & _stat & """ ")
End If
If Not _sample.StringEmpty Then
Call CLI.Append("/sample " & """" & _sample & """ ")
End If
If Not _modify.StringEmpty Then
Call CLI.Append("/modify " & """" & _modify & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' export -i &lt;fba_model> -o &lt;r_script>
''' ```
''' </summary>
'''
Public Function Export(_i As String, _o As String) As Integer
Dim CLI As New StringBuilder("export")
Call CLI.Append(" ")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-o " & """" & _o & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Flux.Coefficient /in &lt;rFBA.result_dumpDIR> [/footprints &lt;footprints.csv> /out &lt;outCsv> /spcc /KEGG]
''' ```
''' </summary>
'''
Public Function FluxCoefficient(_in As String, Optional _footprints As String = "", Optional _out As String = "", Optional _spcc As Boolean = False, Optional _kegg As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Flux.Coefficient")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _footprints.StringEmpty Then
Call CLI.Append("/footprints " & """" & _footprints & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _spcc Then
Call CLI.Append("/spcc ")
End If
If _kegg Then
Call CLI.Append("/kegg ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Flux.KEGG.Filter /in &lt;flux.csv> /model &lt;MetaCyc.sbml> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function KEGGFilter(_in As String, _model As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Flux.KEGG.Filter")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/model " & """" & _model & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Func.Coefficient /func &lt;objfunc_matrix.csv> /in &lt;rFBA.result_dumpDIR> [/footprints &lt;footprints.csv> /out &lt;outCsv> /spcc]
''' ```
''' </summary>
'''
Public Function FuncCoefficient(_func As String, _in As String, Optional _footprints As String = "", Optional _out As String = "", Optional _spcc As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Func.Coefficient")
Call CLI.Append(" ")
Call CLI.Append("/func " & """" & _func & """ ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _footprints.StringEmpty Then
Call CLI.Append("/footprints " & """" & _footprints & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _spcc Then
Call CLI.Append("/spcc ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /gcFBA.Batch /model &lt;model.sbml> /phenotypes &lt;KEGG_modules/pathways.DIR> /footprints &lt;footprints.csv> [/obj-type &lt;pathway/module> /params &lt;rfba.parameters.xml> /stat &lt;RPKM-stat.Csv> /sample &lt;sampleTable.csv> /modify &lt;locus_modify.csv> /out &lt;outDIR> /parallel &lt;2>]
''' ```
''' </summary>
'''
Public Function PhenotypeAnalysisBatch(_model As String, _phenotypes As String, _footprints As String, Optional _obj_type As String = "", Optional _params As String = "", Optional _stat As String = "", Optional _sample As String = "", Optional _modify As String = "", Optional _out As String = "", Optional _parallel As String = "") As Integer
Dim CLI As New StringBuilder("/gcFBA.Batch")
Call CLI.Append(" ")
Call CLI.Append("/model " & """" & _model & """ ")
Call CLI.Append("/phenotypes " & """" & _phenotypes & """ ")
Call CLI.Append("/footprints " & """" & _footprints & """ ")
If Not _obj_type.StringEmpty Then
Call CLI.Append("/obj-type " & """" & _obj_type & """ ")
End If
If Not _params.StringEmpty Then
Call CLI.Append("/params " & """" & _params & """ ")
End If
If Not _stat.StringEmpty Then
Call CLI.Append("/stat " & """" & _stat & """ ")
End If
If Not _sample.StringEmpty Then
Call CLI.Append("/sample " & """" & _sample & """ ")
End If
If Not _modify.StringEmpty Then
Call CLI.Append("/modify " & """" & _modify & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _parallel.StringEmpty Then
Call CLI.Append("/parallel " & """" & _parallel & """ ")
End If


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
Public Function Heatmap(_x As String, Optional _out As String = "", Optional _name As String = "", Optional _width As String = "", Optional _height As String = "") As Integer
Dim CLI As New StringBuilder("/heatmap")
Call CLI.Append(" ")
Call CLI.Append("/x " & """" & _x & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _name.StringEmpty Then
Call CLI.Append("/name " & """" & _name & """ ")
End If
If Not _width.StringEmpty Then
Call CLI.Append("/width " & """" & _width & """ ")
End If
If Not _height.StringEmpty Then
Call CLI.Append("/height " & """" & _height & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /heatmap.scale /x &lt;matrix.csv> [/factor 30 /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ScaleHeatmap(_x As String, Optional _factor As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/heatmap.scale")
Call CLI.Append(" ")
Call CLI.Append("/x " & """" & _x & """ ")
If Not _factor.StringEmpty Then
Call CLI.Append("/factor " & """" & _factor & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Imports /in &lt;sbml.xml>
''' ```
''' </summary>
'''
Public Function ImportsRxns(_in As String) As Integer
Dim CLI As New StringBuilder("/Imports")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")


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
Public Function ObjMAT(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/phenos.MAT")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


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
Public Function PhenosOUTCoefficient(_gene As String, _pheno As String, Optional _footprints As String = "", Optional _out As String = "", Optional _spcc As Boolean = False) As Integer
Dim CLI As New StringBuilder("/phenos.out.Coefficient")
Call CLI.Append(" ")
Call CLI.Append("/gene " & """" & _gene & """ ")
Call CLI.Append("/pheno " & """" & _pheno & """ ")
If Not _footprints.StringEmpty Then
Call CLI.Append("/footprints " & """" & _footprints & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _spcc Then
Call CLI.Append("/spcc ")
End If


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
Public Function PhenoOUT_MAT(_in As String, _samples As String, Optional _out As String = "", Optional _model As String = "") As Integer
Dim CLI As New StringBuilder("/phenos.out.MAT")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/samples " & """" & _samples & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _model.StringEmpty Then
Call CLI.Append("/model " & """" & _model & """ ")
End If


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
Public Function Solve(_i As String, _o As String, _d As String, Optional _m As String = "", Optional _f As String = "", Optional _knock_out As String = "") As Integer
Dim CLI As New StringBuilder("/solve")
Call CLI.Append(" ")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-o " & """" & _o & """ ")
Call CLI.Append("-d " & """" & _d & """ ")
If Not _m.StringEmpty Then
Call CLI.Append("-m " & """" & _m & """ ")
End If
If Not _f.StringEmpty Then
Call CLI.Append("-f " & """" & _f & """ ")
End If
If Not _knock_out.StringEmpty Then
Call CLI.Append("-knock_out " & """" & _knock_out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Solver.KEGG /in &lt;model.xml> /objs &lt;locus.txt> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function KEGGSolver(_in As String, _objs As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Solver.KEGG")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/objs " & """" & _objs & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Solver.rFBA /in &lt;MetaCyc.Sbml> /reg &lt;footprints.csv> /obj &lt;object_function.txt/xml> [/obj-type &lt;lst/pathway/module> /params &lt;rfba.parameters.xml> /stat &lt;stat.Csv> /sample &lt;sampleName> /modify &lt;locus_modify.csv> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function AnalysisPhenotype(_in As String, _reg As String, _obj As String, Optional _obj_type As String = "", Optional _params As String = "", Optional _stat As String = "", Optional _sample As String = "", Optional _modify As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Solver.rFBA")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/reg " & """" & _reg & """ ")
Call CLI.Append("/obj " & """" & _obj & """ ")
If Not _obj_type.StringEmpty Then
Call CLI.Append("/obj-type " & """" & _obj_type & """ ")
End If
If Not _params.StringEmpty Then
Call CLI.Append("/params " & """" & _params & """ ")
End If
If Not _stat.StringEmpty Then
Call CLI.Append("/stat " & """" & _stat & """ ")
End If
If Not _sample.StringEmpty Then
Call CLI.Append("/sample " & """" & _sample & """ ")
End If
If Not _modify.StringEmpty Then
Call CLI.Append("/modify " & """" & _modify & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' compile -i &lt;input_file> -o &lt;output_file>[ -if &lt;sbml/metacyc> -of &lt;fba/fba2> -f &lt;objective_function> -d &lt;max/min>]
''' ```
''' Compile data source into a model file so that the fba program can using the data to performing the simulation calculation.
''' </summary>
'''
Public Function Compile(_i As String, _o As String, Optional _if As String = "", Optional _of As String = "", Optional _f As String = "", Optional _d As String = "") As Integer
Dim CLI As New StringBuilder("compile")
Call CLI.Append(" ")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-o " & """" & _o & """ ")
If Not _if.StringEmpty Then
Call CLI.Append("-if " & """" & _if & """ ")
End If
If Not _of.StringEmpty Then
Call CLI.Append("-of " & """" & _of & """ ")
End If
If Not _f.StringEmpty Then
Call CLI.Append("-f " & """" & _f & """ ")
End If
If Not _d.StringEmpty Then
Call CLI.Append("-d " & """" & _d & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
