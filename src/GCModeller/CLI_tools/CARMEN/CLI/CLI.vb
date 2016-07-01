#Region "Microsoft.VisualBasic::ed45f46239d5e1dda97061e62ea16d58, ..\GCModeller\CLI_tools\CARMEN\CLI\CLI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<PackageNamespace("CARMEN.CLI", Category:=APICategories.CLI_MAN)>
Module CLI

    <ExportAPI("--Reconstruct.KEGG.Online", Usage:="--Reconstruct.KEGG.Online /sp <organism> [/pathway <KEGG.pathwayId> /out <outDIR>]")>
    Public Function Reconstruct(args As CommandLine.CommandLine) As Integer
        Dim sp As String = args("/sp")
        Dim pathway As String = args("/pathway")
        Dim out As String = args.GetValue("/out", __getOutDIR(sp, pathway))

        If Not String.IsNullOrEmpty(pathway) Then
            Return SMRUCC.genomics.AnalysisTools.CARMEN.WebHandler.Reconstruct(sp, pathway, out).CLICode
        Else
            Return __reconstructAll(sp, outDIR:=out)
        End If
    End Function

    Private Function __reconstructAll(sp As String, outDIR As String) As Integer
        If SMRUCC.genomics.AnalysisTools.CARMEN.lstPathways.IsNullOrEmpty Then
            Call SMRUCC.genomics.AnalysisTools.CARMEN.LoadList()
        End If

        For Each pathway In SMRUCC.genomics.AnalysisTools.CARMEN.lstPathways
            Dim name As String = pathway.Value
            Dim DIR As String = outDIR & "/" & name.NormalizePathString

            Try
                Call SMRUCC.genomics.AnalysisTools.CARMEN.WebHandler.Reconstruct(sp, pathway.Key, DIR).CLICode
            Catch ex As Exception
                Call App.LogException(ex)
            End Try
        Next

        Return 0
    End Function

    Private Function __getOutDIR(sp As String, pathway As String) As String
        Dim outDIR As String = App.CurrentDirectory & $"/{sp.NormalizePathString}/"

        Call SMRUCC.genomics.AnalysisTools.CARMEN.LoadList()

        If String.IsNullOrEmpty(pathway) Then
            Return outDIR
        End If

        outDIR = $"{outDIR}/{SMRUCC.genomics.AnalysisTools.CARMEN.lstPathways(pathway).NormalizePathString}/"
        Return outDIR
    End Function

    <ExportAPI("--lstId.Downloads",
               Info:="Download the Avaliable organism name And available pathways' name.",
               Usage:="--lstId.Downloads [/o <out.DIR>]")>
    Public Function DownloadList(args As CommandLine.CommandLine) As Integer
        Dim out As String = args.GetValue("/o", App.CurrentDirectory)
        Call SMRUCC.genomics.AnalysisTools.CARMEN.LoadList()
        Call SMRUCC.genomics.AnalysisTools.CARMEN.lstOrganisms.SaveTo(out & "/Organisms.txt")
        Call SMRUCC.genomics.AnalysisTools.CARMEN.lstPathways.SaveTo(out & "/Pathways.txt")
        Return 0
    End Function
End Module

