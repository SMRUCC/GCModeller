#Region "Microsoft.VisualBasic::17160175759fc3f77c9b548d554a8c1b, analysis\SequenceToolkit\SequenceLogo\SequenceLogo\DrawingDevice.vb"

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

    '   Total Lines: 297
    '    Code Lines: 209 (70.37%)
    ' Comment Lines: 43 (14.48%)
    '    - Xml Docs: 72.09%
    ' 
    '   Blank Lines: 45 (15.15%)
    '     File Size: 16.73 KB


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
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.SequenceModel.FASTA

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

        ''' <summary>
        ''' Drawing the sequence logo just simply modelling this motif site from the clustal multiple sequence alignment.
        ''' (绘制各个残基的出现频率)
        ''' </summary>
        ''' <param name="pwm">The alignment export data from the clustal software.</param>
        ''' <param name="title">The sequence logo display title.</param>
        ''' <returns></returns>
        <ExportAPI("Drawing.Frequency")>
        <Extension>
        Public Function DrawFrequency(pwm As MotifPWM, title$,
                                      Optional height As Integer = 75,
                                      Optional logoPadding$ = "padding: 30% 5% 20% 10%;",
                                      Optional driver As Drivers = Drivers.Default) As GraphicsData

            Dim model As New DrawingModel With {.ModelsId = title}
#If DEBUG Then
            Dim m As String = New String(PWM.pwm.Select(Function(r) r.AsChar))
            Call VBDebugger.WriteLine(m, ConsoleColor.Magenta)
#End If
            model.Residues =
                LinqAPI.Exec(Of ResidueSite, Residue)(pwm.pwm) <=
                    Function(rsd As ResidueSite)
                        Return New Residue With {
                            .Bits = rsd.bits,
                            .Position = rsd.site,
                            .Alphabets = LinqAPI.Exec(Of Alphabet) <= From x As SeqValue(Of Double)
                                                                      In rsd.PWM.SeqIterator
                                                                      Select New Alphabet With {
                                                                          .Alphabet = pwm.alphabets(x.i),
                                                                          .RelativeFrequency = x.value
                                                                      }  ' alphabets
                        }  ' residues
                    End Function

            Return InvokeDrawing(
                model:=model,
                frequencyOrder:=True,
                height:=height,
                driver:=driver,
                logoPadding:=logoPadding
            )
        End Function

        ''' <summary>
        ''' Drawing the sequence logo just simply modelling this motif site from the clustal multiple sequence alignment.
        ''' (绘制各个残基的出现频率)
        ''' </summary>
        ''' <param name="Fasta">The alignment export data from the clustal software.</param>
        ''' <param name="title">The sequence logo display title.</param>
        ''' <returns></returns>
        <ExportAPI("Drawing.Frequency")>
        <Extension>
        Public Function DrawFrequency(fasta As FastaFile,
                                      Optional title$ = "",
                                      Optional ByRef getModel As MotifPWM = Nothing,
                                      Optional height As Integer = 75,
                                      Optional driver As Drivers = Drivers.Default) As GraphicsData

            Dim PWM As MotifPWM = Motif.PWM.FromMla(fasta)

            If String.IsNullOrEmpty(title) Then
                If Not String.IsNullOrEmpty(fasta.FilePath) Then
                    title = fasta.FilePath.BaseName
                Else
                    title = New String(PWM.pwm.Select(Function(r) r.AsChar).ToArray)
                End If
            End If

            getModel = PWM

            Return PWM.DrawFrequency(title, height, driver)
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
        Friend Function CharColorImages(model As DrawingModel, backColor As Color) As Dictionary(Of Char, Image)
            With New ColorSchema(backColor)
                If model.Alphabets = 4 Then
                    Return .NucleotideSchema
                Else
                    Return .ProteinSchema
                End If
            End With
        End Function

        ''' <summary>
        ''' Drawing the sequence logo for the sequence motif model.
        ''' </summary>
        ''' <param name="model">The model can be achieve from clustal alignment or meme software.</param>
        ''' <param name="frequencyOrder">Reorder the alphabets in each residue site in the order of frequency values. default is yes!</param>
        ''' <param name="reverse">Reverse the residue sequence order in the drawing model?</param>
        ''' <param name="height">
        ''' height of the alphabet plot image
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' (绘制SequenceLogo图)
        ''' </remarks>
        <Extension>
        Public Function InvokeDrawing(model As DrawingModel,
                                      Optional frequencyOrder As Boolean = True,
                                      Optional logoPadding$ = "padding: 30% 5% 20% 10%;",
                                      Optional reverse As Boolean = False,
                                      Optional height As Integer = 75,
                                      Optional driver As Drivers = Drivers.Default) As GraphicsData

            Dim width! = (model.Residues.Length + 8) * WordSize
            Dim size1 As New Size(width, (model.Alphabets + 3) * height)
            Dim theme As New Theme With {
                .tagCSS = New CSSFont(FontFace.MicrosoftYaHei, WordSize * 0.8).CSSValue,
                .padding = logoPadding,
                .background = "white",
                .mainCSS = "font-style: strong; font-size: 36; font-family: " & FontFace.MicrosoftYaHei & ";"
            }
            Dim logo As New Logo(model, reverse, frequencyOrder, theme)

            Return logo.Plot($"{size1.Width},{size1.Height}",, driver)
        End Function
    End Module
End Namespace
