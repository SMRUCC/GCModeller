#Region "Microsoft.VisualBasic::15f7480b0889657ebf45ef3a4272c0fb, ..\CLI_tools\Map\CLI\CLI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Oracle.Java.IO.Properties
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.ChromosomeMap
Imports SMRUCC.genomics.Visualize.ChromosomeMap.Configuration
Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels
Imports SMRUCC.genomics.Visualize.Extensions

<ExceptionHelp(Documentation:="http://docs.gcmodeller.org", Debugging:="https://github.com/SMRUCC/GCModeller/wiki", EMailLink:="xie.guigang@gcmodeller.org")>
<CLI> Public Module CLI

    <ExportAPI("--Draw.ChromosomeMap",
               Info:="Drawing the chromosomes map from the PTT object as the basically genome information source.",
               Usage:="--Draw.ChromosomeMap /ptt <genome.ptt> [/conf <config.inf> /out <dir.export> /COG <cog.csv>]")>
    <Argument("/COG", True, CLITypes.File,
              AcceptTypes:={GetType(MyvaCOG)},
              Description:="The gene object color definition, you can using this parameter to overrides the cog definition in the PTT file.")>
    Public Function DrawingChrMap(args As CommandLine) As Integer
        Dim PTT = args.GetObject(Of PTT)("/ptt", AddressOf TabularFormat.PTT.Load)
        Dim out As String = args.GetValue("/out", App.CurrentDirectory)
        Dim confInf As String = args.GetValue("/conf", out & "/config.inf")
        Dim COG As String = args("/COG")

        Return PTT.Draw(COG, confInf, out)
    End Function

    <ExportAPI("/Config.Template", Usage:="/Config.Template [/out <./config.inf>]")>
    Public Function WriteConfigTemplate(args As CommandLine) As Integer
        Dim conf As String = args.GetValue("/out", "./config.inf")

        With GetDefaultConfiguration(conf)
            Return .ToConfigDoc.SaveTo(conf)
        End With
    End Function

    <Extension>
    Private Function Draw(PTT As PTT, COG$, conf$, out$) As Integer
        Dim config As Config

        If Not conf.FileExists(True) Then
Create:     config = ChromosomeMap.GetDefaultConfiguration(conf)
        Else
            Try
                config = ChromosomeMap.LoadConfig(conf)
            Catch ex As Exception
                GoTo Create
            End Try
        End If

        Dim model As ChromesomeDrawingModel = ChromosomeMap.FromPTT(PTT, config)
        Dim COGProfiles As MyvaCOG() = Nothing

        If COG.FileExists(True) Then
            COGProfiles = COG.LoadCsv(Of MyvaCOG).ToArray
            model = ChromosomeMap.ApplyCogColorProfile(model, COGProfiles)
        End If

        With config

            Dim output As GraphicsData() =
                .CreateDevice _
                .InvokeDrawing(model)

            If Not COGProfiles.IsNullOrEmpty Then
                output(output.Length - 1) = output _
                    .Last _
                    .DrawCatalogProfiling(
                        COGProfiles,
                        .Margin,
                        "2000,1200")
            End If

            Return output.SaveImage(out, .SavedFormat)
        End With
    End Function

    <ExportAPI("--Draw.ChromosomeMap.genbank",
               Usage:="--Draw.ChromosomeMap.genbank /gb <genome.gbk> [/conf <config.inf> /out <dir.export> /COG <cog.csv>]")>
    Public Function DrawGenbank(args As CommandLine) As Integer
        Dim gb As String = args("/gb")
        Dim out As String = args.GetValue("/out", App.CurrentDirectory)
        Dim confInf As String = args.GetValue("/conf", out & "/config.inf")
        Dim COG As String = args("/COG")
        Dim PTT As PTT = GBFF.File.Load(gb).GbkffExportToPTT

        Return PTT.Draw(COG, confInf, out)
    End Function
End Module

