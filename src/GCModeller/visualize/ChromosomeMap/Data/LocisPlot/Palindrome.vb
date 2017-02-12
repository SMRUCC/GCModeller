Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Locis

    Public Module Palindrome

        Const d# = 3

        ''' <summary>
        ''' 这个函数可以同时绘制镜像回文以及互补回文位点，只需要设置一下<paramref name="complementPalindrom"/>参数即可
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="palindrome"></param>
        ''' <param name="location"></param>
        ''' <param name="fontStyle$"></param>
        ''' <param name="complementPalindrom">
        ''' 这个函数是否为绘制互补回文片段？默认为False，即绘制镜像回文片段
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function DrawMirrorPalindrome(g As Graphics,
                                             palindrome As PalindromeLoci,
                                             location As PointF,
                                             Optional fontStyle$ = CSSFont.Win10Normal,
                                             Optional complementPalindrom As Boolean = False) As SizeF

            Dim font As Font = CSSFont.TryParse(fontStyle)
            Dim size As SizeF = g.MeasureString("A", font)  ' 计算出单个碱基的绘制大小，因为在这个函数之中是一个碱基一个碱基进行绘图的
            Dim A = palindrome.Loci, B = palindrome.MirrorSite
            Dim left! = location.X
            Dim top! = location.Y
            Dim pt As PointF

            Static gray As SolidBrush = Brushes.Gray

            Dim drawSite = Sub(DNA$)
                               For Each c As Char In DNA
                                   Dim color As SolidBrush = ColorSchema.DNAcolors(c)

                                   pt = New PointF(left, top)
                                   g.DrawString(CStr(c), font, color, pt)
                                   left += size.Width
                               Next
                           End Sub
            Dim drawGray = Sub(DNA$)
                               For Each c As Char In NucleicAcid.Complement(DNA).Reverse
                                   pt = New PointF(left, top)
                                   g.DrawString(CStr(c), font, gray, pt)
                                   left += size.Width
                               Next
                           End Sub

            Call drawSite(DNA:=A)

            left = location.X
            top += size.Height + d

            Call drawGray(DNA:=A)

            location = New PointF(left + d, location.Y)  ' 画另一半序列
            top = location.Y

            If complementPalindrom Then
                Call drawGray(DNA:=B)
            Else
                Call drawSite(DNA:=B)
            End If

            left = location.X
            top += size.Height + d

            If complementPalindrom Then
                Call drawSite(DNA:=B)
            Else
                Call drawGray(DNA:=B)
            End If

            size = New Size(size.Width * (A.Length + B.Length), size.Height * 2 + d#)
            Return size
        End Function
    End Module
End Namespace