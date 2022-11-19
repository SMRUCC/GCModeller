#Region "Microsoft.VisualBasic::f4e8576cda1e437e77dd488beef99f72, GCModeller\core\Bio.Assembly\SequenceModel\MWCalculator.vb"

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

    '   Total Lines: 83
    '    Code Lines: 60
    ' Comment Lines: 16
    '   Blank Lines: 7
    '     File Size: 3.14 KB


    '     Module MolecularWeightCalculator
    ' 
    '         Function: CalcMW_Nucleotides, (+2 Overloads) CalcMW_Polypeptide
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Namespace SequenceModel

    ''' <summary>
    ''' 核酸链或者多肽链分子的相对分子质量的计算工具
    ''' </summary>
    ''' <remarks></remarks>
    <Package("MolecularWeights", Publisher:="xie.guigang@gcmodeller.org")>
    Public Module MolecularWeightCalculator

        Private ReadOnly AminoAcidMolecularWeights As New SortedDictionary(Of AminoAcid, Double) From {
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

        Private ReadOnly NucleicAcidsMolecularWeights As New SortedDictionary(Of Char, Double) From {
            {"A"c, 491.2},
            {"C"c, 467.2},
            {"G"c, 507.2},
            {"T"c, 482.2},
            {"U"c, 324.2}
        }

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
        <Extension> Public Function CalcMW_Polypeptide(seq As String) As Double
            Dim polypeptide = ConstructVector(seq)
            Dim mw As Double = Aggregate aa As AminoAcid
                               In polypeptide
                               Into Sum(AminoAcidMolecularWeights(aa))
            Return mw
        End Function

        ''' <summary>
        ''' 计算核酸序列的相对分子质量
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <ExportAPI("MW.NT")>
        Public Function CalcMW_Nucleotides(seq As ISequenceModel) As Double
            With seq.SequenceData
                Return Aggregate ch As Char
                       In .ToUpper
                       Into Sum(NucleicAcidsMolecularWeights(ch))
            End With
        End Function
    End Module
End Namespace
