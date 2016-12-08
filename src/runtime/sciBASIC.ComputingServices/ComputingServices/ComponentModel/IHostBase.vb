#Region "Microsoft.VisualBasic::87f4d64ea9d69da211f6a96237323a39, ..\sciBASIC.ComputingServices\ComputingServices\ComponentModel\IHostBase.vb"

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

Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Abstract

Namespace ComponentModel

    Public MustInherit Class IHostBase : Inherits IMasterBase(Of TcpSynchronizationServicesSocket)

        Sub New(portal As Integer)
            __host = New TcpSynchronizationServicesSocket(portal)
        End Sub

        Sub New()
        End Sub
    End Class

    Public MustInherit Class IMasterBase(Of TSocket As IServicesSocket)

        Public MustOverride ReadOnly Property Portal As IPEndPoint

        Protected Friend __host As TSocket

        Public Shared Narrowing Operator CType(master As IMasterBase(Of TSocket)) As IPEndPoint
            Return master.Portal
        End Operator

        Public Shared Narrowing Operator CType(master As IMasterBase(Of TSocket)) As System.Net.IPEndPoint
            Return master.Portal.GetIPEndPoint
        End Operator

    End Class
End Namespace
