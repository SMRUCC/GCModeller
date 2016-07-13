Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace gast

    ''' <summary>
    ''' gast -in input_fasta -ref reference_uniques_fasta -rtax reference_dupes_taxonomy [-mp min_pct_id] [-m majority] -out output_file
    ''' </summary>
    Public Structure ARGV

        ''' <summary>
        ''' input_fasta
        ''' </summary>
        ''' <returns></returns>
        Public Property [in] As String
        ''' <summary>
        ''' reference_uniques_fasta
        ''' </summary>
        ''' <returns></returns>
        Public Property ref As String
        ''' <summary>
        ''' reference_dupes_taxonomy 
        ''' </summary>
        ''' <returns></returns>
        Public Property rtax As String
        ''' <summary>
        ''' [min_pct_id] 
        ''' </summary>
        ''' <returns></returns>
        Public Property mp As String
        ''' <summary>
        ''' [majority] 
        ''' </summary>
        ''' <returns></returns>
        Public Property m As String
        ''' <summary>
        ''' output_file
        ''' </summary>
        ''' <returns></returns>
        Public Property out As String

        Sub New(args As CommandLine)
            [in] = args - "-in"
            ref = args - "-ref"
            rtax = args - "-rtax"
            mp = args - "-mp"
            m = args - "-m"
            out = args - "-out"
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace