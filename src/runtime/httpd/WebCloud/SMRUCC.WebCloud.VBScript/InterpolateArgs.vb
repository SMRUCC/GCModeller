﻿#Region "Microsoft.VisualBasic::6ebd6e2f99c4a902aaf34910a9dda4aa, WebCloud\SMRUCC.WebCloud.VBScript\InterpolateArgs.vb"

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

    ' Structure InterpolateArgs
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Public Structure InterpolateArgs

    Dim wwwroot$

    ''' <summary>
    ''' String resource from loader: <see cref="LoadStrings(String)"/>
    ''' </summary>
    Dim resource As Dictionary(Of String, String)
    Dim variables As Dictionary(Of String, String)
    Dim data As Dictionary(Of String, IEnumerable)
    Dim codepage As Encoding

    Public Overrides Function ToString() As String
        Return wwwroot
    End Function
End Structure
