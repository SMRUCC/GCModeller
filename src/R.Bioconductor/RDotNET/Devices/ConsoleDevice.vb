Imports RDotNet.Internals
Imports System
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
        Public Function ReadConsole(ByVal prompt As String, ByVal capacity As Integer, ByVal history As Boolean) As String Implements ICharacterDevice.ReadConsole
            Console.Write(prompt)
            Return Console.ReadLine()
        End Function

        ''' <summary>
        ''' Write output on console.
        ''' </summary>
        ''' <param name="output">The output message</param>
        ''' <param name="length">Parameter is ignored</param>
        ''' <param name="outputType">Parameter is ignored</param>
        Public Sub WriteConsole(ByVal output As String, ByVal length As Integer, ByVal outputType As ConsoleOutputType) Implements ICharacterDevice.WriteConsole
            Console.Write(output)
        End Sub

        ''' <summary>
        ''' Displays the message to the System.Console.
        ''' </summary>
        ''' <param name="message">The message.</param>
        Public Sub ShowMessage(ByVal message As String) Implements ICharacterDevice.ShowMessage
            Console.Write(message)
        End Sub

        ''' <summary>
        ''' This implementation has no effect
        ''' </summary>
        ''' <param name="which"></param>
        Public Sub Busy(ByVal which As BusyType) Implements ICharacterDevice.Busy
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
        Public Function Ask(ByVal question As String) As YesNoCancel Implements ICharacterDevice.Ask
            Console.Write("{0} [y/n/c]: ", question)
            Dim input As String = Console.ReadLine()

            If Not String.IsNullOrEmpty(input) Then
                Select Case Char.ToLower(input(0))
                    Case "y"c
                        Return YesNoCancel.Yes
                    Case "n"c
                        Return YesNoCancel.No
                    Case "c"c
                        Return YesNoCancel.Cancel
                End Select
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' Write the message to standard error output stream.
        ''' </summary>
        ''' <param name="message"></param>
        Public Sub Suicide(ByVal message As String) Implements ICharacterDevice.Suicide
            Console.Error.WriteLine(message)
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
        Public Sub CleanUp(ByVal saveAction As StartupSaveAction, ByVal status As Integer, ByVal runLast As Boolean) Implements ICharacterDevice.CleanUp
            Environment.Exit(status)
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
        Public Function ShowFiles(ByVal files As String(), ByVal headers As String(), ByVal title As String, ByVal delete As Boolean, ByVal pager As String) As Boolean Implements ICharacterDevice.ShowFiles
            Dim count = files.Length

            For index = 0 To count - 1

                Try
                    Console.WriteLine(headers)
                    Console.WriteLine(File.ReadAllText(files(index)))

                    If delete Then
                        File.Delete(files(index))
                    End If

                Catch __unusedIOException1__ As IOException
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
        Public Function ChooseFile(ByVal create As Boolean) As String Implements ICharacterDevice.ChooseFile
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
        Public Sub EditFile(ByVal file As String) Implements ICharacterDevice.EditFile
        End Sub

        ''' <summary>
        ''' Return the NULL SEXP; no other effect
        ''' </summary>
        ''' <param name="call"></param>
        ''' <param name="operation"></param>
        ''' <param name="args"></param>
        ''' <param name="environment"></param>
        ''' <returns></returns>
        Public Function LoadHistory(ByVal [call] As Language, ByVal operation As SymbolicExpression, ByVal args As Pairlist, ByVal environment As REnvironment) As SymbolicExpression Implements ICharacterDevice.LoadHistory
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
        Public Function SaveHistory(ByVal [call] As Language, ByVal operation As SymbolicExpression, ByVal args As Pairlist, ByVal environment As REnvironment) As SymbolicExpression Implements ICharacterDevice.SaveHistory
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
        Public Function AddHistory(ByVal [call] As Language, ByVal operation As SymbolicExpression, ByVal args As Pairlist, ByVal environment As REnvironment) As SymbolicExpression Implements ICharacterDevice.AddHistory
            Return environment.Engine.NilValue
        End Function

#End Region
    End Class
End Namespace
