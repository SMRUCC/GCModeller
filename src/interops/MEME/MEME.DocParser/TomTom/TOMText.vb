Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DataImports

Namespace DocumentFormat.TomTOM

    Public Class TOMText

        ''' <summary>
        ''' #Query ID
        ''' </summary>
        ''' <returns></returns>
        <Column("#Query ID")> Public Property Query As String
        ''' <summary>
        ''' Target ID
        ''' </summary>
        ''' <returns></returns>
        <Column("Target ID")> Public Property Target As String
        <Column("Optimal offset")> Public Property OptimalOffset As Double
        <Column("p-value")> Public Property pvalue As Double
        <Column("E-value")> Public Property evalue As Double
        <Column("q-value")> Public Property qvalue As Double
        Public Property Overlap As Double
        <Column("Query consensus")> Public Property QueryConsensus As String
        <Column("Target consensus")> Public Property TargetConsensus As String
        Public Property Orientation As String

        Public Overrides Function ToString() As String
            Return $"{Query} => {Target}:   {pvalue}  {evalue}  {qvalue}"
        End Function

        Public Shared Function LoadDoc(path As String) As TOMText()
            Return path.Imports(Of TOMText)(delimiter:=vbTab)
        End Function
    End Class
End Namespace