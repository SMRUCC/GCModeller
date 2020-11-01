Namespace Internals
    ''' <summary>
    ''' Specifies the restore action.
    ''' </summary>
    Public Enum StartupRestoreAction
        ''' <summary>
        ''' Not restoring.
        ''' </summary>
        NoRestore = 0

        ''' <summary>
        ''' Restoring.
        ''' </summary>
        Restore = 1

        ''' <summary>
        ''' The default value.
        ''' </summary>
        [Default] = 2
    End Enum

    ''' <summary>
    ''' Specifies the save action.
    ''' </summary>
    Public Enum StartupSaveAction
        ''' <summary>
        ''' The default value.
        ''' </summary>
        [Default] = 2

        ''' <summary>
        ''' No saving.
        ''' </summary>
        NoSave = 3

        ''' <summary>
        ''' Saving.
        ''' </summary>
        Save = 4

        ''' <summary>
        ''' Asking user.
        ''' </summary>
        Ask = 5

        ''' <summary>
        ''' Terminates without any actions.
        ''' </summary>
        Suicide = 6
    End Enum
End Namespace
