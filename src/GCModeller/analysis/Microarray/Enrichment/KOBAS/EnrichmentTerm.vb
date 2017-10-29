Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace KOBAS

    Public Class EnrichmentTerm
        Implements IGoTerm
        Implements IGoTermEnrichment
        Implements IKEGGTerm
        Implements INamedValue

        ''' <summary>
        ''' #Term
        ''' </summary>
        ''' <returns></returns>
        <Column("#Term")>
        Public Property Term As String Implements IKEGGTerm.Term
        Public Property Database As String

        ''' <summary>
        ''' <see cref="INamedValue.Key"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String Implements IGoTerm.Go_ID, IKEGGTerm.ID, INamedValue.Key

        ''' <summary>
        ''' Input number
        ''' </summary>
        ''' <returns></returns>
        <Column("Input number")> Public Property number As Integer

        ''' <summary>
        ''' Background number
        ''' </summary>
        ''' <returns></returns>
        <Column("Background number")> Public Property Backgrounds As Integer

        ''' <summary>
        ''' P-Value
        ''' </summary>
        ''' <returns></returns>
        <Column("P-Value")> Public Property Pvalue As Double Implements IGoTermEnrichment.Pvalue, IKEGGTerm.Pvalue

        ''' <summary>
        ''' Corrected P-Value
        ''' </summary>
        ''' <returns></returns>
        <Column("Corrected P-Value")> Public Property CorrectedPvalue As Double Implements IGoTermEnrichment.CorrectedPvalue

        ''' <summary>
        ''' The group of this input gene id list
        ''' </summary>
        ''' <returns></returns>
        Public Property Input As String
        Public Property ORF As String() Implements IKEGGTerm.ORF

        ''' <summary>
        ''' 用于一些可视化的超链接url
        ''' </summary>
        ''' <returns></returns>
        <Column("Hyperlink")>
        Public Property link As String Implements IKEGGTerm.Link

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace