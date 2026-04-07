#Region "Microsoft.VisualBasic::682652bea11093e1f67575c7a8a44042, analysis\Metagenome\Metagenome\OTUTable\ITaxonomyAbundance.vb"

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


    ' Code Statistics:

    '   Total Lines: 26
    '    Code Lines: 14 (53.85%)
    ' Comment Lines: 3 (11.54%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (34.62%)
    '     File Size: 751 B


    ' Interface ITaxonomyAbundance
    ' 
    '     Properties: ncbi_taxid
    ' 
    ' Class ContigResult
    ' 
    '     Properties: contig_id, expression_value, ncbi_taxid
    ' 
    ' Interface ITaxonomy
    ' 
    '     Properties: ncbi_taxid, taxonomy_string
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel

''' <summary>
''' [id, ncbi_taxid, expression_value]
''' </summary>
Public Interface ITaxonomyAbundance : Inherits IExpressionValue

    Property ncbi_taxid As UInteger

End Interface

Public Class ContigResult : Implements ITaxonomyAbundance

    Public Property contig_id As String Implements IReadOnlyId.Identity
    Public Property ncbi_taxid As UInteger Implements ITaxonomyAbundance.ncbi_taxid
    Public Property expression_value As Double Implements IExpressionValue.ExpressionValue

End Class

Public Interface ITaxonomy

    Property ncbi_taxid As UInteger
    Property taxonomy_string As String

End Interface

