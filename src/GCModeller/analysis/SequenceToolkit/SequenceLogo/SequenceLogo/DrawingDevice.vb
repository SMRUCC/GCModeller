#Region "Microsoft.VisualBasic::e290c44d71f8b8abd6f5a29510aeb771, analysis\SequenceToolkit\SequencePatterns\SequenceLogo\DrawingDevice.vb"

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

    '     Module DrawingDevice
    ' 
    '         Properties: WordSize
    ' 
    '         Function: __getColors, DrawFrequency, E, InvokeDrawing
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace SequenceLogo

    ''' <summary>
    ''' In bioinformatics, a sequence logo is a graphical representation of the sequence conservation 
    ''' of nucleotides (in a strand Of DNA/RNA) Or amino acids (In protein sequences).
    ''' A sequence logo Is created from a collection of aligned sequences And depicts the consensus 
    ''' sequence And diversity of the sequences. Sequence logos are frequently used to depict sequence 
    ''' characteristics such as protein-binding sites in DNA Or functional units in proteins.
    ''' </summary>
    <Package("SequenceLogo",
                        Description:="In bioinformatics, a sequence logo is a graphical representation of the sequence conservation " &
                 "of nucleotides (in a strand Of DNA/RNA) Or amino acids (In protein sequences). " &
                 "A sequence logo Is created from a collection of aligned sequences And depicts the consensus " &
                 "sequence And diversity of the sequences. Sequence logos are frequently used to depict sequence " &
                 "characteristics such as protein-binding sites in DNA Or functional units in proteins.",
                        Cites:="<li>(1990). ""Sequence logos: a New way to display consensus sequences.""</li>///
<li>Crooks;, G. E., et al. (2004). ""WebLogo: A Sequence Logo Generator."" Genome Res 12(1): 47-56.
<p>A systematic computational analysis of protein sequences containing known nuclear domains led to the identification of 28 novel domain families. 
                        This represents a 26% increase in the starting set of 107 known nuclear domain families used for the analysis. Most of the novel domains are present in all major eukaryotic lineages, but 3 are species specific. For about 500 of the 1200 proteins that contain these new domains, nuclear localization could be inferred, and for 700, additional features could be predicted. 
                        For example, we identified a new domain, likely to have a role downstream of the unfolded protein response; a nematode-specific signalling domain; and a widespread domain, likely to be a noncatalytic homolog of ubiquitin-conjugating enzymes.</li>")>
    <Cite(Title:="Sequence logos: a New way to display consensus sequences.",
          Abstract:="A graphical method is presented for displaying the patterns in a set of aligned sequences. 
The characters representing the sequence are stacked on top of each other for each position in the aligned sequences. 
The height of each letter is made proportional to Its frequency, and the letters are sorted so the most common one is on top. 
The height of the entire stack is then adjusted to signify the information content of the sequences at that position. 
From these 'sequence logos', one can determine not only the consensus sequence but also the relative frequency of bases and the information content (measured In bits) at every position in a site or sequence. 
The logo displays both significant residues and subtle sequence patterns.",
          AuthorAddress:="National Cancer Institute, Frederick Cancer Research and Development Center, Laboratory of Mathematical Biology, PO Box B, Frederick, MD 21701, USA",
          Authors:="Thomas D.Schneider* and R.Michael Stephens",
          DOI:="",
          ISSN:="",
          Issue:="20",
          Journal:="Nucleic Acids Research",
          Keywords:="",
          Pages:="6097-6100",
          Year:=1990,
          Volume:=18)>
    <Cite(Title:="WebLogo: A Sequence Logo Generator.", Volume:=12, Issue:="1", Year:=2004, Pages:="47-56", Authors:="Crooks;, G. E., et al.",
          Abstract:="A systematic computational analysis of protein sequences containing known nuclear domains led to the identification of 28 novel domain families. 
This represents a 26% increase in the starting set of 107 known nuclear domain families used for the analysis. Most of the novel domains are present in all major eukaryotic lineages, but 3 are species specific. 
For about 500 of the 1200 proteins that contain these new domains, nuclear localization could be inferred, and for 700, additional features could be predicted. 
For example, we identified a new domain, likely to have a role downstream of the unfolded protein response; a nematode-specific signalling domain; and a widespread domain, likely to be a noncatalytic homolog of ubiquitin-conjugating enzymes.",
          Journal:="Genome Res")>
    Public Module DrawingDevice

        ''' <summary>
        ''' The width of the character in the sequence logo.(字符的宽度)
        ''' </summary>
        Public Property WordSize As Integer = 80
        Dim Height As Integer = 75

        ''' <summary>
        ''' Drawing the sequence logo just simply modelling this motif site from the clustal multiple sequence alignment.
        ''' (绘制各个残基的出现频率)
        ''' </summary>
        ''' <param name="Fasta">The alignment export data from the clustal software.</param>
        ''' <param name="title">The sequence logo display title.</param>
        ''' <returns></returns>
        <ExportAPI("Drawing.Frequency")>
        <Extension>
        Public Function DrawFrequency(fasta As FastaFile, Optional title$ = "", Optional ByRef getModel As MotifPWM = Nothing) As Image
            Dim PWM As MotifPWM = Motif.PWM.FromMla(fasta)
            Dim model As New DrawingModel

#If DEBUG Then
            Dim m As String = New String(PWM.PWM.Select(Function(r) r.AsChar))
            Call VBDebugger.WriteLine(m, ConsoleColor.Magenta)
#End If

            If String.IsNullOrEmpty(title) Then
                If Not String.IsNullOrEmpty(fasta.FilePath) Then
                    model.ModelsId = fasta.FilePath.BaseName
                Else
                    model.ModelsId = New String(PWM.PWM.Select(Function(r) r.AsChar).ToArray)
                End If
            Else
                model.ModelsId = title
            End If

            getModel = PWM
            model.Residues =
                LinqAPI.Exec(Of ResidueSite, Residue)(PWM.PWM) <=
                    Function(rsd As ResidueSite) New Residue With {
                        .Bits = rsd.Bits,
                        .Position = rsd.Site,
                        .Alphabets = LinqAPI.Exec(Of Alphabet) <= From x As SeqValue(Of Double)
                                                                  In rsd.PWM.SeqIterator
                                                                  Select New Alphabet With {
                                                                      .Alphabet = PWM.Alphabets(x.i),
                                                                      .RelativeFrequency = x.value
                                                                  }  ' alphabets
            }  ' residues
            Return InvokeDrawing(model, True)
        End Function

        ''' <summary>
        ''' The approximation for the small-sample correction, en, Is given by
        '''     en = 1/ln2 x (s-1)/2n
        ''' 
        ''' </summary>
        ''' <param name="s">s Is 4 For nucleotides, 20 For amino acids</param>
        ''' <param name="n">n Is the number Of sequences In the alignment</param>
        ''' <returns></returns>
        Public Function E(s As Integer, n As Integer) As Double
            Dim result As Double = 1 / Math.Log(2)
            result *= (s - 1) / 2 * n
            Return result
        End Function

        <Extension>
        Private Function __getColors(model As DrawingModel) As Dictionary(Of Char, Image)
            Return If(model.Alphabets = 4,
                SequenceLogo.ColorSchema.NucleotideSchema,
                SequenceLogo.ColorSchema.ProteinSchema)
        End Function

        ''' <summary>
        ''' Drawing the sequence logo for the sequence motif model.(绘制SequenceLogo图)
        ''' </summary>
        ''' <param name="model">The model can be achieve from clustal alignment or meme software.</param>
        ''' <param name="frequencyOrder">Reorder the alphabets in each residue site in the order of frequency values. default is yes!</param>
        ''' <param name="reverse">Reverse the residue sequence order in the drawing model?</param>
        ''' <returns></returns>
        <ExportAPI("Invoke.Drawing", Info:="Drawing a sequence logo from a generated sequence motif model.")>
        <Extension>
        Public Function InvokeDrawing(model As DrawingModel,
                                      Optional frequencyOrder As Boolean = True,
                                      Optional margin As Integer = 100,
                                      Optional reverse As Boolean = False) As Image

            Dim n As Integer = model.Alphabets
            Dim gSize As New Size(model.Residues.Length * WordSize + 2 * margin, 2 * margin + n * Height)
            Dim gdi As Graphics2D = gSize.CreateGDIDevice(Color.Transparent)
            Dim X, Y As Integer
            Dim font As New Font(FontFace.MicrosoftYaHei, CInt(WordSize * 0.6), FontStyle.Bold)
            Dim size As SizeF

            size =
            gdi.Graphics.MeasureString(model.ModelsId, font)
            gdi.Graphics.DrawString(
                model.ModelsId, font,
                Brushes.Black,
                New Point((gdi.Width - size.Width) / 2, y:=margin / 2.5))

            font = New Font(FontFace.MicrosoftYaHei, CInt(WordSize * 0.4))

#Region "画坐标轴"

            X = margin
            Y = gdi.Height - margin  '坐标轴原点

            Dim maxBits As Double = Math.Log(n, newBase:=2)
            Dim yHeight As Integer = n * DrawingDevice.Height

            Call gdi.Graphics.DrawLine(Pens.Black,
                                        New Point(X, Y - yHeight),
                                        New Point(X, Y))
            Call gdi.Graphics.DrawLine(Pens.Black,
                                        New Point(X, Y),
                                        New Point(X + model.Residues.Length * DrawingDevice.WordSize, y:=Y))

            Dim departs As Integer = If(maxBits = 2, 2, 5)  ' nt 2 steps,  aa 5 steps
            Dim d As Double = maxBits / departs

            yHeight = d / maxBits * (DrawingDevice.Height * n) '步进
            d = Math.Round(d, 1)

            Dim yBits As Double = 0

            For j As Integer = 0 To departs
                size = gdi.Graphics.MeasureString(yBits, font:=font)

                Dim y1 = Y - size.Height / 2

                gdi.Graphics.DrawString(
                    CStr(yBits),
                    font,
                    Brushes.Black,
                    New Point(x:=X - size.Width, y:=y1))

                y1 = Y '- sz.Height / 8
                gdi.Graphics.DrawLine(
                    Pens.Black,
                    New Point(x:=X, y:=y1),
                    New Point(x:=X + 10, y:=y1))

                yBits += d
                Y -= yHeight
            Next

            Dim source As IEnumerable(Of Residue) = If(reverse, model.Residues.Reverse, model.Residues)
            Dim colorSchema As Dictionary(Of Char, Image) = model.__getColors

            Call VBDebugger.WriteLine(New String("-"c, model.Residues.Length), ConsoleColor.Green)

            For Each residue As Residue In source

                Dim order As Alphabet() =
                    If(Not frequencyOrder, residue.Alphabets,
                      (From rsd As Alphabet
                       In residue.Alphabets
                       Select rsd
                       Order By rsd.RelativeFrequency Ascending).ToArray)

                Y = gdi.Height - margin

                ' YHeight is the max height of current residue, and its value is calculate from its Bits value
                yHeight = (n * DrawingDevice.Height) * (If(residue.Bits > maxBits, maxBits, residue.Bits) / maxBits)

                Dim idx As String = CStr(residue.Position)
                Dim loci As New Point(X + size.Width / If(Math.Abs(residue.Position) < 10, 2, 5), Y)

                size =
                gdi.Graphics.MeasureString(idx, font)
                gdi.Graphics.DrawString(idx, font, Brushes.Black, loci)

                For Each Alphabet As Alphabet In order

                    ' H is the drawing height of the current drawing alphabet, 
                    ' this height value can be calculate from the formula that show above. 
                    ' As the YHeight variable is transform from the current residue Bits value, so that from this statement
                    ' The drawing height of the alphabet can be calculated out. 

                    Dim H As Single = Alphabet.RelativeFrequency * yHeight

                    ' Due to the reason of the Y Axis in gdi+ is up side down, so that we needs Subtraction operation, 
                    ' and then this makes the next alphabet move up direction 
                    Y -= H

                    gdi.Graphics.DrawImage(
                        colorSchema(Alphabet.Alphabet),   ' Drawing alphabet
                        CSng(X), CSng(Y),                 ' position
                        CSng(DrawingDevice.WordSize), H)  ' Size and relative height
                Next

                X += DrawingDevice.WordSize
                Call residue.AsChar.Echo
            Next

            Call Console.WriteLine()

            '绘制bits字符串
            font = New Font(font.Name, font.Size / 2)
            size = gdi.Graphics.MeasureString("Bits", font)

            Call gdi.Graphics.RotateTransform(-90)
            Call gdi.Graphics.DrawString("Bits",
                                          font,
                                          Brushes.Black,
                                          New Point((Height - size.Width) / 2, margin / 3))

#End Region
            Return gdi.ImageResource
        End Function
    End Module
End Namespace
