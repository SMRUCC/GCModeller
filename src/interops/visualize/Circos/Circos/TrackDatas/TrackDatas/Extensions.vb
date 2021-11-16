#Region "Microsoft.VisualBasic::728916a1db1b9cb5217d2c38e8122f01, visualize\Circos\Circos\TrackDatas\TrackDatas\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: Distinct, FromColorMapping, Ranges
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq

Namespace TrackDatas

    Public Module Extensions

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="cl"></param>
        ''' <param name="idx"></param>
        ''' <param name="offset"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function FromColorMapping(cl As Circos.Colors.Mappings, idx As Integer, offset As Integer) As ValueTrackData
            Return New ValueTrackData With {
                .formatting = New Formatting With {
                    .fill_color = $"({cl.color.R},{cl.color.G},{cl.color.B})"
                },
                .start = idx,
                .end = idx + 1 + offset,
                .value = cl.value
            }
        End Function

        Public Function Distinct(source As IEnumerable(Of ValueTrackData)) As ValueTrackData()
            Dim LQuery As ValueTrackData() = (From x As ValueTrackData
                                              In source
                                              Select x,
                                                  uid = $"{x.start}..{x.end}"
                                              Group By uid Into Group) _
                                                 .Select(Function(x) x.Group.First.x).ToArray
            Return LQuery
        End Function

        <Extension>
        Public Function Ranges(data As IEnumerable(Of ValueTrackData)) As DoubleRange
            Dim bufs As Double() = data.Select(Function(x) x.value).ToArray
            Return New DoubleRange(bufs)
        End Function
    End Module
End Namespace
