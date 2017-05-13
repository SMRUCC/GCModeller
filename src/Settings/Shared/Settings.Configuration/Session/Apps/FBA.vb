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
'''
''' </summary>
'''
Public Function Analysis_Phenotype(_in As String, _reg As String, _obj As String, Optional _obj_type As String = "", Optional _params As String = "", Optional _stat As String = "", Optional _sample As String = "", Optional _modify As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Analysis.Phenotype")
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
'''
''' </summary>
'''
Public Function Export(_i As String, _o As String) As Integer
Dim CLI As New StringBuilder("/Export")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-o " & """" & _o & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Flux_Coefficient(_in As String, Optional _footprints As String = "", Optional _out As String = "", Optional _spcc As Boolean = False, Optional _kegg As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Flux.Coefficient")
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
'''
''' </summary>
'''
Public Function Flux_KEGG_Filter(_in As String, _model As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Flux.KEGG.Filter")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/model " & """" & _model & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Func_Coefficient(_func As String, _in As String, Optional _footprints As String = "", Optional _out As String = "", Optional _spcc As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Func.Coefficient")
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
'''
''' </summary>
'''
Public Function gcFBA_Batch(_model As String, _phenotypes As String, _footprints As String, Optional _obj_type As String = "", Optional _params As String = "", Optional _stat As String = "", Optional _sample As String = "", Optional _modify As String = "", Optional _out As String = "", Optional _parallel As String = "") As Integer
Dim CLI As New StringBuilder("/gcFBA.Batch")
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
'''Draw heatmap from the correlations between the genes and the metabolism flux.
''' </summary>
'''
Public Function heatmap(_x As String, Optional _out As String = "", Optional _name As String = "", Optional _width As String = "", Optional _height As String = "") As Integer
Dim CLI As New StringBuilder("/heatmap")
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
'''
''' </summary>
'''
Public Function heatmap_scale(_x As String, Optional _factor As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/heatmap.scale")
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
'''
''' </summary>
'''
Public Function [Imports](_in As String) As Integer
Dim CLI As New StringBuilder("/Imports")
Call CLI.Append("/in " & """" & _in & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Merges the objective function result as a Matrix. For calculation the coefficient of the genes with the phenotype objective function.
''' </summary>
'''
Public Function phenos_MAT(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/phenos.MAT")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''2. Coefficient of the genes with the metabolism fluxs from the batch analysis result.
''' </summary>
'''
Public Function phenos_out_Coefficient(_gene As String, _pheno As String, Optional _footprints As String = "", Optional _out As String = "", Optional _spcc As Boolean = False) As Integer
Dim CLI As New StringBuilder("/phenos.out.Coefficient")
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
'''1. Merge flux.csv result as a Matrix, for the calculation of the coefficient of the genes with the metabolism flux.
''' </summary>
'''
Public Function phenos_out_MAT(_in As String, _samples As String, Optional _out As String = "", Optional _model As String = "") As Integer
Dim CLI As New StringBuilder("/phenos.out.MAT")
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
'''solve a FBA model from a specific (SBML) model file.
''' </summary>
'''
Public Function Solve(_i As String, _o As String, _d As String, Optional _m As String = "", Optional _f As String = "", Optional _knock_out As String = "") As Integer
Dim CLI As New StringBuilder("/Solve")
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
'''
''' </summary>
'''
Public Function Solver_KEGG(_in As String, _objs As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Solver.KEGG")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/objs " & """" & _objs & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Solver_rFBA(_in As String, _reg As String, _obj As String, Optional _obj_type As String = "", Optional _params As String = "", Optional _stat As String = "", Optional _sample As String = "", Optional _modify As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Solver.rFBA")
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
'''Compile data source into a model file so that the fba program can using the data to performing the simulation calculation.
''' </summary>
'''
Public Function compile(_i As String, _o As String, Optional _if As String = "", Optional _of As String = "", Optional _f As String = "", Optional _d As String = "") As Integer
Dim CLI As New StringBuilder("compile")
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
