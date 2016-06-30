Imports System.Web.Script.Serialization
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.AnalysisTools.Rfam.Infernal.cmsearch
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Infernal

    Public MustInherit Class STDOUT : Inherits ClassObject

        Public Property version As String
        ''' <summary>
        ''' query sequence file
        ''' </summary>
        ''' <returns></returns>
        Public Property query As String
        ''' <summary>
        ''' target CM database
        ''' </summary>
        ''' <returns></returns>
        Public Property CM As String
    End Class

    Public MustInherit Class IHit : Inherits Contig
        <XmlAttribute> Public Property start As Long
        <XmlAttribute> Public Property [end] As Long
        <XmlAttribute> Public Property strand As Char

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation(start, [end], strand.GetStrands)
        End Function
    End Class

    ''' <summary>
    ''' 同时兼容cm_scan以及cm_search的数据
    ''' </summary>
    Public Class HitDataRow : Inherits IHit

        Public Property ORF As String

        <Column("ORF.strand")>
        Public Property direction As Char
        Public Property distance As Integer

        <Column("loci.describ")>
        Public Property LociDescrib As String

        ''' <summary>
        ''' 通过这个动态属性来兼容cm_scan以及cm_search的输出结果
        ''' </summary>
        ''' <returns></returns>
        <Meta(GetType(String))> Public Property data As Dictionary(Of String, String)

        ''' <summary>
        ''' Rfam编号
        ''' </summary>
        ''' <returns></returns>
        <Ignored> <ScriptIgnore> Public ReadOnly Property RfamAcc As String
            Get
                Dim s As String = Nothing
                Call data.TryGetValue(NameOf(RfamHit.Accession), s)
                Return s
            End Get
        End Property

        Public Function Copy() As HitDataRow
            Dim value As HitDataRow = DirectCast(Me.MemberwiseClone, HitDataRow)
            value.data =
                value.data.ToDictionary(Function(x) x.Key,
                                        Function(x) x.Value)
            Return value
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace