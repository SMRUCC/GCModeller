#Region "Microsoft.VisualBasic::05c9118e385c940a34886d62f2ff0861, analysis\Metagenome\MetaFunction\metaTraits\metaTraitData.vb"

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

    '   Total Lines: 17
    '    Code Lines: 12 (70.59%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (29.41%)
    '     File Size: 464 B


    '     Class metaTraitData
    ' 
    '         Properties: taxon_id, taxon_lineage, taxon_name, traits
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Metagenomics

Namespace metaTraits

    Public Class metaTraitData

        Public Property taxon_id As UInteger
        Public Property taxon_name As String
        Public Property taxon_lineage As Taxonomy
        Public Property traits As TraitData()

        Public Overrides Function ToString() As String
            Return $"{taxon_lineage} [{traits.TryCount} traits]"
        End Function

    End Class
End Namespace
