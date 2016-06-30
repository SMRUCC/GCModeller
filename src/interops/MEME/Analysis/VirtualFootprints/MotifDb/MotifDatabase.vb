Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Analysis.GenomeMotifFootPrints.MotifDb

    <XmlRoot("MotifDatabase", Namespace:="http://code.google.com/p/genome-in-code/meme/motif-database")>
    Public Class MotifDatabase

        <XmlAttribute("Database.BuildTime")>
        Public Property DatabaseBuildTime As String
        <XmlElement("MotifFamilies")>
        Public Property MotifFamilies As MotifFamily()
    End Class

    Public Class MotifFamily : Implements IKeyValuePairObject(Of String, Motif())

        Public Property Family As String Implements IKeyValuePairObject(Of String, Motif()).Identifier
        Public Property Motifs As Motif() Implements IKeyValuePairObject(Of String, Motif()).Value

        Public Overrides Function ToString() As String
            Return String.Format("{0} {1} motifs", Family, Motifs.Count)
        End Function
    End Class
End Namespace