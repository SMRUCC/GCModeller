#Region "Microsoft.VisualBasic::65f007d1d0022e45551ac3609606f80f, ..\visualize\ChromosomeMap\DrawingDevice.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
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
    Dim MARGIN As Integer = 400
    Dim SPLIT_HEIGHT As Integer

    Const ONE_TENTH_MBP_UNIT As Integer = 100 * 1000

    ''' <summary>
    ''' 绘制单位的长度与碱基数目的Mapping，默认初始值为0.1MB
    ''' </summary>
    ''' <remarks></remarks>
    Dim _UnitLength As Integer = ONE_TENTH_MBP_UNIT
    Dim _UnitConvert As Double

    ''' <summary>
    ''' 缩放因子
    ''' </summary>
    ''' <remarks></remarks>
    Dim _ScaleFactor As Double

    ''' <summary>
    ''' 标尺的数值
    ''' </summary>
    ''' <remarks></remarks>
    Dim rlMain As Single  ', rlSecnd As Integer

    ReadOnly _Conf As DataReader
    ReadOnly _RuleFactor As Double
    Dim _TextAlignmentMethod As DrawingModels.SegmentObject.__TextAlignment
    Dim UnitText As String

    Public Property RuleFont As Font

    ''' <summary>
    ''' 核酸序列的1MB的长度数值
    ''' </summary>
    Public Const MB_UNIT As Long = 1000 * 1000

    Sub New(ConfigData As DataReader)
        Dim size As Size = ConfigData.Resolution
        _Conf = ConfigData
        _Width = size.Width
        _Height = size.Height
        SPLIT_HEIGHT = ConfigData.LineHeight
        MARGIN = _Conf.Margin
        _UnitLength = ConfigData.LineLength * MB_UNIT
        _ScaleFactor = (_Width - 4 * MARGIN) / _UnitLength
        _RuleFactor = _UnitLength / 10
        Call __initialization()
    End Sub

    Sub New(width As Integer, height As Integer)
        _Conf = Config.DefaultValue.ToConfigurationModel
        _Width = width
        _Height = height
        SPLIT_HEIGHT = height / 11

        _ScaleFactor = (width - 4 * MARGIN) / _UnitLength
        Call __initialization()
    End Sub

    Sub New(Width As Integer)
        _Width = Width
        _Height = (6 / 19) * Width
        SPLIT_HEIGHT = _Height / 11
        _Conf = Config.DefaultValue.ToConfigurationModel
        _ScaleFactor = (Width - 4 * MARGIN) / _UnitLength
        Call __initialization()
    End Sub

    ''' <summary>
    ''' 进行一些系数的换算
    ''' </summary>
    Private Sub __initialization()
        Me._TextAlignmentMethod = Config.TypeOfAlignment(_Conf.FunctionAlignment)
        RuleFont = _Conf.SecondaryRuleFont

        Me._UnitConvert = Me._UnitLength / ONE_TENTH_MBP_UNIT

        If Me._UnitConvert = 1.0R Then
            UnitText = " MB"
        ElseIf Me._UnitConvert <= 0.1 Then
            UnitText = " KB"
        End If

        Me._UnitConvert = 1 / Me._UnitConvert / 10

        If _Conf.AddLegend Then
            _Height += MARGIN
        End If
    End Sub

    Public Function InvokeDrawing(model As DrawingModels.ChromesomeDrawingModel) As Bitmap()

        Call model.ToString.__DEBUG_ECHO

        If model.MotifSites.IsNullOrEmpty Then model.MotifSites = New DrawingModels.MotifSite() {}
        If model.MutationDatas.IsNullOrEmpty Then model.MutationDatas = New DrawingModels.MultationPointData() {}
        If model.Loci.IsNullOrEmpty Then model.Loci = New DrawingModels.Loci() {}
        If model.TSSs.IsNullOrEmpty Then model.TSSs = New DrawingModels.TSSs() {}

        If model.COGs.IsNullOrEmpty Then
            model.COGs = New Dictionary(Of String, Brush)
        End If

        Call model.COGs.Add("COG_NOT_ASSIGNED", New SolidBrush(_Conf.NoneCogColor))

        Try
            Return __invokeDrawing(model)
        Catch ex As Exception
            Call GDI_PLUS_UNHANDLE_EXCEPTION.Warning
            Throw ex
        End Try
    End Function

    Private Function __invokeDrawing(ObjectModel As DrawingModels.ChromesomeDrawingModel) As Bitmap()
        Dim imageList As New List(Of Bitmap)
        Dim startLen As Integer = 0, geneID As Integer = 0
        Dim First As Boolean = True

        Do While geneID < ObjectModel.GeneObjects.Count
            imageList += __chrMapDrawerProcessor(ObjectModel, startLen, geneID, First)

            If startLen = 0 Then  '当基因组的序列太短的时候，会出现死循环，并且startlength参数的值没有被改变，为0
                Exit Do
            Else
                '  First = False
            End If
        Loop

        Return imageList.ToArray
    End Function

    Const GDI_PLUS_UNHANDLE_EXCEPTION As String = "DataVisualization program crash due to an unexpected exception and a part of the data drawing is not accomplished."
    Const GDI_PLUS_MEMORY_EXCEPTION As String = "It seems the image resolution can not be hold on this computer because the free memory is not reach the requirements of the GDI+ to drawing such a big image file."

    Private Function __chrMapDrawerProcessor(LDM As DrawingModels.ChromesomeDrawingModel,
                                             ByRef startLen%,
                                             ByRef locus%,
                                             isFirst As Boolean) As Bitmap

        Dim FlagLength As Integer = _Conf.FlagLength, FlagHeight As Integer = _Conf.FLAG_HEIGHT
        Dim g As Graphics2D

        Call $"Resolution is {_Width}, {_Height}".__DEBUG_ECHO

        Try
            g = New Size(_Width, _Height).CreateGDIDevice
        Catch ex As Exception
            ex = New Exception($"Resolution is {_Width}, {_Height}", ex)
            ex = New Exception(GDI_PLUS_MEMORY_EXCEPTION, ex)

            Throw ex
        End Try

        Dim Height As Integer
        Dim Line As Integer = 0
        Dim strFlagFont = New Font("Ubuntu", 64, FontStyle.Bold)
        Dim NextLength As Integer = startLen + _UnitLength
        Dim LinePen = New Pen(Brushes.Black, 10)
        Dim PreRight As Integer
        Dim Level As Integer
        Dim ExitFor As Boolean = False
        Dim _Start_Length = startLen
        Dim FF As Boolean = False
        Dim ChangeLine = Function() As Boolean
                             If isFirst Then
                                 Height = Line * SPLIT_HEIGHT + MARGIN
                             Else
                                 If FF Then
                                     Height = Line * SPLIT_HEIGHT + MARGIN
                                 End If
                             End If

                             If Height > _Height - MARGIN Then
                                 ExitFor = True
                             End If

                             Line += 1

                             Return ExitFor
                         End Function
        Dim RightEnd As Integer = _Width - 2 * MARGIN

        Call ChangeLine()
        If isFirst Then
            Call __drawRuleLine(gdi:=g.Graphics,
                                  Height:=Height,
                                  Line:=Line,
                                  LinePen:=LinePen,
                                  strFlagFont:=strFlagFont,
                                  RightEnd:=RightEnd,
                                  ChromesomeLength:=LDM.Size)
            Call __drawChromosomeSites(LDM,
                                           _start_Length:=_Start_Length,
                                           FlagHeight:=FlagHeight,
                                           FlagLength:=FlagLength,
                                           GrDevice:=g.Graphics,
                                           Height:=Height,
                                           NextLength:=NextLength, scale:=_ScaleFactor)
        End If

        '    Dim PreDrawingRight As Integer

        For locus = locus To LDM.GeneObjects.Length - 1
            Dim Gene As DrawingModels.SegmentObject = LDM.GeneObjects(locus)

            If Gene.Left > NextLength Then
                FF = True  '这个变量确保能够正确的换行，不可以修改值以及顺序！~
                ExitFor = ChangeLine()

                If ExitFor Then
                    _Start_Length = NextLength
                    Exit For
                End If

                If NextLength >= LDM.Size Then
                    RightEnd = _Width - (NextLength - LDM.Size) * _ScaleFactor - 2 * MARGIN
                End If

                Call __drawRuleLine(gdi:=g.Graphics,
                                      Height:=Height,
                                      Line:=Line,
                                      LinePen:=LinePen,
                                      strFlagFont:=strFlagFont,
                                      RightEnd:=RightEnd,
                                      ChromesomeLength:=LDM.Size)

                _Start_Length = NextLength
                NextLength = NextLength + _UnitLength

                Call __drawChromosomeSites(ObjectModel:=LDM,
                                               _start_Length:=_Start_Length,
                                               FlagHeight:=FlagHeight,
                                               FlagLength:=FlagLength,
                                               GrDevice:=g.Graphics,
                                               Height:=Height,
                                               NextLength:=NextLength,
                                               scale:=_ScaleFactor)       '每换一行则首先绘制突变数据
            End If

            If Gene.Left < PreRight Then
                Level += 1
            Else
                Level = 0
            End If

            If Gene.Left > PreRight Then PreRight = Gene.Right

            Gene.Height = _Conf.GeneObjectHeight

            Dim drawingLociLeft As Integer = (Gene.Left - _Start_Length) * _ScaleFactor + MARGIN
            Dim drawingSize = Gene.Draw(g:=g.Graphics,
                                            location:=New Point(drawingLociLeft, Height + 100 + Level * 110),
                                            factor:=_ScaleFactor,
                                            RightLimited:=RightEnd,
                                            conf:=Me._Conf)
        Next

        startLen = _Start_Length

        If _Conf.AddLegend Then
            Call g.Graphics.DrawingCOGColors(
                LDM.COGs,
                ref:=New Point(MARGIN, _Height),
                legendFont:=_Conf.LegendFont,
                width:=_Width,
                margin:=MARGIN)
        End If
        Return g.ImageResource
    End Function

    Private Sub __drawRuleLine(gdi As Graphics,
                                   ByRef Height As Integer,
                                   ByRef Line As Integer,
                                   LinePen As Pen,
                                   strFlagFont As Font,
                                   RightEnd As Integer,
                                   ChromesomeLength As String)

        Call gdi.DrawLine(LinePen, New Point(MARGIN, Height), New Point(RightEnd, Height))

        rlMain += 1 * _UnitConvert

        Dim strFlag = String.Format("{0}{1}", __getRuleText(rlMain), UnitText)  '绘制主标尺
        If RightEnd < _Width - 2 * MARGIN Then
            strFlag = String.Format("{0}bp", ChromesomeLength)
        End If

        Dim size = gdi.MeasureString(strFlag, strFlagFont)
        Call gdi.DrawString(strFlag, strFlagFont, Brushes.Black, New Point(RightEnd + 0.2 * MARGIN, Height - 0.5 * size.Height))

        Dim ms = 0, tagFont = Me.RuleFont    '绘制小标尺
        Dim tagsize = gdi.MeasureString("0.00" & UnitText, tagFont)

        For i As Integer = 0 To 9
            Dim Left = i * (_Width - 2 * MARGIN) / 10 + MARGIN + 5
            Call gdi.DrawLine(LinePen, New Point(Left, Height), New Point(Left, Height - 30))
            Dim Tag = __getRuleText(rlMain - 1 * _UnitConvert)
            If InStr(Tag, ".") = 0 Then Tag = String.Format("{0}.{1}{2}", Tag, ms, UnitText) Else Tag = String.Format("{0}{1}{2}", Tag, ms, UnitText)
            Call gdi.DrawString(Tag, tagFont, Brushes.Black, New Point(Left - tagsize.Width / 2, Height - 35 - tagsize.Height))
            ms += 1
        Next
    End Sub

    Private Shared Function __getRuleText(n As Single) As String
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
    ''' <param name="ObjectModel"></param>
    ''' <param name="_start_Length"></param>
    ''' <param name="NextLength"></param>
    ''' <param name="Height"></param>
    ''' <param name="FlagLength"></param>
    ''' <param name="FlagHeight"></param>
    ''' <param name="GrDevice"></param>
    ''' <param name="scale"></param>
    Private Sub __drawChromosomeSites(ObjectModel As DrawingModels.ChromesomeDrawingModel,
                                          _start_Length As Integer,
                                          NextLength As Integer,
                                          Height As Integer,
                                          FlagLength As Integer,
                                          FlagHeight As Integer,
                                          GrDevice As Graphics,
                                          scale As Double)

        Dim MutationSites = __filteringSiteData(ObjectModel.MutationDatas, _start_Length, NextLength)
        For Each Point In MutationSites
            Call Point.Draw(GrDevice, New Point((Point.Left - _start_Length) * _ScaleFactor + MARGIN, Height - 30), FlagLength, FlagHeight)
        Next

        Dim MotifSites = __filteringSiteData(ObjectModel.MotifSites, _start_Length, NextLength)
        For Each Point In MotifSites
            Call Point.Draw(GrDevice, New Point((Point.Left - _start_Length + 0.5 * Point.Width) * _ScaleFactor + MARGIN, Height - 30), Point.Width * 0.6, 20)
        Next

        Dim LociSites = __filteringSiteData(ObjectModel.Loci, _start_Length, NextLength)
        For Each Point In LociSites
            Point.Scale = scale
            Call Point.Draw(GrDevice, New Point((Point.Left - _start_Length + 0.5 * Point.Width) * _ScaleFactor + MARGIN, Height - 30), Point.Width * 0.6, 20)
        Next

        Dim TSSs = __filteringSiteData(ObjectModel.TSSs, _start_Length, NextLength)
        For Each Point In TSSs
            Call Point.Draw(GrDevice, New Point((Point.Left - _start_Length + 0.5 * Point.Width) * _ScaleFactor + MARGIN, Height - 30), Point.Width * 0.6, 20)
        Next
    End Sub

    Private Shared Function __filteringSiteData(Of T As DrawingModels.Site)(data As T(), StartLength As Integer, NextRight As Integer) As T()
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
