#Region "Microsoft.VisualBasic::a29e66d35151b3585f58ee54eccc602f, analysis\SequenceToolkit\SequenceLogo\SequenceLogo\ColorSchema.vb"

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

    '   Total Lines: 121
    '    Code Lines: 89 (73.55%)
    ' Comment Lines: 22 (18.18%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (8.26%)
    '     File Size: 7.29 KB


    '     Module ColorSchema
    ' 
    '         Properties: DNAcolors, NucleotideSchema, ProteinSchema
    ' 
    '         Function: __getTexture
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

Namespace SequenceLogo

    ''' <summary>
    ''' Define two prefix color schema for the sequence logo: <see cref="SequenceModel.NT"/> and <see cref="SequenceModel.AA"/>.
    ''' (包含有两种默认的颜色模式：核酸序列和蛋白质序列)
    ''' </summary>
    ''' <remarks>由于可能会涉及到并行化的原因，
    ''' 多线程操作图片对象很可能会出现<see cref="System.InvalidOperationException"/>: Object is currently in use elsewhere.的错误
    ''' 所以在这里不再使用只读属性的简写形式
    ''' </remarks>
    Public Class ColorSchema

        ''' <summary>
        ''' Color schema for the nucleotide sequence.(核酸Motif的profiles)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NucleotideSchema As Dictionary(Of Char, Image)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New Dictionary(Of Char, Image) From {
                    {"A"c, CreateAlphabetImageTexture(DNAcolors("A"c).Color, "A")},
                    {"T"c, CreateAlphabetImageTexture(DNAcolors("T"c).Color, "T")},
                    {"G"c, CreateAlphabetImageTexture(DNAcolors("G"c).Color, "G")},
                    {"C"c, CreateAlphabetImageTexture(DNAcolors("C"c).Color, "C")}
                }
            End Get
        End Property

        Public ReadOnly Property DNAcolors As New Dictionary(Of Char, SolidBrush) From {
            {"A"c, Brushes.Green},
            {"T"c, Brushes.Red},
            {"G"c, Brushes.Yellow},
            {"C"c, Brushes.Blue}
        }

        ReadOnly backColor As Color = Color.White

        Sub New(backColor As Color)
            Me.backColor = backColor
        End Sub

        ''' <summary>
        ''' Creates the image cache for the alphabet.
        ''' </summary>
        ''' <param name="color"></param>
        ''' <param name="alphabet"></param>
        ''' <returns></returns>
        Private Function CreateAlphabetImageTexture(color As Color, alphabet$) As Image
            Dim font As New Font(FontFace.Ubuntu, 650)
            Dim br As New SolidBrush(color:=color)

            Using g As IGraphics = DriverLoad.CreateGraphicsDevice(New Size(680, 680), backColor, driver:=Drivers.GDI)
                Dim size As SizeF = g.MeasureString(alphabet, font:=font)
                Dim w As Integer = (g.Width - size.Width) / 2
                Dim h As Integer = (g.Height - size.Height) * 0.45
                Dim pos As New PointF(w, h)

                Call g.DrawString(alphabet, font, br, point:=pos)

                Dim img As Image = DirectCast(g, GdiRasterGraphics).ImageResource
                img = img.CorpBlank(blankColor:=backColor)
                Return img
            End Using
        End Function

        ''' <summary>
        ''' Color schema for the protein residues alphabets.(蛋白质Motif的profiles)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ProteinSchema As Dictionary(Of Char, Image)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New Dictionary(Of Char, Image) From {
                    {"A"c, CreateAlphabetImageTexture(Color.CadetBlue, "A")},      'Alanine	     Ala	A	nonpolar	    neutral	        1.8			89
                    {"R"c, CreateAlphabetImageTexture(Color.Black, "R")},          'Arginine	     Arg	R	Basic polar	    positive	   −4.5			174
                    {"N"c, CreateAlphabetImageTexture(Color.Chocolate, "N")},      'Asparagine 	 Asn	N	polar	        neutral        −3.5			132
                    {"D"c, CreateAlphabetImageTexture(Color.Coral, "D")},          'Aspartic acid	 Asp	D	acidic polar	negative	   −3.5			133
                    {"C"c, CreateAlphabetImageTexture(Color.Chartreuse, "C")},     'Cysteine	     Cys	C	nonpolar	    neutral	        2.5	250	0.3	121
                    {"E"c, CreateAlphabetImageTexture(Color.Cyan, "E")},           'Glutamic acid	 Glu	E	acidic polar	negative	   −3.5			147
                    {"Q"c, CreateAlphabetImageTexture(Color.LawnGreen, "Q")},      'Glutamine	     Gln	Q	polar	        neutral	       −3.5			146
                    {"G"c, CreateAlphabetImageTexture(Color.DarkMagenta, "G")},    'Glycine	     Gly	G	nonpolar	    neutral	       −0.4			75
                    {"H"c, CreateAlphabetImageTexture(Color.Gold, "H")},           'Histidine	     His	H	Basic polar	    neutral(90%)   −3.2	211	
                    {"I"c, CreateAlphabetImageTexture(Color.HotPink, "I")},        'Isoleucine     Ile	I	nonpolar	    neutral	        4.5			131
                    {"L"c, CreateAlphabetImageTexture(Color.LightSlateGray, "L")}, 'Leucine	     Leu	L	nonpolar	    neutral	        3.8			131
                    {"K"c, CreateAlphabetImageTexture(Color.Yellow, "K")},         'Lysine	     Lys	K	Basic polar	    positive       −3.9			146
                    {"M"c, CreateAlphabetImageTexture(Color.Teal, "M")},           'Methionine     Met	M	nonpolar	    neutral	        1.9			149
                    {"F"c, CreateAlphabetImageTexture(Color.SaddleBrown, "F")},    'Phenylalanine	 Phe	F	nonpolar	    neutral	        2.8	257, 206, 188	0.2, 9.3, 60.0	165
                    {"P"c, CreateAlphabetImageTexture(Color.Red, "P")},            'Proline	     Pro	P	nonpolar	    neutral	       −1.6			115
                    {"S"c, CreateAlphabetImageTexture(Color.RoyalBlue, "S")},      'Serine	     Ser	S	polar	        neutral	       −0.8			105
                    {"T"c, CreateAlphabetImageTexture(Color.Tomato, "T")},         'Threonine	     Thr	T	polar	        neutral	       −0.7			119
                    {"W"c, CreateAlphabetImageTexture(Color.MediumSeaGreen, "W")}, 'Tryptophan     Trp	W	nonpolar	    neutral	       −0.9	280, 219	5.6, 47.0	204
                    {"Y"c, CreateAlphabetImageTexture(Color.SkyBlue, "Y")},        'Tyrosine	     Tyr	Y	polar	        neutral	       −1.3	274, 222, 193	1.4, 8.0, 48.0	181
                    {"V"c, CreateAlphabetImageTexture(Color.Maroon, "V")}          'Valine	     Val	V	nonpolar	    neutral	        4.2			117
                }
            End Get
        End Property
    End Class
End Namespace
