﻿#Region "Microsoft.VisualBasic::adcf2a706ba16be3bf909ab00d1792b0, Data_science\MachineLearning\DeepLearning\RNN\net\RNNTrainer.vb"

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

    '   Total Lines: 158
    '    Code Lines: 84 (53.16%)
    ' Comment Lines: 31 (19.62%)
    '    - Xml Docs: 38.71%
    ' 
    '   Blank Lines: 43 (27.22%)
    '     File Size: 4.16 KB


    ' 	Class RNNTrainer
    ' 
    ' 	    Properties: SequenceLength, SmoothLoss, TotalSteps
    ' 
    ' 	    Constructor: (+2 Overloads) Sub New
    ' 	    Sub: initialize, loopAround, printDebug, (+2 Overloads) train
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm

Namespace RNN

	' Trains a recurrent neural net on a training set.
	Public Class RNNTrainer
		' Defaults

		Public Const defaultSequenceLength As Integer = 50

		Private net As Trainable

		' Training

		Private sequenceLengthField As Integer ' Steps to unroll the RNN for during training.

		Private trainingSet As TrainingSet ' The training set.
		Private dataTrainedIndex As Integer ' Current index into training data.

		Private ix As Integer() ' Training sequence inputs by time.
		Private iy As Integer() ' Training sequence inputs by time.
		Private smoothLossField As Double ' Loss for training evaluation.
		Private totalStepsField As Integer ' Total step count.

		Private debugMessagesOn As Boolean

		Private initialized As Boolean

		''' <summary>
		''' * Construct ** </summary>

		' Constructs without initializing
		Public Sub New()
			SequenceLength = defaultSequenceLength
		End Sub

		' Constructs and initializes.
		Public Sub New(net As Trainable, data As TrainingSet)
			Me.New()
			initialize(net, data)
		End Sub

		''' <summary>
		''' * Params ** </summary>

		' Set a different sequence length.
		Public Overridable WriteOnly Property SequenceLength As Integer
			Set(value As Integer)
				If value <= 1 Then
					Throw New ArgumentException("Illegal sequence length.")
				End If

				sequenceLengthField = value
				If initialized Then
					ix = New Integer(sequenceLengthField - 1) {}
					iy = New Integer(sequenceLengthField - 1) {}
				End If
			End Set
		End Property

		''' <summary>
		''' * Initialize ** </summary>

		' Initializes training. Requires trainingSet != null.
		Public Overridable Sub initialize(net As Trainable, trainingSet As TrainingSet)
			Me.net = net
			Me.trainingSet = trainingSet

			Dim vocabSize As Integer = trainingSet.vocabularySize()

			If vocabSize = 0 Then
				Throw New Exception("Vocabulary can't be empty.")
			End If

			If trainingSet.size() < sequenceLengthField Then
				Throw New Exception("Data is too small for even a single pass.")
			End If

			' get the temporary index arrays for sequences

			ix = New Integer(sequenceLengthField - 1) {}
			iy = New Integer(sequenceLengthField - 1) {}

			' initialize smooth loss
			smoothLossField = -System.Math.Log(1.0 / vocabSize) * sequenceLengthField

			initialized = True
		End Sub

		''' <summary>
		''' * Train ** </summary>

		' Trains the network until there's no more data.
		Public Overridable Sub train()
			train(Integer.MaxValue)
		End Sub

		' Trains the net for a few steps. Requires steps >= 0.
		Public Overridable Sub train(steps As Integer)
			For Each i As Integer In TqdmWrapper.Range(0, steps)
				' try to extract
				trainingSet.extract(dataTrainedIndex, ix, iy)

				' train

				Dim loss = net.forwardBackward(ix, iy)

				' calculate smooth loss

				smoothLossField = smoothLossField * 0.999 + loss * 0.001

				dataTrainedIndex += sequenceLengthField ' shift the training offset

				totalStepsField += 1
			Next

			' print debug
			If debugMessagesOn Then
				Console.WriteLine("Step: " & totalStepsField.ToString() & ", loss: " & SmoothLoss.ToString())
			End If
		End Sub

		' Reset the data pointer to the beginning.
		Public Overridable Sub loopAround()
			dataTrainedIndex = 0
		End Sub

		''' <summary>
		''' * Get ** </summary>

		' Returns the step count.
		Public Overridable ReadOnly Property TotalSteps As Integer
			Get
				Return totalStepsField
			End Get
		End Property

		' Returns the smooth cross-entropy loss.
		Public Overridable ReadOnly Property SmoothLoss As Double
			Get
				If Not initialized Then
					Throw New InvalidOperationException("Training is uninitialized.")
				End If

				Return smoothLossField / sequenceLengthField * 100
			End Get
		End Property

		''' <summary>
		''' * Set ** </summary>

		' Print debug messages.
		Public Overridable Sub printDebug([on] As Boolean)
			debugMessagesOn = [on]
		End Sub
	End Class

End Namespace