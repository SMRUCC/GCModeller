#Region "Microsoft.VisualBasic::3918f28eab261136ec29c3a6fead9b03, ..\sciBASIC.ComputingServices\ComputingServices\Asymmetric\Instance.vb"

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

Imports System.Reflection
Imports Microsoft.VisualBasic.Net.SSL
Imports Microsoft.VisualBasic.Net.TCPExtensions
Imports sciBASIC.ComputingServices.ComponentModel

Namespace Asymmetric

    ''' <summary>
    ''' 服务实例，只是和管理节点之间的通信的通道
    ''' </summary>
    Public MustInherit Class Instance : Inherits IMasterBase(Of SSLSynchronizationServicesSocket)

        Protected ReadOnly _invokeCA As Net.SSL.Certificate

        Sub New(CLI As CommandLine.CommandLine)
            Call CLI.__DEBUG_ECHO

            _invokeCA = CLI(Protocols.OAuth).GetCA
            __host = New Net.SSL.SSLSynchronizationServicesSocket(
                Net.TCPExtensions.GetFirstAvailablePort,
                _invokeCA,
                container:=Me,
                exHandler:=AddressOf __handleException)
            Call (Sub() __returnPortal(CLI)).BeginInvoke(Nothing, Nothing)
        End Sub

        Protected MustOverride Function __getExternalSvrPortal() As Integer

        Public Sub Run()
            Call __host.Install(_invokeCA, [overrides]:=True)
            Call __host.Run()
        End Sub

        Private Sub __returnPortal(cli As CommandLine.CommandLine)
            Call __host.WaitForStart()
            Call Microsoft.VisualBasic.Parallel.ReturnPortal(cli, __host.LocalPort)
        End Sub

        Protected Overridable Sub __handleException(ex As Exception)
            Call App.LogException(ex, MethodBase.GetCurrentMethod.GetFullName)
        End Sub
    End Class
End Namespace
