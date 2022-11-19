#Region "Microsoft.VisualBasic::ce008cdf849376c6bc640cc00df40082, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\Bacterial_GENOME\GeneObject.vb"

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

    '   Total Lines: 56
    '    Code Lines: 28
    ' Comment Lines: 20
    '   Blank Lines: 8
    '     File Size: 2.15 KB


    '     Class GeneObject
    ' 
    '         Properties: AccessionId, CommonName, ProteinProduct, TranscriptionDirection, TranscriptProduct
    ' 
    '         Function: CastTo, ToString
    '         Class Protein
    ' 
    '             Properties: Domains, Identifier
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels

Namespace GCML_Documents.XmlElements.Bacterial_GENOME

    ''' <summary>
    ''' 基因对象仅是模板信息的承载体，所有的转录动作是发生于转录单元对象之上的
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneObject : Inherits T_MetaCycEntity(Of Slots.Gene)

        ''' <summary>
        ''' NCBI ID
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute("NCBI-AccessionId")> Public Property AccessionId As String
        <XmlElement("COMMON-NAME")> Public Property CommonName As String
        <XmlAttribute> Public Property TranscriptionDirection As String

        ''' <summary>
        ''' 这个基因对象所表达出来的蛋白质分子
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ProteinProduct As String
        ''' <summary>
        ''' 这个基因对象所转录出来的RNA分子
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TranscriptProduct As String

        <XmlType("ProteinProduct")> Public Class Protein : Implements INamedValue
            <XmlAttribute> Public Property Identifier As String Implements INamedValue.Key
            <XmlAttribute("Pfam")> Public Property Domains As String()
        End Class

        Public Overrides Function ToString() As String
            Return Identifier
        End Function

        Public Shared Function CastTo(e As Slots.Gene) As GeneObject
            Dim Gene As GeneObject = New GeneObject With {.BaseType = e}
            Gene.Identifier = e.Identifier
            Gene.AccessionId = e.Accession1
            Gene.CommonName = e.CommonName
            Gene.TranscriptionDirection = e.TranscriptionDirection

            Return Gene
        End Function
    End Class
End Namespace
