﻿#Region "Microsoft.VisualBasic::f904cc12c9573031dc19c48f2dbceff8, Microsoft.VisualBasic.Core\Net\Tcp\StateObject.vb"

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

    '     Class StateObject
    ' 
    '         Function: GetRequest, ToString
    ' 
    '         Sub: (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Net.Sockets
Imports Microsoft.VisualBasic.Net.Protocols

Namespace Net.Tcp

    ''' <summary>
    ''' State object for reading client data asynchronously
    ''' </summary>
    ''' <remarks></remarks>
    Public Class StateObject : Implements IDisposable

        ''' <summary>
        ''' Client  socket.
        ''' </summary>
        ''' <remarks></remarks>
        Public workSocket As Socket
        ''' <summary>
        ''' Size of receive buffer.
        ''' </summary>
        ''' <remarks></remarks>
        Public Const BufferSize As Integer = 1024 * 1024
        ''' <summary>
        ''' Receive buffer.
        ''' </summary>
        ''' <remarks></remarks>
        Public readBuffer(BufferSize) As Byte
        ''' <summary>
        ''' Received data.
        ''' </summary>
        ''' <remarks></remarks>
        Public ChunkBuffer As New List(Of Byte)

        Public Overrides Function ToString() As String
            Return workSocket.RemoteEndPoint.ToString & " <=====> " & workSocket.LocalEndPoint.ToString
        End Function

        Public Function GetRequest() As RequestStream
            Return New RequestStream(ChunkBuffer.ToArray)
        End Function

#Region "IDisposable Support"
        Protected disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then

                    On Error Resume Next

                    Dim ep As String = workSocket.RemoteEndPoint.ToString

                    ' TODO: dispose managed state (managed objects).
                    Call ChunkBuffer.Clear()
                    Call ChunkBuffer.Free
                    Call readBuffer.Free
                    Call workSocket.Shutdown(SocketShutdown.Both)
                    Call workSocket.Free

                    Call Console.WriteLine($"[DEBUG {Now.ToString}] Socket {ep} clean up!")
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
