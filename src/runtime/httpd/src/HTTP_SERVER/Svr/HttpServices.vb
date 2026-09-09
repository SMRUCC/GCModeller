#Region "Microsoft.VisualBasic::a1c8146ec93a384eae2319e3869d8671, src\HTTP_SERVER\HttpServices.vb"

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

    '   Total Lines: 58
    '    Code Lines: 36 (62.07%)
    ' Comment Lines: 10 (17.24%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (20.69%)
    '     File Size: 1.94 KB


    ' Class HttpServices
    ' 
    '     Properties: port
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: StartHttp
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports tcp = Microsoft.VisualBasic.Net.Tcp

Public Class HttpServices : Implements IDisposable

    Public ReadOnly Property port As Integer

    Private disposedValue As Boolean

    Dim web As String
    Dim WithEvents background As Process

    Sub New(wwwroot As String)
        web = wwwroot
    End Sub

    Public Function StartHttp() As HttpServices
        Dim http = Interop.CreateServer
        Dim service As Integer = tcp.GetFirstAvailablePort(BEGIN_PORT:=-1)
        Dim args As String = http.GetlistenCommandLine(web, port:=service)
        Dim task = http.CreateSlave(args, workdir:=App.HOME)

        _port = service
        background = task.Start()

        Return Me
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Try
                    Call background.Kill()
                    Call background.Dispose()
                Catch ex As Exception
                    Call App.LogException(ex)
                End Try
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
