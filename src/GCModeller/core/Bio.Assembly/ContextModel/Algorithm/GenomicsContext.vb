#Region "Microsoft.VisualBasic::4dc27bd1e185ce9668a25ea04697030a, GCModeller\core\Bio.Assembly\ContextModel\Algorithm\GenomicsContext.vb"

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

    '   Total Lines: 35
    '    Code Lines: 13
    ' Comment Lines: 16
    '   Blank Lines: 6
    '     File Size: 1.34 KB


    '     Interface IGenomicsContextProvider
    ' 
    '         Properties: AllFeatures, Feature
    ' 
    '         Function: GetByName, GetRelatedGenes, GetStrandFeatures
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ContextModel

    Public Interface IGenomicsContextProvider(Of T As IGeneBrief)

        ''' <summary>
        ''' Listing all of the features loci sites on the genome. 
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property AllFeatures As T()
        Default ReadOnly Property Feature(locus_tag As String) As T

        Function GetByName(locus_tag As String) As T

        ''' <summary>
        ''' 获取某一个位点在位置上有相关联系的基因
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <param name="unstrand"></param>
        ''' <param name="ATGDist"></param>
        ''' <returns></returns>
        Function GetRelatedGenes(loci As NucleotideLocation,
                                 Optional unstrand As Boolean = False,
                                 Optional ATGDist As Integer = 500) As Relationship(Of T)()

        ''' <summary>
        ''' Gets all of the feature sites on the specific <see cref="Strands"/> nucleotide sequence
        ''' </summary>
        ''' <param name="strand"></param>
        ''' <returns></returns>
        Function GetStrandFeatures(strand As Strands) As T()
    End Interface
End Namespace
