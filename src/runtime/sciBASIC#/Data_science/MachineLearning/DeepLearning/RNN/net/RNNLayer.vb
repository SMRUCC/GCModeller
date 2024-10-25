Imports std = System.Math

Namespace RNN

	' An RNN Layer with support for multi-layer networks.
	<Serializable>
	Public Class RNNLayer
		' Hyperparameters

		Private learningRateField As Double ' Backpropagation parameter.

		' Dimensions

		Private inputSizeField As Integer ' input vector size
		Private hiddenSizeField As Integer ' hidden state size
		Private outputSizeField As Integer ' input vector size

		' Defaults

		Public Const defaultInputSize As Integer = 50
		Public Const defaultHiddenSize As Integer = 100
		Public Const defaultOutputSize As Integer = 50
		Public Const defaultLearningRate As Double = 0.1

		' Network state

		Private Wxh As Matrix ' input layer weights
		Private Whh As Matrix ' hidden layer weights
		Private Why As Matrix ' output layer weights
		Private bh As Matrix ' hidden bias
		Private by As Matrix ' output bias

		Private h As Matrix ' last hidden state

		' Training state

		Private gWxh As Matrix ' gradient descent params: input layer
		Private gWhh As Matrix ' gradient descent params: hidden layer
		Private gWhy As Matrix ' gradient descent params: output layer
		Private gbh As Matrix ' gradient descent params: hidden bias
		Private gby As Matrix ' gradient descent params: output bias

		Private xAt As Matrix() ' input vectors through time
		Private hAt As Matrix() ' hidden state vectors through time
		Private yAt As Matrix() ' unnormalized output probability vectors through
		' time
		Private pAt As Matrix() ' normalized output probability vectors through time
		Private dxAt As Matrix() ' output gradient from a backwards pass

		Private lastSequenceLength As Integer ' Number of steps in the last forward pass
		' (must match the steps for the backward
		' pass)

		Private initialized As Boolean

		' Init 
		' Creates a net with default parameters.
		Public Sub New()
			inputSizeField = defaultInputSize
			hiddenSizeField = defaultHiddenSize
			inputSizeField = defaultOutputSize
			learningRateField = defaultLearningRate
		End Sub

		' Creates a net with custom parameters
		Public Sub New(inputSize As Integer, hiddenSize As Integer, outputSize As Integer, learningRate As Double)
			inputSizeField = inputSize
			hiddenSizeField = hiddenSize
			outputSizeField = outputSize
			learningRateField = learningRate
		End Sub

		' Set the input size.
		Public Overridable Property InputSize As Integer
			Set(value As Integer)
				initialized = False

				If value <= 0 Then
					Throw New ArgumentException("Illegal input size.")
				End If

				inputSizeField = value
			End Set
			Get
				Return inputSizeField
			End Get
		End Property

		' Set the hidden size.
		Public Overridable Property HiddenSize As Integer
			Set(value As Integer)
				initialized = False

				If value <= 0 Then
					Throw New ArgumentException("Illegal hidden size.")
				End If

				hiddenSizeField = value
			End Set
			Get
				Return hiddenSizeField
			End Get
		End Property

		' Set the output size.
		Public Overridable Property OutputSize As Integer
			Set(value As Integer)
				initialized = False

				If value <= 0 Then
					Throw New ArgumentException("Illegal output size.")
				End If

				outputSizeField = value
			End Set
			Get
				Return outputSizeField
			End Get
		End Property

		Public Overridable Property LearningRate As Double
			Set(value As Double)
				learningRateField = value
			End Set
			Get
				Return learningRateField
			End Get
		End Property

		' Initialize the net with random weights.
		Public Overridable Sub initialize()
			If inputSizeField <= 0 OrElse hiddenSizeField <= 0 OrElse outputSizeField <= 0 Then
				Throw New ArgumentException("Illegal layer sizes.")
			End If

			' create weight matrices

			Dim scale = 0.1

			Wxh = Random.randn(hiddenSizeField, inputSizeField).mul(scale)
			Whh = Random.randn(hiddenSizeField, hiddenSizeField).mul(scale)
			Why = Random.randn(outputSizeField, hiddenSizeField).mul(scale)
			bh = Matrix.zeros(hiddenSizeField)
			by = Matrix.zeros(outputSizeField)

			gWxh = Matrix.zerosLike(Wxh)
			gWhh = Matrix.zerosLike(Whh)
			gWhy = Matrix.zerosLike(Why)
			gbh = Matrix.zerosLike(bh)
			gby = Matrix.zerosLike(by)

			h = Random.randn(hiddenSizeField)

			initialized = True
		End Sub

		' 
		' 		    Converts the indices (x's) of a sequence to one-hot vectors
		' 		    for use as the x parameter for the first layer of a network.
		' 	
		' 		    Requirement: ix can't be null, or empty. ix[i] < inputSize
		' 		
		Friend Overridable Function ixTox(ix As Integer()) As Matrix()
			For Each index As Integer In ix
				If index < 0 OrElse index >= inputSizeField Then
					Throw New ArgumentException("Illegal index passed as argument.")
				End If
			Next

			' start at t = 1
			Dim oneHot = New Matrix(ix.Length + 1 - 1) {}
			For t = 1 To ix.Length + 1 - 1
				oneHot(t) = Matrix.oneHot(inputSizeField, ix(t - 1))
			Next

			Return oneHot
		End Function

		' Like ixTox, but a single index instead of an array
		Friend Overridable Function ixTox(ix As Integer) As Matrix
			If ix < 0 OrElse ix >= inputSizeField Then
				Throw New ArgumentException("Illegal index passed as argument.")
			End If

			Return Matrix.oneHot(inputSizeField, ix)
		End Function

		' 
		' 		    Training forward pass.
		' 	
		' 		    TRAINING
		' 	
		' 		    Takes x as input. For the first layer, x[t] should be a one-hot vector,
		' 		    for the others it's the output of the previous layer.
		' 	
		' 		    Yields y: unnormalized probabilities (inputs to next layers)
		' 		          p: normalized probabilities (for sampling)
		' 		    and dy, if provided with targets y's (input for the backward pass)
		' 	
		' 		    SAMPLING
		' 	
		' 		    Same as training, but x should be treated as a seed sequence.
		' 	
		' 		    Requirement:
		' 		    x came from ixTox or the previous layer's forward pass result y.
		' 		
		Friend Overridable Sub forward(x As Matrix())
			If Not initialized Then
				Throw New InvalidOperationException("Layer was not initialized.")
			End If

			If x Is Nothing OrElse x.Length < 2 Then ' starting at t = 1
				Throw New ArgumentException("Expected a sequence of length at least 1.")
			End If


			Dim t = 1

			While t < x.Length
				If x(t) Is Nothing OrElse x(t).getk() <> inputSizeField Then
					Throw New ArgumentException("Bad vector passed as argument.")
				End If

				Threading.Interlocked.Increment(t)
			End While

			' Initialize the forward pass 

			xAt = x ' save the inputs (needed for backpropagation)

			' Reset outputs

			lastSequenceLength = x.Length - 1

			' hidden state vectors through time
			hAt = New Matrix(lastSequenceLength + 1 - 1) {}

			' normalized probability vectors through time
			pAt = New Matrix(lastSequenceLength + 1 - 1) {}

			' normalized probability vectors through time
			yAt = New Matrix(lastSequenceLength + 1 - 1) {}

			hAt(0) = New Matrix(h) ' copy the current state

			' Forward pass 
			t = 1

			While t < lastSequenceLength + 1
				' find the new hidden state
				hAt(t) = Matrix.dot(Wxh, xAt(t)).add(Matrix.dot(Whh, hAt(t - 1))).add(bh).tanh()

				' find unnormalized output probabilities
				yAt(t) = Matrix.dot(Why, hAt(t)).add(by)

				' normalize output probabilities
				pAt(t) = Math.softmax(yAt(t))
				Threading.Interlocked.Increment(t)
			End While

			' Update the hidden state 

			h = hAt(lastSequenceLength)
		End Sub

		' Forward pass for a single seed.
		Friend Overridable Sub forward(x As Matrix)
			If x Is Nothing Then
				Throw New NullReferenceException("x is null.")
			End If

			Dim xa = New Matrix(1) {}
			xa(1) = x
			forward(xa)
		End Sub

		' 
		' 		    Calculates the cross-entropy loss of the last forward pass
		' 		    given target outputs.
		' 	
		' 		    iy - the target indices
		' 	
		' 		    Requirements: iy must be the size of the last sequence length, iy[i] <
		' 		   outputSize
		' 		
		Friend Overridable Function getLoss(iy As Integer()) As Double
			If Not initialized Then
				Throw New InvalidOperationException("Network was not initialized.")
			End If

			If iy Is Nothing OrElse iy.Length <> lastSequenceLength Then
				Throw New ArgumentException("Expected the iy sequence to be the same length as the last x sequence.")
			End If

			If True Then
				For Each index In iy
					If index < 0 OrElse index >= outputSizeField Then
						Throw New ArgumentException("Bad index passed as argument.")
					End If
				Next
			End If

			Dim loss = 0.0
			Dim t = 1

			While t < lastSequenceLength + 1
				loss += -std.Log(pAt(t).at(iy(t - 1))) ' calculate the cross-entropy loss
				Threading.Interlocked.Increment(t) ' start at t = 1
			End While

			Return loss
		End Function

		' Returns y: the unnormalized probabilities - output of the last forward
		' pass, starting at t = 1.
		Friend Overridable Function gety() As Matrix()
			If Not initialized Then
				Throw New InvalidOperationException("Network was not initialized.")
			End If

			Return yAt
		End Function

		' Returns p: the normalized probabilities - output of the last forward
		' pass, starting at t = 1.
		Friend Overridable Function getp() As Matrix()
			If Not initialized Then
				Throw New InvalidOperationException("Network was not initialized.")
			End If

			Return pAt
		End Function

		' 
		' 		    Returns dy: the gradients to be used as input to the last layer's
		' 		    backward pass, starting at t = 1. given iy - the target indices.
		' 	
		' 		    Requirement:
		' 		    iy must be the size of the last sequence length, iy[i] < outputSize
		' 		
		Friend Overridable Function getdy(iy As Integer()) As Matrix()
			If Not initialized Then
				Throw New InvalidOperationException("Network was not initialized.")
			End If

			If iy Is Nothing OrElse iy.Length <> lastSequenceLength Then
				Throw New ArgumentException("Expected the iy sequence to be the same length as the last x sequence.")
			End If

			If True Then
				For Each index In iy
					If index < 0 OrElse index >= outputSizeField Then
						Throw New ArgumentException("Bad index passed as argument.")
					End If
				Next
			End If

			Dim dyAt = New Matrix(lastSequenceLength + 1 - 1) {} ' start at t = 1

			Dim t = 1

			While t < lastSequenceLength + 1
				' backprop into y,
				' http://cs231n.github.io/neural-networks-case-study/#grad
				dyAt(t) = New Matrix(pAt(t))

				Dim expected = iy(t - 1)
				dyAt(t).setAt(expected, dyAt(t).at(expected) - 1)
				Threading.Interlocked.Increment(t)
			End While

			Return dyAt
		End Function

		' 
		' 		    Training backward pass.
		' 	
		' 		    Takes dy - the gradient to backpropagate.
		' 	
		' 		    Yields dx, which can be used as input to previous layer's backward pass,
		' 		    if present, and the updated weights.
		' 	
		' 		    Requirement: x came from this layer's getdy() or the next layer's
		' 		    backward pass result dx.
		' 		
		Friend Overridable Sub backward(dy As Matrix())
			If Not initialized Then
				Throw New InvalidOperationException("Network was not initialized.")
			End If

			If dy Is Nothing OrElse dy.Length <> lastSequenceLength + 1 Then ' starts at t = 1
				Throw New ArgumentException("Expected the y sequence to be the same length as the last x sequence.")
			End If

			Dim t = 1

			While t < lastSequenceLength + 1
				If dy(t) Is Nothing OrElse dy(t).getk() <> outputSizeField Then
					Throw New ArgumentException("Bad index passed as argument.")
				End If

				Threading.Interlocked.Increment(t)
			End While

			' Initialize backward pass 

			Dim dyAt = dy ' just an alias

			Dim dWxh = Matrix.zerosLike(Wxh)
			Dim dWhh = Matrix.zerosLike(Whh)
			Dim dWhy = Matrix.zerosLike(Why)
			Dim dbh = Matrix.zerosLike(bh)
			Dim dby = Matrix.zerosLike(by)

			Dim dhNext = Matrix.zerosLike(h)

			' gradients to be passed to the next backwards pass
			dxAt = New Matrix(lastSequenceLength + 1 - 1) {}

			' Backward pass 

			t = lastSequenceLength

			While t >= 1
				' y updates
				dWhy.add(Matrix.dot(dyAt(t), hAt(t).T()))
				dby.add(dyAt(t))

				' backprop into h and through tanh nonlinearity
				Dim dh As Matrix = Matrix.dot(Why.T(), dyAt(t)).add(dhNext)
				Dim dhRaw As Matrix = Matrix.onesLike(hAt(t)).add((New Matrix(hAt(t))).mul(hAt(t)).neg()).mul(dh)

				' h updates
				dWxh.add(Matrix.dot(dhRaw, xAt(t).T()))
				dWhh.add(Matrix.dot(dhRaw, hAt(t - 1).T()))
				dbh.add(dhRaw)

				' save dhNext for the next iteration
				dhNext = Matrix.dot(Whh.T(), dhRaw)

				' multi-layer only - save dx
				dxAt(t) = Matrix.dot(Wxh.T(), dh)
				Threading.Interlocked.Decrement(t)
			End While

			' clip exploding gradients

			Dim clip_a = -5.0
			Dim dparams = New Matrix() {dWxh, dWhh, dWhy, dbh, dby}

			For Each m In dparams
				m.clip(clip_a, -clip_a)
			Next

			t = 1

			While t < lastSequenceLength + 1
				dxAt(t).clip(clip_a, -clip_a)
				Threading.Interlocked.Increment(t)
			End While


			' Update weights with Adagrad 

			Dim params = New Matrix() {Wxh, Whh, Why, bh, by}
			Dim gparams = New Matrix() {gWxh, gWhh, gWhy, gbh, gby}

			Dim i = 0

			While i < dparams.Length
				Dim param = params(i)
				Dim dparam = dparams(i)
				Dim gparam = gparams(i)

				gparam.add((New Matrix(dparam)).mul(dparam))
				Dim tmp As Matrix = (New Matrix(gparam)).apply(Function(elem) std.Sqrt(elem) + 0.00000001)
				param.add((New Matrix(dparam)).mul(-learningRateField).div(tmp))
				Threading.Interlocked.Increment(i)
			End While
		End Sub

		' 
		' 		    Returns dx: the gradients to be used as input to the previous layer's
		' 		    backward pass.
		' 		
		Friend Overridable Function getdx() As Matrix()
			If Not initialized Then
				Throw New InvalidOperationException("Network was not initialized.")
			End If

			Return dxAt
		End Function

		''' <summary>
		''' * Sampling ** </summary>


		' Returns softmax(last y,temp) as an array of doubles: like last p, but
		' the probabilities of the next indices are normalized using a softmax
		' with temperature = temp in (0.0,1.0].
		Friend Overridable Function getProbabilities(temp As Double) As Double()
			If Not initialized Then
				Throw New InvalidOperationException("Network was not initialized.")
			End If

			Return Math.softmax(New Matrix(yAt(yAt.Length - 1)), temp).unravel()
		End Function

		' Save the hidden state before sampling.
		Friend Overridable Function saveHiddenState() As Matrix
			If Not initialized Then
				Throw New InvalidOperationException("Network was not initialized.")
			End If

			Return New Matrix(h)
		End Function

		' Restore the hidden state after sampling.
		Friend Overridable Sub restoreHiddenState(h As Matrix)
			If Not initialized Then
				Throw New InvalidOperationException("Network was not initialized.")
			End If

			If h.getk() <> hiddenSizeField Then
				Throw New ArgumentException("The hidden state has the wrong size.")
			End If
			Me.h = h
		End Sub

	End Class
End Namespace