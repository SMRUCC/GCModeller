#Region "Microsoft.VisualBasic::85b6bd7e4dcb769ce29230ab48c3ccda, RDotNET\NativeLibrary\Registries.vb"

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

    '     Interface IRegistryKey
    ' 
    '         Function: GetSubKeyNames, GetValue, GetValueNames, OpenSubKey
    ' 
    '     Interface IRegistry
    ' 
    '         Properties: CurrentUser, LocalMachine
    ' 
    '     Class WindowsRegistry
    ' 
    '         Properties: CurrentUser, LocalMachine
    ' 
    '     Class WindowsRegistryKey
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetSubKeyNames, GetValue, GetValueNames, OpenSubKey
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

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
        Function GetValue(name As String) As Object

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
        Function OpenSubKey(name As String) As IRegistryKey
    End Interface

    ''' <summary> Interface for registry.</summary>
    Public Interface IRegistry
        ''' <summary> Gets the local machine.</summary>
        '''
        ''' <value> The local machine.</value>
        ReadOnly Property LocalMachine() As IRegistryKey

        ''' <summary> Gets the current user.</summary>
        '''
        ''' <value> The current user.</value>
        ReadOnly Property CurrentUser() As IRegistryKey
    End Interface

    ''' <summary> The windows registry.</summary>
    Public Class WindowsRegistry
        Implements IRegistry
        ''' <summary> Gets the current user.</summary>
        '''
        ''' <value> The current user.</value>
        Public ReadOnly Property CurrentUser() As IRegistryKey Implements IRegistry.CurrentUser
            Get
                Return New WindowsRegistryKey(Microsoft.Win32.Registry.CurrentUser)
            End Get
        End Property

        ''' <summary> Gets the local machine.</summary>
        '''
        ''' <value> The local machine.</value>
        Public ReadOnly Property LocalMachine() As IRegistryKey Implements IRegistry.LocalMachine
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
        Public Sub New(realKey As Microsoft.Win32.RegistryKey)
            Me.realKey = realKey
        End Sub
        Private realKey As Microsoft.Win32.RegistryKey

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
        Public Function GetValue(name As String) As Object Implements IRegistryKey.GetValue
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
        Public Function OpenSubKey(name As String) As IRegistryKey Implements IRegistryKey.OpenSubKey
            Return New WindowsRegistryKey(realKey.OpenSubKey(name))
        End Function
    End Class
End Namespace

