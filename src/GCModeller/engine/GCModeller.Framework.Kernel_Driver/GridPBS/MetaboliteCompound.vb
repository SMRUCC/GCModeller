#Region "Microsoft.VisualBasic::52c5da4de414112b50d08b424783e465, ..\GCModeller\engine\GCModeller.Framework.Kernel_Driver\GridPBS\MetaboliteCompound.vb"

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

Namespace GridPBS

    ''' <summary>
    ''' 仅用于数据交换的对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure MetaboliteCompound
        Implements IAddressHandle
        Implements INamedValue

        Public Property Handle As Integer Implements IAddressHandle.Address
        Public Property Identifier As String Implements INamedValue.Key
        Public Property Quantity As Double

        Public Sub Dispose() Implements IDisposable.Dispose
            Return 'DO NOTHING
        End Sub
    End Structure
End Namespace
