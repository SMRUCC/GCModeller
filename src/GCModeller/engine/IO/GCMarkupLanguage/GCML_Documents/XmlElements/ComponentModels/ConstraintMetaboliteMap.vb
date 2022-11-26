#Region "Microsoft.VisualBasic::ed4e76c321920bc58e9989d8b45c1791, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\ComponentModels\ConstraintMetaboliteMap.vb"

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

    '   Total Lines: 262
    '    Code Lines: 230
    ' Comment Lines: 17
    '   Blank Lines: 15
    '     File Size: 19.72 KB


    '     Class ConstraintMetaboliteMap
    ' 
    '         Properties: ConstraintId, ModelId
    ' 
    '         Function: CreateEmptyObjects, CreateObjectsWithMetaCyc, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace GCML_Documents.ComponentModels

    Public Class ConstraintMetaboliteMap : Implements INamedValue

        ''' <summary>
        ''' 被GCModeller所识别的代谢底物的标识符
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property ConstraintId As String Implements INamedValue.Key
        ''' <summary>
        ''' 例如MetaCyc数据库中的UniqueId标识符
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("I_Model_UID", Namespace:="http://code.google.com/p/genome-in-code/GCModeller/Dynamics/Mapping")>
        Public Property ModelId As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} <--> {1}", ConstraintId, ModelId)
        End Function

        Public Const CONSTRAINT_ALA_TRNA_CHARGED As String = "ALA-TRNAS+"
        Public Const CONSTRAINT_ARG_TRNA_CHARGED As String = "ARG-TRNAS+"
        Public Const CONSTRAINT_ASN_TRNA_CHARGED As String = "ASN-TRNAS+"
        Public Const CONSTRAINT_ASP_TRNA_CHARGED As String = "ASP-TRNAS+"
        Public Const CONSTRAINT_CYS_TRNA_CHARGED As String = "CYS-TRNAS+"
        Public Const CONSTRAINT_GLN_TRNA_CHARGED As String = "GLN-TRNAS+"
        Public Const CONSTRAINT_GLT_TRNA_CHARGED As String = "GLT-TRNAS+"
        Public Const CONSTRAINT_GLY_TRNA_CHARGED As String = "GLY-TRNAS+"
        Public Const CONSTRAINT_HIS_TRNA_CHARGED As String = "HIS-TRNAS+"
        Public Const CONSTRAINT_ILE_TRNA_CHARGED As String = "ILE-TRNAS+"
        Public Const CONSTRAINT_LEU_TRNA_CHARGED As String = "LEU-TRNAS+"
        Public Const CONSTRAINT_LYS_TRNA_CHARGED As String = "LYS-TRNAS+"
        Public Const CONSTRAINT_MET_TRNA_CHARGED As String = "MET-TRNAS+"
        Public Const CONSTRAINT_PHE_TRNA_CHARGED As String = "PHE-TRNAS+"
        Public Const CONSTRAINT_PRO_TRNA_CHARGED As String = "PRO-TRNAS+"
        Public Const CONSTRAINT_SER_TRNA_CHARGED As String = "SER-TRNAS+"
        Public Const CONSTRAINT_THR_TRNA_CHARGED As String = "THR-TRNAS+"
        Public Const CONSTRAINT_TRP_TRNA_CHARGED As String = "TRP-TRNAS+"
        Public Const CONSTRAINT_TYR_TRNA_CHARGED As String = "TYR-TRNAS+"
        Public Const CONSTRAINT_VAL_TRNA_CHARGED As String = "VAL-TRNAS+"
        Public Const CONSTRAINT_WATER_MOLECULE As String = "H2O"

        Public Const CONSTRAINT_ALA_TRNA As String = "ALA-TRNAS"
        Public Const CONSTRAINT_ARG_TRNA As String = "ARG-TRNAS"
        Public Const CONSTRAINT_ASN_TRNA As String = "ASN-TRNAS"
        Public Const CONSTRAINT_ASP_TRNA As String = "ASP-TRNAS"
        Public Const CONSTRAINT_CYS_TRNA As String = "CYS-TRNAS"
        Public Const CONSTRAINT_GLN_TRNA As String = "GLN-TRNAS"
        Public Const CONSTRAINT_GLT_TRNA As String = "GLT-TRNAS"
        Public Const CONSTRAINT_GLY_TRNA As String = "GLY-TRNAS"
        Public Const CONSTRAINT_HIS_TRNA As String = "HIS-TRNAS"
        Public Const CONSTRAINT_ILE_TRNA As String = "ILE-TRNAS"
        Public Const CONSTRAINT_LEU_TRNA As String = "LEU-TRNAS"
        Public Const CONSTRAINT_LYS_TRNA As String = "LYS-TRNAS"
        Public Const CONSTRAINT_MET_TRNA As String = "MET-TRNAS"
        Public Const CONSTRAINT_PHE_TRNA As String = "PHE-TRNAS"
        Public Const CONSTRAINT_PRO_TRNA As String = "PRO-TRNAS"
        Public Const CONSTRAINT_SER_TRNA As String = "SER-TRNAS"
        Public Const CONSTRAINT_THR_TRNA As String = "THR-TRNAS"
        Public Const CONSTRAINT_TRP_TRNA As String = "TRP-TRNAS"
        Public Const CONSTRAINT_TYR_TRNA As String = "TYR-TRNAS"
        Public Const CONSTRAINT_VAL_TRNA As String = "VAL-TRNAS"

        Public Const CONSTRAINT_ATP As String = "ATP"
        Public Const CONSTRAINT_ADP As String = "ADP"

        Public Const CONSTRAINT_CTP As String = "CTP"
        Public Const CONSTRAINT_GTP As String = "GTP"
        Public Const CONSTRAINT_UTP As String = "UTP"

        Public Const CONSTRAINT_ALA As String = "ALA"
        Public Const CONSTRAINT_ARG As String = "ARG"
        Public Const CONSTRAINT_ASP As String = "ASP"
        Public Const CONSTRAINT_ASN As String = "ASN"
        Public Const CONSTRAINT_CYS As String = "CYS"
        Public Const CONSTRAINT_GLN As String = "GLN"
        Public Const CONSTRAINT_GLU As String = "GLT"
        Public Const CONSTRAINT_GLY As String = "GLY"
        Public Const CONSTRAINT_HIS As String = "HIS"
        Public Const CONSTRAINT_ILE As String = "ILE"
        Public Const CONSTRAINT_LEU As String = "LEU"
        Public Const CONSTRAINT_LYS As String = "LYS"
        Public Const CONSTRAINT_MET As String = "MET"
        Public Const CONSTRAINT_PHE As String = "PHE"
        Public Const CONSTRAINT_PRO As String = "PRO"
        Public Const CONSTRAINT_SER As String = "SER"
        Public Const CONSTRAINT_THR As String = "THR"
        Public Const CONSTRAINT_TRP As String = "TRP"
        Public Const CONSTRAINT_TYR As String = "TYR"
        Public Const CONSTRAINT_VAL As String = "VAL"

        Public Const CONSTRAINT_PI As String = "PI"

        Public Shared Function CreateEmptyObjects() As ConstraintMetaboliteMap()
            Dim CMS As ConstraintMetaboliteMap() = New ConstraintMetaboliteMap() {
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ALA_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ARG_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ASN_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ASP_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ATP},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_CYS_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLN_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLT_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLY_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_HIS_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ILE_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_LEU_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_LYS_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_MET_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PHE_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PRO_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_SER_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_THR_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_TRP_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_TYR_TRNA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_VAL_TRNA},
 _
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ALA_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ARG_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ASN_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ASP_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ADP},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_CYS_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLN_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLT_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLY_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_HIS_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ILE_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_LEU_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_LYS_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_MET_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PHE_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PRO_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_SER_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_THR_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_TRP_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_TYR_TRNA_CHARGED},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_VAL_TRNA_CHARGED},
 _
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ALA},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ARG},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ASN},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ASP},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_CYS},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLN},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLU},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLY},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_HIS},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ILE},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_LEU},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_LYS},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_MET},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PHE},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PRO},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_SER},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_THR},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_TRP},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_TYR},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_VAL},
 _
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_CTP},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GTP},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_UTP},
 _
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_WATER_MOLECULE},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PI}}

            Return CMS
        End Function

        ''' <summary>
        ''' 创建GCModeller与MetaCyc的约束底物的固定相互连接，之后就可以通过MetaCycId来实现映射了
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateObjectsWithMetaCyc() As ConstraintMetaboliteMap()

            Dim CMS As ConstraintMetaboliteMap() = New ConstraintMetaboliteMap() {
 _
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ALA_TRNA, .ModelId = "ALA-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ARG_TRNA, .ModelId = "ARG-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ASN_TRNA, .ModelId = "ASN-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ASP_TRNA, .ModelId = "ASP-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ATP, .ModelId = "ATP"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_CYS_TRNA, .ModelId = "CYS-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLN_TRNA, .ModelId = "GLN-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLT_TRNA, .ModelId = "GLT-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLY_TRNA, .ModelId = "GLY-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_HIS_TRNA, .ModelId = "HIS-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ILE_TRNA, .ModelId = "ILE-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_LEU_TRNA, .ModelId = "LEU-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_LYS_TRNA, .ModelId = "LYS-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_MET_TRNA, .ModelId = "MET-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PHE_TRNA, .ModelId = "PHE-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PRO_TRNA, .ModelId = "PRO-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_SER_TRNA, .ModelId = "SER-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_THR_TRNA, .ModelId = "THR-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_TRP_TRNA, .ModelId = "TRP-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_TYR_TRNA, .ModelId = "TYR-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_VAL_TRNA, .ModelId = "VAL-TRNAS"},
 _
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ALA_TRNA_CHARGED, .ModelId = "CHARGED-ALA-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ARG_TRNA_CHARGED, .ModelId = "CHARGED-ARG-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ASN_TRNA_CHARGED, .ModelId = "CHARGED-ASN-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ASP_TRNA_CHARGED, .ModelId = "CHARGED-ASP-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ADP, .ModelId = "ADP"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_CYS_TRNA_CHARGED, .ModelId = "CHARGED-CYS-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLN_TRNA_CHARGED, .ModelId = "CHARGED-GLN-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLT_TRNA_CHARGED, .ModelId = "CHARGED-GLT-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLY_TRNA_CHARGED, .ModelId = "CHARGED-GLY-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_HIS_TRNA_CHARGED, .ModelId = "CHARGED-HIS-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ILE_TRNA_CHARGED, .ModelId = "CHARGED-ILE-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_LEU_TRNA_CHARGED, .ModelId = "CHARGED-LEU-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_LYS_TRNA_CHARGED, .ModelId = "CHARGED-LYS-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_MET_TRNA_CHARGED, .ModelId = "CHARGED-MET-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PHE_TRNA_CHARGED, .ModelId = "CHARGED-PHE-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PRO_TRNA_CHARGED, .ModelId = "CHARGED-PRO-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_SER_TRNA_CHARGED, .ModelId = "CHARGED-SER-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_THR_TRNA_CHARGED, .ModelId = "CHARGED-THR-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_TRP_TRNA_CHARGED, .ModelId = "CHARGED-TRP-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_TYR_TRNA_CHARGED, .ModelId = "CHARGED-TYR-TRNAS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_VAL_TRNA_CHARGED, .ModelId = "CHARGED-VAL-TRNAS"},
 _
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ALA, .ModelId = "L-ALPHA-ALANINE"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ARG, .ModelId = "ARG"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ASN, .ModelId = "ASN"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ASP, .ModelId = "L-ASPARTATE"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_CYS, .ModelId = "CYS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLN, .ModelId = "GLN"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLU, .ModelId = "GLT"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GLY, .ModelId = "GLY"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_HIS, .ModelId = "HIS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_ILE, .ModelId = "ILE"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_LEU, .ModelId = "LEU"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_LYS, .ModelId = "LYS"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_MET, .ModelId = "MET"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PHE, .ModelId = "PHE"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PRO, .ModelId = "PRO"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_SER, .ModelId = "SER"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_THR, .ModelId = "THR"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_TRP, .ModelId = "TRP"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_TYR, .ModelId = "TYR"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_VAL, .ModelId = "VAL"},
 _
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_CTP, .ModelId = "CTP"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_GTP, .ModelId = "GTP"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_UTP, .ModelId = "UTP"},
 _
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_WATER_MOLECULE, .ModelId = "WATER"},
                New ConstraintMetaboliteMap With {.ConstraintId = CONSTRAINT_PI, .ModelId = "PI"}}

            Return CMS
        End Function
    End Class
End Namespace
