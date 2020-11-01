Imports RDotNet.Internals
Imports RDotNet.Internals.Windows
Imports System
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
            start = New RStart()
            SetDefaultParameter()
        End Sub

        ''' <summary>
        ''' Gets or sets the value indicating that R runs as quiet mode.
        ''' </summary>
        Public Property Quiet As Boolean
            Get
                Return start.Common.R_Quiet
            End Get
            Set(ByVal value As Boolean)
                start.Common.R_Quiet = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value indicating that R runs as slave mode.
        ''' </summary>
        Public Property Slave As Boolean
            Get
                Return start.Common.R_Slave
            End Get
            Set(ByVal value As Boolean)
                start.Common.R_Slave = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value indicating that R runs as interactive mode.
        ''' </summary>
        Public Property Interactive As Boolean
            Get
                Return start.Common.R_Interactive
            End Get
            Set(ByVal value As Boolean)
                start.Common.R_Interactive = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value indicating that R runs as verbose mode.
        ''' </summary>
        Public Property Verbose As Boolean
            Get
                Return start.Common.R_Verbose
            End Get
            Set(ByVal value As Boolean)
                start.Common.R_Verbose = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value indicating that R loads the site file.
        ''' </summary>
        Public Property LoadSiteFile As Boolean
            Get
                Return start.Common.LoadSiteFile
            End Get
            Set(ByVal value As Boolean)
                start.Common.LoadSiteFile = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value indicating that R loads the init file.
        ''' </summary>
        Public Property LoadInitFile As Boolean
            Get
                Return start.Common.LoadInitFile
            End Get
            Set(ByVal value As Boolean)
                start.Common.LoadInitFile = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value indicating that R debugs the init file.
        ''' </summary>
        Public Property DebugInitFile As Boolean
            Get
                Return start.Common.DebugInitFile
            End Get
            Set(ByVal value As Boolean)
                start.Common.DebugInitFile = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value indicating that R restores the history.
        ''' </summary>
        Public Property RestoreAction As StartupRestoreAction
            Get
                Return start.Common.RestoreAction
            End Get
            Set(ByVal value As StartupRestoreAction)
                start.Common.RestoreAction = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value indicating that R saves the history.
        ''' </summary>
        Public Property SaveAction As StartupSaveAction
            Get
                Return start.Common.SaveAction
            End Get
            Set(ByVal value As StartupSaveAction)
                start.Common.SaveAction = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the minimum memory size.
        ''' </summary>
        Public Property MinMemorySize As ULong
            Get
                Return start.Common.vsize.ToUInt64()
            End Get
            Set(ByVal value As ULong)

                If value > EnvironmentDependentMaxSize Then
                    Throw New ArgumentOutOfRangeException("value")
                End If

                start.Common.vsize = New UIntPtr(value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the minimum number of cons cells.
        ''' </summary>
        Public Property MinCellSize As ULong
            Get
                Return start.Common.nsize.ToUInt64()
            End Get
            Set(ByVal value As ULong)

                If value > EnvironmentDependentMaxSize Then
                    Throw New ArgumentOutOfRangeException("value")
                End If

                start.Common.nsize = New UIntPtr(value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the maximum memory size.
        ''' </summary>
        Public Property MaxMemorySize As ULong
            Get
                Return start.Common.max_vsize.ToUInt64()
            End Get
            Set(ByVal value As ULong)

                If value > EnvironmentDependentMaxSize Then
                    Throw New ArgumentOutOfRangeException("value")
                End If

                start.Common.max_vsize = New UIntPtr(value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the maximum number of cons cells.
        ''' </summary>
        Public Property MaxCellSize As ULong
            Get
                Return start.Common.max_nsize.ToUInt64()
            End Get
            Set(ByVal value As ULong)

                If value > EnvironmentDependentMaxSize Then
                    Throw New ArgumentOutOfRangeException("value")
                End If

                start.Common.max_nsize = New UIntPtr(value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the maximum number of protected pointers in stack.
        ''' </summary>
        Public Property StackSize As ULong
            Get
                Return start.Common.ppsize.ToUInt64()
            End Get
            Set(ByVal value As ULong)

                If value > EnvironmentDependentMaxSize Then
                    Throw New ArgumentOutOfRangeException("value")
                End If

                start.Common.ppsize = New UIntPtr(value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value indicating that R does NOT load the environment file.
        ''' </summary>
        Public Property NoRenviron As Boolean
            Get
                Return start.Common.NoRenviron
            End Get
            Set(ByVal value As Boolean)
                start.Common.NoRenviron = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the base directory in which R is installed.
        ''' </summary>
        ''' <remarks>
        ''' Only Windows.
        ''' </remarks>
        Public Property RHome As String
            Get

                If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
                    Throw New NotSupportedException()
                End If

                Return Marshal.PtrToStringAnsi(start.rhome)
            End Get
            Set(ByVal value As String)

                If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
                    Throw New NotSupportedException()
                End If

                start.rhome = Marshal.StringToHGlobalAnsi(value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the default user workspace.
        ''' </summary>
        ''' <remarks>
        ''' Only Windows.
        ''' </remarks>
        Public Property Home As String
            Get

                If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
                    Throw New NotSupportedException()
                End If

                Return Marshal.PtrToStringAnsi(start.home)
            End Get
            Set(ByVal value As String)

                If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
                    Throw New NotSupportedException()
                End If

                start.home = Marshal.StringToHGlobalAnsi(value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the UI mode.
        ''' </summary>
        ''' <remarks>
        ''' Only Windows.
        ''' </remarks>
        Public Property CharacterMode As UiMode
            Get

                If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
                    Throw New NotSupportedException()
                End If

                Return start.CharacterMode
            End Get
            Set(ByVal value As UiMode)

                If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
                    Throw New NotSupportedException()
                End If

                start.CharacterMode = value
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

