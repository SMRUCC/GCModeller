﻿#Region "Microsoft.VisualBasic::224066dfe29f0b7a75aef751f7affed3, Data_science\MachineLearning\DeepLearning\CNN\Layers\RectifiedLinearUnitsLayer.vb"

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

    '   Total Lines: 74
    '    Code Lines: 46 (62.16%)
    ' Comment Lines: 13 (17.57%)
    '    - Xml Docs: 76.92%
    ' 
    '   Blank Lines: 15 (20.27%)
    '     File Size: 2.37 KB


    '     Class RectifiedLinearUnitsLayer
    ' 
    '         Properties: Type
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: forward, ToString
    ' 
    '         Sub: backward
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.MachineLearning.CNN.data

Namespace CNN.layers

    ''' <summary>
    ''' This is a layer of neurons that applies the non-saturating activation
    ''' function f(x)=max(0,x). It increases the nonlinear properties of the
    ''' decision function and of the overall network without affecting the
    ''' receptive fields of the convolution layer.
    ''' 
    ''' @author Daniel Persson (mailto.woden@gmail.com)
    ''' </summary>
    ''' <remarks>
    ''' ReLU
    ''' </remarks>
    Public Class RectifiedLinearUnitsLayer : Inherits DataLink
        Implements Layer

        Public Overridable ReadOnly Iterator Property BackPropagationResult As IEnumerable(Of BackPropResult) Implements Layer.BackPropagationResult
            Get
                ' no data
            End Get
        End Property

        Public Overridable ReadOnly Property Type As LayerTypes Implements Layer.Type
            Get
                Return LayerTypes.ReLU
            End Get
        End Property

        Protected threshold As Double = 0.0

        Sub New()
        End Sub

        Public Overridable Function forward(db As DataBlock, training As Boolean) As DataBlock Implements Layer.forward
            Dim V2 As DataBlock = db.clone()
            Dim N = db.Weights.Length
            Dim V2w = V2.Weights

            in_act = db

            For i As Integer = 0 To N - 1
                If V2w(i) < threshold Then
                    V2.setWeight(i, threshold) ' threshold at 0
                End If
            Next

            out_act = V2

            Return out_act
        End Function

        Public Overridable Sub backward() Implements Layer.backward
            ' zero out gradient wrt data
            Dim V = in_act.clearGradient() ' we need to set dw of this
            Dim V2 = out_act
            Dim N = V.Weights.Length

            For i As Integer = 0 To N - 1
                If V2.getWeight(i) <= threshold Then
                    V.setGradient(i, threshold) ' threshold
                Else
                    V.setGradient(i, V2.getGradient(i))
                End If
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return $"ReLU()"
        End Function
    End Class

End Namespace
