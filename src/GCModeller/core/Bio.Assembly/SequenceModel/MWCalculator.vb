#Region "Microsoft.VisualBasic::19ab246f98f58957a31ff08a5055f96b, core\Bio.Assembly\SequenceModel\MWCalculator.vb"

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

    '   Total Lines: 205
    '    Code Lines: 136 (66.34%)
    ' Comment Lines: 41 (20.00%)
    '    - Xml Docs: 92.68%
    ' 
    '   Blank Lines: 28 (13.66%)
    '     File Size: 7.63 KB


    '     Module MolecularWeightCalculator
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CalcMW_Nucleotides, (+2 Overloads) CalcMW_Polypeptide, DeoxyribonucleotideFormula, PolypeptideFormula, RibonucleotideFormula
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Namespace SequenceModel

    ''' <summary>
    ''' 核酸链或者多肽链分子的相对分子质量的计算工具
    ''' </summary>
    ''' <remarks></remarks>
    <Package("MolecularWeights", Publisher:="xie.guigang@gcmodeller.org")>
    Public Module MolecularWeightCalculator

        ReadOnly AminoAcidX As FormulaData
        ReadOnly AminoAcidMolecularWeights As New SortedDictionary(Of AminoAcid, Double) From {
            {AminoAcid.Alanine, 71.0779},
            {AminoAcid.Arginine, 156.1857},
            {AminoAcid.Asparagine, 114.1026},
            {AminoAcid.AsparticAcid, 115.0874},
            {AminoAcid.Cysteine, 103.1429},
            {AminoAcid.GlutamicAcid, 129.114},
            {AminoAcid.Glutamine, 128.1292},
            {AminoAcid.Glycine, 57.0513},
            {AminoAcid.Histidine, 137.1393},
            {AminoAcid.Isoleucine, 113.1576},
            {AminoAcid.Leucine, 113.1576},
            {AminoAcid.Lysine, 128.1723},
            {AminoAcid.Methionine, 131.1961},
            {AminoAcid.Phenylalanine, 147.1739},
            {AminoAcid.Praline, 97.1152},
            {AminoAcid.Serine, 87.0773},
            {AminoAcid.Threonine, 101.1039},
            {AminoAcid.Tryptophane, 186.2099},
            {AminoAcid.Tyrosine, 163.1733},
            {AminoAcid.Valine, 99.1311}
        }

#Region "Deoxyribonucleotide - DNA"
        ''' <summary>
        ''' C10H12N5O6P + H2O
        ''' </summary>
        Const dAMP As Double = 347.0625338201
        ''' <summary>
        ''' C10H13N2O8P + H2O
        ''' </summary>
        Const dTMP As Double = 338.0509664201
        ''' <summary>
        ''' C9H12N3O7P + H2O
        ''' </summary>
        Const dCMP As Double = 323.0513008201
        ''' <summary>
        ''' C10H12N5O7P + H2O
        ''' </summary>
        Const dGMP As Double = 363.0574488201
#End Region

#Region "Ribonucleotide - RNA"
        ''' <summary>
        ''' C10H12N5O7P
        ''' </summary>
        Const AMP As Double = 345.0468846201
        ''' <summary>
        ''' C9H11N2O9P
        ''' </summary>
        Const UMP As Double = 322.0196680201
        ''' <summary>
        ''' C9H12N3O8P
        ''' </summary>
        Const CMP As Double = 321.0356516201
        ''' <summary>
        ''' C10H12N5O8P
        ''' </summary>
        Const GMP As Double = 361.0417996201
#End Region

        ReadOnly Deoxyribonucleotide As New Dictionary(Of Char, Double) From {
            {"A"c, dAMP}, {"T"c, dTMP}, {"C"c, dCMP}, {"G"c, dGMP}
        }

        ReadOnly Ribonucleotide As New Dictionary(Of Char, Double) From {
            {"A"c, AMP}, {"U"c, UMP}, {"C"c, CMP}, {"G"c, GMP}
        }

        Sub New()
            Dim n As Integer = AminoAcidObjUtility.OneLetterFormula.Count

            AminoAcidX = New FormulaData(New Dictionary(Of String, Integer))

            For Each formula As FormulaData In AminoAcidObjUtility.OneLetterFormula.Values
                AminoAcidX = AminoAcidX + formula
            Next

            AminoAcidX = AminoAcidX / n
        End Sub

        ''' <summary>
        ''' 计算蛋白质序列的相对分子质量
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <ExportAPI("MW.Polypeptide")>
        Public Function CalcMW_Polypeptide(seq As ISequenceModel) As Double
            Return seq.SequenceData.CalcMW_Polypeptide
        End Function

        <ExportAPI("MW.Polypeptide")>
        <Extension>
        Public Function CalcMW_Polypeptide(seq As String) As Double
            Static aa_avg As Double = AminoAcidObjUtility.OneChar2Mass.Values.Average
            ' X is the unknown amino acid
            Dim mw As Double = Aggregate aa As Char
                               In seq.ToUpper
                               Let w As Double = If(
                                   aa = "X"c OrElse
                                   aa = "-"c OrElse
                                   Not AminoAcidObjUtility.OneChar2Mass.ContainsKey(aa),
                                   aa_avg, AminoAcidObjUtility.OneChar2Mass(aa))
                               Into Sum(w)
            Dim water As Double = (seq.Length - 1) * PeriodicTable.H2O

            Return mw - water
        End Function

        ''' <summary>
        ''' Create formula for protein polypeptide sequence
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        Public Function PolypeptideFormula(seq As String) As FormulaData
            Dim formula As FormulaData = FormulaData.Empty
            Dim water As FormulaData = FormulaData.H2O * (seq.Length - 1)

            For Each aa As Char In seq.ToUpper
                If aa = "X"c OrElse aa = "-"c OrElse Not AminoAcidObjUtility.OneLetterFormula.ContainsKey(aa) Then
                    formula = formula + AminoAcidX
                Else
                    formula = formula + AminoAcidObjUtility.OneLetterFormula(aa)
                End If
            Next

            Return formula - water
        End Function

        ''' <summary>
        ''' Create formula for DNA nucleotide sequence
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        Public Function DeoxyribonucleotideFormula(seq As String) As FormulaData
            Dim water As FormulaData = FormulaData.H2O * (seq.Length - 1)
            Dim formula As FormulaData = FormulaData.Empty

            Static any As FormulaData = Bases.Deoxyribonucleotide.Values.Sum / 4

            For Each c As Char In seq.ToUpper
                If c = "-"c Then
                    formula = formula + any
                Else
                    formula = formula + Bases.Deoxyribonucleotide(c)
                End If
            Next

            Return formula - water
        End Function

        ''' <summary>
        ''' Create formula for RNA nucleotide sequence
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        Public Function RibonucleotideFormula(seq As String) As FormulaData
            Dim water As FormulaData = FormulaData.H2O * (seq.Length - 1)
            Dim formula As FormulaData = FormulaData.Empty

            Static any As FormulaData = Bases.Ribonucleotide.Values.Sum / 4

            For Each c As Char In seq.ToUpper
                If c = "-"c Then
                    formula = formula + any
                Else
                    formula = formula + Bases.Ribonucleotide(c)
                End If
            Next

            Return formula - water
        End Function

        ''' <summary>
        ''' 计算核酸序列的相对分子质量
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function CalcMW_Nucleotides(seq As ISequenceModel, Optional is_rna As Boolean = False) As Double
            Return CalcMW_Nucleotides(seq.SequenceData, is_rna)
        End Function

        ''' <summary>
        ''' 计算核酸序列的相对分子质量
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function CalcMW_Nucleotides(seq As String, Optional is_rna As Boolean = False) As Double
            Dim total As Double
            Dim water As Double = (seq.Length - 1) * PeriodicTable.H2O

            seq = seq.ToUpper

            If is_rna Then
                total = Aggregate ch As Char
                        In seq
                        Into Sum(Ribonucleotide(ch))
            Else
                total = Aggregate ch As Char
                        In seq
                        Into Sum(Deoxyribonucleotide(ch))
            End If

            Return total - water
        End Function
    End Module
End Namespace
