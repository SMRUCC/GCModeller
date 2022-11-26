#Region "Microsoft.VisualBasic::8d1e60d8d2f642f7fed36c553f312ba7, GCModeller\analysis\SequenceToolkit\SequenceLogo\ClustalVisual.vb"

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

    '   Total Lines: 140
    '    Code Lines: 93
    ' Comment Lines: 25
    '   Blank Lines: 22
    '     File Size: 5.67 KB


    ' Module ClustalVisual
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: (+2 Overloads) InvokeDrawing
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.Patterns

''' <summary>
''' Visualization for the result of Clustal multiple sequence alignment.
''' (多序列比对的结果可视化)
''' </summary>
''' <remarks></remarks>
''' 
<Package("MultipleAlignment.Visualization",
                    Category:=APICategories.UtilityTools,
                    Description:="Data visualization for the Clustal multiple alignment output fasta file.",
                    Publisher:="amethyst.asuka@gcmodeller.org",
                    Url:="http://gcmodeller.org")>
Public Module ClustalVisual

    <DataFrameColumn("MLA.Margin")> Dim Margin As Integer = 20
    ''' <summary>
    ''' 一个点默认占据10个像素
    ''' </summary>
    ''' <remarks></remarks>
    <DataFrameColumn("MLA.DotSize")> Dim DotSize As Integer = 10
    <DataFrameColumn("MLA.FontSize")> Dim FontSize As Integer = 12

    ''' <summary>
    ''' Colors profile for the residues.
    ''' </summary>
    ReadOnly __colours As Dictionary(Of String, Color)

    Sub New()
        ClustalVisual.__colours = Polypeptides.MEGASchema.ToDictionary(Function(x) x.Key.ToString, Function(x) x.Value)

        Call ClustalVisual.__colours.Add("-", Color.FromArgb(0, 0, 0, 0))
        Call ClustalVisual.__colours.Add(".", Color.FromArgb(0, 0, 0, 0))
        Call ClustalVisual.__colours.Add("*", Color.FromArgb(0, 0, 0, 0))
        Call ClustalVisual.__colours.Add("A", Color.Green)
        Call ClustalVisual.__colours.Add("T", Color.Blue)
        Call ClustalVisual.__colours.Add("G", Color.Red)
        Call ClustalVisual.__colours.Add("C", Color.Brown)
        Call ClustalVisual.__colours.Add("U", Color.CadetBlue)
    End Sub

    ''' <summary>
    ''' Drawing.Clustal
    ''' </summary>
    ''' <param name="aln">The file path of the clustal alignment result fasta file.</param>
    ''' <returns></returns>
    <ExportAPI("Drawing.Clustal")>
    Public Function InvokeDrawing(<Parameter("aln.File",
                                             "The file path of the clustal alignment result fasta file.")>
                                  aln As String) As Image
        Dim fa As FASTA.FastaFile = FASTA.FastaFile.Read(aln)
        Dim res As Image = fa.InvokeDrawing
        Return res
    End Function

    ''' <summary>
    ''' 蛋白质序列和核酸序列都可以使用本函数
    ''' </summary>
    ''' <param name="aln"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Drawing.Clustal")>
    <Extension> Public Function InvokeDrawing(aln As FASTA.FastaFile) As Image
        Dim titleMaxLen = (From fa As FASTA.FastaSeq
                           In aln
                           Select l = Len(fa.Title),
                               fa.Title
                           Order By l Descending).First

        Dim titleFont As New Font(FontFace.Ubuntu, FontSize)
        Dim StringSize As SizeF = titleMaxLen.Title.MeasureSize(New Size(1, 1).CreateGDIDevice, titleFont)
        Dim DotSize As Integer = ClustalVisual.DotSize

        DotSize = Math.Max(DotSize, StringSize.Height) + 5

        Dim grSize As New Size(
            aln.Max(Function(fa) fa.Length) * DotSize + StringSize.Width + 2 * Margin,
            (aln.Count + 1) * DotSize + 2.5 * Margin)

        Dim gdi As Graphics2D = grSize.CreateGDIDevice
        Dim X As Integer = 0.5 * Margin + StringSize.Width + 10
        Dim Y As Integer = Margin
        Dim DotFont As New Font(FontFace.Ubuntu, FontSize + 1, FontStyle.Bold)
        Dim ConservedSites As Integer() =
            LinqAPI.Exec(Of Integer) <= From site As SeqValue(Of SimpleSite)
                                        In Patterns.Frequency(aln).Residues.SeqIterator
                                        Where site.value.IsConserved
                                        Select site.i
        Dim idx As Integer = 0

        Call gdi.ImageAddFrame(offset:=1)

        For Each ch As Char In aln.First.SequenceData
            If Array.IndexOf(ConservedSites, idx) = -1 Then
                X += DotSize
                idx += 1
                Continue For
            End If

            Call gdi.Graphics.DrawString("*", DotFont, Brushes.Black, New Point(X, Y))

            idx += 1
            X += DotSize
        Next

        X = 0.5 * Margin + StringSize.Width + 10
        Y += DotSize

        For Each fa As FASTA.FastaSeq In aln
            Call gdi.Graphics.DrawString(fa.Title, titleFont, Brushes.Black, New Point(Margin * 0.75, Y))

            For Each ch As Char In fa.SequenceData
                Dim s As String = ch.ToString
                Dim br As New SolidBrush(ClustalVisual.__colours(s))
                Dim rect As New Rectangle(New Point(X, Y), New Size(DotSize, DotSize))

                Call gdi.Graphics.FillRectangle(br, rect)
                Call gdi.Graphics.DrawString(s, DotFont, Brushes.Black, New Point(X, Y))

                X += DotSize
            Next

            X = 0.5 * Margin + StringSize.Width + 10
            Y += DotSize
        Next

        Return gdi.ImageResource
    End Function
End Module
