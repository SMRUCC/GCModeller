﻿#Region "Microsoft.VisualBasic::4a26f253cd9e18053dc69ba592a3783c, Data_science\MachineLearning\NeuralNetwork\ActiveFunctions\SigmoidFunction.vb"

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

    '     Class SigmoidFunction
    ' 
    '         Properties: Store
    ' 
    '         Function: [Function], Derivative, Derivative2
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.StoreProcedure

Namespace NeuralNetwork.Activations

    ''' <summary>
    ''' https://github.com/trentsartain/Neural-Network/blob/master/NeuralNetwork/NeuralNetwork/Network/Sigmoid.cs
    ''' </summary>
    Public NotInheritable Class SigmoidFunction : Inherits IActivationFunction

        Public Overrides ReadOnly Property Store As ActiveFunction
            Get
                Return New ActiveFunction With {
                    .Arguments = {},
                    .name = NameOf(SigmoidFunction)
                }
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function Derivative(x As Double) As Double
            Return x * (1 - x)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function [Function](x As Double) As Double
            Return If(x < -45.0, 0.0, If(x > 45.0, 1.0, 1.0 / (1.0 + Math.Exp(-x))))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function Derivative2(y As Double) As Double
            Return Derivative(y)
        End Function

        Public Overrides Function ToString() As String
            Return $"{NameOf(SigmoidFunction)}()"
        End Function
    End Class
End Namespace
