Imports System

Namespace Internals
    ''' <summary>
    ''' Specifies output mode.
    ''' </summary>
    <Flags>
    Public Enum OutputMode
        ''' <summary>
        ''' No option.
        ''' </summary>
        None = &H0

        ''' <summary>
        ''' Quiet mode.
        ''' </summary>
        Quiet = &H1

        ''' <summary>
        ''' Slave mode.
        ''' </summary>
        Slave = &H2

        ''' <summary>
        ''' Verbose mode.
        ''' </summary>
        Verbose = &H4
    End Enum
End Namespace
