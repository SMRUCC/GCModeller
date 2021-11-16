#Region "Microsoft.VisualBasic::ea62d740f8c8dbf37d84f3c8c288a9ec, visualize\Circos\Circos\Colors\Maps\NtPropsMapsExtensions.vb"

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

    '     Module NtPropsMapsExtensions
    ' 
    '         Function: FromAT, FromGC, PropertyMaps
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.NtProps

Namespace Colors

    ''' <summary>
    ''' Maps the colors based on the nt property
    ''' </summary>
    Public Module NtPropsMapsExtensions

        <Extension>
        Public Function FromGC(source As IEnumerable(Of GeneObjectGC)) As Mappings()
            Dim GC As Double() = source.Select(Function(x) x.value).ToArray
            Return GradientMaps.GradientMappings(GC)
        End Function

        <Extension>
        Public Function FromAT(source As IEnumerable(Of GeneObjectGC)) As Mappings()
            Dim AT As Double() = source.Select(Function(x) x.value).ToArray
            Return GradientMaps.GradientMappings(AT)
        End Function

        <Extension>
        Public Function PropertyMaps(source As IEnumerable(Of FastaSeq)) As NtPropsMaps
            Dim genome As New FastaFile(source)
            Dim props As GeneObjectGC() = GCProps.GetGCContentForGenes(genome)
            Dim AT As Mappings() = props.FromAT
            Dim GC As Mappings() = props.FromGC

            Return New NtPropsMaps With {
                .source = genome,
                .props = (From x As GeneObjectGC
                          In props
                          Select x
                          Group x By x.Title Into Group) _
                               .ToDictionary(Function(x) x.Title,
                                             Function(x) x.Group.First),
                .AT = (From x As Mappings
                       In AT
                       Select x
                       Group x By x.value Into Group) _
                            .ToDictionary(Function(x) x.value,
                                          Function(x) x.Group.First.CircosColor),
                .GC = (From x As Mappings
                       In GC
                       Select x
                       Group x By x.value Into Group) _
                            .ToDictionary(Function(x) x.value,
                                          Function(x) x.Group.First.CircosColor)
            }
        End Function
    End Module
End Namespace
