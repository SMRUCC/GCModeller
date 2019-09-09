#Region "Microsoft.VisualBasic::5453de28d2b2ca53d0b254197d169e0f, RDotNET\RDotNET\Devices\NullCharacterDevice.vb"

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

    ' 	Class NullCharacterDevice
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

Namespace Devices
	''' <summary>
	''' A sink with (almost) no effect, similar in purpose to /dev/null
	''' </summary>
	Public Class NullCharacterDevice
		Implements ICharacterDevice
		#Region "ICharacterDevice Members"

		''' <summary>
		''' Read input from console.
		''' </summary>
		''' <param name="prompt">The prompt message.</param>
		''' <param name="capacity">The buffer's capacity in byte.</param>
		''' <param name="history">Whether the input should be added to any command history.</param>
		''' <returns>A null reference</returns>
		Public Function ReadConsole(prompt As String, capacity As Integer, history As Boolean) As String Implements ICharacterDevice.ReadConsole
			Return Nothing
		End Function

		''' <summary>
		''' This implementation has no effect
		''' </summary>
		''' <param name="output">The output message</param>
		''' <param name="length">The output's length in byte.</param>
		''' <param name="outputType">The output type.</param>
		Public Sub WriteConsole(output As String, length As Integer, outputType As ConsoleOutputType) Implements ICharacterDevice.WriteConsole
		End Sub

		''' <summary>
		''' This implementation has no effect
		''' </summary>
		''' <param name="message">The message.</param>
		Public Sub ShowMessage(message As String) Implements ICharacterDevice.ShowMessage
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
		''' Always return the default value of the YesNoCancel enum (yes?)
		''' </summary>
		''' <param name="question"></param>
		''' <returns></returns>
		Public Function Ask(question As String) As YesNoCancel Implements ICharacterDevice.Ask
			Return Nothing
		End Function

		''' <summary>
		''' Ignores the message, but triggers a CleanUp, a termination with no action.
		''' </summary>
		''' <param name="message"></param>
		Public Sub Suicide(message As String) Implements ICharacterDevice.Suicide
			CleanUp(StartupSaveAction.Suicide, 2, False)
		End Sub

		''' <summary>
		''' This implementation has no effect
		''' </summary>
		Public Sub ResetConsole() Implements ICharacterDevice.ResetConsole
		End Sub

		''' <summary>
		''' This implementation has no effect
		''' </summary>
		Public Sub FlushConsole() Implements ICharacterDevice.FlushConsole
		End Sub

		''' <summary>
		''' This implementation has no effect
		''' </summary>
		Public Sub ClearErrorConsole() Implements ICharacterDevice.ClearErrorConsole
		End Sub

		''' <summary>
		''' Clean up action; exit the process with a specified status
		''' </summary>
		''' <param name="saveAction">Ignored</param>
		''' <param name="status"></param>
		''' <param name="runLast">Ignored</param>
		Public Sub CleanUp(saveAction As StartupSaveAction, status As Integer, runLast As Boolean) Implements ICharacterDevice.CleanUp
			Environment.[Exit](status)
		End Sub

		''' <summary>
		''' Always returns false, no other side effect
		''' </summary>
		''' <param name="files"></param>
		''' <param name="headers"></param>
		''' <param name="title"></param>
		''' <param name="delete"></param>
		''' <param name="pager"></param>
		''' <returns>Returns false</returns>
		Public Function ShowFiles(files As String(), headers As String(), title As String, delete As Boolean, pager As String) As Boolean Implements ICharacterDevice.ShowFiles
			Return False
		End Function

		''' <summary>
		''' Always returns null; no other side effect
		''' </summary>
		''' <param name="create">ignored</param>
		''' <returns>null</returns>
		Public Function ChooseFile(create As Boolean) As String Implements ICharacterDevice.ChooseFile
			Return Nothing
		End Function

		''' <summary>
		''' No effect
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

