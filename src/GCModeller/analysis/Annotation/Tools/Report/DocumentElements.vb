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
