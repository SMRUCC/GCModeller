#Region "Microsoft.VisualBasic::d75caece16675716ce43c0d52528af1e, ..\GCModeller\models\ncbi-BioSystems\Sys_set.vb"

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

''' <summary>
''' 
''' </summary>
Public Class Sys_set
    Public Property sysid As sysid
    Public Property source As source
    Public Property externalaccn As String
    Public Property recordurl As String
    Public Property names As String()
    Public Property description As String

End Class

Public Class sysid
    Public Property bsid As String
    Public Property version As String
End Class

Public Class source

    Public Property source As sourceInner

    Public Class sourceInner
        Public Property db As String
        Public Property tag As tag
    End Class
End Class

Public Class tag
    Public Property id As String
End Class
