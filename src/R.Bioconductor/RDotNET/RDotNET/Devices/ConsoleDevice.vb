#Region "Microsoft.VisualBasic::17a8fd931be27dbd4f0602d18c8b67e1, RDotNET\RDotNET\Devices\ConsoleDevice.vb"

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

    ' 	Class ConsoleDevice
    ' 
    ' 	    Function: AddHistory, Ask, ChooseFile, LoadHistory, ReadConsole
    '                SaveHistory, ShowFiles
    ' 
    ' 	    Sub: Busy, Callback, CleanUp, ClearErrorConsole, EditFile
    '           FlushConsole, ResetConsole, ShowMessage, Suicide, WriteConsole
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports System.IO

Namespace Devices
	''' <summary>
	''' The default IO device, using the System.Console
	''' </summary>
	Public Class ConsoleDevice
		Implements ICharacterDevice
		#Region "ICharacterDevice Members"

		''' <summary>
		''' Read input from console.
		''' </summary>
		''' <param name="prompt">The prompt message.</param>
		''' <param name="capacity">Parameter is ignored</param>
		''' <param name="history">Parameter is ignored</param>
		''' <returns>The input.</returns>
		Public Function ReadConsole(prompt As String, capacity As Integer, history As Boolean) As String Implements ICharacterDevice.ReadConsole
			Console.Write(prompt)
			Return Console.ReadLine()
		End Function

		''' <summary>
		''' Write output on console.
		''' </summary>
		''' <param name="output">The output message</param>
		''' <param name="length">Parameter is ignored</param>
		''' <param name="outputType">Parameter is ignored</param>
		Public Sub WriteConsole(output As String, length As Integer, outputType As ConsoleOutputType) Implements ICharacterDevice.WriteConsole
			Console.Write(output)
		End Sub

		''' <summary>
		''' Displays the message to the System.Console.
		''' </summary>
		''' <param name="message">The message.</param>
		Public Sub ShowMessage(message As String) Implements ICharacterDevice.ShowMessage
			Console.Write(message)
		End Sub

		''' <summary>
		''' This implementation has no effect
		''' </summary>
		''' <param name="which"></param>
		Public Sub Busy(which As BusyType) Implements ICharacterDevice.Busy
		End Sub

		''' <summary>
		''' This implementation has no effect
		''' </summary>
		Public Sub Callback() Implements ICharacterDevice.Callback
		End Sub

		''' <summary>
		''' Ask a question to the user with three choices.
		''' </summary>
		''' <param name="question">The question to write to the console</param>
		''' <returns></returns>
		Public Function Ask(question As String) As YesNoCancel Implements ICharacterDevice.Ask
			Console.Write("{0} [y/n/c]: ", question)
			Dim input As String = Console.ReadLine()
			If Not String.IsNullOrEmpty(input) Then
				Select Case [Char].ToLower(input(0))
					Case "y"C
						Return YesNoCancel.Yes

					Case "n"C
						Return YesNoCancel.No

					Case "c"C
						Return YesNoCancel.Cancel
				End Select
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Write the message to standard error output stream.
		''' </summary>
		''' <param name="message"></param>
		Public Sub Suicide(message As String) Implements ICharacterDevice.Suicide
			Console.[Error].WriteLine(message)
			CleanUp(StartupSaveAction.Suicide, 2, False)
		End Sub

		''' <summary>
		''' Clears the System.Console
		''' </summary>
		Public Sub ResetConsole() Implements ICharacterDevice.ResetConsole
			Console.Clear()
		End Sub

		''' <summary>
		''' Flush the System.Console
		''' </summary>
		Public Sub FlushConsole() Implements ICharacterDevice.FlushConsole
			Console.Write(String.Empty)
		End Sub

		''' <summary>
		''' Clears the System.Console
		''' </summary>
		Public Sub ClearErrorConsole() Implements ICharacterDevice.ClearErrorConsole
			Console.Clear()
		End Sub

		''' <summary>
		''' Terminate the process with the given status
		''' </summary>
		''' <param name="saveAction">Parameter is ignored</param>
		''' <param name="status">The status code on exit</param>
		''' <param name="runLast">Parameter is ignored</param>
		Public Sub CleanUp(saveAction As StartupSaveAction, status As Integer, runLast As Boolean) Implements ICharacterDevice.CleanUp
			Environment.[Exit](status)
		End Sub

		''' <summary>
		''' Displays the contents of files.
		''' </summary>
		''' <remarks>
		''' Only Unix.
		''' </remarks>
		''' <param name="files">The file paths.</param>
		''' <param name="headers">The header before the contents is printed.</param>
		''' <param name="title">Ignored by this implementation</param>
		''' <param name="delete">Whether the file will be deleted.</param>
		''' <param name="pager">Ignored by this implementation</param>
		''' <returns>true on successful completion, false if an IOException was caught</returns>
		Public Function ShowFiles(files As String(), headers As String(), title As String, delete As Boolean, pager As String) As Boolean Implements ICharacterDevice.ShowFiles
			Dim count As Integer = files.Length
			For index As Integer = 0 To count - 1
				Try
					Console.WriteLine(headers)
					Console.WriteLine(File.ReadAllText(files(index)))
					If delete Then
						File.Delete(files(index))
					End If
				Catch generatedExceptionName As IOException
					Return False
				End Try
			Next
			Return True
		End Function

		''' <summary>
		''' Chooses a file.
		''' </summary>
		''' <remarks>
		''' Only Unix.
		''' </remarks>
		''' <param name="create">To be created.</param>
		''' <returns>The length of input.</returns>
		Public Function ChooseFile(create As Boolean) As String Implements ICharacterDevice.ChooseFile
			Dim path As String = Console.ReadLine()
			If String.IsNullOrWhiteSpace(path) Then
				Return Nothing
			End If
			If create AndAlso Not File.Exists(path) Then
				File.Create(path).Close()
			End If
			If File.Exists(path) Then
				Return path
			End If
			Return Nothing
		End Function

		''' <summary>
		''' This implementation does nothing
		''' </summary>
		''' <param name="file"></param>
		Public Sub EditFile(file As String) Implements ICharacterDevice.EditFile
		End Sub

		''' <summary>
		''' Return the NULL SEXP; no other effect
		''' </summary>
		''' <param name="call"></param>
		''' <param name="operation"></param>
		''' <param name="args"></param>
		''' <param name="environment"></param>
		''' <returns></returns>
		Public Function LoadHistory([call] As Language, operation As SymbolicExpression, args As Pairlist, environment As REnvironment) As SymbolicExpression Implements ICharacterDevice.LoadHistory
			Return environment.Engine.NilValue
		End Function

		''' <summary>
		''' Return the NULL SEXP; no other effect
		''' </summary>
		''' <param name="call"></param>
		''' <param name="operation"></param>
		''' <param name="args"></param>
		''' <param name="environment"></param>
		''' <returns></returns>
		Public Function SaveHistory([call] As Language, operation As SymbolicExpression, args As Pairlist, environment As REnvironment) As SymbolicExpression Implements ICharacterDevice.SaveHistory
			Return environment.Engine.NilValue
		End Function

		''' <summary>
		''' Return the NULL SEXP; no other effect
		''' </summary>
		''' <param name="call"></param>
		''' <param name="operation"></param>
		''' <param name="args"></param>
		''' <param name="environment"></param>
		''' <returns></returns>
		Public Function AddHistory([call] As Language, operation As SymbolicExpression, args As Pairlist, environment As REnvironment) As SymbolicExpression Implements ICharacterDevice.AddHistory
			Return environment.Engine.NilValue
		End Function

		#End Region
	End Class
End Namespace

