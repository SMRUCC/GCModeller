﻿#Region "Microsoft.VisualBasic::5073774c14621e94487fa66a7e0781e1, Bio.Repository\NCBI\GEODataSet.vb"

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

    '   Total Lines: 18
    '    Code Lines: 6 (33.33%)
    ' Comment Lines: 7 (38.89%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 5 (27.78%)
    '     File Size: 397 B


    ' Class GEODataSet
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' helper module for download geo dataset
''' </summary>
Public Class GEODataSet

    ReadOnly geo_url As String



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="geo_acc">the GEO dataset accession id.</param>
    Sub New(geo_acc As String)
        geo_url = $"https://www.ncbi.nlm.nih.gov/geo/query/acc.cgi?acc={geo_acc}"
    End Sub

End Class
