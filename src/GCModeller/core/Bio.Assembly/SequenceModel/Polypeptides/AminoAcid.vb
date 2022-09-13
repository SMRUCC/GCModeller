#Region "Microsoft.VisualBasic::97568352e7c5052a719001ba5d3e01f1, GCModeller\core\Bio.Assembly\SequenceModel\Polypeptides\AminoAcid.vb"

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

    '   Total Lines: 146
    '    Code Lines: 27
    ' Comment Lines: 95
    '   Blank Lines: 24
    '     File Size: 3.26 KB


    '     Enum AminoAcid
    ' 
    '         Alanine, Arginine, Asparagine, AsparticAcid, Cysteine
    '         GlutamicAcid, Glutamine, Glycine, Histidine, Isoleucine
    '         Leucine, Lysine, Methionine, Phenylalanine, Praline
    '         Serine, Threonine, Tryptophane, Tyrosine, Valine
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace SequenceModel.Polypeptides

    ''' <summary>
    ''' Enumerates all of the 20 amino acids.(枚举20种常见的氨基酸)
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum AminoAcid As Byte

        ''' <summary>
        ''' 用于表示空载的tRNA
        ''' </summary>
        ''' <remarks></remarks>
        NULL = 0

        ''' <summary>
        ''' Ala(A) 丙氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Alanine

        ''' <summary>
        ''' Arg(R) 精氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Arginine

        ''' <summary>
        ''' Asn(N) 天冬酰胺
        ''' </summary>
        ''' <remarks></remarks>
        Asparagine

        ''' <summary>
        ''' Asp(D) 天冬氨酸
        ''' </summary>
        ''' <remarks></remarks>
        AsparticAcid

        ''' <summary>
        ''' Cys(C) 半胱氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Cysteine

        ''' <summary>
        ''' Gln(Q) 谷氨酰胺
        ''' </summary>
        ''' <remarks></remarks>
        Glutamine

        ''' <summary>
        ''' Glu(E) 谷氨酸
        ''' </summary>
        ''' <remarks></remarks>
        GlutamicAcid

        ''' <summary>
        ''' Gly(G) 甘氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Glycine

        ''' <summary>
        ''' His(H) 组氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Histidine

        ''' <summary>
        ''' Ile(I) 异亮氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Isoleucine

        ''' <summary>
        ''' Leu(L) 亮氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Leucine

        ''' <summary>
        ''' Lys(K) 赖氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Lysine

        ''' <summary>
        ''' Met(M) 甲硫氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Methionine

        ''' <summary>
        ''' Ser(S) 丝氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Serine

        ''' <summary>
        ''' Phe(F) 苯丙氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Phenylalanine

        ''' <summary>
        ''' Pro(P) 脯氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Praline

        ''' <summary>
        ''' Thr(T) 苏氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Threonine

        ''' <summary>
        ''' TrP(W) 色氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Tryptophane

        ''' <summary>
        ''' Tyr(Y) 酪氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Tyrosine

        ''' <summary>
        ''' Val(V) 结氨酸
        ''' </summary>
        ''' <remarks></remarks>
        Valine

        ''' <summary>
        ''' 终止密码子
        ''' </summary>
        ''' <remarks></remarks>
        StopCode = 100
        ''' <summary>
        ''' 起始密码子
        ''' </summary>
        InitCode = 200
    End Enum

End Namespace
