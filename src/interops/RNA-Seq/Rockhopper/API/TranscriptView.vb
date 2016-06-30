Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection.Reflector

Public Class TranscriptView
#Region "TSS INFORMATION"

    Public Property TSS_id As String
#Region "Position"
    Public Property pStart As Integer
    Public Property pStop As Integer
#End Region
    Public Property Strand As String
    Public Property Replicon As String

    Public Property Category As String 'Rockhopper.AnalysisAPI.Transcripts.Categories
    ''' <summary>
    ''' Subcategory Of asRNA
    ''' </summary>
    ''' <returns></returns>
    <Column("Subcategory of asRNA")> Public Property Subcategory As String
    ''' <summary>
    ''' First 3 nt
    ''' </summary>
    ''' <returns></returns>
    <Column("First 3 nt")> Public Property First As String
    ''' <summary>
    ''' Distance To start codon
    ''' </summary>
    ''' <returns></returns>
    <Column("Distance to start codon")> Public Property ATGDistance As String
    Public Property Coverage As String
#End Region
#Region "ASSOCIATED GENE"
    <Column("Gene id")> Public Property GeneId As String
    ''' <summary>
    ''' Type Of gene	
    ''' </summary>
    ''' <returns></returns>
    <Column("Type of gene")> Public Property Type As String
#Region "Position"
    Public Property gpStart As Integer
    Public Property gpStop As Integer
#End Region
    ''' <summary>
    ''' Gene product (protein-coding genes)
    ''' </summary>
    ''' <returns></returns>
    Public Property Product As String
    Public Property COG As String
    ''' <summary>
    ''' Target gene Of asRNA
    ''' </summary>
    ''' <returns></returns>
    <Column("Target gene of asRNA")> Public Property Target As String

    Public Property gStrand As String
#End Region
End Class
