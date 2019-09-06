#Region "Microsoft.VisualBasic::866a6c82b7d720cd43e6c9100049f2a7, RDotNET.Extensions.VisualBasic\Extensions\Serialization\csvHandle.vb"

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

    '     Class csvHandle
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ToString
    ' 
    '         Sub: Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports RDotNET.Extensions.VisualBasic.API.utils

Namespace Serialization

    Public Class csvHandle : Inherits StreamWriter
        Implements IDisposable

        ReadOnly R_server As ExtendedEngine

        ReadOnly tmp$
        ReadOnly name$

        Public Sub New(path$, var$, append As Boolean, encoding As Encoding, Optional Rengine As ExtendedEngine = Nothing)
            MyBase.New(path, append, encoding)

            tmp = path
            name = var
            R_server = Rengine Or ExtendedEngine.MyDefault
        End Sub

        Public Overrides Function ToString() As String
            Return $"{name} @ {tmp}"
        End Function

        Protected Overrides Sub Dispose(disposing As Boolean)
            ' 在这里发生了文件的写入操作
            ' 并关闭了文件句柄
            MyBase.Dispose(disposing)

            SyncLock R_server
                With R_server
                    .Assign(name) = read.csv(tmp)
                End With
            End SyncLock
        End Sub
    End Class
End Namespace
