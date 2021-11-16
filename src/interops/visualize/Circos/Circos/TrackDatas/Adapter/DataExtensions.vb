#Region "Microsoft.VisualBasic::decb9e728337a88dc4cb28a03f69ef6a, visualize\Circos\Circos\TrackDatas\Adapter\DataExtensions.vb"

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

    '     Module DataExtensions
    ' 
    '         Function: GetchrLabels
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace TrackDatas

    Public Module DataExtensions

        <Extension>
        Public Function GetchrLabels(karyotype As Karyotype.SkeletonInfo,
                                     Optional getKey As Func(Of Karyotype.Karyotype, String) = Nothing) As Dictionary(Of String, Karyotype.Karyotype)

            If getKey Is Nothing Then
                getKey = Function(x) x.chrName
            End If

            Return karyotype.Karyotypes.ToDictionary(getKey)
        End Function
    End Module
End Namespace
