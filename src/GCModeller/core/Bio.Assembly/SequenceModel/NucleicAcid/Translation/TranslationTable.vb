#Region "Microsoft.VisualBasic::bdd2c6957211fd4e2d19525e6019fad7, GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Translation\TranslationTable.vb"

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

    '   Total Lines: 110
    '    Code Lines: 66
    ' Comment Lines: 31
    '   Blank Lines: 13
    '     File Size: 4.07 KB


    '     Module TranslationTable
    ' 
    '         Function: (+2 Overloads) IsStopCoden, ToCodonCollection, (+4 Overloads) Translate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace SequenceModel.NucleotideModels.Translation

    ''' <summary>
    ''' transl_table=1 标准密码子表
    ''' </summary>
    ''' 
    <Package("NT.Translation",
                      Category:=APICategories.UtilityTools,
                      Description:="",
                      Publisher:="amethyst.asuka@gcmodeller.org",
                      Url:="")>
    Public Module TranslationTable

#Region "Translation Coden Table"

        ''' <summary>
        ''' 判断某一个密码子是否为终止密码子
        ''' </summary>
        ''' <param name="hash">该密码子的哈希值</param>
        ''' <returns>这个密码子是否为一个终止密码</returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Is.StopCoden")>
        <Extension>
        Public Function IsStopCoden(hash As Integer, code As GeneticCodes) As Boolean
            Return Array.IndexOf(TranslTable.GetTable(code).StopCodons, hash) > -1
        End Function

        <ExportAPI("Is.StopCoden")>
        <Extension>
        Public Function IsStopCoden(coden As Codon, code As GeneticCodes) As Boolean
            Return Array.IndexOf(TranslTable.GetTable(code).StopCodons, coden.TranslHashCode) > -1
        End Function
#End Region

        ''' <summary>
        ''' 自动尝试使用不同的密码表进行序列的翻译
        ''' 取最长的翻译结果
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <returns></returns>
        Public Function Translate(nt As IPolymerSequenceModel) As FastaSeq
            Dim maxSeq As New FastaSeq With {.SequenceData = ""}
            Dim prot As String
            Dim operations As String()

            For Each table As TranslTable In TranslTable._tables.Values
                operations = Nothing
                prot = table.Translate(
                    nucleicAcid:=nt.SequenceData,
                    bypassStop:=False,
                    operations:=operations
                )

                If prot.Length > maxSeq.Length Then
                    maxSeq = New FastaSeq With {
                        .Headers = {table.ToString}.Join(operations).ToArray,
                        .SequenceData = prot
                    }
                End If
            Next

            Return maxSeq
        End Function

        ''' <summary>
        ''' 将一条核酸链翻译为蛋白质序列
        ''' </summary>
        ''' <param name="NucleicAcid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Translate")>
        <Extension>
        Public Function Translate(NucleicAcid As String, Optional code As GeneticCodes = GeneticCodes.StandardCode, Optional force As Boolean = False) As String
            Return TranslTable.GetTable(code).Translate(NucleicAcid, force)
        End Function

        <ExportAPI("Translate")>
        <Extension>
        Public Function Translate(nt As NucleicAcid, code As GeneticCodes, Optional force As Boolean = False) As String
            Return TranslTable.GetTable(code).Translate(nt, force)
        End Function

        <ExportAPI("Translate")>
        <Extension>
        Public Function Translate(codon As Codon, code As GeneticCodes) As Char
            Dim AAValue = TranslTable.GetTable(code).CodenTable(codon.TranslHashCode)
            Return Polypeptides.Polypeptide.ToChar(AAValue)
        End Function

        ''' <summary>
        ''' 没有终止密码子
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("To.CodonList")>
        <Extension>
        Public Function ToCodonCollection(nt As NucleicAcid, code As GeneticCodes) As Codon()
            Return TranslTable.GetTable(code).ToCodonCollection(nt)
        End Function
    End Module
End Namespace
