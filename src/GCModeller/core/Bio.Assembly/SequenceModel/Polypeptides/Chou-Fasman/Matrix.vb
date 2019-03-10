#Region "Microsoft.VisualBasic::1cae69105bb239d1034657a94c6d5334, Bio.Assembly\SequenceModel\Polypeptides\Chou-Fasman\Matrix.vb"

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

    '     Module MatrixAPI
    ' 
    '         Function: Avg
    ' 
    '     Structure ChouFasmanParameter
    ' 
    '         Properties: ChouFasmanTable
    ' 
    '         Function: Get_Pa, Get_Pb, Get_Pt
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace SequenceModel.Polypeptides.SecondaryStructure.ChouFasmanRules

    Module MatrixAPI

        Public ReadOnly ChouFasmanTable As ChouFasmanParameter() = ChouFasmanParameter.ChouFasmanTable

        Public Const PROPORTION As Double = 4 / 6
        Public Const CORE_LENGTH As Integer = 6

        Public Function Avg(ChunkBuffer As SequenceModel.Polypeptides.AminoAcid(), GetValue As Func(Of ChouFasmanParameter, Integer)) As Double
            Dim LQuery = (From Token In ChunkBuffer Select GetValue(ChouFasmanTable(Token))).ToArray
            Return LQuery.Average
        End Function
    End Module

    Public Structure ChouFasmanParameter

        Dim AminoAcid As SequenceModel.Polypeptides.AminoAcid
        Dim P_a As Integer
        Dim P_b As Integer
        Dim P_t As Integer
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
        Private Shared ReadOnly _A As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Alanine, .P_a = 142, .P_b = 83, .P_t = 66, .f = New Double() {0.06, 0.076, 0.035, 0.058}}
        ''' <summary>
        ''' 精氨酸（R）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _R As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Arginine, .P_a = 98, .P_b = 93, .P_t = 95, .f = New Double() {0.07, 0.106, 0.099, 0.085}}
        ''' <summary>
        ''' 天冬酰胺（N）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _N As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Asparagine, .P_a = 67, .P_b = 89, .P_t = 156, .f = New Double() {0.161, 0.083, 0.191, 0.091}}
        ''' <summary>
        ''' 天冬氨酸（D）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _D As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.AsparticAcid, .P_a = 101, .P_b = 54, .P_t = 146, .f = New Double() {0.147, 0.11, 0.179, 0.081}}
        ''' <summary>
        ''' 半胱氨酸（C）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _C As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Cysteine, .P_a = 70, .P_b = 119, .P_t = 119, .f = New Double() {0.149, 0.05, 0.117, 0.128}}
        ''' <summary>
        ''' 谷氨酸（E）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _E As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.GlutamicAcid, .P_a = 151, .P_b = 37, .P_t = 74, .f = New Double() {0.056, 0.06, 0.077, 0.064}}
        ''' <summary>
        ''' 谷氨酰胺（Q）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _Q As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Glutamine, .P_a = 111, .P_b = 110, .P_t = 98, .f = New Double() {0.074, 0.098, 0.037, 0.098}}
        ''' <summary>
        ''' 甘氨酸（G）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _G As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Glycine, .P_a = 57, .P_b = 75, .P_t = 156, .f = New Double() {0.102, 0.085, 0.19, 0.152}}
        ''' <summary>
        ''' 组氨酸（H）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _H As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Histidine, .P_a = 100, .P_b = 87, .P_t = 95, .f = New Double() {0.14, 0.047, 0.093, 0.054}}
        ''' <summary>
        ''' 异亮氨酸（I）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _I As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Isoleucine, .P_a = 108, .P_b = 160, .P_t = 47, .f = New Double() {0.043, 0.034, 0.013, 0.056}}
        ''' <summary>
        ''' 亮氨酸（L）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _L As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Leucine, .P_a = 121, .P_b = 130, .P_t = 59, .f = New Double() {0.061, 0.025, 0.036, 0.07}}
        ''' <summary>
        ''' 赖氨酸（K）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _K As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Lysine, .P_a = 114, .P_b = 74, .P_t = 101, .f = New Double() {0.055, 0.115, 0.072, 0.095}}
        ''' <summary>
        ''' 甲硫氨酸（M）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _M As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Methionine, .P_a = 145, .P_b = 105, .P_t = 60, .f = New Double() {0.068, 0.082, 0.014, 0.055}}
        ''' <summary>
        ''' 苯丙氨酸（F）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _F As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Phenylalanine, .P_a = 113, .P_b = 138, .P_t = 60, .f = New Double() {0.059, 0.041, 0.065, 0.065}}
        ''' <summary>
        ''' 脯氨酸（P）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _P As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Praline, .P_a = 57, .P_b = 55, .P_t = 152, .f = New Double() {0.102, 0.301, 0.034, 0.068}}
        ''' <summary>
        ''' 丝氨酸（S）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _S As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Serine, .P_a = 77, .P_b = 75, .P_t = 143, .f = New Double() {0.12, 0.139, 0.125, 0.106}}
        ''' <summary>
        ''' 苏氨酸（T）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _T As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Threonine, .P_a = 83, .P_b = 119, .P_t = 96, .f = New Double() {0.086, 0.108, 0.065, 0.079}}
        ''' <summary>
        ''' 色氨酸（W）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _W As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Tryptophane, .P_a = 108, .P_b = 137, .P_t = 96, .f = New Double() {0.077, 0.013, 0.064, 0.167}}
        ''' <summary>
        ''' 酪氨酸（Y）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _Y As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Tyrosine, .P_a = 69, .P_b = 147, .P_t = 114, .f = New Double() {0.082, 0.065, 0.114, 0.125}}
        ''' <summary>
        ''' 缬氨酸（V）
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared ReadOnly _V As ChouFasmanParameter = New ChouFasmanParameter With {.AminoAcid = SequenceModel.Polypeptides.AminoAcid.Valine, .P_a = 106, .P_b = 170, .P_t = 50, .f = New Double() {0.062, 0.048, 0.028, 0.053}}

        Public Shared ReadOnly Property ChouFasmanTable As ChouFasmanParameter() =
            New ChouFasmanParameter() {_A, _R, _N, _D, _C, _E, _Q, _G, _H, _I, _L, _K, _M, _F, _P, _S, _T, _W, _Y, _V}

#End Region

        Public Shared Function Get_Pa(Token As ChouFasmanParameter) As Integer
            Return Token.P_a
        End Function

        Public Shared Function Get_Pb(Token As ChouFasmanParameter) As Integer
            Return Token.P_b
        End Function

        Public Shared Function Get_Pt(Token As ChouFasmanParameter) As Integer
            Return Token.P_t
        End Function
    End Structure
End Namespace
