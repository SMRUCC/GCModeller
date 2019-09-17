#Region "Microsoft.VisualBasic::9a591aba7f60509d7d7ffdc51e8073d4, CLI_tools\eggHTS\CLIGroups.vb"

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

    ' Class CLIGroups
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Friend NotInheritable Class CLIGroups

    Public Const Samples_CLI As String = "0. Samples CLI tools"
    Public Const SamplesExpressions_CLI$ = "0. Samples expression analysis"
    Public Const Annotation_CLI As String = "1. uniprot annotation CLI tools"
    Public Const DEP_CLI As String = "2. DEP analysis CLI tools"

#Region "Functional enrichment tools"
    Public Const Enrichment_CLI As String = "3. Enrichment analysis tools"
    Public Const KOBAS$ = Enrichment_CLI & ": KOBAS"
    Public Const DAVID$ = Enrichment_CLI & ": DAVID"
    Public Const ClusterProfiler$ = Enrichment_CLI & ": clusterProfiler"
    Public Const NetworkEnrichment_CLI$ = "4. Network enrichment visualize tools"
#End Region

    Public Const Repository_CLI$ = "Repository data tools"

    Public Const iTraqTool$ = "iTraq data analysis tool"
    Public Const LabelFreeTool$ = "Label Free data analysis tools"
    Public Const DataVisualize_cli$ = "Data visualization tool"

    Public Const UniProtCLI$ = "UniProt tools"

    Private Sub New()
    End Sub

End Class
