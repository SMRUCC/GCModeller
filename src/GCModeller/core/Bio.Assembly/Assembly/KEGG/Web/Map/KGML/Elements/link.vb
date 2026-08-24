#Region "Microsoft.VisualBasic::af11ef80db51a6c555f1eec505092080, core\Bio.Assembly\Assembly\KEGG\Web\Map\KGML\Elements\link.vb"

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
    '    Code Lines: 10 (55.56%)
    ' Comment Lines: 3 (16.67%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (27.78%)
    '     File Size: 413 B


    '     Class link
    ' 
    '         Properties: type
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.KEGG.WebServices.KGML

    ''' <summary>
    ''' Network edges
    ''' </summary>
    Public Class link

        <XmlAttribute> Public Property type As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace
