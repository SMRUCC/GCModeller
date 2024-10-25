Imports System
Imports System.IO

Namespace RNN

	''' <summary>
	''' Application options.
	''' </summary>
	Public Class Options
		''' <summary>
		''' * Model parameters ** </summary>

		Private hiddenSizeField As Integer ' Size of a single RNN layer hidden state.
		Friend Const hiddenSizeDefault As Integer = 100

		Private layersField As Integer ' How many layers in a net?
		Friend Const layersDefault As Integer = 2

		''' <summary>
		''' * Training parameters ** </summary>

		Private sequenceLengthField As Integer ' How many steps to unroll during training?
		Friend Const sequenceLengthDefault As Integer = 50

		Private learningRateField As Double ' The network learning rate.
		Friend Const learningRateDefault As Double = 0.1


		' * Sampling parameters **

		' Sampling temperature (0.0, 1.0]. Lower
		' temperature means more conservative
		' predictions.
		Private samplingTempField As Double
		Friend Const samplingTempDefault As Double = 1.0

		''' <summary>
		''' * Other options ** </summary>

		Private printOptionsField As Boolean ' Print options at the start.
		Friend Const printOptionsDefault As Boolean = True

		Private trainingSampleLengthField As Integer ' Length of a sample during training.
		Friend Const trainingSampleLengthDefault As Integer = 400

		Private snapshotEveryNSamplesField As Integer ' Take a network's snapshot every N samples.
		Friend Const snapshotEveryNSamplesDefault As Integer = 50

		Private loopAroundTimesField As Integer ' Loop around the training data this many times.
		Friend Const loopAroundTimesDefault As Integer = 0

		Private sampleEveryNStepsField As Integer ' Take a sample during training every N steps.
		Friend Const sampleEveryNStepsDefault As Integer = 100

		Private inputFileField As String ' The training data.
		Friend Const inputFileDefault As String = "input.txt"

		Private useSingleLayerNetField As Boolean ' Use the simple, single layer net.
		Friend Const useSingleLayerNetDefault As Boolean = False


		''' <summary>
		''' * Load ** </summary>

		Private prop As Object

		' Constructs, uses the defaults.
		Friend Sub New()
			' prop = new Properties();
			setDefaults()
		End Sub

		' Loads the options from the file, or uses defaults if not found.
		Friend Sub New(file As String)
			Me.New()
			Try
				Using [in] As Stream = New FileStream(file, FileMode.Open, FileAccess.Read)
					' prop.load(@in);
					'Properties;
				End Using
			Catch e As IOException
				Throw New IOException("Loading config from " & file & " failed.", e)
			End Try
		End Sub

		''' <summary>
		''' * Print ** </summary>

		' Prints the options.
		Friend Overridable Sub print()
			setProperties()
			' prop.list(System.out);
		End Sub

		''' <summary>
		''' * Save ** </summary>

		' Saves to file.
		Friend Overridable Sub save(file As String)
			setProperties()
			Try
				Using out As Stream = New FileStream(file, FileMode.Create, FileAccess.Write)
					' prop.store(@out, "---RNN properties---");
				End Using
			Catch e As IOException
				Throw New IOException("Saving config to " & file & " failed.", e)
			End Try
		End Sub

		''' <summary>
		''' * Get ** </summary>

		Friend Overridable ReadOnly Property HiddenSize As Integer
			Get
				Return hiddenSizeField
			End Get
		End Property

		Friend Overridable ReadOnly Property Layers As Integer
			Get
				Return layersField
			End Get
		End Property

		Friend Overridable ReadOnly Property SequenceLength As Integer
			Get
				Return sequenceLengthField
			End Get
		End Property

		Friend Overridable ReadOnly Property LearningRate As Double
			Get
				Return learningRateField
			End Get
		End Property

		Friend Overridable ReadOnly Property SamplingTemp As Double
			Get
				Return samplingTempField
			End Get
		End Property

		Friend Overridable ReadOnly Property PrintOptions As Boolean
			Get
				Return printOptionsField
			End Get
		End Property

		Friend Overridable ReadOnly Property InputFile As String
			Get
				Return inputFileField
			End Get
		End Property

		Friend Overridable ReadOnly Property UseSingleLayerNet As Boolean
			Get
				Return useSingleLayerNetField
			End Get
		End Property

		Friend Overridable ReadOnly Property TrainingSampleLength As Integer
			Get
				Return trainingSampleLengthField
			End Get
		End Property

		Friend Overridable ReadOnly Property LoopAroundTimes As Integer
			Get
				Return loopAroundTimesField
			End Get
		End Property

		Friend Overridable ReadOnly Property SampleEveryNSteps As Integer
			Get
				Return sampleEveryNStepsField
			End Get
		End Property

		Friend Overridable ReadOnly Property SnapshotEveryNSamples As Integer
			Get
				Return snapshotEveryNSamplesField
			End Get
		End Property

		''' <summary>
		''' * Helper ** </summary>

		' Sets the default values.
		Private Sub setDefaults()
			hiddenSizeField = hiddenSizeDefault
			layersField = layersDefault

			sequenceLengthField = sequenceLengthDefault
			learningRateField = learningRateDefault

			samplingTempField = samplingTempDefault

			printOptionsField = printOptionsDefault
			trainingSampleLengthField = trainingSampleLengthDefault
			loopAroundTimesField = loopAroundTimesDefault
			sampleEveryNStepsField = sampleEveryNStepsDefault
			snapshotEveryNSamplesField = snapshotEveryNSamplesDefault
			inputFileField = inputFileDefault
			useSingleLayerNetField = useSingleLayerNetDefault
		End Sub

		' Validates the properties and sets to default values where failed.
		Private Sub validateProperties()
			validateHiddenSize()
			validateLayers()
			validateSequenceLength()
			validateLoopAroundTimes()
			validateSampleEveryNSteps()
			validateSnapshotEveryNSamples()
			validateLearningRate()
			validateSamplingTemp()
			validateTrainingSampleLength()
		End Sub

		Private Sub validateHiddenSize()
			If hiddenSizeField < 1 Then
				hiddenSizeField = hiddenSizeDefault
				Console.WriteLine("Hidden size must be >= 1. Using default " & Convert.ToString(hiddenSizeField) & ".")
			End If
		End Sub

		Private Sub validateLayers()
			If layersField < 1 Then
				layersField = layersDefault
				Console.WriteLine("Layer count must be >= 1. Using default " & Convert.ToString(layersField) & ".")
			End If
		End Sub
		Private Sub validateSequenceLength()
			If sequenceLengthField < 1 Then
				sequenceLengthField = sequenceLengthDefault
				Console.WriteLine("Sequence length must be >= 1. Using default " & Convert.ToString(sequenceLengthField) & ".")
			End If
		End Sub

		Private Sub validateLoopAroundTimes()

			If loopAroundTimesField < 0 Then
				loopAroundTimesField = loopAroundTimesDefault
				Console.WriteLine("Loop around times must be >= 0. Using default " & Convert.ToString(loopAroundTimesField) & ".")
			End If
		End Sub

		Private Sub validateSampleEveryNSteps()
			If sampleEveryNStepsField < 1 Then
				sampleEveryNStepsField = sampleEveryNStepsDefault
				Console.WriteLine("Sample every N steps: N must be >= 1. Using default " & Convert.ToString(sampleEveryNStepsField) & ".")
			End If
		End Sub

		Private Sub validateSnapshotEveryNSamples()
			If snapshotEveryNSamplesField < 1 Then
				snapshotEveryNSamplesField = snapshotEveryNSamplesDefault
				Console.WriteLine("Snapshot every N samples: N must be >= 1. Using default " & Convert.ToString(snapshotEveryNSamplesField) & ".")
			End If
		End Sub

		Private Sub validateLearningRate()
			If learningRateField < 0.0 Then
				learningRateField = learningRateDefault
				Console.WriteLine("Learning rate must be >= 0. Using default " & Convert.ToString(learningRateField) & ".")
			End If
		End Sub

		Private Sub validateSamplingTemp()
			If Math.close(samplingTempField, 0.0) OrElse samplingTempField < 0.0 OrElse samplingTempField > 1.0 + Math.eps() Then
				learningRateField = learningRateDefault
				Console.WriteLine("Learning rate must be in (0.0,1.0]. Using default " & Convert.ToString(learningRateField) & ".")
			End If
		End Sub

		Private Sub validateTrainingSampleLength()
			If trainingSampleLengthField < 1 Then
				trainingSampleLengthField = trainingSampleLengthDefault
				Console.WriteLine("Training sample length must be >= 1. Using default " & Convert.ToString(trainingSampleLengthField) & ".")
			End If
		End Sub

		' Gets the properties from the Properties class.
		Private Sub getProperties()
			hiddenSizeField = parseInt("hiddenSize", hiddenSizeDefault)
			layersField = parseInt("layers", layersDefault)
			sequenceLengthField = parseInt("sequenceLength", sequenceLengthDefault)
			learningRateField = parseDouble("learningRate", learningRateDefault)
			samplingTempField = parseDouble("samplingTemp", samplingTempDefault)
			printOptionsField = parseBool("printOptions", printOptionsDefault)
			trainingSampleLengthField = parseInt("trainingSampleLength", trainingSampleLengthDefault)
			loopAroundTimesField = parseInt("loopAroundTimes", loopAroundTimesDefault)
			sampleEveryNStepsField = parseInt("sampleEveryNSteps", sampleEveryNStepsDefault)
			snapshotEveryNSamplesField = parseInt("snapshotEveryNSamples", snapshotEveryNSamplesDefault)
			inputFileField = prop.getProperty("inputFile")
			useSingleLayerNetField = parseBool("useSingleLayerNet", useSingleLayerNetDefault)

			validateProperties()
		End Sub

		' Saves the properties in the Properties class.
		Private Sub setProperties()
			prop.setProperty("hiddenSize", Convert.ToString(hiddenSizeField))
			prop.setProperty("layers", Convert.ToString(layersField))
			prop.setProperty("sequenceLength", Convert.ToString(sequenceLengthField))
			prop.setProperty("learningRate", Convert.ToString(learningRateField))
			prop.setProperty("samplingTemp", Convert.ToString(samplingTempField))
			prop.setProperty("printOptions", Convert.ToString(printOptionsField))
			prop.setProperty("trainingSampleLength", Convert.ToString(trainingSampleLengthField))
			prop.setProperty("loopAroundTimes", Convert.ToString(loopAroundTimesField))
			prop.setProperty("sampleEveryNSteps", Convert.ToString(sampleEveryNStepsField))
			prop.setProperty("snapshotEveryNSamples", Convert.ToString(snapshotEveryNSamplesField))
			prop.setProperty("inputFile", inputFileField)
			prop.setProperty("useSingleLayerNet", Convert.ToString(useSingleLayerNetField))
		End Sub

		' Parses int, returns the default value if failed.
		Private Function parseInt(name As String, defaultValue As Integer) As Integer
			Try
				Return Integer.Parse(prop.getProperty(name))
			Catch __unusedFormatException1__ As FormatException
				Console.WriteLine("Error parsing " & name & ": " & prop.getProperty(name).ToString().ToString() & ", defaulting to: ".ToString() & Convert.ToString(defaultValue))
				Return defaultValue
			End Try
		End Function

		' Parses double, returns the default value if failed.
		Private Function parseDouble(name As String, defaultValue As Double) As Double
			Try
				Return Double.Parse(prop.getProperty(name))
			Catch __unusedFormatException1__ As FormatException
				Console.WriteLine("Error parsing " & name & ": " & prop.getProperty(name).ToString().ToString() & ", defaulting to: ".ToString() & Convert.ToString(defaultValue))
				Return defaultValue
			End Try
		End Function

		' Parses boolean, returns the default value if failed.
		Private Function parseBool(name As String, defaultValue As Boolean) As Boolean
			Try
				Return Boolean.Parse(prop.getProperty(name))
			Catch __unusedFormatException1__ As FormatException
				Console.WriteLine("Error parsing " & name & ": " & prop.getProperty(name).ToString().ToString() & ", defaulting to: ".ToString() & Convert.ToString(defaultValue))
				Return defaultValue
			End Try
		End Function
	End Class
End Namespace