Imports System.Xml.Serialization
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace FileSystem

    <XmlType("PfamString", [Namespace]:="http://gcmodeller.org/tools/sanger-pfam/prot_family")>
    Public Class PfamString ': Implements SequenceModel.FASTA.I_FastaToken

        <XmlAttribute> Public Property LocusTag As String
        <XmlElement> Public Property PfamString As String
        <XmlAttribute> Public Property Length As Integer
        <XmlAttribute> Public Property Domains As String()
        ' <XmlElement> Public Property Description As String

        Public Overrides Function ToString() As String
            Return $"{LocusTag}  {PfamString}"
        End Function

        Public Function AsPfamString() As Sanger.Pfam.PfamString.PfamString
            Return New Sanger.Pfam.PfamString.PfamString With {
                .Length = Length,
                .PfamString = PfamString.Split("+"c),
                .ProteinId = LocusTag,'                .Description = Description,
                .Domains = Domains
            }
        End Function

        Public Shared Function CreateObject(stringPfam As Sanger.Pfam.PfamString.PfamString) As PfamString
            Return New PfamString With {
                .Domains = stringPfam.Domains,'.Description = stringPfam.Description,
                .PfamString = stringPfam.PfamString.JoinBy("+"),
                .Length = stringPfam.Length,
                .LocusTag = stringPfam.ProteinId
            }
        End Function
    End Class
End Namespace