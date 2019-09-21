#Region "Microsoft.VisualBasic::ebcffd3e3b9c214e1df3ad28098dbc9a, analysis\Motifs\VirtualFootprint\KEGGRegulon.vb"

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

    ' Class KEGGRegulon
    ' 
    '     Properties: [Class], Category, Family, Members, ModId
    '                 Name, Regulator, Type
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

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
