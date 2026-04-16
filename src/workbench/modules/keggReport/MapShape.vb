#Region "Microsoft.VisualBasic::c435e04e2784e7408c81fb8dec7efe68, modules\keggReport\MapShape.vb"

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

    '   Total Lines: 17
    '    Code Lines: 10 (58.82%)
    ' Comment Lines: 4 (23.53%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (17.65%)
    '     File Size: 419 B


    ' Class MapShape
    ' 
    '     Properties: entities, isEntity, location, shape, title
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Public Class MapShape

    Public Property shape As String
    Public Property location As Double()
    ''' <summary>
    ''' kegg id list
    ''' </summary>
    ''' <returns></returns>
    Public Property entities As String()
    Public Property title As String
    Public Property isEntity As Boolean

    Public Overrides Function ToString() As String
        Return title
    End Function

End Class
