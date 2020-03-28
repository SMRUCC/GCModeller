#Region "Microsoft.VisualBasic::5ebb14ab9947fa842306517340211958, R#\metagenomics_kit\metagenomicsKit.vb"

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

    ' Module metagenomicsKit
    ' 
    '     Function: createEmptyCompoundOriginProfile
    ' 
    ' /********************************************************************************/

#End Region


Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Model.Network.Microbiome

<Package("metagenomics_kit")>
Module metagenomicsKit

    <ExportAPI("compounds.origin.profile")>
    Public Function createEmptyCompoundOriginProfile(taxonomy As NcbiTaxonomyTree, organism As String) As CompoundOrigins
        Return CompoundOrigins.CreateEmptyCompoundsProfile(taxonomy, organism)
    End Function
End Module

