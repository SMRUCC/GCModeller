#Region "Microsoft.VisualBasic::f01921903760aa72ea8fe1b061febe06, RDotNET\RDotNET\Devices\CharacterDeviceAdapter.vb"

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

    ' 	Class CharacterDeviceAdapter
    ' 
    ' 	    Properties: Device, Engine
    ' 
    ' 	    Constructor: (+1 Overloads) Sub New
    ' 
    ' 	    Function: AddHistory, Ask, CallDeviceFunction, ChooseFile, ConvertSeparatorToUnixStylePath
    '                GetFunction, LoadHistory, ReadConsole, SaveHistory, ShowFiles
    '                ToNativeUnixPath
    ' 
    ' 	    Sub: Busy, Callback, CleanUp, ClearErrorConsole, Dispose
    '           EditFile, FlushConsole, Install, ResetConsole, SetupUnixDevice
    '           SetupWindowsDevice, ShowMessage, Suicide, WriteConsole, WriteConsoleEx
    ' 		Delegate Function
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports RDotNet.Internals.Unix
Imports RDotNet.NativeLibrary
Imports System.IO
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

		Private ReadOnly m_device As ICharacterDevice
		Private m_engine As REngine

		''' <summary>
		''' Creates an instance.
		''' </summary>
		''' <param name="device">The implementation.</param>
		Public Sub New(device As ICharacterDevice)
			If device Is Nothing Then
				Throw New ArgumentNullException("device")
			End If
			lastDevice = device
			Me.m_device = device
		End Sub

		''' <summary>
		''' Gets the implementation of <see cref="ICharacterDevice"/> interface.
		''' </summary>
		Public ReadOnly Property Device() As ICharacterDevice
			Get
				If Me Is Nothing Then
					Return lastDevice
				Else
					Return Me.m_device
				End If
			End Get
		End Property

		Private ReadOnly Property Engine() As REngine
			Get
				Return Me.m_engine
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

		Friend Sub Install(engine As REngine, parameter As StartupParameter)
			Me.m_engine = engine
			Select Case NativeUtility.GetPlatform()
				Case PlatformID.Win32NT
					SetupWindowsDevice(parameter)
					Exit Select

				Case PlatformID.MacOSX, PlatformID.Unix
					SetupUnixDevice()
					Exit Select
			End Select
		End Sub

		Private Sub SetupWindowsDevice(parameter As StartupParameter)
			If parameter.RHome Is Nothing Then
				parameter.start.rhome = ToNativeUnixPath(NativeUtility.GetRHomeEnvironmentVariable())
			End If
			If parameter.Home Is Nothing Then
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

		Private Shared Function ToNativeUnixPath(path As String) As IntPtr
			Return Marshal.StringToHGlobalAnsi(ConvertSeparatorToUnixStylePath(path))
		End Function

		Private Sub SetupUnixDevice()
			Dim suicidePointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_Suicide")
			Dim newSuicide As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf Suicide, ptr_R_Suicide))
			Marshal.WriteIntPtr(suicidePointer, newSuicide)
			Dim showMessagePointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_ShowMessage")
			Dim newShowMessage As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf ShowMessage, ptr_R_ShowMessage))
			Marshal.WriteIntPtr(showMessagePointer, newShowMessage)
			Dim readConsolePointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_ReadConsole")
			Dim newReadConsole As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf ReadConsole, ptr_R_ReadConsole))
			Marshal.WriteIntPtr(readConsolePointer, newReadConsole)
			Dim writeConsolePointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_WriteConsole")
			Dim newWriteConsole As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf WriteConsole, ptr_R_WriteConsole))
			Marshal.WriteIntPtr(writeConsolePointer, newWriteConsole)
			Dim writeConsoleExPointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_WriteConsoleEx")
			Dim newWriteConsoleEx As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf WriteConsoleEx, ptr_R_WriteConsoleEx))
			Marshal.WriteIntPtr(writeConsoleExPointer, newWriteConsoleEx)
			Dim resetConsolePointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_ResetConsole")
			Dim newResetConsole As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf ResetConsole, ptr_R_ResetConsole))
			Marshal.WriteIntPtr(resetConsolePointer, newResetConsole)
			Dim flushConsolePointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_FlushConsole")
			Dim newFlushConsole As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf FlushConsole, ptr_R_FlushConsole))
			Marshal.WriteIntPtr(flushConsolePointer, newFlushConsole)
			Dim clearerrConsolePointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_ClearerrConsole")
			Dim newClearerrConsole As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf ClearErrorConsole, ptr_R_ClearerrConsole))
			Marshal.WriteIntPtr(clearerrConsolePointer, newClearerrConsole)
			Dim busyPointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_Busy")
			Dim newBusy As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf Busy, ptr_R_Busy))
			Marshal.WriteIntPtr(busyPointer, newBusy)
			Dim cleanUpPointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_CleanUp")
			Dim newCleanUp As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf CleanUp, ptr_R_CleanUp))
			Marshal.WriteIntPtr(cleanUpPointer, newCleanUp)
			Dim showFilesPointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_ShowFiles")
			Dim newShowFiles As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf ShowFiles, ptr_R_ShowFiles))
			Marshal.WriteIntPtr(showFilesPointer, newShowFiles)
			Dim chooseFilePointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_ChooseFile")
			Dim newChooseFile As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf ChooseFile, ptr_R_ChooseFile))
			Marshal.WriteIntPtr(chooseFilePointer, newChooseFile)
			Dim editFilePointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_EditFile")
			Dim newEditFile As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf EditFile, ptr_R_EditFile))
			Marshal.WriteIntPtr(editFilePointer, newEditFile)
			Dim loadHistoryPointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_loadhistory")
			Dim newLoadHistory As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf LoadHistory, ptr_R_loadhistory))
			Marshal.WriteIntPtr(loadHistoryPointer, newLoadHistory)
			Dim saveHistoryPointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_savehistory")
			Dim newSaveHistory As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf SaveHistory, ptr_R_savehistory))
			Marshal.WriteIntPtr(saveHistoryPointer, newSaveHistory)
			Dim addHistoryPointer As IntPtr = Me.m_engine.DangerousGetHandle("ptr_R_addhistory")
			Dim newAddHistory As IntPtr = Marshal.GetFunctionPointerForDelegate(DirectCast(AddressOf AddHistory, ptr_R_addhistory))
			Marshal.WriteIntPtr(addHistoryPointer, newAddHistory)
		End Sub

        Private Shared Function ConvertSeparatorToUnixStylePath(path As String) As String
            Return path.Replace(IO.Path.DirectorySeparatorChar, "/"c)
        End Function

		Private Function ReadConsole(prompt As String, buffer As StringBuilder, count As Integer, history As Boolean) As Boolean
			buffer.Clear()
			Dim input As String = Device.ReadConsole(prompt, count, history)
			buffer.Append(input).Append(vbLf)
			' input must end with '\n\0' ('\0' is appended during marshalling).
			Return input IsNot Nothing
		End Function

		Private Sub WriteConsole(buffer As String, length As Integer)
			WriteConsoleEx(buffer, length, ConsoleOutputType.None)
		End Sub

		Private Sub WriteConsoleEx(buffer As String, length As Integer, outputType As ConsoleOutputType)
			Device.WriteConsole(buffer, length, outputType)
		End Sub

		Private Sub ShowMessage(message As String)
			Device.ShowMessage(message)
		End Sub

		Private Sub Busy(which As BusyType)
			Device.Busy(which)
		End Sub

		Private Sub Callback()
			Device.Callback()
		End Sub

		Private Function Ask(question As String) As YesNoCancel
			Return Device.Ask(question)
		End Function

		Private Sub Suicide(message As String)
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

		Private Sub CleanUp(saveAction As StartupSaveAction, status As Integer, runLast As Boolean)
			Device.CleanUp(saveAction, status, runLast)
		End Sub

		Private Function ShowFiles(count As Integer, files As String(), headers As String(), title As String, delete As Boolean, pager As String) As Boolean
			Return Device.ShowFiles(files, headers, title, delete, pager)
		End Function

		Private Function ChooseFile(create As Boolean, buffer As StringBuilder, length As Integer) As Integer
			Dim path As String = Device.ChooseFile(create)
			Return If(path Is Nothing, 0, Encoding.ASCII.GetByteCount(path))
		End Function

		Private Sub EditFile(file As String)
			Device.EditFile(file)
		End Sub

		Private Function CallDeviceFunction([call] As IntPtr, operation As IntPtr, args As IntPtr, environment As IntPtr, func As Func(Of Language, SymbolicExpression, Pairlist, REnvironment, SymbolicExpression)) As IntPtr
			Dim c = New Language(Engine, [call])
			Dim op = New SymbolicExpression(Engine, operation)
			Dim arglist = New Pairlist(Engine, args)
			Dim env = New REnvironment(Engine, environment)
			Dim result As SymbolicExpression = func(c, op, arglist, env)
			Return result.DangerousGetHandle()
		End Function

		Private Function LoadHistory([call] As IntPtr, operation As IntPtr, args As IntPtr, environment As IntPtr) As IntPtr
			Return CallDeviceFunction([call], operation, args, environment, AddressOf Device.LoadHistory)
		End Function

		Private Function SaveHistory([call] As IntPtr, operation As IntPtr, args As IntPtr, environment As IntPtr) As IntPtr
			Return CallDeviceFunction([call], operation, args, environment, AddressOf Device.SaveHistory)
		End Function

		Private Function AddHistory([call] As IntPtr, operation As IntPtr, args As IntPtr, environment As IntPtr) As IntPtr
			Return CallDeviceFunction([call], operation, args, environment, AddressOf Device.AddHistory)
		End Function

		#Region "Nested type: getValue"

		<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
		Private Delegate Function getValue() As IntPtr

		#End Region
	End Class
End Namespace

