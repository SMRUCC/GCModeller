#Region "Microsoft.VisualBasic::e07c271d78ba202131573779d48f3c42, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\FeatureBriefs\PTT\Rpt.vb"

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

    '     Class Rpt
    ' 
    '         Properties: Accession, CDSCount, GeneticCode, GI, NumberOfGenes
    '                     Others, ProteinCount, PseudoCDSCount, PseudoGeneCount, Publications
    '                     RNACount, Size, Taxid, Taxname, Total
    ' 
    '         Function: CopyTo, GetValue, Load, (+2 Overloads) Save, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Class Rpt : Implements ISaveHandle

        Public Property Accession As String
        Public Property GI As String
        ''' <summary>
        ''' The chromesome DNA total length in bp
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Size As Integer
        Public Property Taxname As String
        Public Property Taxid As String
        Public Property GeneticCode As String
        Public Property Publications As String()
        Public Property ProteinCount As Integer
        Public Property CDSCount As Integer
        Public Property PseudoCDSCount As Integer
        Public Property RNACount As Integer
        Public Property NumberOfGenes As Integer
        Public Property PseudoGeneCount As Integer
        Public Property Others As Integer
        Public Property Total As Integer

        Public Overrides Function ToString() As String
            Return Taxname
        End Function

        ''' <summary>
        ''' 从一个*.rpt文件之中加载一个基因组的摘要信息
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="FilePath"></param>
        ''' <returns></returns>
        Public Shared Function Load(Of T As Rpt)(FilePath As String) As T
            Dim Rpt As T = Activator.CreateInstance(Of T)()
            Dim innerBuffer As String() = FilePath.ReadAllLines

            Rpt.Accession = GetValue(innerBuffer, "Accession: ")
            Rpt.GI = GetValue(innerBuffer, "GI: ")
            Rpt.Size = Val(GetValue(innerBuffer, "DNA  length = "))
            Rpt.Taxname = GetValue(innerBuffer, "Taxname: ")
            Rpt.Taxid = GetValue(innerBuffer, "Taxid: ")
            Rpt.GeneticCode = GetValue(innerBuffer, "Genetic Code: ")
            Rpt.Publications = Strings.Split(GetValue(innerBuffer, "Publications: "), "; ")
            Rpt.ProteinCount = GetValue(innerBuffer, "Protein count: ").ParseInteger
            Rpt.CDSCount = GetValue(innerBuffer, "CDS count: ").ParseInteger
            Rpt.PseudoCDSCount = GetValue(innerBuffer, "Pseudo CDS count: ").ParseInteger
            Rpt.RNACount = GetValue(innerBuffer, "RNA count: ").ParseInteger
            Rpt.NumberOfGenes = GetValue(innerBuffer, "Gene count: ").ParseInteger
            Rpt.PseudoGeneCount = GetValue(innerBuffer, "Pseudo gene count: ").ParseInteger
            Rpt.Others = GetValue(innerBuffer, "Others: ").ParseInteger
            Rpt.Total = GetValue(innerBuffer, "Total: ").ParseInteger

            Return Rpt
        End Function

        Public Overloads Function CopyTo(Of T As Rpt)() As T
            Dim obj As T = Activator.CreateInstance(Of T)()

            obj.Accession = Accession
            obj.GI = GI
            obj.Size = Size
            obj.Taxname = Taxname
            obj.Taxid = Taxid
            obj.GeneticCode = GeneticCode
            obj.Publications = Publications
            obj.ProteinCount = ProteinCount
            obj.CDSCount = CDSCount
            obj.PseudoCDSCount = PseudoCDSCount
            obj.RNACount = RNACount
            obj.NumberOfGenes = NumberOfGenes
            obj.PseudoGeneCount = PseudoGeneCount
            obj.Others = Others
            obj.Total = Total

            Return obj
        End Function

        Private Shared Function GetValue(data As String(), Key As String) As String
            Dim LQuery As String =
                LinqAPI.DefaultFirst(Of String) <= From sValue As String In data
                                                   Let strKey As String = Mid(sValue, 1, Len(Key))
                                                   Where String.Equals(strKey, Key, StringComparison.OrdinalIgnoreCase)
                                                   Select sValue.Replace(Key, "")
            Return Strings.Trim(LQuery)
        End Function

        Public Function Save(FilePath As String, Encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Throw New NotImplementedException()
        End Function

        Public Function Save(FilePath As String, Optional Encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(FilePath, Encoding.CodePage)
        End Function
    End Class
End Namespace
