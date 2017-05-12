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
'''
''' </summary>
'''
Public Function Analysis_Phenotype(_in As String, _reg As String, _obj As String, Optional _obj_type As String = "", Optional _params As String = "", Optional _stat As String = "", Optional _sample As String = "", Optional _modify As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Analysis.Phenotype /in ""{_in}"" /reg ""{_reg}"" /obj ""{_obj}"" /obj-type ""{_obj_type}"" /params ""{_params}"" /stat ""{_stat}"" /sample ""{_sample}"" /modify ""{_modify}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Export(_i As String, _o As String) As Integer
Dim CLI$ = $"export -i ""{_i}"" -o ""{_o}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Flux_Coefficient(_in As String, Optional _footprints As String = "", Optional _out As String = "", Optional _spcc As Boolean = False, Optional _kegg As Boolean = False) As Integer
Dim CLI$ = $"/Flux.Coefficient /in ""{_in}"" /footprints ""{_footprints}"" /out ""{_out}"" {If(_spcc, "/spcc", "")} {If(_kegg, "/kegg", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Flux_KEGG_Filter(_in As String, _model As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Flux.KEGG.Filter /in ""{_in}"" /model ""{_model}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Func_Coefficient(_func As String, _in As String, Optional _footprints As String = "", Optional _out As String = "", Optional _spcc As Boolean = False) As Integer
Dim CLI$ = $"/Func.Coefficient /func ""{_func}"" /in ""{_in}"" /footprints ""{_footprints}"" /out ""{_out}"" {If(_spcc, "/spcc", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function gcFBA_Batch(_model As String, _phenotypes As String, _footprints As String, Optional _obj_type As String = "", Optional _params As String = "", Optional _stat As String = "", Optional _sample As String = "", Optional _modify As String = "", Optional _out As String = "", Optional _parallel As String = "") As Integer
Dim CLI$ = $"/gcFBA.Batch /model ""{_model}"" /phenotypes ""{_phenotypes}"" /footprints ""{_footprints}"" /obj-type ""{_obj_type}"" /params ""{_params}"" /stat ""{_stat}"" /sample ""{_sample}"" /modify ""{_modify}"" /out ""{_out}"" /parallel ""{_parallel}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Draw heatmap from the correlations between the genes and the metabolism flux.
''' </summary>
'''
Public Function heatmap(_x As String, Optional _out As String = "", Optional _name As String = "", Optional _width As String = "", Optional _height As String = "") As Integer
Dim CLI$ = $"/heatmap /x ""{_x}"" /out ""{_out}"" /name ""{_name}"" /width ""{_width}"" /height ""{_height}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function heatmap_scale(_x As String, Optional _factor As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/heatmap.scale /x ""{_x}"" /factor ""{_factor}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function [Imports](_in As String) As Integer
Dim CLI$ = $"/Imports /in ""{_in}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Merges the objective function result as a Matrix. For calculation the coefficient of the genes with the phenotype objective function.
''' </summary>
'''
Public Function phenos_MAT(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/phenos.MAT /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''2. Coefficient of the genes with the metabolism fluxs from the batch analysis result.
''' </summary>
'''
Public Function phenos_out_Coefficient(_gene As String, _pheno As String, Optional _footprints As String = "", Optional _out As String = "", Optional _spcc As Boolean = False) As Integer
Dim CLI$ = $"/phenos.out.Coefficient /gene ""{_gene}"" /pheno ""{_pheno}"" /footprints ""{_footprints}"" /out ""{_out}"" {If(_spcc, "/spcc", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''1. Merge flux.csv result as a Matrix, for the calculation of the coefficient of the genes with the metabolism flux.
''' </summary>
'''
Public Function phenos_out_MAT(_in As String, _samples As String, Optional _out As String = "", Optional _model As String = "") As Integer
Dim CLI$ = $"/phenos.out.MAT /in ""{_in}"" /samples ""{_samples}"" /out ""{_out}"" /model ""{_model}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''solve a FBA model from a specific (SBML) model file.
''' </summary>
'''
Public Function Solve(_i As String, _o As String, _d As String, Optional _m As String = "", Optional _f As String = "", Optional _knock_out As String = "") As Integer
Dim CLI$ = $"/solve -i ""{_i}"" -o ""{_o}"" -d ""{_d}"" -m ""{_m}"" -f ""{_f}"" -knock_out ""{_knock_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Solver_KEGG(_in As String, _objs As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Solver.KEGG /in ""{_in}"" /objs ""{_objs}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Solver_rFBA(_in As String, _reg As String, _obj As String, Optional _obj_type As String = "", Optional _params As String = "", Optional _stat As String = "", Optional _sample As String = "", Optional _modify As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Solver.rFBA /in ""{_in}"" /reg ""{_reg}"" /obj ""{_obj}"" /obj-type ""{_obj_type}"" /params ""{_params}"" /stat ""{_stat}"" /sample ""{_sample}"" /modify ""{_modify}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Compile data source into a model file so that the fba program can using the data to performing the simulation calculation.
''' </summary>
'''
Public Function compile(_i As String, _o As String, Optional _if As String = "", Optional _of As String = "", Optional _f As String = "", Optional _d As String = "") As Integer
Dim CLI$ = $"compile -i ""{_i}"" -o ""{_o}"" -if ""{_if}"" -of ""{_of}"" -f ""{_f}"" -d ""{_d}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function
End Class
End Namespace
