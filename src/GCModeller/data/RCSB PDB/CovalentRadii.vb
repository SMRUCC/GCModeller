Imports Microsoft.VisualBasic.Imaging.Math2D
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords.HETATM

Public Class CovalentRadii

    ''' <summary>
    ''' https://chem.libretexts.org/Ancillary_Materials/Reference/Reference_Tables/Atomic_and_Molecular_Properties/A3%3A_Covalent_Radii
    ''' </summary>
    ''' <remarks>
    ''' pm
    ''' </remarks>
    Const Covalent_Radii_Table = "
1	H	31	32	-	-
2	He	28	46	-	-
3	Li	128	133	124	-
4	Be	96	102	90	85
5	B	84	85	78	73
6	C	76	75	67	60
7	N	71	71	60	54
8	O	66	63	57	53
9	F	57	64	59	53
10	Ne	58	67	96	-
11	Na	166	155	160	-
12	Mg	141	139	132	127
13	Al	121	126	113	111
14	Si	111	116	107	102
15	P	107	111	102	94
16	S	105	103	94	95
17	Cl	102	99	95	93
18	Ar	106	96	107	96
19	K	203	196	193	-
20	Ca	176	171	147	133
21	Sc	170	148	116	114
22	Ti	160	136	117	108
23	v	153	134	112	106
24	Cr	139	122	111	103
25	Mn	150	119	105	103
26	Fe	142	116	109	102
27	Co	138	111	103	96
28	Ni	124	110	101	101
29	Cu	132	112	115	120
30	Zn	122	118	120	-
31	Ga	122	124	117	121
32	Ge	120	121	111	114
33	As	119	121	114	106
34	Se	120	116	107	107
35	Br	120	114	109	110
36	Kr	116	117	121	108
37	Rb	220	210	202	-
38	Sr	195	185	157	139
39	Y	190	163	130	124
40	Zr	175	154	127	121
41	Nb	164	147	125	116
42	Mo	154	138	121	113
43	Tc	147	128	120	110
44	Ru	146	125	114	103
45	Rh	142	125	110	106
46	Pd	139	120	117	112
47	Ag	145	128	139	137
48	Cd	144	136	144	-
49	In	142	142	136	146
50	Sn	139	140	130	132
51	Sb	139	140	133	127
52	Te	138	136	128	121
53	I	139	133	129	125
54	Xe	140	131	135	122
55	Cs	244	232	209	-
56	Ba	215	196	161	149
57	La	207	180	139	139
58	Ce	204	163	137	131
59	Pr	203	176	138	128
60	Nd	201	174	137	-
61	Pm	199	173	135	-
62	Sm	198	172	134	-
63	Eu	198	168	134	-
64	Gd	196	169	135	132
65	Tb	194	168	135	-
66	Dy	192	167	133	-
67	Ho	192	166	133	-
68	Er	189	165	133	-
69	Tm	190	164	131	-
70	Yb	187	170	129	-
71	Lu	187	162	131	131
72	Hf	175	152	128	122
73	Ta	170	146	126	119
74	W	162	137	120	115
75	Re	151	131	119	110
76	Os	144	129	116	109
77	Ir	141	122	115	107
78	Pt	136	123	112	110
79	Au	136	124	121	123
80	Hg	132	133	142	-
81	Tl	145	144	142	150
82	Pb	146	144	135	137
83	Bi	148	151	141	135
84	Po	140	145	135	129
85	At	150	147	138	138
86	Rn	150	142	145	133
87	Fr	260	223	218	-
88	Ra	221	201	173	159
89	Ac	215	186	153	140
90	Th	206	175	143	136
91	Pa	200	169	138	129
92	U	196	170	134	118
93	Np	190	171	136	116
94	Pu	187	172	135	-
95	Am	180	166	135	-
96	Cm	169	166	136	-
97	Bk	-	168	139	-
98	Cf	-	168	140	-
99	Es	-	165	140	-
100	Fm	-	167	-	-
101	Md	-	173	139	-
102	No	-	176	159	-
103	Lr	-	161	141	-
104	Rf	-	157	140	131
105	Db	-	149	136	126
106	Sg	-	143	128	121
107	Bh	-	141	128	119
108	Hs	-	134	125	118
109	Mt	-	129	125	113
110	Ds	-	128	116	112
111	Rg	-	121	116	118
112	Cn	-	122	137	130
113	Uut	-	136	-	-
114	Uuq	-	143	-	-
115	Uup	-	162	-	-
116	Uuh	-	175	-	-
117	Uus	-	165	-	-
118	Uuo	-	157	-	-
"

    Public Property ID As Integer
    Public Property Atom As String
    Public Property Single_Bond1 As Double
    Public Property Single_Bond2 As Double
    Public Property Double_Bond As Double
    Public Property Triple_Bond As Double

    Shared ReadOnly atoms As New Dictionary(Of String, CovalentRadii)

    Shared Sub New()
        Dim table As String() = Covalent_Radii_Table.Trim.LineTokens

        For Each line As String In table
            Dim t As String() = line.StringSplit("\s+")
            Dim atom As New CovalentRadii With {
                .ID = CInt(t(0)),
                .Atom = t(1),
                .Single_Bond1 = If(t(2) = "-", -1, CInt(t(2))) / 100.0, ' 转换为埃米单位（原始数据单位为pm，1Å = 100 pm）
                .Single_Bond2 = If(t(3) = "-", -1, CInt(t(3))) / 100.0, ' 转换为埃米单位（原始数据单位为pm，1Å = 100 pm）
                .Double_Bond = If(t(4) = "-", -1, CInt(t(4))) / 100.0,  ' 转换为埃米单位（原始数据单位为pm，1Å = 100 pm）
                .Triple_Bond = If(t(5) = "-", -1, CInt(t(5))) / 100.0   ' 转换为埃米单位（原始数据单位为pm，1Å = 100 pm）
            }

            Call atoms.Add(atom.Atom, atom)
        Next
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ligand"></param>
    ''' <param name="toleranceFactor">
    ''' 容忍度系数，用于判断实际距离是否在共价半径理论和的范围内
    ''' </param>
    ''' <returns></returns>
    Public Shared Function MeasureBonds(ligand As HETATMRecord(), Optional toleranceFactor As Double = 0.2) As IEnumerable(Of ConnectBond)
        Return MeasureBonds(ligand, table:=atoms, toleranceFactor:=toleranceFactor)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ligand"></param>
    ''' <param name="table"></param>
    ''' <param name="toleranceFactor">
    ''' 容忍度系数，用于判断实际距离是否在共价半径理论和的范围内
    ''' </param>
    ''' <returns></returns>
    Private Shared Iterator Function MeasureBonds(ligand As HETATMRecord(), table As Dictionary(Of String, CovalentRadii), toleranceFactor As Double) As IEnumerable(Of ConnectBond)
        ' 遍历所有原子对，避免重复比较（j > i）
        For i As Integer = 0 To ligand.Length - 2
            For j As Integer = i + 1 To ligand.Length - 1
                Dim atom1 As HETATMRecord = ligand(i)
                Dim atom2 As HETATMRecord = ligand(j)

                ' 获取原子1的元素符号对应的共价半径数据
                Dim radii1 As CovalentRadii = Nothing
                If Not table.TryGetValue(atom1.ElementSymbol, radii1) Then
                    Continue For ' 如果未找到元素数据，跳过此对原子
                End If

                ' 获取原子2的元素符号对应的共价半径数据
                Dim radii2 As CovalentRadii = Nothing
                If Not table.TryGetValue(atom2.ElementSymbol, radii2) Then
                    Continue For ' 如果未找到元素数据，跳过此对原子
                End If

                ' 计算原子间的欧几里得距离
                Dim distance As Double = atom1.DistanceTo3D(atom2)

                ' 计算不同键类型下的理论共价半径和
                Dim sumSingle As Double = (radii1.Single_Bond1 + radii2.Single_Bond1)
                Dim sumDouble As Double = (radii1.Double_Bond + radii2.Double_Bond)
                Dim sumTriple As Double = (radii1.Triple_Bond + radii2.Triple_Bond)

                ' 容忍度范围：理论键长和 ± 容忍度
                Dim lowerBoundSingle As Double = sumSingle - toleranceFactor
                Dim upperBoundSingle As Double = sumSingle + toleranceFactor
                Dim lowerBoundDouble As Double = sumDouble - toleranceFactor
                Dim upperBoundDouble As Double = sumDouble + toleranceFactor
                Dim lowerBoundTriple As Double = sumTriple - toleranceFactor
                Dim upperBoundTriple As Double = sumTriple + toleranceFactor

                ' 判断距离是否落在任何键类型的容忍度范围内，并确定键型
                Dim bondType As Integer = 0 ' 0表示无键
                If distance >= lowerBoundTriple AndAlso distance <= upperBoundTriple Then
                    bondType = 3 ' 三键
                ElseIf distance >= lowerBoundDouble AndAlso distance <= upperBoundDouble Then
                    bondType = 2 ' 双键
                ElseIf distance >= lowerBoundSingle AndAlso distance <= upperBoundSingle Then
                    bondType = 1 ' 单键
                End If

                ' 如果检测到键，返回原子对和键型
                If bondType <> 0 Then
                    Yield New ConnectBond(atom1, atom2, bondType)
                End If
            Next
        Next
    End Function

End Class

Public Class ConnectBond

    Public Property atom1 As HETATMRecord
    Public Property atom2 As HETATMRecord
    Public Property bondType As Integer

    Sub New(atom1 As HETATMRecord, atom2 As HETATMRecord, bondType As Integer)
        Me.atom1 = atom1
        Me.atom2 = atom2
        Me.bondType = bondType
    End Sub

End Class