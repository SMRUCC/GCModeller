#Region "Microsoft.VisualBasic::79d3afd74e9a9fa4f1bc21705fe28429, GCModeller\core\Bio.Assembly\Assembly\iGEM\iGEMQuery.vb"

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

    '   Total Lines: 24
    '    Code Lines: 18
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 866 B


    '     Class iGEMQuery
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: FetchByIDList, partListParser, urlCreator
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text.Xml

Namespace Assembly.iGEM

    Public Class iGEMQuery : Inherits WebQuery(Of String)

        Sub New(cache As String)
            Call MyBase.New(AddressOf urlCreator, Function(id) id, AddressOf partListParser, cache:=cache)
        End Sub

        Private Shared Function partListParser(text As String, type As Type) As Object
            Return text.RemoveXmlComments.LoadFromXml(type,)
        End Function

        Private Shared Function urlCreator(partId As String) As String
            Return $"http://parts.igem.org/cgi/xml/part.cgi?part={partId}"
        End Function

        Public Function FetchByIDList(id As IEnumerable(Of String)) As IEnumerable(Of String)
            Return Me.queryText(id, "Xml")
        End Function
    End Class
End Namespace
