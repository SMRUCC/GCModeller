Imports System.Web.Script.Serialization
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

''' <summary>
''' Simple site information of the TF motif site.
''' (简单的Motif位点)
''' </summary>
Public Class MotifLog : Inherits SimpleSegment
    Implements INumberTag

    Public Property Family As String
    Public Property BiologicalProcess As String
    Public Property Regulog As String
    Public Property Taxonomy As String
    Public Property ATGDist As Integer Implements INumberTag.Tag
    ''' <summary>
    ''' 基因组上下文之中的位置的描述
    ''' </summary>
    ''' <returns></returns>
    Public Property Location As String
    Public Property tag As String

    ''' <summary>
    ''' 当前的这个位点对象是否是在启动子区
    ''' </summary>
    ''' <returns></returns>
    <ScriptIgnore>
    <Ignored>
    Public ReadOnly Property InPromoterRegion As Boolean
        Get
            If Location Is Nothing Then
                Return False
            End If
            Return InStr(Location, "In the promoter region of", CompareMethod.Text) > 0 OrElse
                InStr(Location, "Overlap on up_stream with", CompareMethod.Text) > 0
        End Get
    End Property

    <Meta(GetType(String))>
    Public Property tags As Dictionary(Of String, String)

    Sub New()
    End Sub

    ''' <summary>
    ''' 复制
    ''' </summary>
    ''' <param name="loci"></param>
    Sub New(loci As MotifLog)
        Call MyBase.New(loci)

        Family = loci.Family
        BiologicalProcess = loci.BiologicalProcess
        Regulog = loci.Regulog
        Taxonomy = loci.Taxonomy
        ATGDist = loci.ATGDist
        Location = loci.Location
    End Sub

    Sub New(loci As SimpleSegment)
        Call MyBase.New(loci)
    End Sub
End Class