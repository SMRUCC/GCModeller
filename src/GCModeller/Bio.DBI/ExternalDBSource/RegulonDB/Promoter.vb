Imports Oracle.LinuxCompatibility.MySQL.Client.Reflection.DbAttributes

Namespace RegulonDB

    Public Class Promoter : Inherits LANS.SystemsBiology.ExternalDBSource.RegulonDB.ObjectItem
        <DatabaseField("promoter_id")> <Xml.Serialization.XmlElement("PROMOTER_ID")> Public Property ID As String
        <DatabaseField("promoter_name")> <Xml.Serialization.XmlElement("PROMOTER_NAME")> Public Property Name As String
        <DatabaseField("promoter_strand")> <Xml.Serialization.XmlElement("PROMOTER_STRAND")> Public Property Strand As String
        <DatabaseField("pos_1")> <Xml.Serialization.XmlElement("POS_1")> Public Property Position As Decimal
        <DatabaseField("sigma_factor")> <Xml.Serialization.XmlElement("SIGMA_FACTOR")> Public Property SigmaFactor As String
        <DatabaseField("basal_trans_val")> <Xml.Serialization.XmlElement("BASAL_TRANS_VAL")> Public Property BasalTransValue As Decimal
        <DatabaseField("equilibrium_const")> <Xml.Serialization.XmlElement("EQUILIBRIUM_CONST")> Public Property EquilibriumConst As Decimal
        <DatabaseField("kinetic_const")> <Xml.Serialization.XmlElement("KINETIC_CONST")> Public Property KineticConst As Decimal
        <DatabaseField("strength_seq")> <Xml.Serialization.XmlElement("STRENGTH_SEQ")> Public Property StrengthSeq As Decimal
        <DatabaseField("promoter_sequence")> <Xml.Serialization.XmlElement("PROMOTER_SEQUENCE")> Public Property Sequence As String
        <DatabaseField("promoter_note")> <Xml.Serialization.XmlElement("PROMOTER_NOTE")> Public Property Note As String
        <DatabaseField("key_id_org")> <Xml.Serialization.XmlElement("KEY_ID_ORG")> Public Property KeyIdOrg As String
        <DatabaseField("promoter_internal_comment")> <Xml.Serialization.XmlIgnore> Public Property Comment As String

        Public Shared Function ExportFasta(Table As Generic.IEnumerable(Of Promoter)) As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            Dim LQuery = From Promoter As Promoter In Table
                         Select New LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject With
                                {
                                    .SequenceData = Promoter.Sequence,
                                    .Attributes = New String() {Promoter.ID, Promoter.Name, Promoter.SigmaFactor}
                                } '
            Return LQuery.ToList
        End Function

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class
End Namespace