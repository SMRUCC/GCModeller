#Region "Microsoft.VisualBasic::65adf1086abd5dbc08bea24303dfdc95, localblast\PanGenome\Output\OrthologyLink.vb"

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
    '    Code Lines: 13 (72.22%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (27.78%)
    '     File Size: 363 B


    ' Class OrthologyLink
    ' 
    '     Properties: Tuple
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Public Class OrthologyLink

    <XmlAttribute>
    Public Property Tuple As String()

    Sub New()
    End Sub

    Sub New(ParamArray geneSet As String())
        Tuple = geneSet
    End Sub

    Public Overrides Function ToString() As String
        Return $"[{Tuple.JoinBy(", ")}]"
    End Function
End Class
