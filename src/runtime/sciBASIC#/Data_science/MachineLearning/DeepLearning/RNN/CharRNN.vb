Imports System.IO

Namespace RNN

	''' <summary>
	''' 
	''' </summary>
	''' <remarks>
	''' https://github.com/garstka/char-rnn-java
	''' </remarks>
	Public Class CharRNN
		Public Shared Sub Main(args As String())
			Dim configFile = "config.properties"
			Dim options As Options = Nothing
			'Try
			'	options = New Options(configFile)
			'Catch __unusedIOException1__ As IOException
			'	options = New Options()
			'	Console.WriteLine("Using the defaults.")

			'	' Save config.
			'	Try
			'		options.save(configFile)
			'	Catch __unusedIOException1__ As IOException
			'		Console.WriteLine("Couldn't save the options.")
			'	End Try
			'End Try

			If options.PrintOptions Then ' Print options
				options.print()
			End If

			' StreamReader scanner = new Scanner(System.in);
			While True
				Console.WriteLine("Choose an action:")
				Console.WriteLine("1. Create a new network and start training it.")
				Console.WriteLine("2. Restore a snapshot and continue training.")
				Console.WriteLine("3. Restore a snapshot and sample (generate text).")
				Console.WriteLine("(anything else to quit)")

				Const optionCreate = 1
				Const optionContinue = 2
				Const optionSample = 3

				Try
					Dim net As CharLevelRNN = Nothing
					Dim networkName As String = Nothing

					Dim nextChar As Integer = Integer.Parse(Console.ReadLine())
					If nextChar = optionCreate Then ' Create a new network
						Console.WriteLine("New network name: ")
						networkName = Console.ReadLine()
						net = initialize(options)
					ElseIf nextChar = optionContinue OrElse nextChar = optionSample Then ' From snapshot
						Console.WriteLine(".snapshot file name: ")
						networkName = Console.ReadLine()
						Try
							net = loadASnapshot(networkName)
						Catch __unusedIOException1__ As IOException
							Console.WriteLine("Couldn't load from file.")
							Continue While
						End Try ' Exit
					Else
						Exit While
					End If

					If nextChar = optionCreate OrElse nextChar = optionContinue Then ' train
						train(options, net, networkName) ' sample
					Else
						Dim temp = options.SamplingTemp
						While True
							Console.WriteLine("How many characters to sample (<1 to exit): ")
							Dim characters As Integer = Integer.Parse(Console.ReadLine())

							If characters < 1 Then
								Exit While
							End If
							Console.WriteLine("Seed string: ")
							Dim seed As String = Console.ReadLine()
							If seed.Length = 0 Then
								Console.WriteLine("Seed must not be empty.")
								Continue While
							End If

							sample(characters, seed, temp, net)
						End While
					End If
				Catch __unusedFormatException1__ As FormatException
					Console.WriteLine("Expected a number.")
				End Try
			End While
		End Sub

		' Initialize a network for training.
		Private Shared Function initialize(options As Options) As CharLevelRNN
			If options.UseSingleLayerNet Then
				' legacy network, single layer only
				Dim net As SingleLayerCharLevelRNN = New SingleLayerCharLevelRNN()
				net.HiddenSize = options.HiddenSize
				net.LearningRate = options.LearningRate
				Return net ' Multi layer network.
			Else
				Dim net As MultiLayerCharLevelRNN = New MultiLayerCharLevelRNN()

				' Use the same hidden size for all layers.
				Dim hiddenSize = options.HiddenSize
				Dim hidden = New Integer(options.Layers - 1) {}
				For i = 0 To hidden.Length - 1
					hidden(i) = hiddenSize
				Next
				net.HiddenSize = hidden
				net.LearningRate = options.LearningRate
				Return net
			End If
		End Function

		' Trains the network.
		Private Shared Sub train(options As Options, net As CharLevelRNN, snapshotName As String)
			If options Is Nothing Then
				Throw New NullReferenceException("Options can't be null.")
			End If

			If net Is Nothing Then
				Throw New NullReferenceException("Network can't be null.")
			End If

			If ReferenceEquals(snapshotName, Nothing) Then
				Throw New NullReferenceException("Snapshot name can't be null.")
			End If

			Try

				' Load the training set.

				Dim trainingSet = StringTrainingSet.fromFile(options.InputFile)

				Console.WriteLine("Data size: " & trainingSet.size().ToString() & ", vocabulary size: " & trainingSet.vocabularySize().ToString())

				' Initialize the network and its trainer.

				If Not net.Initialized Then ' Only if not restored from a snapshot.
					net.initialize(trainingSet.Alphabet)
				End If

				Dim trainer As RNNTrainer = New RNNTrainer()
				trainer.SequenceLength = options.SequenceLength
				trainer.initialize(net, trainingSet)
				trainer.printDebug(True)

				' For sampling during training, pick the temperature from options
				' and the first character in the training set as seed.
				Dim seed = Convert.ToString(trainingSet.Data(0))
				Dim samplingTemperature = options.SamplingTemp
				Dim sampleLength = options.TrainingSampleLength


				Dim loopTimes = options.LoopAroundTimes
				Dim sampleEveryNSteps = options.SampleEveryNSteps
				Dim snapshotEveryNSamples = options.SnapshotEveryNSamples
				Dim nextSnapshotNumber = 1
				While True ' Go over the whole training set.

					Try

						' Train for some steps and sample a short string
						' for evaluation.
						Dim batchCount = 0
						While True

							If System.Math.Min(Threading.Interlocked.Increment(batchCount), batchCount - 1) Mod snapshotEveryNSamples = 0 Then
								Call saveASnapshot(snapshotName & "-" & System.Math.Min(Threading.Interlocked.Increment(nextSnapshotNumber), nextSnapshotNumber - 1).ToString(), net)
							End If

							Console.WriteLine("___________________")

							trainer.train(sampleEveryNSteps)

							Console.WriteLine(net.sampleString(sampleLength, seed, samplingTemperature, False))
						End While
					Catch __unusedNoMoreTrainingDataException1__ As Exception
						Console.WriteLine("Out of training data.")
					End Try

					Call saveASnapshot(snapshotName & "-" & System.Math.Min(Threading.Interlocked.Increment(nextSnapshotNumber), nextSnapshotNumber - 1).ToString(), net)

					If loopTimes <= 0 Then
						Exit While
					End If

					Console.WriteLine("Looping around " & System.Math.Max(Threading.Interlocked.Decrement(loopTimes), loopTimes + 1).ToString() & "more time(s).")

					trainer.loopAround()
				End While
			Catch ex As IOException
				Console.WriteLine("Couldn't open the file.")
				Console.WriteLine("Bad training set.")
				Console.WriteLine("Different alphabet - can't train on this dataset.")
			End Try
		End Sub

		' Saves a network snapshot with this name to file.
		Private Shared Sub saveASnapshot(name As String, net As CharLevelRNN)
			If ReferenceEquals(name, Nothing) Then
				Throw New NullReferenceException("Network name can't be null.")
			End If

			If net Is Nothing Then
				Throw New NullReferenceException("Network can't be null.")
			End If

			' Take a snapshot
			Try
				Using str As FileStream = New FileStream(name & ".snapshot", FileMode.Create, FileAccess.Write)
					'ObjectOutputStream ostr = new ObjectOutputStream(str);
					'ostr.writeObject(net);
					'ostr.close();
				End Using
			Catch __unusedIOException1__ As IOException
				Console.WriteLine("Couldn't save a snapshot.")
				Return
			End Try
			Console.WriteLine("Saved as " & name & ".snapshot")
		End Sub

		' Loads a network snapshot with this name from file.
		Private Shared Function loadASnapshot(name As String) As CharLevelRNN
			If ReferenceEquals(name, Nothing) Then
				Throw New NullReferenceException("Name can't be null.")
			End If

			Dim net As CharLevelRNN = Nothing

			' Load the snapshot
			Try
				Using str As FileStream = New FileStream(name & ".snapshot", FileMode.Open, FileAccess.Read)
					'ObjectInputStream ostr = new ObjectInputStream(str);
					'net = (CharLevelRNN) ostr.readObject();
					'ostr.close();
				End Using
			Catch e As Exception When TypeOf e Is IOException
				Throw New IOException("Couldn't load the snapshot from file.", e)
			End Try
			Return net
		End Function


		' 
		' 		    Samples the net for n characters and prints the result.
		' 		    Requirements:
		' 		     - n >= 1,
		' 		     - seed != null
		' 		     - net != null, must be initialized
		' 		     - temperature in (0.0,1.0]
		' 		 
		Private Shared Sub sample(n As Integer, seed As String, temperature As Double, net As CharLevelRNN)
			If n < 1 Then
				Throw New ArgumentException("n must be at least 1")
			End If

			If net Is Nothing Then
				Throw New NullReferenceException("Network can't be null.")
			End If

			If ReferenceEquals(seed, Nothing) Then
				Throw New NullReferenceException("Seed can't be null.")
			End If

			If Not net.Initialized Then
				Throw New ArgumentException("Network must be initialized.")
			End If

			Try
				Console.WriteLine(net.sampleString(n, seed, temperature)) ' sample and advance
			Catch __unusedCharacterNotInAlphabetException1__ As Exception
				Console.WriteLine("Error: Character not in alphabet.")
			End Try
		End Sub
	End Class
End Namespace