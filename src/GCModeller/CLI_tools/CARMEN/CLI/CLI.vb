#Region "Microsoft.VisualBasic::e313d9334c50b47adbd49fabc11e327a, CLI_tools\CARMEN\CLI\CLI.vb"

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

    ' Module CLI
    ' 
    '     Function: __getOutDIR, __reconstructAll, DownloadList, Reconstruct
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops

<Package("CARMEN.CLI", Category:=APICategories.CLI_MAN)>
Module CLI

    <ExportAPI("--Reconstruct.KEGG.Online", Usage:="--Reconstruct.KEGG.Online /sp <organism> [/pathway <KEGG.pathwayId> /out <outDIR>]")>
    Public Function Reconstruct(args As CommandLine) As Integer
        Dim sp As String = args("/sp")
        Dim pathway As String = args("/pathway")
        Dim out As String = args.GetValue("/out", __getOutDIR(sp, pathway))

        If Not String.IsNullOrEmpty(pathway) Then
            Return CARMEN.WebHandler.Reconstruct(sp, pathway, out).CLICode
        Else
            Return __reconstructAll(sp, outDIR:=out)
        End If
    End Function

    Private Function __reconstructAll(sp As String, outDIR As String) As Integer
        If CARMEN.lstPathways.IsNullOrEmpty Then
            Call CARMEN.LoadList()
        End If

        For Each pathway In CARMEN.lstPathways
            Dim name As String = pathway.Value
            Dim DIR As String = outDIR & "/" & name.NormalizePathString

            Try
                Call CARMEN.WebHandler.Reconstruct(sp, pathway.Key, DIR).CLICode
            Catch ex As Exception
                Call App.LogException(ex)
            End Try
        Next

        Return 0
    End Function

    Private Function __getOutDIR(sp As String, pathway As String) As String
        Dim outDIR As String = App.CurrentDirectory & $"/{sp.NormalizePathString}/"

        Call CARMEN.LoadList()

        If String.IsNullOrEmpty(pathway) Then
            Return outDIR
        End If

        outDIR = $"{outDIR}/{CARMEN.lstPathways(pathway).NormalizePathString}/"
        Return outDIR
    End Function

    <ExportAPI("--lstId.Downloads",
               Info:="Download the Avaliable organism name And available pathways' name.",
               Usage:="--lstId.Downloads [/o <out.DIR>]")>
    Public Function DownloadList(args As CommandLine) As Integer
        Dim out As String = args.GetValue("/o", App.CurrentDirectory)
        Call CARMEN.LoadList()
        Call CARMEN.lstOrganisms.SaveTo(out & "/Organisms.txt")
        Call CARMEN.lstPathways.SaveTo(out & "/Pathways.txt")
        Return 0
    End Function
End Module
