Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SangerSNPs

    Module DefineConstants

        Public Const KS_SEP_SPACE = 0
        Public Const KS_SEP_TAB = 1
        Public Const KS_SEP_LINE = 2
        Public Const KS_SEP_MAX = 2
        Public Const MAXIMUM_NUMBER_OF_ALT_BASES = 30
        Public Const MAX_SAMPLE_NAME_SIZE = 2048
        Public Const DEFAULT_NUM_SAMPLES = 65536
        Public Const PROGRAM_NAME = "snp-sites"
        Public Const FILENAME_MAX = 256
    End Module

    Public Structure SNPsAln
        Public number_of_samples As Integer
        Public number_of_snps As Integer
        Public sequence_names As String()
        Public snp_locations As Integer()
        Public pseudo_reference_sequence As Char()
        Public length_of_genome As Integer

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace