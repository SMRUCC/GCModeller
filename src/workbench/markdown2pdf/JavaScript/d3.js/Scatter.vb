#Region "Microsoft.VisualBasic::2300e92fe01ae678a094450dde21ade2, markdown2pdf\JavaScript\d3.js\Scatter.vb"

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

    ' Class serialData
    ' 
    '     Properties: data, name, total
    ' 
    ' Structure date_count
    ' 
    '     Properties: [date], count
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class serialData

    Public Property name As String
    Public Property data As date_count()
    Public Property total As Long

End Class

Public Structure date_count
    Implements INamedValue

    Public Property [date] As String Implements INamedValue.Key
    Public Property count As Integer

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure
