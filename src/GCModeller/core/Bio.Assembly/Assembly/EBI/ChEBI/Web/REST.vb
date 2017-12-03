#Region "Microsoft.VisualBasic::adfed2485fa504f7e6fe5dbad916c474, ..\GCModeller\core\Bio.Assembly\Assembly\EBI\ChEBI\Web\REST.vb"

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

Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.XML

Namespace Assembly.EBI.ChEBI.WebServices

    <XmlRoot("getCompleteEntityResponse", [Namespace]:="http://www.ebi.ac.uk/webservices/chebi")>
    Public Structure REST

        <XmlElement>
        Public Property [return] As ChEBIEntity()

        Public Shared Function ParsingRESTData(result$) As ChEBIEntity()
            Dim xml As XmlDocument = result.LoadXmlDocument
            Dim nodes = xml.GetElementsByTagName("S:Body")
            Dim out As New List(Of ChEBIEntity)

            For Each node As XmlNode In nodes
                result = node.InnerXml
                Try
                    out += result _
                        .CreateObjectFromXmlFragment(Of REST) _
                        .return
                Catch ex As Exception
                    Throw New Exception(node.InnerText)
                End Try
            Next

            Return out
        End Function
    End Structure
End Namespace
