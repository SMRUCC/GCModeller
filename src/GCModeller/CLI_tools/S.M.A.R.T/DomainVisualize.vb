#Region "Microsoft.VisualBasic::982729a0993b163c6e895e04ac06647b, CLI_tools\S.M.A.R.T\DomainVisualize.vb"

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

    ' Class DomainVisualize
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetColorProfiles, GetDomainDescription, Visualize
    ' 
    '     Sub: CreateMainRule, DrawCite, DrawTitle
    '     Class ColorSchema
    ' 
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing.Drawing2D
Imports System.Reflection
Imports SMRUCC.genomics.Assembly.NCBI.CDD
Imports SMRUCC.genomics.ProteinModel
Imports Microsoft.VisualBasic.Imaging

Public Class DomainVisualize

    Public Class ColorSchema
        Dim DomainName As String
        Dim Color As Brush

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1}", DomainName, Color.ToString)
        End Function
    End Class

    Protected Friend ReadOnly Colors As Brush() = New Brush() {
        Brushes.AliceBlue, Brushes.AntiqueWhite, Brushes.Aqua, Brushes.Aquamarine, Brushes.Azure, Brushes.Beige, Brushes.Bisque,
        Brushes.Black, Brushes.BlanchedAlmond, Brushes.Blue, Brushes.BlueViolet, Brushes.Brown, Brushes.BurlyWood,
        Brushes.CadetBlue, Brushes.Chartreuse, Brushes.Chocolate, Brushes.Coral, Brushes.CornflowerBlue}


    ''' <summary>
    ''' 默認的字體
    ''' </summary>
    ''' <remarks></remarks>
    Const FONT_MICROSOFT_YAHEI = "Microsoft YaHei"
    Const START_LOCATION_OFFSET = 75
    Const DIAGRAM_Y_OFFSET As Integer = 30

    ''' <summary>
    ''' 標尺綫的垂直位置
    ''' </summary>
    ''' <remarks></remarks>
    Const RULE_HEIGHT = DIAGRAM_Y_OFFSET + 30

    Dim _Cdd As CDDLoader
    Dim _PfamCache As DbFile
    Dim _SmartCache As DbFile

    Sub New(Cdd As CDDLoader)
        _Cdd = Cdd
        _PfamCache = Cdd.GetPfam
        _SmartCache = Cdd.GetSmart
    End Sub

    Private Shared Sub CreateMainRule(gr As Graphics, Length As Integer)
        Dim Right As Integer = START_LOCATION_OFFSET + Length
        Dim DrawingFont As Font = New Font(FONT_MICROSOFT_YAHEI, 8)

        Const MainRuleHeight = 4

        Call gr.DrawLine(Pens.Black, New Point(START_LOCATION_OFFSET, RULE_HEIGHT), New Point(Right, RULE_HEIGHT))  '标尺
        Call gr.DrawLine(Pens.Black, New Point(START_LOCATION_OFFSET, RULE_HEIGHT), New Point(START_LOCATION_OFFSET, RULE_HEIGHT - MainRuleHeight))
        Call gr.DrawLine(Pens.Black, New Point(Right, RULE_HEIGHT), New Point(Right, RULE_HEIGHT - MainRuleHeight))
        Call gr.DrawString("0", DrawingFont, Brushes.Black, New Point(START_LOCATION_OFFSET - 4, RULE_HEIGHT + 3))
        Dim size = gr.MeasureString(Length, DrawingFont).Width
        Call gr.DrawString(Length, New Font(FONT_MICROSOFT_YAHEI, 8), Brushes.Black, New Point(Length + START_LOCATION_OFFSET - size / 2, RULE_HEIGHT + 3))
    End Sub

    Private Shared Sub DrawTitle(gr As Graphics, strTitle As String, seqRECT As Rectangle)
        Dim TitleFont As Font = New Font(FONT_MICROSOFT_YAHEI, 12)
        Dim s = gr.MeasureString(strTitle, TitleFont)
        Dim Position As Point = New Point(seqRECT.Left + (seqRECT.Width - s.Width) / 2, seqRECT.Top + 40)  '居中設置

        Call gr.DrawString(strTitle, New Font(FONT_MICROSOFT_YAHEI, 10), Brushes.Black, Position) '标题
    End Sub

    Private Shared Sub DrawCite(gr As Graphics, DeviceSzie As Size)
        Const ButtomOffset = 30
        Const _Offset = 25

        Call gr.DrawLine(New System.Drawing.Pen(Brushes.Black, 1), New Point(_Offset, DeviceSzie.Height - ButtomOffset), New Point(DeviceSzie.Width - _Offset, DeviceSzie.Height - ButtomOffset))
        Call gr.DrawString("* xie.guigang@gcmodeller.org", New Font(FONT_MICROSOFT_YAHEI, 10), Brushes.Black, New Point(_Offset + 20, DeviceSzie.Height - ButtomOffset + 5))
    End Sub

    ''' <summary>
    ''' Pfam数据库可能会和CDD之中的Pfam数据库的版本不一致，故而可能会返回空值
    ''' </summary>
    ''' <param name="Id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDomainDescription(Id As String) As SmpFile
        Dim data As SmpFile = _PfamCache.ContainsId_p(Id)

        If Not data Is Nothing Then
            Return data
        End If

        data = _SmartCache.ContainsId_p(Id)
        If Not data Is Nothing Then
            Return data
        End If

        Return _Cdd.GetItem(Id)
    End Function

    Public Function Visualize(Protein As Protein) As Image
        Dim Domains = (From d In Protein.Domains Select d Order By d.Position.Left Ascending).ToArray
        Dim DiagramImage As Image = New Bitmap(Protein.Length + 150, Protein.Domains.Count * 40 + 200)
        Dim ColorProfiles = GetColorProfiles(Domains)

        Dim DomainCache As Dictionary(Of String, SmpFile) =
            New Dictionary(Of String, SmpFile)
        For Each Id As String In ColorProfiles.Keys
            Call DomainCache.Add(Id, Me.GetDomainDescription(Id))
        Next

        Call DiagramImage.FillBlank(Brushes.White)
        Using Gr As Graphics = Graphics.FromImage(DiagramImage)
            Dim Seq As Rectangle = New Rectangle(New Point(START_LOCATION_OFFSET, DIAGRAM_Y_OFFSET), New Size(Protein.Length, 8))  '先画出蛋白质序列

            Gr.CompositingQuality = CompositingQuality.HighQuality
            Gr.CompositingMode = CompositingMode.SourceOver
            Gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit

            Call DrawTitle(Gr, String.Format("Domain Architectures of ""{0}""", Protein.Id), Seq)
            Call CreateMainRule(Gr, Protein.Length)
            Call Gr.FillRectangle(Brushes.Black, Seq)
            Call DrawCite(Gr, DiagramImage.Size)

            Dim Y As Integer = 4 * DIAGRAM_Y_OFFSET, X As Integer = START_LOCATION_OFFSET + 10
            Dim ColorRectSize As Size = New Size(80, 25)
            Dim DomainLableFont As Font = New Font("Microsoft YaHei", 10)
            Dim LabelHeight As Integer = Gr.MeasureString("X", DomainLableFont).Height

            For Each ProfileItem In ColorProfiles
                Dim DomainR = New Rectangle(New Point(X, Y), ColorRectSize)
                Call Gr.FillRectangle(ProfileItem.Value, DomainR)

                Dim DomainDescription = DomainCache(ProfileItem.Key)
                Dim Label As String = If(DomainDescription Is Nothing, ProfileItem.Key, String.Format("{0}:{1}, {2}", DomainDescription.Name, DomainDescription.CommonName, DomainDescription.Describes))

                Call Gr.DrawString(Label, DomainLableFont, Brushes.Black, New Point(DomainR.X + DomainR.Width + 10, Y + (DomainR.Height - LabelHeight) / 2))

                Y += 5 + ColorRectSize.Height
            Next

            For Each Domain In Domains
                Dim DomainLeft = START_LOCATION_OFFSET + Domain.Position.Left
                Dim DomainRight = Domain.Position.FragmentSize + DomainLeft

                Dim DomainR As Rectangle = New Rectangle(New Point(DomainLeft, DIAGRAM_Y_OFFSET - 4), New Size(Domain.Position.FragmentSize, 24))
                'Call Gr.DrawLine(Pens.Black, New Point(DomainLeft - 2, RULE_HEIGHT), New Point(DomainLeft - 2, RULE_HEIGHT - 2))
                'Call Gr.DrawLine(Pens.Black, New Point(DomainRight, RULE_HEIGHT), New Point(DomainRight, RULE_HEIGHT - 2))
                'Call Gr.DrawString(Domain.Position.Left, New Font(FONT_MICROSOFT_YAHEI, 8), Brushes.Black, New Point(DomainLeft - 3, RULE_HEIGHT))
                'Call Gr.DrawString(Domain.Position.Right, New Font(FONT_MICROSOFT_YAHEI, 8), Brushes.Black, New Point(DomainRight - 3, RULE_HEIGHT))

                Dim Color As Brush = ColorProfiles(Domain.Name)

                Call Gr.FillRectangle(Color, DomainR)
            Next

            Return DiagramImage
        End Using
    End Function

    Private Shared Function GetColorProfiles(Domains As DomainObject()) As Dictionary(Of String, Brush)
        Dim IdList As String() = (From item In Domains Select item.Name Distinct).ToArray
        Dim ProfileData As Dictionary(Of String, Brush) = New Dictionary(Of String, Brush)
        Dim AllKnownColors As Color() = AllDotNetPrefixColors
        Dim i As Integer = 0

        For Each DomainId As String In IdList
            Call ProfileData.Add(DomainId, New SolidBrush(AllKnownColors(i)))
            i += 1
        Next

        Return ProfileData
    End Function
End Class
