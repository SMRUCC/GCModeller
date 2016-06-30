Imports System.Drawing
Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.GCModeller.DataVisualization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Imaging

Namespace ChromosomeMap

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

        ReadOnly _Conf As Conf
        ReadOnly _RuleFactor As Double
        Dim _TextAlignmentMethod As DrawingModels.SegmentObject.__TextAlignment
        Dim UnitText As String

        Public Property RuleFont As Font

        ''' <summary>
        ''' 核酸序列的1MB的长度数值
        ''' </summary>
        Public Const MB_UNIT As Long = 1000 * 1000

        Sub New(ConfigData As Conf)
            Dim size = ConfigData.Resolution
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
            _Conf = Configurations.DefaultValue.ToConfigurationModel
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
            _Conf = Configurations.DefaultValue.ToConfigurationModel
            _ScaleFactor = (Width - 4 * MARGIN) / _UnitLength
            Call __initialization()
        End Sub

        Private Sub __initialization()
            Me._TextAlignmentMethod = Configurations.TypeOfAlignment(_Conf.FunctionAlignment)
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

        Public Function InvokeDrawing(ObjectModel As DrawingModels.ChromesomeDrawingModel) _
            As KeyValuePair(Of Imaging.ImageFormat, System.Drawing.Bitmap())

            Call ObjectModel.ToString.__DEBUG_ECHO

            If ObjectModel.MotifSites.IsNullOrEmpty Then ObjectModel.MotifSites = New DrawingModels.MotifSite() {}
            If ObjectModel.MutationDatas.IsNullOrEmpty Then ObjectModel.MutationDatas = New DrawingModels.MultationPointData() {}
            If ObjectModel.Loci.IsNullOrEmpty Then ObjectModel.Loci = New DrawingModels.Loci() {}
            If ObjectModel.TSSs.IsNullOrEmpty Then ObjectModel.TSSs = New DrawingModels.TSSs() {}

            If ObjectModel.MyvaCogColorProfile.IsNullOrEmpty Then
                ObjectModel.MyvaCogColorProfile = New Dictionary(Of String, Brush)
            End If

            Call ObjectModel.MyvaCogColorProfile.Add("COG_NOT_ASSIGNED", New SolidBrush(_Conf.NoneCogColor))

            Try
                Return New KeyValuePair(Of System.Drawing.Imaging.ImageFormat, Bitmap())(
                    _Conf.SavedFormat,
                    __invokeDrawing(ObjectModel))
            Catch ex As Exception
                Call GDI_PLUS_UNHANDLE_EXCEPTION.__DEBUG_ECHO
                Throw
            End Try
        End Function

        Private Function __invokeDrawing(ObjectModel As DrawingModels.ChromesomeDrawingModel) As Bitmap()
            Dim ImageList As List(Of Bitmap) = New List(Of Bitmap)
            Dim StartLength As Integer = 0, GeneId As Integer = 0
            Dim First As Boolean = True

            Do While GeneId < ObjectModel.GeneObjects.Count
                Dim Frame = __chrMapDrawerProcessor(ObjectModel, StartLength, GeneId, First)
                Call ImageList.Add(item:=Frame)

                If StartLength = 0 Then  '当基因组的序列太短的时候，会出现死循环，并且startlength参数的值没有被改变，为0
                    Exit Do
                Else
                    '  First = False
                End If
            Loop

            Return ImageList.ToArray
        End Function

        Const GDI_PLUS_UNHANDLE_EXCEPTION As String = "DataVisualization program crash due to an unexpected exception and a part of the data drawing is not accomplished."
        Const GDI_PLUS_MEMORY_EXCEPTION As String = "It seems the image resolution can not be hold on this computer because the free memory is not reach the requirements of the GDI+ to drawing such a big image file."

        Private Function __chrMapDrawerProcessor(LDM As DrawingModels.ChromesomeDrawingModel,
                                                 ByRef startLen As Integer,
                                                 ByRef locus As Integer,
                                                 IsFirst As Boolean) As Bitmap

            Dim FlagLength As Integer = _Conf.FlagLength, FlagHeight As Integer = _Conf.FLAG_HEIGHT
            Dim Gr As GDIPlusDeviceHandle

            Try
                Call $"Resolution is {_Width}, {_Height}".__DEBUG_ECHO
                Call FlushMemory()

                Gr = New Size(_Width, _Height).CreateGDIDevice
            Catch ex As Exception
                ex = New Exception($"Resolution is {_Width}, {_Height}", ex)
                ex = New Exception(GDI_PLUS_MEMORY_EXCEPTION, ex)

                Throw ex
            End Try

            Gr.CompositingMode = Drawing2D.CompositingMode.SourceOver
            Gr.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
            Gr.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBilinear

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
                                 If IsFirst Then
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
            If IsFirst Then
                Call __drawRuleLine(gdi:=Gr.Graphics,
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
                                           GrDevice:=Gr.Graphics,
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

                    Call __drawRuleLine(gdi:=Gr.Graphics,
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
                                               GrDevice:=Gr.Graphics,
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
                Dim drawingSize = Gene.Draw(Gr:=Gr.Graphics,
                                            Location:=New Point(drawingLociLeft, Height + 100 + Level * 110),
                                            ConvertFactor:=_ScaleFactor,
                                            RightLimited:=RightEnd,
                                            conf:=Me._Conf)
            Next

            startLen = _Start_Length
            If _Conf.AddLegend Then Call Gr.Graphics.DrawingCOGColors(LDM.MyvaCogColorProfile,
                                                          ref:=New Point(MARGIN, _Height),
                                                          legendFont:=_Conf.LegendFont,
                                                          width:=_Width,
                                                          margin:=MARGIN)
            Return Gr.ImageResource
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

        Public Function ExportColorProfiles(ObjectModel As ChromosomeMap.DrawingModels.ChromesomeDrawingModel) As Image
            Dim GrDevice As GDIPlusDeviceHandle
            Dim _Width As Integer = 1920, _Height As Integer = 1200

            Try
                Call $"Resolution is {_Width}, {_Height}".__DEBUG_ECHO
                Call FlushMemory()

                GrDevice = New Size(_Width, _Height).CreateGDIDevice
            Catch ex As Exception
                Call Console.WriteLine(GDI_PLUS_MEMORY_EXCEPTION & vbCrLf)
                Throw
            End Try

            Dim Font As Font = New Font("Ubuntu", 20, FontStyle.Bold)
            Dim Left As Integer
            Dim Top As Integer

            Dim str As String = ObjectModel.Taxname '绘制NCBI上面的基本信息
            Dim size As SizeF = GrDevice.MeasureString(str, Font)

            Call GrDevice.DrawString(str, Font, Brushes.Black, New Point(Left, Top))
            Top += size.Height * 1.5 : Left += 0.1 * size.Width
            str = String.Format("Chromesome DNA Length: {0}", ObjectModel.Size)
            Font = New Font("Ubuntu", 16)
            size = GrDevice.MeasureString(str, Font)
            Call GrDevice.DrawString(str, Font, Brushes.Black, New Point(Left, Top))
            Top = _Height * 0.4
            Call GrDevice.DrawString("Footprint map information", Font, Brushes.Black, New Point(Left, Top))

            If ObjectModel.MotifSites.IsNullOrEmpty Then
                Call GrDevice.DrawString("No motif site on the map.", Font, Brushes.Gray, New Point(Left, Top + 10))
            End If

            Return GrDevice.ImageResource
        End Function
    End Class
End Namespace