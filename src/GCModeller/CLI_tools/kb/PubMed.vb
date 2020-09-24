#Region "Microsoft.VisualBasic::d4b7c3a0ac0d0b1d5089442cdfc9a2b3, CLI_tools\kb\PubMed.vb"

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
    '     Function: BuildPubMedDatabase
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base

Partial Module CLI

    <ExportAPI("/pubmed.kb")>
    <Usage("/pubmed.kb /term <term_string> [/out <out_directory>]")>
    Public Function BuildPubMedDatabase(args As CommandLine) As Integer
        Dim term$ = args <= "/term"
        Dim out$ = args("/out") Or $"./pubmed_{term.NormalizePathString}"
        Dim idlist = PubMed.QueryPubmed(term, pageSize:=50000).Distinct.ToArray

        Call idlist.GetJson.SaveTo($"{out}/id.json")

        ' 下载文献摘要数据
        Throw New NotImplementedException
    End Function
End Module
