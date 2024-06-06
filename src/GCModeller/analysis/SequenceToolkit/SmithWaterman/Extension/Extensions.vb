#Region "Microsoft.VisualBasic::51b223ea80175678a9c027f0f5eadad6, analysis\SequenceToolkit\SmithWaterman\Extension\Extensions.vb"

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

    '   Total Lines: 44
    '    Code Lines: 31 (70.45%)
    ' Comment Lines: 5 (11.36%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (18.18%)
    '     File Size: 1.53 KB


    ' Module Coverage
    ' 
    '     Function: Length, QueryLength, SubjectLength
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Linq
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Module Coverage

    ''' <summary>
    ''' 可能会有重叠或者不连续，这个函数是为了计算高分区的Coverage而创建的
    ''' </summary>
    ''' <param name="regions"></param>
    ''' <returns></returns>
    Public Function Length(regions As IEnumerable(Of Coordinate)) As Integer
        Dim points As New List(Of Integer)

        For Each x In regions
            Call points.Add(CInt(x.Y - x.X).Sequence(offset:=CInt(x.X)))
        Next

        Dim array = (From x As Integer In points Select x Distinct).ToArray
        Return array.Length
    End Function

    <Extension> Public Function QueryLength(source As IEnumerable(Of HSP)) As Integer
        Dim nlst As Coordinate() = source _
            .Select(Function(x)
                        Return New Coordinate With {.X = x.fromA, .Y = x.toA}
                    End Function) _
            .ToArray

        Return Length(nlst)
    End Function

    <Extension> Public Function SubjectLength(source As IEnumerable(Of HSP)) As Integer
        Dim nlst As Coordinate() = source _
            .Select(Function(x)
                        Return New Coordinate With {.X = x.fromB, .Y = x.toB}
                    End Function) _
            .ToArray

        Return Length(nlst)
    End Function
End Module
