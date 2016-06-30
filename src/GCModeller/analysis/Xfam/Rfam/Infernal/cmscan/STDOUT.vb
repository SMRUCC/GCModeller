Imports System.Xml.Serialization
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Infernal.cmscan

    Public Class ScanSites : Inherits Infernal.STDOUT
        Public Property QueryHits As Query
    End Class

    Public Class Query : Inherits ClassObject
        Public Property title As String
        Public Property Length As Long
        Public Property Description As String
        Public Property hits As Hit()
        ''' <summary>
        ''' ------ inclusion threshold ------
        ''' </summary>
        ''' <returns></returns>
        Public Property uncertainHits As Hit()
    End Class

    Public Class Hit : Inherits IHit

        <XmlAttribute> Public Property rank As String
        <XmlAttribute> <Column("E-value")>
        Public Property Evalue As Double
        <XmlAttribute> Public Property score As Double
        <XmlAttribute> Public Property bias As Double
        <XmlAttribute> Public Property modelname As String
        <XmlAttribute> Public Property mdl As String
        <XmlAttribute> Public Property trunc As String
        <XmlAttribute> Public Property gc As Double
        <XmlAttribute> Public Property description As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class HitAlign


    End Class
End Namespace