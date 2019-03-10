﻿#Region "Microsoft.VisualBasic::9a03fb2597e73605960086094cc6c621, Data\BinaryData\BinaryData\SQLite3\Helpers\StreamHelper.vb"

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

    '     Module StreamHelper
    ' 
    '         Function: (+2 Overloads) ReadFully
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO

Namespace ManagedSqlite.Core.Helpers
    Module StreamHelper

        <System.Runtime.CompilerServices.Extension>
        Public Function ReadFully(stream As Stream, length As Integer) As Byte()
            Dim data As Byte() = New Byte(length - 1) {}
            stream.ReadFully(data, 0, data.Length)

            Return data
        End Function

        <System.Runtime.CompilerServices.Extension>
        Public Function ReadFully(stream As Stream, buffer As Byte(), offset As Integer, length As Integer) As Integer
            Dim totalRead As Integer = 0
            Dim numRead As Integer = stream.Read(buffer, offset, length)
            While numRead > 0
                totalRead += numRead
                If totalRead = length Then
                    Exit While
                End If

                numRead = stream.Read(buffer, offset + totalRead, length - totalRead)
            End While

            Return totalRead
        End Function
    End Module
End Namespace

