Imports System.Runtime.CompilerServices
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
    Public Module DegenerateBasesExtensions

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
        <Extension>
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
    End Module
End Namespace