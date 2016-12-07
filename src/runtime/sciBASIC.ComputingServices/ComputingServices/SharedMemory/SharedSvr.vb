#Region "Microsoft.VisualBasic::c70c6ed47382556ceb066e0070e001ba, ..\sciBASIC.ComputingServices\ComputingServices\SharedMemory\SharedSvr.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SharedMemory

    ''' <summary>
    ''' Memory shared services.
    ''' </summary>
    ''' 
    <Protocol(GetType(Protocols.MemoryProtocols))>
    Public Class SharedSvr : Implements IDisposable
        Implements IObjectModel_Driver

        ReadOnly __localSvr As TcpSynchronizationServicesSocket
        ''' <summary>
        ''' 这个是提供给远程主机读取使用的
        ''' </summary>
        ReadOnly __variables As New Dictionary(Of HashValue)

        Sub New(local As Integer)
            __localSvr = New TcpSynchronizationServicesSocket(local)
            __localSvr.Responsehandler = AddressOf New ProtocolHandler(Me).HandleRequest
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="value"></param>
        ''' <param name="[overrides]">If the value type is not equals, overrides the exists value?</param>
        ''' <returns>Write or update the memory success?</returns>
        Public Function Allocate(name As String, value As Object, Optional [overrides] As Boolean = False) As Boolean
            If __variables.ContainsKey(name) Then
                Dim x As HashValue = __variables ^ name
                Dim type As Type = value.GetType
                If type.Equals(x.value.GetType) Then
                    x.value = value
                Else
                    If [overrides] Then
                        x.value = value
                    Else
                        Return False
                    End If
                End If
            Else
                __variables.Add(name, New HashValue(name, value))
            End If

            Return True
        End Function

        <Protocol(MemoryProtocols.TypeOf)>
        Private Function __typeOf(CA As Long, args As RequestStream, remote As System.Net.IPEndPoint) As RequestStream
            Dim name As String = args.GetUTF8String
            Dim type As TypeInfo

            If __variables.ContainsKey(name) Then
                type = __variables(name).Type
            Else
                type = New TypeInfo(GetType(Void))
            End If

            Return New RequestStream(HTTP_RFC.RFC_OK, HTTP_RFC.RFC_OK, type.GetJson)
        End Function

        <Protocol(MemoryProtocols.Read)>
        Private Function Read(CA As Long, args As RequestStream, remote As System.Net.IPEndPoint) As RequestStream
            Dim name As String = args.GetUTF8String

            If __variables.ContainsKey(name) Then
                Return New RequestStream(HTTP_RFC.RFC_OK, HTTP_RFC.RFC_OK, __variables(name).GetValueJson)
            Else
                Return New RequestStream(HTTP_RFC.RFC_OK, HTTP_RFC.RFC_OK, "null")
            End If
        End Function

        <Protocol(MemoryProtocols.Write)>
        Private Function Write(CA As Long, args As RequestStream, remote As System.Net.IPEndPoint) As RequestStream
            Dim value As Argv = args.LoadObject(Of Argv)
            Dim b As Long = If(Allocate(value.Identifier, value.value.GetValue), HTTP_RFC.RFC_OK, HTTP_RFC.RFC_INTERNAL_SERVER_ERROR)
            Return New RequestStream(b, b, CStr(b))
        End Function

        Public Overrides Function ToString() As String
            Return __localSvr.ToString
        End Function

        Public Function Run() As Integer Implements IObjectModel_Driver.Run
            Return __localSvr.Run
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call __localSvr.Dispose()
                    Call __variables.Clear()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
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
