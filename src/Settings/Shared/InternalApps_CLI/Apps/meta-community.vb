Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: D:/GCModeller/GCModeller/bin/meta-community.exe

Namespace GCModellerApps


''' <summary>
''' meta_community.CLI
''' </summary>
'''
Public Class meta_community : Inherits InteropService

    Public Const App$ = "meta-community.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

''' <summary>
''' ```
''' /box.plot /in &lt;data.csv> /groups &lt;sampleInfo.csv> [/out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function Boxplot([in] As String, groups As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/box.plot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/groups " & """" & groups & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /handle.hmp.manifest /in &lt;manifest.tsv> [/out &lt;save.directory>]
''' ```
''' </summary>
'''
Public Function Download16sSeq([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/handle.hmp.manifest")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /heatmap.plot /in &lt;data.csv> /groups &lt;sampleInfo.csv> [/schema &lt;default=YlGnBu:c9> /tsv /group /title &lt;title> /size &lt;2700,3000> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function HeatmapPlot([in] As String, groups As String, Optional schema As String = "YlGnBu:c9", Optional title As String = "", Optional size As String = "", Optional out As String = "", Optional tsv As Boolean = False, Optional group As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/heatmap.plot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/groups " & """" & groups & """ ")
    If Not schema.StringEmpty Then
            Call CLI.Append("/schema " & """" & schema & """ ")
    End If
    If Not title.StringEmpty Then
            Call CLI.Append("/title " & """" & title & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If tsv Then
        Call CLI.Append("/tsv ")
    End If
    If group Then
        Call CLI.Append("/group ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /hmp.manifest.files /in &lt;manifest.tsv> [/out &lt;list.txt>]
''' ```
''' </summary>
'''
Public Function ExportFileList([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/hmp.manifest.files")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /LefSe.Matrix /in &lt;Species_abundance.csv> /ncbi_taxonomy &lt;NCBI_taxonomy> [/all_rank /out &lt;out.tsv>]
''' ```
''' Processing the relative aboundance matrix to the input format file as it describ: http://huttenhower.sph.harvard.edu/galaxy/root?tool_id=lefse_upload
''' </summary>
'''
Public Function LefSeMatrix([in] As String, ncbi_taxonomy As String, Optional out As String = "", Optional all_rank As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/LefSe.Matrix")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/ncbi_taxonomy " & """" & ncbi_taxonomy & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If all_rank Then
        Call CLI.Append("/all_rank ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /microbiome.metabolic.network /metagenome &lt;list.txt/OTU.tab> /ref &lt;reaction.repository.XML> /uniprot &lt;repository.directory> [/out &lt;network.directory>]
''' ```
''' </summary>
'''
Public Function MetabolicComplementationNetwork(metagenome As String, ref As String, uniprot As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/microbiome.metabolic.network")
    Call CLI.Append(" ")
    Call CLI.Append("/metagenome " & """" & metagenome & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Rank_Abundance /in &lt;OTU.table.csv> [/schema &lt;color schema, default=Rainbow> /out &lt;out.DIR>]
''' ```
''' https://en.wikipedia.org/wiki/Rank_abundance_curve
''' </summary>
'''
Public Function Rank_Abundance([in] As String, Optional schema As String = "Rainbow", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Rank_Abundance")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not schema.StringEmpty Then
            Call CLI.Append("/schema " & """" & schema & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Relative_abundance.barplot /in &lt;dataset.csv> [/group &lt;sample_group.csv> /desc /asc /take &lt;-1> /size &lt;3000,2700> /column.n &lt;default=9> /interval &lt;10px> /out &lt;out.png>]
''' ```
''' </summary>
'''
Public Function Relative_abundance_barplot([in] As String, Optional group As String = "", Optional take As String = "", Optional size As String = "", Optional column_n As String = "9", Optional interval As String = "", Optional out As String = "", Optional desc As Boolean = False, Optional asc As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Relative_abundance.barplot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not group.StringEmpty Then
            Call CLI.Append("/group " & """" & group & """ ")
    End If
    If Not take.StringEmpty Then
            Call CLI.Append("/take " & """" & take & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not column_n.StringEmpty Then
            Call CLI.Append("/column.n " & """" & column_n & """ ")
    End If
    If Not interval.StringEmpty Then
            Call CLI.Append("/interval " & """" & interval & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If desc Then
        Call CLI.Append("/desc ")
    End If
    If asc Then
        Call CLI.Append("/asc ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Relative_abundance.stacked.barplot /in &lt;dataset.csv> [/group &lt;sample_group.csv> /out &lt;out.png>]
''' ```
''' </summary>
'''
Public Function Relative_abundance_stackedbarplot([in] As String, Optional group As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Relative_abundance.stacked.barplot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not group.StringEmpty Then
            Call CLI.Append("/group " & """" & group & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /significant.difference /in &lt;data.csv> /groups &lt;sampleInfo.csv> [/out &lt;out.csv.DIR>]
''' ```
''' </summary>
'''
Public Function SignificantDifference([in] As String, groups As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/significant.difference")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/groups " & """" & groups & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SILVA.headers /in &lt;silva.fasta> /out &lt;headers.tsv>
''' ```
''' </summary>
'''
Public Function SILVA_headers([in] As String, out As String) As Integer
    Dim CLI As New StringBuilder("/SILVA.headers")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/out " & """" & out & """ ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /UPGMA.Tree /in &lt;in.csv> [/out &lt;>]
''' ```
''' </summary>
'''
Public Function UPGMATree([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/UPGMA.Tree")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
