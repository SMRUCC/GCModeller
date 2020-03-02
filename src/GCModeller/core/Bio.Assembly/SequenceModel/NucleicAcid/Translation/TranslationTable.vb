#Region "Microsoft.VisualBasic::0ef35444bcad0df153f34da0cc0e1f72, core\Bio.Assembly\SequenceModel\NucleicAcid\Translation\TranslationTable.vb"

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

    '     Module TranslationTable
    ' 
    '         Properties: CodenTable
    ' 
    '         Function: (+2 Overloads) IsStopCoden, ToCodonCollection, (+3 Overloads) Translate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel.Polypeptides.Polypeptide
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid
Imports System.Text
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.CommandLine.Reflection

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
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' 第一个碱基*1000+第二个碱基*100+第三个碱基
        ''' 
        ''' Public Enum Ribonucleotides
        '''    AMP = 0
        '''    GMP = 1
        '''    CMP = 2
        '''    UMP = 3
        ''' End Enum
        ''' 
        ''' 终止密码子
        ''' UAA 3*1000+0*100+0 -> 3000
        ''' UAG 3*1000+0*100+1 -> 3001
        ''' UGA 3*1000+1*100+0 -> 3100
        ''' </remarks>
        Public ReadOnly Property CodenTable As TranslTable = Translation.TranslTable.ParseTable(My.Resources.transl_table_1)

#Region "终止密码子的哈希值枚举"
        Public Const UAA As Integer = DNA.dTMP * 1000 + DNA.dAMP * 100 + DNA.dAMP * 10000
        Public Const UAG As Integer = DNA.dTMP * 1000 + DNA.dAMP * 100 + DNA.dGMP * 10000
        Public Const UGA As Integer = DNA.dTMP * 1000 + DNA.dGMP * 100 + DNA.dAMP * 10000
#End Region

        ''' <summary>
        ''' 判断某一个密码子是否为终止密码子
        ''' </summary>
        ''' <param name="hash">该密码子的哈希值</param>
        ''' <returns>这个密码子是否为一个终止密码</returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Is.StopCoden")>
        <Extension> Public Function IsStopCoden(hash As Integer) As Boolean
            Return hash = UAA OrElse hash = UAG OrElse hash = UGA
        End Function

        <ExportAPI("Is.StopCoden")>
        <Extension> Public Function IsStopCoden(Coden As NucleotideModels.Translation.Codon) As Boolean
            Dim hash As Integer = Coden.TranslHash
            Return hash = UAA OrElse hash = UAG OrElse hash = UGA
        End Function
#End Region

        ''' <summary>
        ''' 将一条核酸链翻译为蛋白质序列
        ''' </summary>
        ''' <param name="NucleicAcid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Translate")>
        <Extension> Public Function Translate(NucleicAcid As String, Optional force As Boolean = False) As String
            Return CodenTable.Translate(NucleicAcid, force)
        End Function

        <ExportAPI("Translate")>
        <Extension> Public Function Translate(SequenceData As NucleicAcid, Optional force As Boolean = False) As String
            Return CodenTable.Translate(SequenceData, force)
        End Function

        <ExportAPI("Translate")>
        <Extension> Public Function Translate(codon As Codon) As Char
            Dim AAValue = CodenTable.CodenTable(codon.TranslHash)
            Return Polypeptides.Polypeptide.ToChar(AAValue)
        End Function

        ''' <summary>
        ''' 没有终止密码子
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("To.CodonList")>
        <Extension> Public Function ToCodonCollection(SequenceData As NucleicAcid) As Codon()
            Return CodenTable.ToCodonCollection(SequenceData)
        End Function
    End Module
End Namespace
