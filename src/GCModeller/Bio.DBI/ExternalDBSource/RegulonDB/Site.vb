Imports Oracle.LinuxCompatibility.MySQL.Client.Reflection.DbAttributes
Imports LANS.SystemsBiology.Assembly.SequenceModel.FASTA

Namespace RegulonDB

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE `site` (
    '''  `site_id` char(12) NOT NULL,
    '''  `site_posleft` decimal(10,0) NOT NULL,
    '''  `site_posright` decimal(10,0) NOT NULL,
    '''  `site_sequence` varchar(100) DEFAULT NULL,
    '''  `site_note` varchar(2000) DEFAULT NULL,
    '''  `site_internal_comment` longtext,
    '''  `key_id_org` varchar(5) NOT NULL,
    '''  `site_length` decimal(10,0) DEFAULT NULL
    ''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
    ''' </remarks>
    Public Class Site
        Implements Global.LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaExportMethods.FsaObject

        <FastaExportMethods.FsaAttributeItem(0, "site")> <DatabaseField("site_id")> Public Property site_id As String
        <DatabaseField("site_posleft")> Public Property site_posleft As Decimal
        <DatabaseField("site_posright")> Public Property site_posright As Decimal
        <FastaExportMethods.FsaSequence> <DatabaseField("site_sequence")> Public Property site_sequence As String
        <DatabaseField("site_note")> Public Property site_note As String
        <DatabaseField("site_internal_comment")> Public Property site_internal_comment As String
        <DatabaseField("key_id_org")> Public Property key_id_org As String
        <DatabaseField("site_length")> Public Property site_length As Decimal

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", site_id, site_sequence)
        End Function

        Public Function GetSequenceData() As String Implements FastaExportMethods.FsaObject.GetSequenceData
            Return site_sequence
        End Function
    End Class
End Namespace