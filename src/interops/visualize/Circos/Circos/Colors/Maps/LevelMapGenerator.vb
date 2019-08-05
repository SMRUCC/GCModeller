#Region "Microsoft.VisualBasic::1f91771a7cc5184d12b8de49d5a78b68, visualize\Circos\Circos\Colors\Maps\LevelMapGenerator.vb"

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

    '     Class LevelMapGenerator
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateMaps
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports ColorPattern = Microsoft.VisualBasic.Imaging.ColorMap

Namespace Colors

    Friend Class LevelMapGenerator

        Public values As Double()
        Public clSequence As Color()
        Public replaceBase As Boolean

        Dim highest As Color
        Dim offset As Integer

        Sub New(values As IEnumerable(Of Double), name As String, mapLevels As Integer, offsetPercentage#, replaceBase As Boolean)
            Dim maps As New ColorPattern(mapLevels * 2)
            Me.clSequence = ColorSequence(maps, name).Reverse.ToArray
            Me.values = values.ToArray
            Me.replaceBase = replaceBase
            Me.highest = clSequence.Last
            Me.offset = CInt(clSequence.Length * offsetPercentage)
        End Sub

        Public Function CreateMaps(lv As Integer, index As Integer) As Mappings
            Dim value As Double = values(index)
            Dim Color As Color

            If lv <= 1 AndAlso replaceBase Then
                Color = Color.WhiteSmoke
            Else
                Dim idx As Integer = lv + offset

                If idx < clSequence.Length Then
                    Color = clSequence(idx)
                Else
                    Color = highest
                End If
            End If

            Return New Mappings With {
                .value = value,
                .level = lv,
                .color = Color
            }
        End Function
    End Class
End Namespace
