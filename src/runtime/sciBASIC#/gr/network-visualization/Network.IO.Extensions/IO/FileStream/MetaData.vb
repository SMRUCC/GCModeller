﻿#Region "Microsoft.VisualBasic::5075c8bee37fb26aeb10077fb30242a7, gr\network-visualization\Network.IO.Extensions\IO\FileStream\MetaData.vb"

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

    '     Class MetaData
    ' 
    '         Properties: additionals, create_time, creators, description, keywords
    '                     links, title
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace FileStream

    Public Class MetaData

        Public Property creators As String()
        Public Property description As String
        Public Property title As String
        Public Property create_time As String
        Public Property links As String()
        Public Property keywords As String()

        Public Property additionals As Dictionary(Of String, String)

        Public Shared Function Union(a As MetaData, b As MetaData) As MetaData

        End Function
    End Class
End Namespace
