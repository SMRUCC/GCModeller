Imports Microsoft.VisualBasic.ComponentModel

Namespace GCML_Documents.ComponentModels

    Public Module SystemVariables

        Public Const ID_RNA_POLYMERASE_PROTEIN = "ID_RNA_POLYMERASE_PROTEIN"
        Public Const ID_DNA_POLYMERASE_PROTEIN = "ID_DNA_POLYMERASE_PROTEIN"
        Public Const ID_RIBOSOME_ASSEMBLY_COMPLEXES = "ID_RIBOSOME_ASSEMBLY_COMPLEXES"
        Public Const ID_POLYPEPTIDE_DISPOSE_CATALYST = "ID_POLYPEPTIDE_DISPOSE_CATALYST"
        Public Const ID_TRANSCRIPT_DISPOSE_CATALYST = "ID_TRANSCRIPT_DISPOSE_CATALYST"
        Public Const ID_ENERGY_COMPOUNDS = "ID_ENERGY_COMPOUNDS"

        Public Const ID_COMPARTMENT_METABOLISM As String = "ID_COMPARTMENT_METABOLISM"
        Public Const ID_COMPARTMENT_CULTIVATION_MEDIUMS As String = "ID_COMPARTMENT_CULTIVATION_MEDIUMS"
        Public Const PARA_TRANSCRIPTION = "PARA_TRANSCRIPTION"
        Public Const PARA_TRANSLATION As String = "PARA_TRANSLATION"
        Public Const ID_PROTON As String = "ID_PROTON"
        Public Const ID_WATER As String = "ID_WATER"

        Public Const PARA_MRNA_BASAL_LEVEL As String = "PARA_MRNA_BASAL_LEVEL"
        Public Const PARA_TRNA_BASAL_LEVEL As String = "PARA_TRNA_BASAL_LEVEL"
        Public Const PARA_RRNA_BASAL_LEVEL As String = "PARA_RRNA_BASAL_LEVEL"
        Public Const _URL_CHIPDATA As String = "_URL_CHIPDATA"
        Public Const PARA_SVD_CUTOFF As String = "PARA_SVD_CUTOFF"

        ''' <summary>
        ''' 只有两种类型：Medium和Broth，当类型为Broth的时候，细胞内的水充足
        ''' </summary>
        ''' <remarks></remarks>
        Public Const PARA_CULTIVATION_MEDIUM_TYPE As String = "PARA_CULTIVATION_MEDIUM_TYPE"

        Public Function CreateDefault() As KeyValuePair()

            Return New KeyValuePair() {
 _
                    New KeyValuePair With {.Key = SystemVariables.ID_RNA_POLYMERASE_PROTEIN},
                    New KeyValuePair With {.Key = SystemVariables.ID_DNA_POLYMERASE_PROTEIN},
                    New KeyValuePair With {.Key = SystemVariables.ID_POLYPEPTIDE_DISPOSE_CATALYST},
                    New KeyValuePair With {.Key = SystemVariables.ID_RIBOSOME_ASSEMBLY_COMPLEXES},
                    New KeyValuePair With {.Key = SystemVariables.ID_TRANSCRIPT_DISPOSE_CATALYST},
                    New KeyValuePair With {.Key = SystemVariables.ID_ENERGY_COMPOUNDS},
                    New KeyValuePair With {.Key = SystemVariables.ID_COMPARTMENT_CULTIVATION_MEDIUMS},
                    New KeyValuePair With {.Key = SystemVariables.ID_COMPARTMENT_METABOLISM},
                    New KeyValuePair With {.Key = SystemVariables.PARA_TRANSCRIPTION},
                    New KeyValuePair With {.Key = SystemVariables.PARA_TRANSLATION},
                    New KeyValuePair With {.Key = SystemVariables.ID_PROTON},
                    New KeyValuePair With {.Key = SystemVariables.PARA_MRNA_BASAL_LEVEL},
                    New KeyValuePair With {.Key = SystemVariables.PARA_RRNA_BASAL_LEVEL},
                    New KeyValuePair With {.Key = SystemVariables.PARA_TRNA_BASAL_LEVEL},
                    New KeyValuePair With {.Key = SystemVariables.PARA_SVD_CUTOFF},
                    New KeyValuePair With {.Key = SystemVariables.PARA_CULTIVATION_MEDIUM_TYPE},
                    New KeyValuePair With {.Key = SystemVariables.ID_WATER}}

        End Function
    End Module
End Namespace