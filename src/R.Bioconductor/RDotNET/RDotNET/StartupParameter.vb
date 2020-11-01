#Region "Microsoft.VisualBasic::1a19ff46ffb33131da7e062318531501, RDotNET\RDotNET\StartupParameter.vb"

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

    ' Class StartupParameter
    ' 
    '     Properties: CharacterMode, DebugInitFile, Home, Interactive, LoadInitFile
    '                 LoadSiteFile, MaxCellSize, MaxMemorySize, MinCellSize, MinMemorySize
    '                 NoRenviron, Quiet, RestoreAction, RHome, SaveAction
    '                 Slave, StackSize, Verbose
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: SetDefaultParameter
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports RDotNet.Internals.Windows
Imports System.Runtime.InteropServices

''' <summary>
''' Represents parameters on R's startup.
''' </summary>
''' <remarks>
''' Wraps RStart struct.
''' </remarks>
Public Class StartupParameter
	Private Shared ReadOnly EnvironmentDependentMaxSize As ULong = If(Environment.Is64BitProcess, ULong.MaxValue, UInteger.MaxValue)

	' Windows style RStart includes Unix-style RStart.
	Friend start As RStart

	''' <summary>
	''' Create a new Startup parameter, using some default parameters
	''' </summary>
	Public Sub New()
		Me.start = New RStart()
		SetDefaultParameter()
	End Sub

	''' <summary>
	''' Gets or sets the value indicating that R runs as quiet mode.
	''' </summary>
	Public Property Quiet() As Boolean
		Get
			Return Me.start.Common.R_Quiet
		End Get
		Set
			Me.start.Common.R_Quiet = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the value indicating that R runs as slave mode.
	''' </summary>
	Public Property Slave() As Boolean
		Get
			Return Me.start.Common.R_Slave
		End Get
		Set
			Me.start.Common.R_Slave = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the value indicating that R runs as interactive mode.
	''' </summary>
	Public Property Interactive() As Boolean
		Get
			Return Me.start.Common.R_Interactive
		End Get
		Set
			Me.start.Common.R_Interactive = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the value indicating that R runs as verbose mode.
	''' </summary>
	Public Property Verbose() As Boolean
		Get
			Return Me.start.Common.R_Verbose
		End Get
		Set
			Me.start.Common.R_Verbose = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the value indicating that R loads the site file.
	''' </summary>
	Public Property LoadSiteFile() As Boolean
		Get
			Return Me.start.Common.LoadSiteFile
		End Get
		Set
			Me.start.Common.LoadSiteFile = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the value indicating that R loads the init file.
	''' </summary>
	Public Property LoadInitFile() As Boolean
		Get
			Return Me.start.Common.LoadInitFile
		End Get
		Set
			Me.start.Common.LoadInitFile = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the value indicating that R debugs the init file.
	''' </summary>
	Public Property DebugInitFile() As Boolean
		Get
			Return Me.start.Common.DebugInitFile
		End Get
		Set
			Me.start.Common.DebugInitFile = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the value indicating that R restores the history.
	''' </summary>
	Public Property RestoreAction() As StartupRestoreAction
		Get
			Return Me.start.Common.RestoreAction
		End Get
		Set
			Me.start.Common.RestoreAction = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the value indicating that R saves the history.
	''' </summary>
	Public Property SaveAction() As StartupSaveAction
		Get
			Return Me.start.Common.SaveAction
		End Get
		Set
			Me.start.Common.SaveAction = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the minimum memory size.
	''' </summary>
	Public Property MinMemorySize() As ULong
		Get
			Return Me.start.Common.vsize.ToUInt64()
		End Get
		Set
			If value > EnvironmentDependentMaxSize Then
				Throw New ArgumentOutOfRangeException("value")
			End If
			Me.start.Common.vsize = New UIntPtr(value)
		End Set
	End Property

	''' <summary>
	''' Gets or sets the minimum number of cons cells.
	''' </summary>
	Public Property MinCellSize() As ULong
		Get
			Return Me.start.Common.nsize.ToUInt64()
		End Get
		Set
			If value > EnvironmentDependentMaxSize Then
				Throw New ArgumentOutOfRangeException("value")
			End If
			Me.start.Common.nsize = New UIntPtr(value)
		End Set
	End Property

	''' <summary>
	''' Gets or sets the maximum memory size.
	''' </summary>
	Public Property MaxMemorySize() As ULong
		Get
			Return Me.start.Common.max_vsize.ToUInt64()
		End Get
		Set
			If value > EnvironmentDependentMaxSize Then
				Throw New ArgumentOutOfRangeException("value")
			End If
			Me.start.Common.max_vsize = New UIntPtr(value)
		End Set
	End Property

	''' <summary>
	''' Gets or sets the maximum number of cons cells.
	''' </summary>
	Public Property MaxCellSize() As ULong
		Get
			Return Me.start.Common.max_nsize.ToUInt64()
		End Get
		Set
			If value > EnvironmentDependentMaxSize Then
				Throw New ArgumentOutOfRangeException("value")
			End If
			Me.start.Common.max_nsize = New UIntPtr(value)
		End Set
	End Property

	''' <summary>
	''' Gets or sets the maximum number of protected pointers in stack.
	''' </summary>
	Public Property StackSize() As ULong
		Get
			Return Me.start.Common.ppsize.ToUInt64()
		End Get
		Set
			If value > EnvironmentDependentMaxSize Then
				Throw New ArgumentOutOfRangeException("value")
			End If
			Me.start.Common.ppsize = New UIntPtr(value)
		End Set
	End Property

	''' <summary>
	''' Gets or sets the value indicating that R does NOT load the environment file.
	''' </summary>
	Public Property NoRenviron() As Boolean
		Get
			Return Me.start.Common.NoRenviron
		End Get
		Set
			Me.start.Common.NoRenviron = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the base directory in which R is installed.
	''' </summary>
	''' <remarks>
	''' Only Windows.
	''' </remarks>
	Public Property RHome() As String
		Get
			If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
				Throw New NotSupportedException()
			End If
			Return Marshal.PtrToStringAnsi(Me.start.rhome)
		End Get
		Set
			If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
				Throw New NotSupportedException()
			End If
			Me.start.rhome = Marshal.StringToHGlobalAnsi(value)
		End Set
	End Property

	''' <summary>
	''' Gets or sets the default user workspace.
	''' </summary>
	''' <remarks>
	''' Only Windows.
	''' </remarks>
	Public Property Home() As String
		Get
			If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
				Throw New NotSupportedException()
			End If
			Return Marshal.PtrToStringAnsi(Me.start.home)
		End Get
		Set
			If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
				Throw New NotSupportedException()
			End If
			Me.start.home = Marshal.StringToHGlobalAnsi(value)
		End Set
	End Property

	''' <summary>
	''' Gets or sets the UI mode.
	''' </summary>
	''' <remarks>
	''' Only Windows.
	''' </remarks>
	Public Property CharacterMode() As UiMode
		Get
			If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
				Throw New NotSupportedException()
			End If
			Return Me.start.CharacterMode
		End Get
		Set
			If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
				Throw New NotSupportedException()
			End If
			Me.start.CharacterMode = value
		End Set
	End Property

	Private Sub SetDefaultParameter()
		Quiet = True
		'Slave = false;
		Interactive = True
		'Verbose = false;
		RestoreAction = StartupRestoreAction.NoRestore
		SaveAction = StartupSaveAction.NoSave
		LoadSiteFile = True
		LoadInitFile = True
		'DebugInitFile = false;
		MinMemorySize = 6291456
		MinCellSize = 350000
		MaxMemorySize = EnvironmentDependentMaxSize
		MaxCellSize = EnvironmentDependentMaxSize
		StackSize = 50000
		'NoRenviron = false;
		If Environment.OSVersion.Platform = PlatformID.Win32NT Then
			CharacterMode = UiMode.LinkDll
		End If
	End Sub
End Class

