#Region "Microsoft.VisualBasic::d1ed3f982c85f6cd0127442f09b43679, GCModeller\visualize\ChromosomeMap\DrawingDevice.vb"

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

    '   Total Lines: 444
    '    Code Lines: 319
    ' Comment Lines: 47
    '   Blank Lines: 78
    '     File Size: 17.35 KB


    ' Class DrawingDevice
    ' 
    '     Properties: Height, RulerFont, Width
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: chrMapDrawerProcessor, drawingImpl, ExportColorProfiles, filteringSiteData, getRulerText
    '               InvokeDrawing
    ' 
    '     Sub: __initialization, doRender, drawChromosomeSites, drawRulerLine
    '     Class args
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.ChromosomeMap.Configuration
Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels

Public Class DrawingDevice

    Public ReadOnly Property Width As Integer
    ''' <summary>
    ''' 图像的高度
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property Height As Integer
    ''' <summary>
    ''' 边距
    ''' </summary>
    ''' <remarks></remarks>
    Dim margin As Integer = 400
    Dim SPLIT_HEIGHT As Integer

    Const ONE_TENTH_MBP_UNIT As Integer = 100 * 1000

    ''' <summary>
    ''' 绘制单位的长度与碱基数目的Mapping，默认初始值为0.1MB
    ''' </summary>
    ''' <remarks></remarks>
    Dim unitLength As Integer = ONE_TENTH_MBP_UNIT
    Dim unitConvert As Double

    ''' <summary>
    ''' 缩放因子
    ''' </summary>
    ''' <remarks></remarks>
    Dim scaleFactor As Double

    ''' <summary>
    ''' 标尺的数值
    ''' </summary>
    ''' <remarks></remarks>
    Dim rlMain As Single  ', rlSecnd As Integer

    ReadOnly config As DataReader
    ReadOnly ruleFactor As Double
    Dim textAlignmentMethod As TextPadding
    Dim unitText As String

    Public Property RulerFont As Font

    ''' <summary>
    ''' 核酸序列的1MB的长度数值
    ''' </summary>
    Public Const MB_UNIT As Long = 1000 * 1000

    Sub New(config As DataReader)
        Dim size As Size = config.Resolution
        Me.config = config
        _Width = size.Width
        _Height = size.Height
        SPLIT_HEIGHT = config.LineHeight
        margin = Me.config.Margin
        unitLength = config.LineLength * MB_UNIT
        scaleFactor = (_Width - 4 * margin) / unitLength
        ruleFactor = unitLength / 10
        Call __initialization()
    End Sub

    Sub New(width As Integer, height As Integer)
        config = Configuration.Config.DefaultValue.DefaultValue.ToConfigurationModel
        _Width = width
        _Height = height
        SPLIT_HEIGHT = height / 11

        scaleFactor = (width - 4 * margin) / unitLength
        Call __initialization()
    End Sub

    Sub New(Width As Integer)
        _Width = Width
        _Height = (6 / 19) * Width
        SPLIT_HEIGHT = _Height / 11
        config = Configuration.Config.DefaultValue.DefaultValue.ToConfigurationModel
        scaleFactor = (Width - 4 * margin) / unitLength
        Call __initialization()
    End Sub

    ''' <summary>
    ''' 进行一些系数的换算
    ''' </summary>
    Private Sub __initialization()
        Me.textAlignmentMethod = Configuration.Config.TypeOfAlignment(config.FunctionAlignment)
        RulerFont = config.SecondaryRuleFont

        Me.unitConvert = Me.unitLength / ONE_TENTH_MBP_UNIT

        If Me.unitConvert = 1.0R Then
            unitText = " MB"
        ElseIf Me.unitConvert <= 0.1 Then
            unitText = " KB"
        End If

        Me.unitConvert = 1 / Me.unitConvert / 10

        If config.AddLegend Then
            _Height += margin
        End If
    End Sub

    Public Function InvokeDrawing(model As ChromesomeDrawingModel) As GraphicsData()
        With model
            Call .ToString.__DEBUG_ECHO

            .MotifSites = .MotifSites Or Empty(Of MotifSite)()
            .MutationDatas = .MutationDatas Or Empty(Of MultationPointData)()
            .Loci = .Loci Or Empty(Of Loci)()
            .TSSs = .TSSs Or Empty(Of TSSs)()

            If model.COGs.IsNullOrEmpty Then
                model.COGs = New Dictionary(Of String, Brush)
            End If
        End With

        Call model.COGs.Add("COG_NOT_ASSIGNED", New SolidBrush(config.NoneCogColor))

        'Try
        Return drawingImpl(model)
        'Catch ex As Exception
        'Call GDI_PLUS_UNHANDLE_EXCEPTION.Warning
        '    Throw ex
        'End Try
    End Function

    Private Function drawingImpl(chr As ChromesomeDrawingModel) As GraphicsData()
        Dim imageList As New List(Of GraphicsData)
        Dim args As New args With {
            .isFirst = True
        }

        Do While args.locus < chr.GeneObjects.Length
            imageList += chrMapDrawerProcessor(chr, args)

            If args.startLen = 0 Then  '当基因组的序列太短的时候，会出现死循环，并且startlength参数的值没有被改变，为0
                Exit Do
            Else
                '  First = False
            End If
        Loop

        Return imageList.ToArray
    End Function

    Const GDI_PLUS_UNHANDLE_EXCEPTION As String = "DataVisualization program crash due to an unexpected exception and a part of the data drawing is not accomplished."
    Const GDI_PLUS_MEMORY_EXCEPTION As String = "It seems the image resolution can not be hold on this computer because the free memory is not reach the requirements of the GDI+ to drawing such a big image file."

    ''' <summary>
    ''' 因为调用函数<see cref="drawingImpl"/>还需要根据<see cref="chrMapDrawerProcessor"/>的处理情况来判断结束条件
    ''' </summary>
    Private Class args
        Public startLen%
        Public locus%
        Public isFirst As Boolean
    End Class

    Private Function chrMapDrawerProcessor(chr As ChromesomeDrawingModel, args As args) As GraphicsData
        Dim plotInternal = Sub(ByRef g As IGraphics, region As GraphicsRegion)
                               Call doRender(g, region, chr, args)
                           End Sub

        Call $"Resolution is {_Width}, {_Height}".__DEBUG_ECHO

        Return g.GraphicsPlots(New Size(_Width, _Height), g.DefaultPadding, "white", plotInternal)
    End Function

    Private Sub doRender(ByRef g As IGraphics, region As GraphicsRegion, chr As ChromesomeDrawingModel, __args As args)
        Dim FlagLength As Integer = config.FlagLength, FlagHeight As Integer = config.FLAG_HEIGHT
        Dim Height As Integer
        Dim Line As Integer = 0
        Dim strFlagFont = New Font("Ubuntu", 64, FontStyle.Bold)
        Dim NextLength As Integer = __args.startLen + unitLength
        Dim LinePen = New Pen(Brushes.Black, 10)
        Dim PreRight As Integer
        Dim Level As Integer
        Dim ExitFor As Boolean = False
        Dim _Start_Length = __args.startLen
        Dim FF As Boolean = False
        Dim isFirst As Boolean = __args.isFirst
        Dim ChangeLine = Function() As Boolean
                             If isFirst Then
                                 Height = Line * SPLIT_HEIGHT + margin
                             Else
                                 If FF Then
                                     Height = Line * SPLIT_HEIGHT + margin
                                 End If
                             End If

                             If Height > _Height - margin Then
                                 ExitFor = True
                             End If

                             Line += 1

                             Return ExitFor
                         End Function
        Dim RightEnd As Integer = _Width - 2 * margin

        Call ChangeLine()

        If isFirst Then
            Call drawRulerLine(g,
                                  Height:=Height,
                                  Line:=Line,
                                  LinePen:=LinePen,
                                  strFlagFont:=strFlagFont,
                                  RightEnd:=RightEnd,
                                  ChromesomeLength:=chr.Size)
            Call drawChromosomeSites(chr,
                                           startLen:=_Start_Length,
                                           FlagHeight:=FlagHeight,
                                           FlagLength:=FlagLength,
                                           g:=g,
                                           height:=Height,
                                           NextLength:=NextLength, scale:=scaleFactor)
        End If

        For __args.locus = __args.locus To chr.GeneObjects.Length - 1
            Dim gene As SegmentObject = chr.GeneObjects(__args.locus)

            If gene.Left > NextLength Then
                FF = True  '这个变量确保能够正确的换行，不可以修改值以及顺序！~
                ExitFor = ChangeLine()

                If ExitFor Then
                    _Start_Length = NextLength
                    Exit For
                End If

                If NextLength >= chr.Size Then
                    RightEnd = _Width - (NextLength - chr.Size) * scaleFactor - 2 * margin
                End If

                Call drawRulerLine(g:=g,
                                      Height:=Height,
                                      Line:=Line,
                                      LinePen:=LinePen,
                                      strFlagFont:=strFlagFont,
                                      RightEnd:=RightEnd,
                                      ChromesomeLength:=chr.Size)

                _Start_Length = NextLength
                NextLength = NextLength + unitLength

                ' 每换一行则首先绘制突变数据
                Call drawChromosomeSites(chr,
                                               startLen:=_Start_Length,
                                               FlagHeight:=FlagHeight,
                                               FlagLength:=FlagLength,
                                               g:=g,
                                               height:=Height,
                                               NextLength:=NextLength,
                                               scale:=scaleFactor)
            End If

            If gene.Left < PreRight Then
                Level += 1
            Else
                Level = 0
            End If

            If gene.Left > PreRight Then PreRight = gene.Right

            gene.Height = config.GeneObjectHeight

            Dim drawingLociLeft As Integer = (gene.Left - _Start_Length) * scaleFactor + margin
            Dim drawingSize = gene.Draw(g:=g,
                                            location:=New Point(drawingLociLeft, Height + 100 + Level * 110),
                                            factor:=scaleFactor,
                                            RightLimited:=RightEnd,
                                            locusTagFont:=Me.config.LocusTagFont)
        Next

        __args.startLen = _Start_Length

        If config.AddLegend Then
            Call g.DrawingCOGColors(
                chr.COGs,
                ref:=New Point(margin, _Height),
                legendFont:=config.LegendFont,
                width:=_Width,
                margin:=margin)
        End If
    End Sub

    Private Sub drawRulerLine(g As IGraphics,
                                   ByRef Height As Integer,
                                   ByRef Line As Integer,
                                   LinePen As Pen,
                                   strFlagFont As Font,
                                   RightEnd As Integer,
                                   ChromesomeLength As String)

        Call g.DrawLine(LinePen, New Point(margin, Height), New Point(RightEnd, Height))

        rlMain += 1 * unitConvert

        Dim strFlag = String.Format("{0}{1}", getRulerText(rlMain), unitText)  '绘制主标尺
        If RightEnd < _Width - 2 * margin Then
            strFlag = String.Format("{0}bp", ChromesomeLength)
        End If

        Dim size = g.MeasureString(strFlag, strFlagFont)
        Call g.DrawString(strFlag, strFlagFont, Brushes.Black, New Point(RightEnd + 0.2 * margin, Height - 0.5 * size.Height))

        Dim ms = 0, tagFont = Me.RulerFont    '绘制小标尺
        Dim tagsize = g.MeasureString("0.00" & unitText, tagFont)

        For i As Integer = 0 To 9
            Dim Left = i * (_Width - 2 * margin) / 10 + margin + 5
            Call g.DrawLine(LinePen, New Point(Left, Height), New Point(Left, Height - 30))
            Dim Tag = getRulerText(rlMain - 1 * unitConvert)
            If InStr(Tag, ".") = 0 Then Tag = String.Format("{0}.{1}{2}", Tag, ms, unitText) Else Tag = String.Format("{0}{1}{2}", Tag, ms, unitText)
            Call g.DrawString(Tag, tagFont, Brushes.Black, New Point(Left - tagsize.Width / 2, Height - 35 - tagsize.Height))
            ms += 1
        Next
    End Sub

    Private Shared Function getRulerText(n As Single) As String
        Dim s As String = Format(n, "##.#")
        Dim p = InStr(s, ".")

        If p = 0 Then
            If String.IsNullOrEmpty(s) Then
                Return "0.0"
            Else
                Return s
            End If
        Else
            s = Mid(s, 1, p + 1)
            If s.First = "."c Then
                s = 0 & s
            End If

            Return s
        End If
    End Function

    ''' <summary>
    ''' 在这里绘制基因组上面的所有的位点的数据
    ''' </summary>
    ''' <param name="chr"></param>
    ''' <param name="startLen"></param>
    ''' <param name="NextLength"></param>
    ''' <param name="height"></param>
    ''' <param name="FlagLength"></param>
    ''' <param name="FlagHeight"></param>
    ''' <param name="g"></param>
    ''' <param name="scale"></param>
    Private Sub drawChromosomeSites(chr As ChromesomeDrawingModel,
                                    startLen As Integer,
                                    NextLength As Integer,
                                    height As Integer,
                                    FlagLength As Integer,
                                    FlagHeight As Integer,
                                    g As IGraphics,
                                    scale As Double)

        Dim position As Point
        Dim MutationSites = filteringSiteData(chr.MutationDatas, startLen, NextLength)

        For Each point As MultationPointData In MutationSites
            position = New Point With {
                .X = (point.Left - startLen) * scaleFactor + margin,
                .Y = height - 30
            }

            Call point.Draw(g, position, FlagLength, FlagHeight)
        Next

        Dim MotifSites = filteringSiteData(chr.MotifSites, startLen, NextLength)
        For Each site In MotifSites
            position = New Point With {
                .X = (site.Left - startLen + 0.5 * site.Width) * scaleFactor + margin,
                .Y = height - 30
            }

            Call site.Draw(g, position, 30, site.Width * 0.65)
        Next

        Dim LociSites = filteringSiteData(chr.Loci, startLen, NextLength)
        For Each Point In LociSites
            Point.Scale = scale
            Call Point.Draw(g, New Point((Point.Left - startLen + 0.5 * Point.Width) * scaleFactor + margin, height - 30), Point.Width * 0.6, 20)
        Next

        Dim TSSs = filteringSiteData(chr.TSSs, startLen, NextLength)
        For Each Point In TSSs
            Call Point.Draw(g, New Point((Point.Left - startLen + 0.5 * Point.Width) * scaleFactor + margin, height - 30), Point.Width * 0.6, 20)
        Next
    End Sub

    Private Shared Function filteringSiteData(Of T As DrawingModels.Site)(data As T(), StartLength As Integer, NextRight As Integer) As T()
        Dim LQuery = (From site In data Where site.Left > StartLength AndAlso site.Right < NextRight Select site).ToArray
        Return LQuery
    End Function

    Public Function ExportColorProfiles(ObjectModel As ChromesomeDrawingModel) As Image
        Dim g As Graphics2D
        Dim _Width As Integer = 1920, _Height As Integer = 1200

        Try
            Call $"Resolution is {_Width}, {_Height}".__DEBUG_ECHO
            Call FlushMemory()

            g = New Size(_Width, _Height).CreateGDIDevice
        Catch ex As Exception
            Call Console.WriteLine(GDI_PLUS_MEMORY_EXCEPTION & vbCrLf)
            Throw
        End Try

        Dim Font As Font = New Font("Ubuntu", 20, FontStyle.Bold)
        Dim Left As Integer
        Dim Top As Integer

        Dim str As String = ObjectModel.Taxname '绘制NCBI上面的基本信息
        Dim size As SizeF = g.MeasureString(str, Font)

        Call g.DrawString(str, Font, Brushes.Black, New Point(Left, Top))
        Top += size.Height * 1.5 : Left += 0.1 * size.Width
        str = String.Format("Chromesome DNA Length: {0}", ObjectModel.Size)
        Font = New Font("Ubuntu", 16)
        size = g.MeasureString(str, Font)
        Call g.DrawString(str, Font, Brushes.Black, New Point(Left, Top))
        Top = _Height * 0.4
        Call g.DrawString("Footprint map information", Font, Brushes.Black, New Point(Left, Top))

        If ObjectModel.MotifSites.IsNullOrEmpty Then
            Call g.DrawString("No motif site on the map.", Font, Brushes.Gray, New Point(Left, Top + 10))
        End If

        Return g.ImageResource
    End Function
End Class
