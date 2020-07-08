#Region "Microsoft.VisualBasic::be568b2998d0d64a3bf4dc1b93b26c25, Bio.Assembly\Assembly\ELIXIR\EBI\ChEBI\Web\REST.vb"

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

    '     Structure REST
    ' 
    '         Properties: [return]
    ' 
    '         Function: ParsingRESTData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.XML

Namespace Assembly.ELIXIR.EBI.ChEBI.WebServices

    <XmlRoot("getCompleteEntityResponse", [Namespace]:="http://www.ebi.ac.uk/webservices/chebi")>
    Public Structure REST

        <XmlElement("return")> Public Property [return] As ChEBIEntity

        Public Shared Function ParsingRESTData(result$) As ChEBIEntity()
            Dim xml As XmlDocument = result.LoadXmlDocument
            Dim nodes = xml.GetElementsByTagName("S:Body")
            Dim out As New List(Of ChEBIEntity)

            For Each node As XmlNode In nodes
                For Each rep As XmlNode In node.ChildNodes
                    For Each child As XmlNode In rep.ChildNodes
                        If child.Name <> "return" Then
                            Continue For
                        End If

                        result = child.InnerXml
                        result = $"<{NameOf(ChEBIEntity)}>" & result & $"</{NameOf(ChEBIEntity)}>"
                        result = result.Replace(" xmlns=""https://www.ebi.ac.uk/webservices/chebi""", "")

                        Try
                            out += result.CreateObjectFromXmlFragment(Of ChEBIEntity)
                        Catch ex As Exception
                            Throw New Exception(node.InnerText, ex)
                        End Try
                    Next
                Next
            Next

            Return out
        End Function
    End Structure
End Namespace
