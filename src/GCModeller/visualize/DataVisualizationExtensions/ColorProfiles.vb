﻿#Region "Microsoft.VisualBasic::83c9019fa9be037200ba63f745b80933, GCModeller.DataVisualization\ColorProfiles.vb"

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

    ' Class ColorProfiles
    ' 
    '     Properties: ColorProfiles
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic

Public Class ColorProfiles

    ReadOnly _defaultColor As Color

    Public ReadOnly Property ColorProfiles As Dictionary(Of String, Color)

    Default Public ReadOnly Property GetColor(Name As String) As Color
        Get
            If _ColorProfiles.ContainsKey(Name) Then
                Return _ColorProfiles(Name)
            Else
                Return _defaultColor
            End If
        End Get
    End Property

    Sub New(ColorProfiles As IEnumerable(Of String), Optional DefaultColor As Color = Nothing)
        _ColorProfiles = ColorProfiles.GenerateColorProfiles
        _defaultColor = DefaultColor

        If _defaultColor = Nothing Then _defaultColor = Color.Black
    End Sub

    Public Overrides Function ToString() As String
        Return String.Join(__describ, ColorProfiles.Count, _defaultColor.ToString)
    End Function

    Const __describ$ = "{0} color(s) in the rendering profile, default color is ""{1}"""

End Class
