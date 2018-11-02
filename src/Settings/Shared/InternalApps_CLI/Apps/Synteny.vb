Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\Synteny.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   1.0.0.*
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // 
' 
' 
'  < Synteny.CLI >
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /mapping.plot:     
'  /test:             
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    You can using "Settings ??<commandName>" for getting more details command help.

Namespace GCModellerApps


''' <summary>
''' Synteny.CLI
''' </summary>
'''
Public Class Synteny : Inherits InteropService

    Public Const App$ = "Synteny.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

''' <summary>
''' ```
''' /mapping.plot /mapping &lt;blastn_mapping.csv> /query &lt;query.gff3> /ref &lt;subject.gff3> [/Ribbon &lt;default=Spectral:c6> /size &lt;default=6000,4000> /auto.reverse &lt;default=0.9> /grep &lt;default="-"> /out &lt;Synteny.png>]
''' ```
''' </summary>
'''
Public Function PlotMapping(mapping As String, query As String, ref As String, Optional ribbon As String = "Spectral:c6", Optional size As String = "6000,4000", Optional auto_reverse As String = "0.9", Optional grep As String = "-", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/mapping.plot")
    Call CLI.Append(" ")
    Call CLI.Append("/mapping " & """" & mapping & """ ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    If Not ribbon.StringEmpty Then
            Call CLI.Append("/ribbon " & """" & ribbon & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not auto_reverse.StringEmpty Then
            Call CLI.Append("/auto.reverse " & """" & auto_reverse & """ ")
    End If
    If Not grep.StringEmpty Then
            Call CLI.Append("/grep " & """" & grep & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' 
''' ```
''' </summary>
'''
Public Function Test() As Integer
    Dim CLI As New StringBuilder("/test")
    Call CLI.Append(" ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
