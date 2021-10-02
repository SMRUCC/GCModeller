#Region "Microsoft.VisualBasic::5cc6d088b91bb0ad75463610c4e5c2cb, DataTools\Tools\Report\DocumentElements.vb"

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

    '     Class ProteinAnnotationResult
    ' 
    '         Properties: AnnotationSource, COG, Headers, Orthologs, Paralogs
    '                     PossibleFunction, Protein, ProteinSequence, Title
    ' 
    '         Function: ToString
    ' 
    '     Class AnnotationSource
    ' 
    '         Properties: Evalue, Hit, Identities, OrganismSpecies
    ' 
    '         Function: ToString
    ' 
    '     Class Paralogs
    ' 
    ' 
    ' 
    '     Class Orthologs
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Reports.DocumentElements

    Public Class ProteinAnnotationResult : Implements IAbstractFastaToken

        <XmlAttribute("GeneID")> Public Property Protein As String
        Public Property Orthologs As Orthologs()
        Public Property Paralogs As Paralogs()

        <XmlAttribute> Public Property COG As String
        Public Property PossibleFunction As String
        Public Property AnnotationSource As [Property]
        <XmlText> Public Property ProteinSequence As String Implements IPolymerSequenceModel.SequenceData

        Public ReadOnly Property Title As String Implements SequenceModel.FASTA.IAbstractFastaToken.Title
            Get
                Return PossibleFunction
            End Get
        End Property

        Public Property Headers As String() Implements IAbstractFastaToken.Headers

        Public Overrides Function ToString() As String
            Return String.Format("{0} :   {1}", Protein, PossibleFunction)
        End Function
    End Class

    Public MustInherit Class AnnotationSource

        <XmlAttribute> Public Property OrganismSpecies As String

        ''' <summary>
        ''' The hit protein gene id.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Hit As String
        <XmlAttribute> Public Property Evalue As Double
        <XmlAttribute> Public Property Identities As Double

        Public Overrides Function ToString() As String
            Return Hit
        End Function
    End Class

    Public Class Paralogs : Inherits AnnotationSource

    End Class

    Public Class Orthologs : Inherits AnnotationSource

    End Class
End Namespace
