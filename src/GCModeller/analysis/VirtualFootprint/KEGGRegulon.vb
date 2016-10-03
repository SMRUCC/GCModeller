#Region "Microsoft.VisualBasic::d0c6dd3c290b70f664ba90052c4eab6a, ..\GCModeller\analysis\VirtualFootprint\KEGGRegulon.vb"

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

Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class KEGGRegulon

    Public Property Regulator As String()
    Public Property Members As String()
    Public Property Family As String()
    Public Property ModId As String
    Public Property Type As String
    Public Property [Class] As String
    Public Property Category As String
    Public Property Name As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
