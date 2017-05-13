Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/MapPlot.exe

Namespace GCModellerApps


''' <summary>
''' Map.CLI
''' </summary>
'''
Public Class MapPlot : Inherits InteropService

Public Const App$ = "MapPlot.exe"

Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
''' ```
''' /Config.Template [/out &lt;./config.inf>]
''' ```
''' </summary>
'''
Public Function WriteConfigTemplate(Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Config.Template")
Call CLI.Append(" ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Visual.BBH /in &lt;bbh.Xml> /PTT &lt;genome.PTT> /density &lt;genomes.density.DIR> [/limits &lt;sp-list.txt> /out &lt;image.png>]
''' ```
''' Visualize the blastp result.
''' </summary>
'''
Public Function BBHVisual(_in As String, _PTT As String, _density As String, Optional _limits As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Visual.BBH")
Call CLI.Append(" ")
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
''' ```
''' /Visualize.blastn.alignment /in &lt;alignmentTable.txt> /genbank &lt;genome.gb> [/ORF.catagory &lt;catagory.tsv> /region &lt;left,right> /local /out &lt;image.png>]
''' ```
''' Blastn result alignment visualization from the NCBI web blast. This tools is only works for a plasmid blastn search result or a small gene cluster region in a large genome.
''' </summary>
'''
Public Function BlastnVisualizeWebResult(_in As String, _genbank As String, Optional _orf_catagory As String = "", Optional _region As String = "", Optional _out As String = "", Optional _local As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Visualize.blastn.alignment")
Call CLI.Append(" ")
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
''' ```
''' --Draw.ChromosomeMap /ptt &lt;genome.ptt> [/conf &lt;config.inf> /out &lt;dir.export> /COG &lt;cog.csv>]
''' ```
''' Drawing the chromosomes map from the PTT object as the basically genome information source.
''' </summary>
'''
Public Function DrawingChrMap(_ptt As String, Optional _conf As String = "", Optional _out As String = "", Optional _cog As String = "") As Integer
Dim CLI As New StringBuilder("--Draw.ChromosomeMap")
Call CLI.Append(" ")
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
''' ```
''' --Draw.ChromosomeMap.genbank /gb &lt;genome.gbk> [/conf &lt;config.inf> /out &lt;dir.export> /COG &lt;cog.csv>]
''' ```
''' </summary>
'''
Public Function DrawGenbank(_gb As String, Optional _conf As String = "", Optional _out As String = "", Optional _cog As String = "") As Integer
Dim CLI As New StringBuilder("--Draw.ChromosomeMap.genbank")
Call CLI.Append(" ")
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
