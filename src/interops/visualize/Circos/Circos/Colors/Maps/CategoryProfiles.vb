#Region "Microsoft.VisualBasic::976991402234a5bc03d9de466e45a94d, visualize\Circos\Circos\Colors\Maps\CategoryProfiles.vb"

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

    '     Module CategoryProfiles
    ' 
    '         Function: GenerateColors
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Option Strict Off

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Colors

    <Package("Circos.COGs.ColorAPI")>
    Public Module CategoryProfiles

        ''' <summary>
        ''' {Key, ColorString}
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Colors.Maps")>
        Public Function GenerateColors(categories$()) As Dictionary(Of String, String)
            Dim colors$() = PerlColor.Colors.Shuffles
            Dim maps As New Dictionary(Of String, String)

            For Each key As SeqValue(Of String) In categories.SeqIterator
                Call maps.Add(key.value$, colors(key))
            Next

            Return maps
        End Function
    End Module
End Namespace
