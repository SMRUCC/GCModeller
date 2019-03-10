Imports RDotNet.Internals

Namespace Devices
	''' <summary>
	''' A console class handles user's inputs and outputs.
	''' </summary>
	Public Interface ICharacterDevice
		''' <summary>
		''' Read input from console.
		''' </summary>
		''' <param name="prompt">The prompt message.</param>
		''' <param name="capacity">The buffer's capacity in byte.</param>
		''' <param name="history">Whether the input should be added to any command history.</param>
		''' <returns>The input.</returns>
		Function ReadConsole(prompt As String, capacity As Integer, history As Boolean) As String

		''' <summary>
		''' Write output on console.
		''' </summary>
		''' <param name="output">The output message</param>
		''' <param name="length">The output's length in byte.</param>
		''' <param name="outputType">The output type.</param>
		Sub WriteConsole(output As String, length As Integer, outputType As ConsoleOutputType)

		''' <summary>
		''' Displays the message.
		''' </summary>
		''' <remarks>
		''' It should be brought to the user's attention immediately.
		''' </remarks>
		''' <param name="message">The message.</param>
		Sub ShowMessage(message As String)

		''' <summary>
		''' Invokes actions.
		''' </summary>
		''' <param name="which">The state.</param>
		Sub Busy(which As BusyType)

		''' <summary>
		''' Callback function.
		''' </summary>
		Sub Callback()

		''' <summary>
		''' Asks user's decision.
		''' </summary>
		''' <param name="question">The question.</param>
		''' <returns>User's decision.</returns>
		Function Ask(question As String) As YesNoCancel

		''' <summary>
		''' Abort R environment itself as soon as possible.
		''' </summary>
		''' <remarks>
		''' Only Unix.
		''' </remarks>
		''' <param name="message">The message.</param>
		Sub Suicide(message As String)

		''' <summary>
		''' Clear the console.
		''' </summary>
		''' <remarks>
		''' Only Unix.
		''' </remarks>
		Sub ResetConsole()

		''' <summary>
		''' Flush the console.
		''' </summary>
		''' <remarks>
		''' Only Unix.
		''' </remarks>
		Sub FlushConsole()

		''' <summary>
		''' Clear the error console.
		''' </summary>
		''' <remarks>
		''' Only Unix.
		''' </remarks>
		Sub ClearErrorConsole()

		''' <summary>
		''' Invokes any actions which occur at system termination.
		''' </summary>
		''' <remarks>
		''' Only Unix.
		''' </remarks>
		''' <param name="saveAction">The save type.</param>
		''' <param name="status">Exit code.</param>
		''' <param name="runLast">Whether R should execute <code>.Last</code>.</param>
		Sub CleanUp(saveAction As StartupSaveAction, status As Integer, runLast As Boolean)

		''' <summary>
		''' Displays the contents of files.
		''' </summary>
		''' <remarks>
		''' Only Unix.
		''' </remarks>
		''' <param name="files">The file paths.</param>
		''' <param name="headers">The header before the contents is printed.</param>
		''' <param name="title">The window title.</param>
		''' <param name="delete">Whether the file will be deleted.</param>
		''' <param name="pager">The pager used.</param>
		''' <returns></returns>
		Function ShowFiles(files As String(), headers As String(), title As String, delete As Boolean, pager As String) As Boolean

		''' <summary>
		''' Chooses a file.
		''' </summary>
		''' <remarks>
		''' Only Unix.
		''' </remarks>
		''' <param name="create">To be created.</param>
		''' <returns>The length of input.</returns>
		Function ChooseFile(create As Boolean) As String

		''' <remarks>
		''' Only Unix.
		''' </remarks>
		Sub EditFile(file As String)

		''' <remarks>
		''' Only Unix.
		''' </remarks>
		Function LoadHistory([call] As Language, operation As SymbolicExpression, args As Pairlist, environment As REnvironment) As SymbolicExpression

		''' <remarks>
		''' Only Unix.
		''' </remarks>
		Function SaveHistory([call] As Language, operation As SymbolicExpression, args As Pairlist, environment As REnvironment) As SymbolicExpression

		''' <remarks>
		''' Only Unix.
		''' </remarks>
		Function AddHistory([call] As Language, operation As SymbolicExpression, args As Pairlist, environment As REnvironment) As SymbolicExpression
	End Interface
End Namespace
