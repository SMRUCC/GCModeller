#Region "Microsoft.VisualBasic::f54c77d57c889dacd4b5cad498218c81, R#\metagenomics_kit\TaxonomyKit.vb"

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

    ' Module TaxonomyKit
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: LoadNcbiTaxonomyTree, printTaxonomy
    ' 
    ' /********************************************************************************/

#End Region


Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports Taxonomy = SMRUCC.genomics.Metagenomics.Taxonomy
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter

<Package("taxonomy_kit")>
Module TaxonomyKit

    Sub New()
        REnv.AttachConsoleFormatter(Of Taxonomy)(AddressOf printTaxonomy)
    End Sub

    Private Function printTaxonomy(taxonomy As Taxonomy) As String
        Return $"<{taxonomy.lowestLevel}> {taxonomy.ToString(BIOMstyle:=True)}"
    End Function

    <ExportAPI("Ncbi.taxonomy_tree")>
    Public Function LoadNcbiTaxonomyTree(repo$) As NcbiTaxonomyTree
        Return New NcbiTaxonomyTree(repo)
    End Function

End Module

