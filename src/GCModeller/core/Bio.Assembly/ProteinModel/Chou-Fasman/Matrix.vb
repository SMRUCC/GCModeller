#Region "Microsoft.VisualBasic::bca3276ad2217d70c3f199ce89e4fa86, GCModeller\core\Bio.Assembly\ProteinModel\Chou-Fasman\Matrix.vb"

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

    '   Total Lines: 137
    '    Code Lines: 42
    ' Comment Lines: 84
    '   Blank Lines: 11
    '     File Size: 7.23 KB


    '     Module MatrixAPI
    ' 
    '         Function: Avg
    ' 
    '     Structure ChouFasmanParameter
    ' 
    '         Properties: ChouFasmanTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel

Namespace ProteinModel.ChouFasmanRules

    Module MatrixAPI

        Public ReadOnly ChouFasmanTable As ChouFasmanParameter() = ChouFasmanParameter.ChouFasmanTable

        Public Const PROPORTION As Double = 4 / 6
        Public Const CORE_LENGTH As Integer = 6

        Public Function Avg(ChunkBuffer As Polypeptides.AminoAcid(), GetValue As Func(Of ChouFasmanParameter, Integer)) As Double
            Dim LQuery = (From Token In ChunkBuffer Select GetValue(ChouFasmanTable(Token))).ToArray
            Return LQuery.Average
        End Function
    End Module

    Public Structure ChouFasmanParameter

        Dim AminoAcid As Polypeptides.AminoAcid
        Dim Pa As Integer
        Dim Pb As Integer
        Dim Pt As Integer
        ''' <summary>
        ''' f(i), f(i+1), f(i+2), f(i+3)
        ''' </summary>
        ''' <remarks></remarks>
        Dim f As Double()

#Region "Public Shared ReadOnly Property ChouFasmanTable As ChouFasmanParameter()"

        ''' <summary>
        ''' 丙氨酸（A）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _A As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Alanine, .Pa = 142, .Pb = 83, .Pt = 66, .f = New Double() {0.06, 0.076, 0.035, 0.058}}
        ''' <summary>
        ''' 精氨酸（R）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _R As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Arginine, .Pa = 98, .Pb = 93, .Pt = 95, .f = New Double() {0.07, 0.106, 0.099, 0.085}}
        ''' <summary>
        ''' 天冬酰胺（N）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _N As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Asparagine, .Pa = 67, .Pb = 89, .Pt = 156, .f = New Double() {0.161, 0.083, 0.191, 0.091}}
        ''' <summary>
        ''' 天冬氨酸（D）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _D As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.AsparticAcid, .Pa = 101, .Pb = 54, .Pt = 146, .f = New Double() {0.147, 0.11, 0.179, 0.081}}
        ''' <summary>
        ''' 半胱氨酸（C）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _C As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Cysteine, .Pa = 70, .Pb = 119, .Pt = 119, .f = New Double() {0.149, 0.05, 0.117, 0.128}}
        ''' <summary>
        ''' 谷氨酸（E）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _E As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.GlutamicAcid, .Pa = 151, .Pb = 37, .Pt = 74, .f = New Double() {0.056, 0.06, 0.077, 0.064}}
        ''' <summary>
        ''' 谷氨酰胺（Q）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _Q As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Glutamine, .Pa = 111, .Pb = 110, .Pt = 98, .f = New Double() {0.074, 0.098, 0.037, 0.098}}
        ''' <summary>
        ''' 甘氨酸（G）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _G As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Glycine, .Pa = 57, .Pb = 75, .Pt = 156, .f = New Double() {0.102, 0.085, 0.19, 0.152}}
        ''' <summary>
        ''' 组氨酸（H）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _H As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Histidine, .Pa = 100, .Pb = 87, .Pt = 95, .f = New Double() {0.14, 0.047, 0.093, 0.054}}
        ''' <summary>
        ''' 异亮氨酸（I）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _I As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Isoleucine, .Pa = 108, .Pb = 160, .Pt = 47, .f = New Double() {0.043, 0.034, 0.013, 0.056}}
        ''' <summary>
        ''' 亮氨酸（L）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _L As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Leucine, .Pa = 121, .Pb = 130, .Pt = 59, .f = New Double() {0.061, 0.025, 0.036, 0.07}}
        ''' <summary>
        ''' 赖氨酸（K）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _K As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Lysine, .Pa = 114, .Pb = 74, .Pt = 101, .f = New Double() {0.055, 0.115, 0.072, 0.095}}
        ''' <summary>
        ''' 甲硫氨酸（M）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _M As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Methionine, .Pa = 145, .Pb = 105, .Pt = 60, .f = New Double() {0.068, 0.082, 0.014, 0.055}}
        ''' <summary>
        ''' 苯丙氨酸（F）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _F As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Phenylalanine, .Pa = 113, .Pb = 138, .Pt = 60, .f = New Double() {0.059, 0.041, 0.065, 0.065}}
        ''' <summary>
        ''' 脯氨酸（P）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _P As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Praline, .Pa = 57, .Pb = 55, .Pt = 152, .f = New Double() {0.102, 0.301, 0.034, 0.068}}
        ''' <summary>
        ''' 丝氨酸（S）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _S As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Serine, .Pa = 77, .Pb = 75, .Pt = 143, .f = New Double() {0.12, 0.139, 0.125, 0.106}}
        ''' <summary>
        ''' 苏氨酸（T）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _T As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Threonine, .Pa = 83, .Pb = 119, .Pt = 96, .f = New Double() {0.086, 0.108, 0.065, 0.079}}
        ''' <summary>
        ''' 色氨酸（W）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _W As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Tryptophane, .Pa = 108, .Pb = 137, .Pt = 96, .f = New Double() {0.077, 0.013, 0.064, 0.167}}
        ''' <summary>
        ''' 酪氨酸（Y）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _Y As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Tyrosine, .Pa = 69, .Pb = 147, .Pt = 114, .f = New Double() {0.082, 0.065, 0.114, 0.125}}
        ''' <summary>
        ''' 缬氨酸（V）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _V As New ChouFasmanParameter With {.AminoAcid = Polypeptides.AminoAcid.Valine, .Pa = 106, .Pb = 170, .Pt = 50, .f = New Double() {0.062, 0.048, 0.028, 0.053}}

        Public Shared ReadOnly Property ChouFasmanTable As ChouFasmanParameter() = {_A, _R, _N, _D, _C, _E, _Q, _G, _H, _I, _L, _K, _M, _F, _P, _S, _T, _W, _Y, _V}

#End Region
    End Structure
End Namespace
