
Namespace NativeLibrary
    ''' <summary> Interface for registry keys.</summary>
    Public Interface IRegistryKey
        ''' <summary> Gets sub key names.</summary>
        '''
        ''' <returns> An array of string.</returns>
        Function GetSubKeyNames() As String()

        ''' <summary> Gets a value of a key-value pair</summary>
        '''
        ''' <param name="name"> The name.</param>
        '''
        ''' <returns> The value.</returns>
        Function GetValue(ByVal name As String) As Object

        ''' <summary> Retrieves an array of strings that contains all the value names associated with
        '''        this key</summary>
        '''
        ''' <returns> An array of string.</returns>
        Function GetValueNames() As String()

        ''' <summary> Opens sub key.</summary>
        '''
        ''' <param name="name"> The name.</param>
        '''
        ''' <returns> An IRegistryKey.</returns>
        Function OpenSubKey(ByVal name As String) As IRegistryKey

        ''' <summary>
        ''' Retrieve the realkey for testing against null
        ''' </summary>
        ''' <returns>The RegistryKey it holds</returns>
        Function GetRealKey() As Object
    End Interface

    ''' <summary> Interface for registry.</summary>
    Public Interface IRegistry
        ''' <summary> Gets the local machine.</summary>
        '''
        ''' <value> The local machine.</value>
        ReadOnly Property LocalMachine As IRegistryKey

        ''' <summary> Gets the current user.</summary>
        '''
        ''' <value> The current user.</value>
        ReadOnly Property CurrentUser As IRegistryKey
    End Interface

    ''' <summary> The windows registry.</summary>
    Public Class WindowsRegistry
        Implements IRegistry
        ''' <summary> Gets the current user.</summary>
        '''
        ''' <value> The current user.</value>
        Public ReadOnly Property CurrentUser As IRegistryKey Implements IRegistry.CurrentUser
            Get
                Return New WindowsRegistryKey(Microsoft.Win32.Registry.CurrentUser)
            End Get
        End Property

        ''' <summary> Gets the local machine.</summary>
        '''
        ''' <value> The local machine.</value>
        Public ReadOnly Property LocalMachine As IRegistryKey Implements IRegistry.LocalMachine
            Get
                Return New WindowsRegistryKey(Microsoft.Win32.Registry.LocalMachine)
            End Get
        End Property
    End Class

    ''' <summary> The windows registry key.</summary>
    Public Class WindowsRegistryKey
        Implements IRegistryKey
        ''' <summary> Constructor.</summary>
        '''
        ''' <param name="realKey"> The real key.</param>
        Public Sub New(ByVal realKey As Microsoft.Win32.RegistryKey)
            Me.realKey = realKey
        End Sub

        Private realKey As Microsoft.Win32.RegistryKey

        ''' <summary>
        ''' Get the real key
        ''' </summary>
        ''' <returns>Object</returns>
        Public Function GetRealKey() As Object Implements IRegistryKey.GetRealKey
            Return realKey
        End Function

        ''' <summary> Gets sub key names.</summary>
        '''
        ''' <returns> An array of string.</returns>
        Public Function GetSubKeyNames() As String() Implements IRegistryKey.GetSubKeyNames
            Return realKey.GetSubKeyNames()
        End Function

        ''' <summary> Gets a value of a key-value pair.</summary>
        '''
        ''' <param name="name"> The name.</param>
        '''
        ''' <returns> The value.</returns>
        Public Function GetValue(ByVal name As String) As Object Implements IRegistryKey.GetValue
            Return realKey.GetValue(name)
        End Function

        ''' <summary> Retrieves an array of strings that contains all the value names associated with
        '''                 this key.</summary>
        '''
        ''' <returns> An array of string.</returns>
        Public Function GetValueNames() As String() Implements IRegistryKey.GetValueNames
            Return realKey.GetValueNames()
        End Function

        ''' <summary> Opens sub key.</summary>
        '''
        ''' <param name="name"> The name.</param>
        '''
        ''' <returns> An IRegistryKey.</returns>
        Public Function OpenSubKey(ByVal name As String) As IRegistryKey Implements IRegistryKey.OpenSubKey
            Return New WindowsRegistryKey(realKey.OpenSubKey(name))
        End Function
    End Class
End Namespace
