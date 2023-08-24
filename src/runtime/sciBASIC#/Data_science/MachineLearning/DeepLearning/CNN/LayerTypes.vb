﻿#Region "Microsoft.VisualBasic::5f3982435f32d28c23c167c80b18a9cb, sciBASIC#\Data_science\MachineLearning\DeepLearning\CNN\LayerTypes.vb"

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

    '   Total Lines: 13
    '    Code Lines: 11
    ' Comment Lines: 0
    '   Blank Lines: 2
    '     File Size: 338 B


    '     Enum LayerTypes
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Namespace Convolutional

    Public Enum LayerTypes

        ''' <summary>
        ''' conv
        ''' </summary>
        <Description("conv")> Convolution
        ''' <summary>
        ''' input
        ''' </summary>
        <Description("input")> Input
        ''' <summary>
        ''' output
        ''' </summary>
        <Description("output")> Output
        <Description("pool")> Pool
        <Description("relu")> ReLU
        <Description("softmax")> SoftMax

        samp
    End Enum
End Namespace
