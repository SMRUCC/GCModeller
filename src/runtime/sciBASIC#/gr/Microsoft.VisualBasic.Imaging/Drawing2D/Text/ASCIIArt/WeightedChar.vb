﻿#Region "Microsoft.VisualBasic::9f430dc8e3e33bd60043afa933912e9f, gr\Microsoft.VisualBasic.Imaging\Drawing2D\Text\ASCIIArt\WeightedChar.vb"

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

    '     Class WeightedChar
    ' 
    '         Properties: Character, CharacterImage, Weight
    ' 
    '         Function: getDefaultCharSet, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.Default

Namespace Drawing2D.Text.ASCIIArt

    Public Class WeightedChar

        Public Property Character As String
        Public Property CharacterImage As Bitmap
        Public Property Weight As Double

        Public Overrides Function ToString() As String
            Return $"{Character} ({Weight})"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Shared Function getDefaultCharSet() As DefaultValue(Of WeightedChar())
            Return CharSet.GenerateFontWeights()
        End Function
    End Class
End Namespace
