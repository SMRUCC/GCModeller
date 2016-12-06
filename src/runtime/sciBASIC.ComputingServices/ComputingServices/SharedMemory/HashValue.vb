#Region "Microsoft.VisualBasic::fc49cf03b614c5c86405d85c15c579a1, ..\ComputingServices\SharedMemory\HashValue.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SharedMemory

    ''' <summary>
    ''' The shared variable on the remote machine.
    ''' </summary>
    Public Class HashValue : Implements INamedValue

        ''' <summary>
        ''' The variable name
        ''' </summary>
        ''' <returns></returns>
        Public Property Identifier As String Implements INamedValue.Key
        ''' <summary>
        ''' variable value
        ''' </summary>
        ''' <returns></returns>
        Public Property value As Object
        ''' <summary>
        ''' Simple type information
        ''' </summary>
        ''' <returns></returns>
        Public Property Type As TypeInfo

        Sub New(name As String, x As Object)
            Identifier = name
            value = x
            Type = New TypeInfo(x.GetType)
        End Sub

        ''' <summary>
        ''' Json serialization for the network transfer.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetValueJson() As String
            Return JsonContract.GetObjectJson(value, Type.GetType(True))
        End Function

        Public Overrides Function ToString() As String
            Return $"Dim {Identifier} As {Type.ToString} = {JsonContract.GetObjectJson(value, Type.GetType(True))}"
        End Function
    End Class

    ''' <summary>
    ''' Variable value for the network transfer
    ''' </summary>
    Public Structure Argv : Implements INamedValue

        ''' <summary>
        ''' The variable name
        ''' </summary>
        ''' <returns></returns>
        Public Property Identifier As String Implements INamedValue.Key
        ''' <summary>
        ''' Json value, and the type information is also included in this property.
        ''' </summary>
        ''' <returns></returns>
        Public Property value As TaskHost.Argv

        Sub New(name As String, x As Object)
            Identifier = name
            value = New TaskHost.Argv(x)
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
