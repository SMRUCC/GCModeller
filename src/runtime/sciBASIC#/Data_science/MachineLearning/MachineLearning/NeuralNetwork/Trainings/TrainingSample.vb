﻿#Region "Microsoft.VisualBasic::21b04f3b550cd2ed60f8e0eaf0971cde, Data_science\MachineLearning\MachineLearning\NeuralNetwork\Trainings\TrainingSample.vb"

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

'     Structure TrainingSample
' 
'         Properties: isEmpty
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.MachineLearning.ComponentModel.StoreProcedure
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace NeuralNetwork

    Public Structure TrainingSample

        Dim sampleID As String
        Dim sample As Double()

        ''' <summary>
        ''' The output result.
        ''' </summary>
        Dim classify As Double()

        Public ReadOnly Property isEmpty As Boolean
            Get
                Return sample.IsNullOrEmpty OrElse classify.IsNullOrEmpty
            End Get
        End Property

        Sub New(sample As Sample)
            Me.sampleID = sample.ID
            Me.sample = sample.vector
            Me.classify = sample.target
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{sampleID}] {sample.JoinBy(", ")} -> {classify.GetJson}"
        End Function

    End Structure
End Namespace
