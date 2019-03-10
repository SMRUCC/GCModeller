﻿#Region "Microsoft.VisualBasic::26f016719507de2532c6c0b6cd5992f5, analysis\SequenceToolkit\DNA_Comparative\gwANI\gwANIExtensions.vb"

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

    '     Module gwANIExtensions
    ' 
    '         Sub: Evaluate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text

Namespace gwANI

    Public Module gwANIExtensions

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="[in]">必须要是经过多序列比对对齐了的序列</param>
        ''' <param name="out"></param>
        ''' <param name="fast"></param>
        Public Sub Evaluate([in] As String, out As String, Optional fast As Boolean = True)
            Call "".SaveTo(out, Encoding.ASCII)

            Dim file As New FileStream(out, FileMode.OpenOrCreate)
            Dim write As New StreamWriter(file)

            If fast Then
                Call gwANI.fast_calculate_gwani([in], write)
            Else
                Call gwANI.calculate_and_output_gwani([in], write)
            End If
        End Sub
    End Module
End Namespace
