#Region "Microsoft.VisualBasic::6c7112db2fb0957d8e9549f3b237dcbb, ..\GCModeller\engine\GCModeller.Framework.Kernel_Driver\DataServices\StorageInterface\ObjectHandle.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
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

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace DataStorage.FileModel

    Public Class ObjectHandle : Inherits BaseClass
        Implements IKeyValuePairObject(Of String, Integer)
        Implements IAddressOf
        Implements INamedValue

        Public Property Identifier As String Implements INamedValue.Key, IKeyValuePairObject(Of String, Integer).Key
        Public Property Handle As Integer Implements IAddressOf.Address, IKeyValuePairObject(Of String, Integer).Value
    End Class
End Namespace
