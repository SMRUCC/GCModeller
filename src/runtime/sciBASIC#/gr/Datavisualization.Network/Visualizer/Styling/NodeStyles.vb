#Region "Microsoft.VisualBasic::0865f3be7f9d5c216039d775f8658e40, ..\sciBASIC#\gr\Datavisualization.Network\NetworkCanvas\Styling\NodeStyles.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Styling

    Public Module NodeStyles

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="nodes"></param>
        ''' <param name="getDegree"></param>
        ''' <param name="sizeRange">将节点的大小映射到这个半径大小的区间之内</param>
        ''' <returns></returns>
        <Extension> Public Function DegreeAsSize(Of T)(nodes As IEnumerable(Of T),
                                                       getDegree As Func(Of T, Double),
                                                       sizeRange As DoubleRange) As Map(Of T, Double)()
            Dim array As T() = nodes.ToArray
            Dim degrees#() = array.Select(getDegree).ToArray
            Dim size#() = degrees.RangeTransform([to]:=sizeRange)
            Dim out As Map(Of T, Double)() =
                array _
                .SeqIterator _
                .Select(Function(x)
                            Return New Map(Of T, Double) With {
                                .Key = x.value,
                                .Maps = size(x)
                            }
                        End Function) _
                .ToArray
            Return out
        End Function
    End Module
End Namespace
