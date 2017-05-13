Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/MapPlot.exe

Namespace GCModellerApps


''' <summary>
'''Map.CLI
''' </summary>
'''
Public Class MapPlot : Inherits InteropService


Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
'''
''' </summary>
'''
Public Function Config_Template(Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Config.Template")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Visualize the blastp result.
''' </summary>
'''
Public Function Visual_BBH(_in As String, _PTT As String, _density As String, Optional _limits As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Visual.BBH")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
Call CLI.Append("/density " & """" & _density & """ ")
If Not _limits.StringEmpty Then
Call CLI.Append("/limits " & """" & _limits & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Blastn result alignment visualization from the NCBI web blast. This tools is only works for a plasmid blastn search result or a small gene cluster region in a large genome.
''' </summary>
'''
Public Function Visualize_blastn_alignment(_in As String, _genbank As String, Optional _orf_catagory As String = "", Optional _region As String = "", Optional _out As String = "", Optional _local As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Visualize.blastn.alignment")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/genbank " & """" & _genbank & """ ")
If Not _orf_catagory.StringEmpty Then
Call CLI.Append("/orf.catagory " & """" & _orf_catagory & """ ")
End If
If Not _region.StringEmpty Then
Call CLI.Append("/region " & """" & _region & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _local Then
Call CLI.Append("/local ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Drawing the chromosomes map from the PTT object as the basically genome information source.
''' </summary>
'''
Public Function Draw_ChromosomeMap(_ptt As String, Optional _conf As String = "", Optional _out As String = "", Optional _cog As String = "") As Integer
Dim CLI As New StringBuilder("--Draw.ChromosomeMap")
Call CLI.Append("/ptt " & """" & _ptt & """ ")
If Not _conf.StringEmpty Then
Call CLI.Append("/conf " & """" & _conf & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _cog.StringEmpty Then
Call CLI.Append("/cog " & """" & _cog & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Draw_ChromosomeMap_genbank(_gb As String, Optional _conf As String = "", Optional _out As String = "", Optional _cog As String = "") As Integer
Dim CLI As New StringBuilder("--Draw.ChromosomeMap.genbank")
Call CLI.Append("/gb " & """" & _gb & """ ")
If Not _conf.StringEmpty Then
Call CLI.Append("/conf " & """" & _conf & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _cog.StringEmpty Then
Call CLI.Append("/cog " & """" & _cog & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
