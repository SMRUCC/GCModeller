Imports System.IO

Namespace RNN

	''' <summary>
	''' Application options.
	''' </summary>
	Public Class Options

		' * Model parameters ** 

		Public Property hiddenSizeField As Integer = 100 ' Size of a single RNN layer hidden state.

		Public Property layersField As Integer = 2 ' How many layers in a net?


		' * Training parameters ** 

		Public Property sequenceLengthField As Integer = 50 ' How many steps to unroll during training?

		Public Property learningRateField As Double = 0.1 ' The network learning rate.


		' * Sampling parameters **

		' Sampling temperature (0.0, 1.0]. Lower
		' temperature means more conservative
		' predictions.
		Public Property samplingTempField As Double = 1.0

		' * Other options ** 

		Public Property printOptionsField As Boolean = True ' Print options at the start.

		Public Property trainingSampleLengthField As Integer = 400 ' Length of a sample during training.

		Public Property snapshotEveryNSamplesField As Integer = 50 ' Take a network's snapshot every N samples.

		Public Property loopAroundTimesField As Integer = 0 ' Loop around the training data this many times.


		Public Property sampleEveryNStepsField As Integer = 100 ' Take a sample during training every N steps.

		Public Property inputFileField As String = "input.txt" ' The training data.

		Public Property useSingleLayerNetField As Boolean = False ' Use the simple, single layer net.

	End Class
End Namespace