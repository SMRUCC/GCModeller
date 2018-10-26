#Region "Microsoft.VisualBasic::f5da36a39fdb185759a03533ab6eaa85, CLI_tools\gcc\CLI\Project.vb"

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

' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Assembly.NCBI.GenBank

Partial Module CLI

    <ExportAPI("/compile.KEGG")>
    <Usage("/compile.KEGG /in <genome.gb> /KO <ko.assign.csv> /maps <kegg.pathways.repository> /compounds <kegg.compounds.repository> /reactions <kegg.reaction.repository> [/out <out.model.Xml/xlsx>]")>
    Public Function CompileKEGG(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim KO$ = args <= "/KO"
        Dim maps$ = args <= "/maps"
        Dim compounds = args <= "/compounds"
        Dim reactions = args <= "/reactions"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.GCMarkup"
        Dim genome As GBFF.File = GBFF.File.Load(path:=[in])

    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function IsGCMarkup(out As String) As Boolean
        Return out.ExtensionSuffix.TextEquals("GCMarkup")
    End Function
End Module