﻿#Region "Microsoft.VisualBasic::2a1c1959b7a366caf94f8d97b7328bce, Data_science\DataMining\Microsoft.VisualBasic.DataMining.Framework\KMeans\Parallel\StreamAPI.vb"

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

    '     Module StreamAPI
    ' 
    '         Function: GetObject, GetObjects, (+2 Overloads) GetRaw
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Serialization.BinaryDumping

Namespace KMeans.Parallel

    Public Module StreamAPI

        <Extension>
        Public Function GetRaw(source As IEnumerable(Of Entity)) As Byte()
            Return BufferAPI.CreateBuffer(source, AddressOf GetRaw)
        End Function

        <Extension>
        Public Iterator Function GetObjects(raw As Byte()) As IEnumerable(Of Entity)
            Yield BufferAPI.GetBuffer(raw, AddressOf GetObject)
        End Function

        <Extension> Public Function GetRaw(x As Entity) As Byte()
            Dim name As Byte() = Encoding.Unicode.GetBytes(x.uid)
            Dim buffer As Byte() =
                New Byte(name.Length + RawStream.INT32 + (x.Properties.Length * RawStream.DblFloat) - 1) {}
            Dim i As VBInteger = 0
            Dim nameLen As Byte() = BitConverter.GetBytes(name.Length)

            Call Array.ConstrainedCopy(nameLen, Scan0, buffer, i + nameLen.Length, nameLen.Length)
            Call Array.ConstrainedCopy(name, Scan0, buffer, i + name.Length, name.Length)

            For Each d As Double In x.Properties
                Call Array.ConstrainedCopy(BitConverter.GetBytes(d), Scan0, buffer, i + RawStream.DblFloat, RawStream.DblFloat)
            Next

            Return buffer
        End Function

        <Extension> Public Function GetObject(buffer As Byte()) As Entity
            Dim nameLen As Byte() = New Byte(RawStream.INT32 - 1) {}
            Dim p As VBInteger = 0
            Call Array.ConstrainedCopy(buffer, p + nameLen.Length, nameLen, Scan0, nameLen.Length)

            Dim name As Byte() = New Byte(BitConverter.ToInt32(nameLen, Scan0) - 1) {}
            Call Array.ConstrainedCopy(buffer, p + name.Length, name, Scan0, name.Length)

            Dim props As Double() =
                buffer.Skip(nameLen.Length + name.Length).Split(RawStream.DblFloat) _
                      .Select(Function(buf) BitConverter.ToDouble(buf, Scan0)).ToArray

            Return New Entity With {
                .Properties = props,
                .uid = Encoding.Unicode.GetString(name)
            }
        End Function
    End Module
End Namespace
