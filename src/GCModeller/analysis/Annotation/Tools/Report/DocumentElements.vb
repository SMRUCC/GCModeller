#Region "Microsoft.VisualBasic::052bf2c1064e4223811bf467fcb4aecb, ..\GCModeller\analysis\Annotation\Tools\Report\DocumentElements.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Xml.Serialization
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Reports.DocumentElements

    Public Class ProteinAnnotationResult : Implements SMRUCC.genomics.SequenceModel.FASTA.IAbstractFastaToken

        <XmlAttribute("GeneID")> Public Property Protein As String
        Public Property Orthologs As Orthologs()
        Public Property Paralogs As Paralogs()

        <XmlAttribute> Public Property COG As String
        Public Property PossibleFunction As String
        Public Property AnnotationSource As Microsoft.VisualBasic.ComponentModel.TripleKeyValuesPair
        <XmlText> Public Property ProteinSequence As String Implements I_PolymerSequenceModel.SequenceData

        Public ReadOnly Property Title As String Implements SequenceModel.FASTA.IAbstractFastaToken.Title
            Get
                Return PossibleFunction
            End Get
        End Property

        Public Property Attributes As String() Implements IAbstractFastaToken.Attributes

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

