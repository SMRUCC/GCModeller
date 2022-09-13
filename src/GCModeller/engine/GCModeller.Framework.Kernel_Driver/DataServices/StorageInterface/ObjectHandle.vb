#Region "Microsoft.VisualBasic::11a5ed5221a0c83cfd52023f54a0c111, GCModeller\engine\GCModeller.Framework.Kernel_Driver\DataServices\StorageInterface\ObjectHandle.vb"

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


    ' Code Statistics:

    '   Total Lines: 19
    '    Code Lines: 15
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 741 B


    '     Class ObjectHandle
    ' 
    '         Properties: Handle, ID
    ' 
    '         Sub: Assign
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace DataStorage.FileModel

    Public Class ObjectHandle
        Implements IKeyValuePairObject(Of String, Integer)
        Implements IAddressOf
        Implements INamedValue

        Public Property ID As String Implements INamedValue.Key, IKeyValuePairObject(Of String, Integer).Key
        Public Property Handle As Integer Implements IAddressOf.Address, IKeyValuePairObject(Of String, Integer).Value

        Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Me.Handle = address
        End Sub
    End Class
End Namespace
