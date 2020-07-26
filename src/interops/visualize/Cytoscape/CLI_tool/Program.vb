#Region "Microsoft.VisualBasic::cb5193e2096534c301ec8262fcc8cdfb, visualize\Cytoscape\CLI_tool\Program.vb"

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

    ' Module Program
    ' 
    '     Function: Main
    ' 
    '     Sub: test33333linkages
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.Visualize.Cytoscape.Tables
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Data.visualize.Network

Module Program

    Public Function Main() As Integer
        '  Call ssss.asfdsfd()
        '  sdfsdf()
        '  Call ExportNetwork()
        'Call test33333linkages()

        '   Dim s = Regex.Replace("(n+1) C02174 C00404(n);C00404; n C00001", "(\(.+?\))|([nm] )", "", RegexICSng)

        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function

    Sub test33333linkages()
        Dim data As File = File.Load("C:\Users\xieguigang\OneDrive\4.7\sqq\chebi-kegg.csv")
        Call LinkageNetwork.BuildNetwork(data).Save("x:\test.network/")
    End Sub
End Module
'    Public Sub sdfsdf()
'        Dim snp = "C:\Users\xieguigang\Desktop\New folder\snp-prot-vir_network\snp.csv".LoadCsv(Of ssss)
'        Dim vir = "C:\Users\xieguigang\Desktop\New folder\snp-prot-vir_network\vir.csv".LoadCsv(Of ssss)

'        Dim virhash = (From x In vir Select x Group x By x.B Into Group).ToDictionary(Function(x) x.B, Function(x) x.Group.ToArray)
'        Dim out As New List(Of ssss)

'        For Each x In snp
'            Dim virs = virhash(x.B)

'            For Each y In virs
'                Dim copy = x.Copy
'                copy.B = y.A
'                out += copy
'            Next
'        Next

'        Call out.SaveTo("C:\Users\xieguigang\Desktop\New folder\snp-prot-vir_network\snp-vir_net.csv")
'    End Sub


'    Public Class ssss
'        Public Property A As String
'        Public Property B As String
'        Public Property tag As String

'        Public Property meta As Dictionary(Of String, String)

'        Public Overrides Function ToString() As String
'            Return A
'        End Function

'        Public Function Copy() As ssss
'            Return New ssss With {.A = A, .B = B, .tag = tag, .meta = New Dictionary(Of String, String)}
'        End Function

'        Public Shared Function asfdsfd()
'            Dim snp = "C:\Users\xieguigang\Desktop\New folder\8.3\SNP_Viral_Network\SNP_Viral_Network\New folder\snp.csv".LoadCsv(Of ssss)
'            Dim vir = "C:\Users\xieguigang\Desktop\New folder\8.3\SNP_Viral_Network\SNP_Viral_Network\New folder\vir.csv".LoadCsv(Of ssss)
'            Dim out As New List(Of ssss)

'            Dim vhash = vir.GroupBy(Function(x) x.B).ToDictionary(Function(x) x.First.B, Function(x) x.ToArray)

'            For Each s In snp
'                For Each v In vhash(s.B)
'                    Dim cp As ssss = s.Copy
'                    cp.meta.Add("vir", v.A)
'                    cp.meta.Add("prot", v.B)
'                    cp.meta.Add("tag2", v.tag)

'                    out += cp
'                Next
'            Next

'            Call out.SaveTo("C:\Users\xieguigang\Desktop\New folder\8.3\SNP_Viral_Network\SNP_Viral_Network\New folder\snp-prot-vir.csv")
'        End Function
'    End Class

'    Public Sub ExportNetwork()
'        Dim snp_human As IO.File = IO.File.Load("C:\Users\xieguigang\Desktop\New folder\8.3\SNP_Viral_Network\SNP-Human.csv")
'        Dim viral_human As IO.File = IO.File.Load("C:\Users\xieguigang\Desktop\New folder\8.3\SNP_Viral_Network\Viral-Human.csv")
'        Dim commons As String() = "C:\Users\xieguigang\Desktop\New folder\8.3\SNP_Viral_Network\commons.csv".ReadAllLines
'        Dim nodes As New Dictionary(Of Node)
'        Dim types As String() = snp_human.First.ToArray
'        Dim net As New List(Of NetworkEdge)

'        For Each row In snp_human.Skip(1)
'            Dim key As String = $"{types(0)}-{row(0)}".Replace(" ", "_")

'            If Not nodes.ContainsKey(key) Then
'                nodes += New Node With {
'                    .ID = key,
'                    .NodeType = types(0),
'                    .Properties = New Dictionary(Of String, String) From {{"display", row(0)}}
'                }
'            End If

'            Dim key2 = $"{types(1)}-{row(1)}".Replace(" ", "_")

'            If Not nodes.ContainsKey(key2) Then
'                nodes += New Node With {
'                    .ID = key2,
'                    .NodeType = types(1),
'                    .Properties = New Dictionary(Of String, String) From {{"display", row(1)}}
'                }
'            End If

'            net += New NetworkEdge With {.FromNode = key, .ToNode = key2, .InteractionType = "SNP - HumanProtein"}
'        Next

'        types = viral_human.First.ToArray

'        For Each row In viral_human.Skip(1)
'            Dim key As String = $"{types(0)}-{row(0)}".Replace(" ", "_")

'            If Not nodes.ContainsKey(key) Then
'                nodes += New Node With {.ID = key, .NodeType = types(0), .Properties = New Dictionary(Of String, String) From {{"display", row(0)}}}
'            End If

'            Dim key2 = $"{types(1)}-{row(1)}".Replace(" ", "_")

'            If Not nodes.ContainsKey(key2) Then
'                nodes += New Node With {.ID = key2, .NodeType = types(1), .Properties = New Dictionary(Of String, String) From {{"display", row(1)}}}
'            End If

'            net += New NetworkEdge With {.ToNode = key, .FromNode = key2, .InteractionType = "Viral - HumanProtein"}
'        Next

'        For Each id As String In commons
'            id = ("Human Protein-" & id).Replace(" ", "_")

'            If nodes.ContainsKey(id) Then
'                nodes(id).NodeType = "[Common] " & nodes(id).NodeType
'                nodes(id).Properties.Add("common", "true")
'            End If
'        Next

'        Dim network As New Network(nodes.Values, net)
'        Call network.Save("C:\Users\xieguigang\Desktop\New folder\8.3\SNP_Viral_Network\SNP_Viral_Network")
'    End Sub
'End Module


'Public Class genome
'    Public Property gi As String
'    Public Property Sum As Double
'    Public Property w As Double
'    Public Property hit_name As String
'    Public Property name As String

'End Class
