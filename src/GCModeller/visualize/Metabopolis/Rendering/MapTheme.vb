#Region "Metabopolis: map theme"

' ============================================================================
' 地图主题
' ----------------------------------------------------------------------------
' 集中定义渲染配色与字号：
'   * 街区调色板 —— 每个代谢类别一种底色（浅色填充 + 深色描边）；
'   * 九色角色编码 —— 区分两个类别之间代谢物的底物/产物组合（论文 Figure 4b）；
'   * 道路、建筑块、枢纽节点与文字样式。
' ============================================================================

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Metabopolis.Model

Namespace Rendering

    ''' <summary>
    ''' Metabopolis 地图的视觉主题。
    ''' </summary>
    Public Class MapTheme

        ''' <summary>街区底色（按类别序号循环取用）。</summary>
        Public Property CategoryPalette As Color()

        ''' <summary>街区底色的填充透明度（0-255）。</summary>
        Public Property CategoryFillAlpha As Integer = 48

        ''' <summary>街区描边透明度。</summary>
        Public Property CategoryStrokeAlpha As Integer = 190

        ''' <summary>九色角色编码对应的边色。</summary>
        Public Property RolePalette As Color()

        ''' <summary>建筑块填充色。</summary>
        Public Property BuildingFill As Color = Color.FromArgb(228, 231, 235)

        ''' <summary>建筑块描边色。</summary>
        Public Property BuildingBorder As Color = Color.FromArgb(190, 195, 200)

        ''' <summary>块内车道颜色。</summary>
        Public Property LaneColor As Color = Color.FromArgb(150, 158, 166)

        ''' <summary>枢纽代谢物颜色。</summary>
        Public Property JunctionColor As Color = Color.FromArgb(240, 150, 40)

        ''' <summary>画布背景色。</summary>
        Public Property BackgroundColor As Color = Color.FromArgb(252, 252, 250)

        ''' <summary>文字颜色。</summary>
        Public Property LabelColor As Color = Color.FromArgb(45, 45, 45)

        ''' <summary>次级文字颜色。</summary>
        Public Property SubLabelColor As Color = Color.FromArgb(110, 110, 110)

        ''' <summary>块内车道线宽。</summary>
        Public Property LaneWidth As Single = 0.9

        ''' <summary>块间路由线宽。</summary>
        Public Property RouteWidth As Single = 1.4

        ''' <summary>街区描边线宽。</summary>
        Public Property BlockStrokeWidth As Single = 1.8

        Private _blockFont As Font
        Private _smallFont As Font
        Private _titleFont As Font

        ''' <summary>街区标题字体。</summary>
        Public Property BlockFont As Font
            Get
                If _blockFont Is Nothing Then
                    _blockFont = SafeFont("Arial", 11, FontStyle.Bold)
                End If

                Return _blockFont
            End Get
            Set(value As Font)
                _blockFont = value
            End Set
        End Property

        ''' <summary>小号字体（枢纽标签、图例）。</summary>
        Public Property SmallFont As Font
            Get
                If _smallFont Is Nothing Then
                    _smallFont = SafeFont("Arial", 8)
                End If

                Return _smallFont
            End Get
            Set(value As Font)
                _smallFont = value
            End Set
        End Property

        ''' <summary>标题字体。</summary>
        Public Property TitleFont As Font
            Get
                If _titleFont Is Nothing Then
                    _titleFont = SafeFont("Arial", 16, FontStyle.Bold)
                End If

                Return _titleFont
            End Get
            Set(value As Font)
                _titleFont = value
            End Set
        End Property

        ''' <summary>按类别序号取底色。</summary>
        Public Function CategoryColor(index As Integer) As Color
            If CategoryPalette Is Nothing OrElse CategoryPalette.Length = 0 Then
                Return Color.SteelBlue
            End If

            Dim i As Integer = ((index Mod CategoryPalette.Length) + CategoryPalette.Length) Mod CategoryPalette.Length
            Return CategoryPalette(i)
        End Function

        ''' <summary>按九色角色编码取边色。</summary>
        Public Function RoleColor(role As EdgeRole) As Color
            If RolePalette Is Nothing OrElse RolePalette.Length = 0 Then
                Return Color.Gray
            End If

            Dim i As Integer = CInt(role)

            If i < 0 OrElse i >= RolePalette.Length Then
                i = 0
            End If

            Return RolePalette(i)
        End Function

        ''' <summary>九色角色编码的可读名称（用于图例）。</summary>
        Public Shared Function RoleName(role As EdgeRole) As String
            Select Case role
                Case EdgeRole.ReactantToReactant
                    Return "reactant -> reactant"
                Case EdgeRole.ReactantToProduct
                    Return "reactant -> product"
                Case EdgeRole.ReactantToBoth
                    Return "reactant -> both"
                Case EdgeRole.ProductToReactant
                    Return "product -> reactant"
                Case EdgeRole.ProductToProduct
                    Return "product -> product"
                Case EdgeRole.ProductToBoth
                    Return "product -> both"
                Case EdgeRole.BothToReactant
                    Return "both -> reactant"
                Case EdgeRole.BothToProduct
                    Return "both -> product"
                Case Else
                    Return "both -> both"
            End Select
        End Function

        Private Shared Function SafeFont(name As String, size As Single, Optional style As FontStyle = FontStyle.Regular) As Font
            Return New Font(name, size, style)
        End Function

        ''' <summary>
        ''' 默认主题：Tableau 十色作为街区底色，Tableau 分类色作为九色角色编码。
        ''' </summary>
        Public Shared Function CreateDefault() As MapTheme
            Return New MapTheme With {
                .CategoryPalette = New Color() {
                    Color.FromArgb(76, 120, 168),
                    Color.FromArgb(245, 133, 24),
                    Color.FromArgb(84, 162, 75),
                    Color.FromArgb(228, 87, 86),
                    Color.FromArgb(114, 183, 178),
                    Color.FromArgb(178, 121, 162),
                    Color.FromArgb(255, 157, 166),
                    Color.FromArgb(157, 117, 93),
                    Color.FromArgb(186, 176, 172),
                    Color.FromArgb(140, 86, 75),
                    Color.FromArgb(148, 103, 189),
                    Color.FromArgb(214, 39, 40)
                },
                .RolePalette = New Color() {
                    Color.FromArgb(78, 121, 167),
                    Color.FromArgb(242, 142, 43),
                    Color.FromArgb(89, 161, 79),
                    Color.FromArgb(225, 87, 89),
                    Color.FromArgb(118, 183, 178),
                    Color.FromArgb(176, 122, 161),
                    Color.FromArgb(255, 157, 167),
                    Color.FromArgb(156, 117, 95),
                    Color.FromArgb(186, 176, 172)
                }
            }
        End Function

    End Class

End Namespace

#End Region
