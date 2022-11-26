#Region "Microsoft.VisualBasic::bc6a11bfdeabf7435d7de1acefb3b080, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\ComponentModels\SystemVariables.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 57
    '    Code Lines: 44
    ' Comment Lines: 4
    '   Blank Lines: 9
    '     File Size: 3.44 KB


    '     Module SystemVariables
    ' 
    '         Function: CreateDefault
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text.Xml.Models

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
