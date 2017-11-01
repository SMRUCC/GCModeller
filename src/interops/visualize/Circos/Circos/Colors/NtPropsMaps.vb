#Region "Microsoft.VisualBasic::7e435fd0567ecf067e4e5b4b98a9c683, ..\interops\visualize\Circos\Circos\Colors\NtPropsMaps.vb"

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
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.NtProps
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Colors

    ''' <summary>
    ''' Maps the colors based on the nt property
    ''' </summary>
    Public Module NtPropsMapsExtensions

        <Extension>
        Public Function FromGC(source As IEnumerable(Of GeneObjectGC)) As Mappings()
            Dim GC As Double() = source.Select(Function(x) x.value)
            Return GradientMaps.GradientMappings(GC)
        End Function

        <Extension>
        Public Function FromAT(source As IEnumerable(Of GeneObjectGC)) As Mappings()
            Dim AT As Double() = source.Select(Function(x) x.value).ToArray
            Return GradientMaps.GradientMappings(AT)
        End Function

        <Extension>
        Public Function PropertyMaps(source As IEnumerable(Of FastaToken)) As NtPropsMaps
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

    Public Structure NtPropsMaps

        Public source As FastaFile
        ''' <summary>
        ''' {value, circos color expression}
        ''' </summary>
        Public AT As Dictionary(Of Double, String)
        ''' <summary>
        ''' {value, circos color expression}
        ''' </summary>
        Public GC As Dictionary(Of Double, String)
        Public props As Dictionary(Of String, GeneObjectGC)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
