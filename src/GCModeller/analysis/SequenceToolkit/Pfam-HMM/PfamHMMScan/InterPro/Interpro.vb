#Region "Microsoft.VisualBasic::aed399604e9683287f6de12e1afb7375, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\InterPro\Interpro.vb"

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

    '   Total Lines: 57
    '    Code Lines: 45 (78.95%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (21.05%)
    '     File Size: 2.03 KB


    '     Class Interpro
    ' 
    '         Properties: abstract, contains, external_doc_list, found_in, id
    '                     member_list, name, parent_list, protein_count, pub_list
    '                     sec_list, short_name, structure_db_links, taxonomy_distribution, type
    ' 
    '         Function: ToString
    ' 
    '     Class abstract
    ' 
    '         Properties: html
    ' 
    '         Function: CleanText, ToString, TrimInternalMarkup
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace InterPro.Xml

    <XmlType("interpro")>
    Public Class Interpro : Inherits LLMDocument
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property protein_count As Integer
        <XmlAttribute> Public Property short_name As String
        <XmlAttribute> Public Property type As String

        Public Property name As String
        Public Property abstract As abstract
        Public Property pub_list As Publication()
        Public Property parent_list As RelRef()
        Public Property contains As RelRef()
        Public Property found_in As RelRef()
        Public Property member_list As db_xref()
        Public Property external_doc_list As db_xref()
        Public Property structure_db_links As db_xref()
        Public Property taxonomy_distribution As TaxonData()
        Public Property sec_list As SecAcc()

        Public Overrides Function ToString() As String
            Return $"[{type}] {short_name}   {name}"
        End Function
    End Class

    Public Class abstract : Inherits LLMDocument

        <XmlText> Public Property html As String

        Public Overrides Function ToString() As String
            Return html
        End Function

        Public Shared Function CleanText(text As String) As String
            Dim abstract = text.Match("[<]abstract.+[<][/]abstract[>]", RegexICSng)

            If abstract = "" Then
                Return text
            Else
                Return text.Replace(abstract, TrimInternalMarkup(abstract))
            End If
        End Function

        Private Shared Function TrimInternalMarkup(abstract As String) As String
            Dim text As String = abstract.GetStackValue(">", "<")
            Dim escape As String = text.Replace("<", "&lt;")

            Return abstract.Replace(text, escape)
        End Function

    End Class
End Namespace
