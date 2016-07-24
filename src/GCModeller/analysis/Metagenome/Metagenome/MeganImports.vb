Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.SequenceModel.SAM

''' <summary>
''' Megan Csv imports
''' </summary>
Public Class MeganImports

    Public Property READ_NAME As String

    <Column("CLASS-NAME")>
    Public Property CLASS_NAME As String
    Public Property SCORE As String
    Public Property COUNT As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sam">*.sam</param>
    ''' <param name="gi2taxid">txt/bin</param>
    ''' <param name="tax_dmp">nodes/names.dmp DIR</param>
    ''' <returns></returns>
    Public Shared Iterator Function [Imports](sam As String, gi2taxid As String, tax_dmp As String) As IEnumerable(Of MeganImports)
        Dim giTaxid As BucketDictionary(Of Integer, Integer) = Taxonomy.AcquireAuto(gi2taxid)
        Dim tree As New NcbiTaxonomyTree(tax_dmp)
        Dim reads As New Dictionary(Of String, List(Of String))

        For Each read As AlignmentReads In New SamStream(sam).ReadBlock

        Next
    End Function
End Class