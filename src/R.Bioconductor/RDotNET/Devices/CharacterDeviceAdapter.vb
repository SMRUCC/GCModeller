Imports RDotNet.Internals
Imports RDotNet.Internals.Unix
Imports RDotNet.NativeLibrary
Imports System
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Devices
    Friend Class CharacterDeviceAdapter
        Implements IDisposable
        ''' <summary>
        ''' When R calls the character device (unamanged R calling managed code),
        ''' it sometimes calls the method with 'this == null' when writing/reading
        ''' from console (this seems to happen on Mono and may be a bug).
        '''
        ''' The (somewhat incorrect) workaround is to keep the last device in a static
        ''' field and use it when 'this == null' (the check is done in 'this.Device').
        ''' This workarounds: http://rdotnet.codeplex.com/workitem/154
        ''' </summary>
        Private Shared lastDevice As ICharacterDevice
        Private ReadOnly deviceField As ICharacterDevice
        Private engineField As REngine
        Private suicideDelegate As ptr_R_Suicide
        Private showMessageDelegate As ptr_R_ShowMessage
        Private readConsoleDelegate As ptr_R_ReadConsole
        Private writeConsoleDelegate As ptr_R_WriteConsole
        Private writeConsoleExDelegate As ptr_R_WriteConsoleEx
        Private resetConsoleDelegate As ptr_R_ResetConsole
        Private flushConsoleDelegate As ptr_R_FlushConsole
        Private clearerrConsoleDelegate As ptr_R_ClearerrConsole
        Private busyDelegate As ptr_R_Busy
        Private cleanUpDelegate As ptr_R_CleanUp
        Private showFilesDelegate As ptr_R_ShowFiles
        Private chooseFileDelegate As ptr_R_ChooseFile
        Private editFileDelegate As ptr_R_EditFile
        Private loadHistoryDelegate As ptr_R_loadhistory
        Private saveHistoryDelegate As ptr_R_savehistory
        Private addHistoryDelegate As ptr_R_addhistory

        ''' <summary>
        ''' Creates an instance.
        ''' </summary>
        ''' <param name="device">The implementation.</param>
        Public Sub New(ByVal device As ICharacterDevice)
            If device Is Nothing Then
                Throw New ArgumentNullException("device")
            End If

            lastDevice = device
            deviceField = device
        End Sub

        ''' <summary>
        ''' Gets the implementation of <see cref="ICharacterDevice"/> interface.
        ''' </summary>
        Public ReadOnly Property Device As ICharacterDevice
            Get

                If Me Is Nothing Then
                    Return lastDevice
                Else
                    Return deviceField
                End If
            End Get
        End Property

        Private ReadOnly Property Engine As REngine
            Get
                Return engineField
            End Get
        End Property

        Protected Function GetFunction(Of TDelegate As Class)() As TDelegate
            Return Engine.GetFunction(Of TDelegate)()
        End Function

#Region "IDisposable Members"

        Public Sub Dispose() Implements IDisposable.Dispose
            GC.KeepAlive(Me)
        End Sub

#End Region

        Friend Sub Install(ByVal engine As REngine, ByVal parameter As StartupParameter)
            engineField = engine

            Select Case NativeUtility.GetPlatform()
                Case PlatformID.Win32NT
                    SetupWindowsDevice(parameter)
                Case PlatformID.MacOSX, PlatformID.Unix
                    SetupUnixDevice()
            End Select
        End Sub

        Private Sub SetupWindowsDevice(ByVal parameter As StartupParameter)
            If Equals(parameter.RHome, Nothing) Then
                parameter.start.rhome = ToNativeUnixPath(NativeUtility.GetRHomeEnvironmentVariable())
            End If

            If Equals(parameter.Home, Nothing) Then
                Dim home As String = Marshal.PtrToStringAnsi(Engine.GetFunction(Of getValue)("getRUser")())
                parameter.start.home = ToNativeUnixPath(home)
            End If

            parameter.start.ReadConsole = AddressOf ReadConsole
            parameter.start.WriteConsole = AddressOf WriteConsole
            parameter.start.WriteConsoleEx = AddressOf WriteConsoleEx
            parameter.start.CallBack = AddressOf Callback
            parameter.start.ShowMessage = AddressOf ShowMessage
            parameter.start.YesNoCancel = AddressOf Ask
            parameter.start.Busy = AddressOf Busy
        End Sub

        Private Shared Function ToNativeUnixPath(ByVal path As String) As IntPtr
            Return Marshal.StringToHGlobalAnsi(ConvertSeparatorToUnixStylePath(path))
        End Function

        Private Sub SetupUnixDevice()
            Dim suicidePointer = engineField.DangerousGetHandle("ptr_R_Suicide")
            suicideDelegate = AddressOf Suicide
            Dim newSuicide = Marshal.GetFunctionPointerForDelegate(suicideDelegate)
            Marshal.WriteIntPtr(suicidePointer, newSuicide)
            Dim showMessagePointer = engineField.DangerousGetHandle("ptr_R_ShowMessage")
            showMessageDelegate = AddressOf ShowMessage
            Dim newShowMessage = Marshal.GetFunctionPointerForDelegate(showMessageDelegate)
            Marshal.WriteIntPtr(showMessagePointer, newShowMessage)
            Dim readConsolePointer = engineField.DangerousGetHandle("ptr_R_ReadConsole")
            readConsoleDelegate = AddressOf ReadConsole
            Dim newReadConsole = Marshal.GetFunctionPointerForDelegate(readConsoleDelegate)
            Marshal.WriteIntPtr(readConsolePointer, newReadConsole)
            Dim writeConsolePointer = engineField.DangerousGetHandle("ptr_R_WriteConsole")
            writeConsoleDelegate = AddressOf WriteConsole
            Dim newWriteConsole = Marshal.GetFunctionPointerForDelegate(writeConsoleDelegate)
            Marshal.WriteIntPtr(writeConsolePointer, newWriteConsole)
            Dim writeConsoleExPointer = engineField.DangerousGetHandle("ptr_R_WriteConsoleEx")
            writeConsoleExDelegate = AddressOf WriteConsoleEx
            Dim newWriteConsoleEx = Marshal.GetFunctionPointerForDelegate(writeConsoleExDelegate)
            Marshal.WriteIntPtr(writeConsoleExPointer, newWriteConsoleEx)
            Dim resetConsolePointer = engineField.DangerousGetHandle("ptr_R_ResetConsole")
            resetConsoleDelegate = AddressOf ResetConsole
            Dim newResetConsole = Marshal.GetFunctionPointerForDelegate(resetConsoleDelegate)
            Marshal.WriteIntPtr(resetConsolePointer, newResetConsole)
            Dim flushConsolePointer = engineField.DangerousGetHandle("ptr_R_FlushConsole")
            flushConsoleDelegate = AddressOf FlushConsole
            Dim newFlushConsole = Marshal.GetFunctionPointerForDelegate(flushConsoleDelegate)
            Marshal.WriteIntPtr(flushConsolePointer, newFlushConsole)
            Dim clearerrConsolePointer = engineField.DangerousGetHandle("ptr_R_ClearerrConsole")
            clearerrConsoleDelegate = AddressOf ClearErrorConsole
            Dim newClearerrConsole = Marshal.GetFunctionPointerForDelegate(clearerrConsoleDelegate)
            Marshal.WriteIntPtr(clearerrConsolePointer, newClearerrConsole)
            Dim busyPointer = engineField.DangerousGetHandle("ptr_R_Busy")
            busyDelegate = AddressOf Busy
            Dim newBusy = Marshal.GetFunctionPointerForDelegate(busyDelegate)
            Marshal.WriteIntPtr(busyPointer, newBusy)
            Dim cleanUpPointer = engineField.DangerousGetHandle("ptr_R_CleanUp")
            cleanUpDelegate = AddressOf CleanUp
            Dim newCleanUp = Marshal.GetFunctionPointerForDelegate(cleanUpDelegate)
            Marshal.WriteIntPtr(cleanUpPointer, newCleanUp)
            Dim showFilesPointer = engineField.DangerousGetHandle("ptr_R_ShowFiles")
            showFilesDelegate = AddressOf ShowFiles
            Dim newShowFiles = Marshal.GetFunctionPointerForDelegate(showFilesDelegate)
            Marshal.WriteIntPtr(showFilesPointer, newShowFiles)
            Dim chooseFilePointer = engineField.DangerousGetHandle("ptr_R_ChooseFile")
            chooseFileDelegate = AddressOf ChooseFile
            Dim newChooseFile = Marshal.GetFunctionPointerForDelegate(chooseFileDelegate)
            Marshal.WriteIntPtr(chooseFilePointer, newChooseFile)
            Dim editFilePointer = engineField.DangerousGetHandle("ptr_R_EditFile")
            editFileDelegate = AddressOf EditFile
            Dim newEditFile = Marshal.GetFunctionPointerForDelegate(editFileDelegate)
            Marshal.WriteIntPtr(editFilePointer, newEditFile)
            Dim loadHistoryPointer = engineField.DangerousGetHandle("ptr_R_loadhistory")
            loadHistoryDelegate = AddressOf LoadHistory
            Dim newLoadHistory = Marshal.GetFunctionPointerForDelegate(loadHistoryDelegate)
            Marshal.WriteIntPtr(loadHistoryPointer, newLoadHistory)
            Dim saveHistoryPointer = engineField.DangerousGetHandle("ptr_R_savehistory")
            saveHistoryDelegate = AddressOf SaveHistory
            Dim newSaveHistory = Marshal.GetFunctionPointerForDelegate(saveHistoryDelegate)
            Marshal.WriteIntPtr(saveHistoryPointer, newSaveHistory)
            Dim addHistoryPointer = engineField.DangerousGetHandle("ptr_R_addhistory")
            addHistoryDelegate = AddressOf AddHistory
            Dim newAddHistory = Marshal.GetFunctionPointerForDelegate(addHistoryDelegate)
            Marshal.WriteIntPtr(addHistoryPointer, newAddHistory)
        End Sub

        Private Shared Function ConvertSeparatorToUnixStylePath(ByVal path As String) As String
            Return path.Replace(IO.Path.DirectorySeparatorChar, "/"c)
        End Function

        Private Function ReadConsole(ByVal prompt As String, ByVal buffer As StringBuilder, ByVal count As Integer, ByVal history As Boolean) As Boolean
            buffer.Clear()
            Dim input = Device.ReadConsole(prompt, count, history)
            buffer.Append(CStr(input)).Append(Microsoft.VisualBasic.Constants.vbLf) ' input must end with '\n\0' ('\0' is appended during marshalling).
            Return Not Equals(input, Nothing)
        End Function

        Private Sub WriteConsole(ByVal buffer As String, ByVal length As Integer)
            WriteConsoleEx(buffer, length, ConsoleOutputType.None)
        End Sub

        Private Sub WriteConsoleEx(ByVal buffer As String, ByVal length As Integer, ByVal outputType As ConsoleOutputType)
            Device.WriteConsole(buffer, length, outputType)
        End Sub

        Private Sub ShowMessage(ByVal message As String)
            Device.ShowMessage(message)
        End Sub

        Private Sub Busy(ByVal which As BusyType)
            Device.Busy(which)
        End Sub

        Private Sub Callback()
            Device.Callback()
        End Sub

        Private Function Ask(ByVal question As String) As YesNoCancel
            Return Device.Ask(question)
        End Function

        Private Sub Suicide(ByVal message As String)
            Device.Suicide(message)
        End Sub

        Private Sub ResetConsole()
            Device.ResetConsole()
        End Sub

        Private Sub FlushConsole()
            Device.FlushConsole()
        End Sub

        Private Sub ClearErrorConsole()
            Device.ClearErrorConsole()
        End Sub

        Private Sub CleanUp(ByVal saveAction As StartupSaveAction, ByVal status As Integer, ByVal runLast As Boolean)
            Device.CleanUp(saveAction, status, runLast)
        End Sub

        Private Function ShowFiles(ByVal count As Integer, ByVal files As String(), ByVal headers As String(), ByVal title As String, ByVal delete As Boolean, ByVal pager As String) As Boolean
            Return Device.ShowFiles(files, headers, title, delete, pager)
        End Function

        Private Function ChooseFile(ByVal create As Boolean, ByVal buffer As StringBuilder, ByVal length As Integer) As Integer
            Dim path = Device.ChooseFile(create)
            Return If(Equals(path, Nothing), 0, Encoding.ASCII.GetByteCount(path))
        End Function

        Private Sub EditFile(ByVal file As String)
            Device.EditFile(file)
        End Sub

        Private Function CallDeviceFunction(ByVal [call] As IntPtr, ByVal operation As IntPtr, ByVal args As IntPtr, ByVal environment As IntPtr, ByVal func As Func(Of Language, SymbolicExpression, Pairlist, REnvironment, SymbolicExpression)) As IntPtr
            Dim c = New Language(Engine, [call])
            Dim op = New SymbolicExpression(Engine, operation)
            Dim arglist = New Pairlist(Engine, args)
            Dim env = New REnvironment(Engine, environment)
            Dim result = func(c, op, arglist, env)
            Return result.DangerousGetHandle()
        End Function

        Private Function LoadHistory(ByVal [call] As IntPtr, ByVal operation As IntPtr, ByVal args As IntPtr, ByVal environment As IntPtr) As IntPtr
            Return CallDeviceFunction([call], operation, args, environment, New Func(Of Language, SymbolicExpression, Pairlist, REnvironment, SymbolicExpression)(AddressOf Device.LoadHistory))
        End Function

        Private Function SaveHistory(ByVal [call] As IntPtr, ByVal operation As IntPtr, ByVal args As IntPtr, ByVal environment As IntPtr) As IntPtr
            Return CallDeviceFunction([call], operation, args, environment, New Func(Of Language, SymbolicExpression, Pairlist, REnvironment, SymbolicExpression)(AddressOf Device.SaveHistory))
        End Function

        Private Function AddHistory(ByVal [call] As IntPtr, ByVal operation As IntPtr, ByVal args As IntPtr, ByVal environment As IntPtr) As IntPtr
            Return CallDeviceFunction([call], operation, args, environment, New Func(Of Language, SymbolicExpression, Pairlist, REnvironment, SymbolicExpression)(AddressOf Device.AddHistory))
        End Function

#Region "Nested type: getValue"

        <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
        Private Delegate Function getValue() As IntPtr

#End Region
    End Class
End Namespace
