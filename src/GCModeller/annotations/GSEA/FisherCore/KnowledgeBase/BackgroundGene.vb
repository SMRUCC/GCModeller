#Region "Microsoft.VisualBasic::7df5a8dcf879f284d482cc281558a1e1, GCModeller\annotations\GSEA\FisherCore\KnowledgeBase\BackgroundGene.vb"

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

    '   Total Lines: 23
    '    Code Lines: 14
    ' Comment Lines: 4
    '   Blank Lines: 5
    '     File Size: 599 B


    ' Class BackgroundGene
    ' 
    '     Properties: locus_tag, name, term_id
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

''' <summary>
''' a member gene in a gsea cluster model
''' </summary>
<XmlType("gene")>
Public Class BackgroundGene : Inherits Synonym

    ''' <summary>
    ''' The gene name
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property name As String

    <XmlElement>
    Public Property term_id As NamedValue()

    ''' <summary>
    ''' a tuple data of ``[geneId => description]`` mapping.
    ''' </summary>
    ''' <returns></returns>
    Public Property locus_tag As NamedValue

    Public Overrides Function ToString() As String
        Return $"{MyBase.ToString}  [{locus_tag.text}]"
    End Function

    Public Shared Iterator Function UnknownTerms(ParamArray term_ids As String()) As IEnumerable(Of NamedValue)
        For Each id As String In term_ids
            Yield New NamedValue With {
                .name = "Unknown",
                .text = id
            }
        Next
    End Function

End Class
