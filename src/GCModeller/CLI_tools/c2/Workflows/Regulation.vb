Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Public Class Regulation : Inherits LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.Html.MEMEHtml.MEMEOutput

    ''' <summary>
    ''' 如果二者在同一个Operon之中，则，本字段为该Operon的值，否则为空。当相同的Operon的时候，此时可能为自调控了
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column("Same-Operon")> Public Property SameOperon As String

    ''' <summary>
    ''' GeneId Property
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column("GeneId")> Public Overrides Property Name As String
        Get
            Return MyBase.Name
        End Get
        Set(value As String)
            MyBase.Name = value
        End Set
    End Property

    <Column("MatchedRegulator")> Public Overloads Property MatchedRegulator As String
    <Column("Family")> Public Property Family As String
    <Column("Effector")> Public Property Effector As String
    <Column("BiologicalProcess")> Public Property BiologicalProcess As String
    <Column("Regulation")> Public Property Regulation As String
    <Column("RegpreciseRegulog")> Public Property RegpreciseRegulog As String
    <Column("WGCNAWeight")> Public Property WGCNAWeight As Double
    <Column("Pcc")> Public Property Pcc As Double

    Friend Function SetSameOperon(DoorOperons As LANS.SystemsBiology.Assembly.Door.OperonView) As Regulation
        SameOperon = DoorOperons.SameOperon(MatchedRegulator, Name)
        Return Me
    End Function
End Class
