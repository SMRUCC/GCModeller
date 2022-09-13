#Region "Microsoft.VisualBasic::00d833ff34f7f1cc2acd8e2a8d648af5, GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Objects\DegenerateBases.vb"

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

    '   Total Lines: 104
    '    Code Lines: 61
    ' Comment Lines: 30
    '   Blank Lines: 13
    '     File Size: 4.36 KB


    '     Class DegenerateBasesExtensions
    ' 
    '         Properties: BaseDegenerateEntries, DegenerateBases
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CountWithDegenerateBases, Equals
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' ###### 简并碱基
    ''' 
    ''' 简并碱基是根据密码子的兼并性,常用一个符号代替某两个或者更多碱基。
    ''' 根据密码子的兼并性,常用一个符号代替某两个或者更多碱基。例如，编译丙氨酸的
    ''' 可以有4个密码子``GCU\GCC\GCA\GCG``,这时生物学上为了方便，用字母N指代UCAG
    ''' 四个碱基，故说编译丙氨酸的密码子是GCN，其中N就是简并碱基。
    ''' 
    ''' 通常生物学上根据蛋白质序列设计引物进行对应DNA克隆时，由于密码子的兼并性，
    ''' 对设计的引物上的某些碱基就会用简并碱基标示，在合成时各种碱基等量分配，
    ''' 也就是说，合成的简并引物是很多种引物的集合，其区别就在于简并碱基。
    ''' 
    ''' 简并度：简并引物的种类数，等于该简并引物内所有简并碱基的简并个数之积，
    ''' 即共有多少种不同的引物，其中只有一条是真正可以和模板配对的。
    ''' </summary>
    Public NotInheritable Class DegenerateBasesExtensions

        Public ReadOnly Property DegenerateBases As New Dictionary(Of Char, Char()) From {
            {"R"c, {"A"c, "G"c}},
            {"Y"c, {"C"c, "T"c}},
            {"M"c, {"A"c, "C"c}},
            {"K"c, {"G"c, "T"c}},
            {"S"c, {"G"c, "C"c}},
            {"W"c, {"A"c, "T"c}},
            {"H"c, {"A"c, "T"c, "C"c}},
            {"B"c, {"G"c, "T"c, "C"c}},
            {"V"c, {"G"c, "A"c, "C"c}},
            {"D"c, {"G"c, "A"c, "T"c}},
            {"N"c, {"A"c, "T"c, "C"c, "G"c}}
        }

        Public ReadOnly Property BaseDegenerateEntries As Dictionary(Of Char, Char())

        Sub New()
            Dim gDNAs = From dgBase
                        In DegenerateBases
                        Select From b As Char
                               In dgBase.Value
                               Select base = b, dg = dgBase.Key
            Dim group = gDNAs _
                .IteratesALL _
                .GroupBy(Function(base) base.base)

            BaseDegenerateEntries = group _
                .ToDictionary(Function(base) base.Key,
                              Function(dgBases)
                                  Return dgBases _
                                      .Select(Function(d) d.dg) _
                                      .Distinct _
                                      .ToArray
                              End Function)
        End Sub

        ''' <summary>
        ''' 会将简并碱基也进行计算
        ''' </summary>
        ''' <param name="nt$"></param>
        ''' <param name="base">just **ATGC**</param>
        ''' <returns></returns>
        ''' 
        Public Function CountWithDegenerateBases(nt$, base As Char) As Double
            Dim n# = nt.Count(base)
            Dim dbEntries = BaseDegenerateEntries(base)

            For Each dgBase As Char In dbEntries
                Dim cd% = nt.Count(dgBase)
                Dim l = 1 / DegenerateBases(dgBase).Length
                n += cd * l  ' 因为计算简并碱基的时候，是平均分配的，所以在这里就除以该简并碱基的可替换的碱基数量
            Next

            ' 故而包含有简并碱基的计算结果应该是带有小数的
            Return n
        End Function

        ''' <summary>
        ''' Elements sequence equals.
        ''' </summary>
        ''' <param name="vx"></param>
        ''' <param name="vy"></param>
        ''' <returns></returns>
        Public Overloads Shared Function Equals(vx As DNA(), vy As DNA()) As Boolean
            If vx.Length <> vy.Length Then
                Return False
            End If

            For i As Integer = 0 To vx.Length - 1
                Dim a = vx(i), b = vy(i)

                If a <> b Then
                    ' 可能是简并碱基，还需要额外的判断才能够下定论
                    If Not Conversion.Equals(a, b) Then
                        Return False
                    End If
                End If
            Next

            Return True
        End Function
    End Class
End Namespace
