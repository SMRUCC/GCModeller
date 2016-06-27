Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging

Namespace SequenceLogo

    ''' <summary>
    ''' Define two prefix color schema for the sequence logo: <see cref="NT"/> and <see cref="AA"/>.
    ''' (包含有两种默认的颜色模式：核酸序列和蛋白质序列)
    ''' </summary>
    ''' <remarks>由于可能会涉及到并行化的原因，
    ''' 多线程操作图片对象很可能会出现<see cref="System.InvalidOperationException"/>: Object is currently in use elsewhere.的错误
    ''' 所以在这里不再使用只读属性的简写形式
    ''' </remarks>
    Public Module ColorSchema

        ''' <summary>
        ''' Color schema for the nucleotide sequence.(核酸Motif的profiles)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NucleotideSchema As Dictionary(Of Char, Image)
            Get
                Return New Dictionary(Of Char, Image) From {
                    {"A"c, ColorSchema.__getTexture(Color.Green, "A")},
                    {"T"c, ColorSchema.__getTexture(Color.Red, "T")},
                    {"G"c, ColorSchema.__getTexture(Color.Yellow, "G")},
                    {"C"c, ColorSchema.__getTexture(Color.Blue, "C")}
                }
            End Get
        End Property

        ''' <summary>
        ''' Enumeration for nucleotide residues
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NT As IReadOnlyCollection(Of Char) = {"A"c, "T"c, "G"c, "C"c}
        ''' <summary>
        ''' Enumeration for amino acid.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AA As IReadOnlyCollection(Of Char) = {"A"c, "R"c, "N"c, "D"c, "C"c, "E"c, "Q"c, "G"c, "H"c, "I"c, "L"c, "K"c, "M"c, "F"c, "P"c, "S"c, "T"c, "W"c, "Y"c, "V"c}

        ''' <summary>
        ''' Creates the image cache for the alphabet.
        ''' </summary>
        ''' <param name="color"></param>
        ''' <param name="alphabet"></param>
        ''' <returns></returns>
        Private Function __getTexture(color As Color, alphabet As String) As Image
            Dim bitmap As New Bitmap(680, 680)
            Dim font As New Font(FontFace.Ubuntu, 650)
            Dim br As New SolidBrush(color:=color)

            Using gdi As Graphics = Graphics.FromImage(bitmap)
                Dim size As SizeF = gdi.MeasureString(alphabet, font:=font)
                Dim w As Integer = (bitmap.Width - size.Width) / 2
                Dim h As Integer = (bitmap.Height - size.Height) * 0.45
                Dim pos As New Point(w, h)

                gdi.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                gdi.CompositingMode = Drawing2D.CompositingMode.SourceOver

                Call gdi.DrawString(alphabet, font, br, point:=pos)
            End Using

            Return bitmap
        End Function

        ''' <summary>
        ''' Color schema for the protein residues alphabets.(蛋白质Motif的profiles)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ProteinSchema As Dictionary(Of Char, Image)
            Get
                Return New Dictionary(Of Char, Image) From {
                    {"A"c, ColorSchema.__getTexture(Color.CadetBlue, "A")},      'Alanine	     Ala	A	nonpolar	    neutral	        1.8			89
                    {"R"c, ColorSchema.__getTexture(Color.Black, "R")},          'Arginine	     Arg	R	Basic polar	    positive	   −4.5			174
                    {"N"c, ColorSchema.__getTexture(Color.Chocolate, "N")},      'Asparagine 	 Asn	N	polar	        neutral        −3.5			132
                    {"D"c, ColorSchema.__getTexture(Color.Coral, "D")},          'Aspartic acid	 Asp	D	acidic polar	negative	   −3.5			133
                    {"C"c, ColorSchema.__getTexture(Color.Chartreuse, "C")},     'Cysteine	     Cys	C	nonpolar	    neutral	        2.5	250	0.3	121
                    {"E"c, ColorSchema.__getTexture(Color.Cyan, "E")},           'Glutamic acid	 Glu	E	acidic polar	negative	   −3.5			147
                    {"Q"c, ColorSchema.__getTexture(Color.LawnGreen, "Q")},      'Glutamine	     Gln	Q	polar	        neutral	       −3.5			146
                    {"G"c, ColorSchema.__getTexture(Color.DarkMagenta, "G")},    'Glycine	     Gly	G	nonpolar	    neutral	       −0.4			75
                    {"H"c, ColorSchema.__getTexture(Color.Gold, "H")},           'Histidine	     His	H	Basic polar	    neutral(90%)   −3.2	211	
                    {"I"c, ColorSchema.__getTexture(Color.HotPink, "I")},        'Isoleucine     Ile	I	nonpolar	    neutral	        4.5			131
                    {"L"c, ColorSchema.__getTexture(Color.LightSlateGray, "L")}, 'Leucine	     Leu	L	nonpolar	    neutral	        3.8			131
                    {"K"c, ColorSchema.__getTexture(Color.Yellow, "K")},         'Lysine	     Lys	K	Basic polar	    positive       −3.9			146
                    {"M"c, ColorSchema.__getTexture(Color.Teal, "M")},           'Methionine     Met	M	nonpolar	    neutral	        1.9			149
                    {"F"c, ColorSchema.__getTexture(Color.SaddleBrown, "F")},    'Phenylalanine	 Phe	F	nonpolar	    neutral	        2.8	257, 206, 188	0.2, 9.3, 60.0	165
                    {"P"c, ColorSchema.__getTexture(Color.Red, "P")},            'Proline	     Pro	P	nonpolar	    neutral	       −1.6			115
                    {"S"c, ColorSchema.__getTexture(Color.RoyalBlue, "S")},      'Serine	     Ser	S	polar	        neutral	       −0.8			105
                    {"T"c, ColorSchema.__getTexture(Color.Tomato, "T")},         'Threonine	     Thr	T	polar	        neutral	       −0.7			119
                    {"W"c, ColorSchema.__getTexture(Color.MediumSeaGreen, "W")}, 'Tryptophan     Trp	W	nonpolar	    neutral	       −0.9	280, 219	5.6, 47.0	204
                    {"Y"c, ColorSchema.__getTexture(Color.SkyBlue, "Y")},        'Tyrosine	     Tyr	Y	polar	        neutral	       −1.3	274, 222, 193	1.4, 8.0, 48.0	181
                    {"V"c, ColorSchema.__getTexture(Color.Maroon, "V")}          'Valine	     Val	V	nonpolar	    neutral	        4.2			117
                }
            End Get
        End Property
    End Module
End Namespace