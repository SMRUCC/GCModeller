Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.DatabaseServices.Regtransbase.WebServices
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Regprecise

    ''' <summary>
    ''' regulogs.Xml RegPrecise数据库之中已经完成的Motif位点的数据
    ''' </summary>
    <XmlType("MotifSite")> Public Class MotifSitelog
        Implements sIdEnumerable

        Public Property Family As String
        Public Property RegulationMode As String
        Public Property BiologicalProcess As String
        Public Property Effector As String
        Public Property Regulog As KeyValuePair
        Public Property Taxonomy As KeyValuePair
        Public Property Sites As FastaObject()
        <XmlAttribute> Public Property logo As String

        Private Property Identifier As String Implements sIdEnumerable.Identifier
            Get
                Return Regulog.Key
            End Get
            Set(value As String)
                Regulog.Key = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function ExportMotifs() As FastaToken()
            Dim LQuery As FastaToken() =
                LinqAPI.Exec(Of FastaToken) <= From fa As FastaObject
                                               In Sites
                                               Let attrs As String() = {String.Format("[gene={0}] [family={1}] [regulog={2}]", fa.LocusTag, Family, Regulog.Key)}
                                               Select New FastaToken With {
                                                   .SequenceData = Regtransbase.WebServices.Regulator.SequenceTrimming(fa),
                                                   .Attributes = attrs
                                               }
            Return LQuery
        End Function

        Public Shared Function Name(log As MotifSitelog) As String
            Dim s As String = log.Taxonomy.Key.NormalizePathString & "_" & log.Regulog.Key.NormalizePathString
            s = s.Replace(" ", "_").Replace("-", "_")
            Return s
        End Function
    End Class
End Namespace