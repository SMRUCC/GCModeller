Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms.VisualStyles

'TODO: RTL
'TODO: gray shield
'TODO: measuring
'TODO: note
'TODO: &

''' <summary>
''' Vista-like command link.
''' </summary>
<ToolboxBitmap(GetType(Button)), ToolboxItem(True), ToolboxItemFilter("System.Windows.Forms"), Description("Raises an event when the user clicks it.")> _
Public Class CommandLink

    Public Sub New()
        InitializeComponent()
        MyBase.BackColor = Color.Transparent
        BackColor = Color.Transparent
        ForeColor = Color.FromArgb(21, 28, 85) '(0, 102, 204) ' 0 51 153
        _hotForeColor = Color.FromArgb(7, 74, 229) '(0, 102, 204) ' 0 51 153
        'Font * 4 / 3
        _pressedBorder = Color.FromArgb(153, 118, 118, 118)
        _pressedBackground = Color.FromArgb(102, 222, 225, 225)
        _activeBorderHot1 = Color.FromArgb(163, 0, 201, 253)
        _activeBorderHot2 = Color.FromArgb(128, 0, 201, 253)
        _activeBorderCold = Color.FromArgb(41, 0, 201, 253)
        _hoveredBorder1 = Color.FromArgb(107, 119, 119, 119)
        _hoveredBorder2 = Color.FromArgb(252, 255, 255, 255)
        _hoveredBorder3 = Color.FromArgb(183, 249, 249, 249)
        _hoveredBackground1 = Color.White
        _hoveredBackground2 = Color.FromArgb(135, 242, 242, 242)
        _hoveredBackground3 = Color.FromArgb(102, 232, 232, 232)
        animatingTo = FrameType.Normal
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.SupportsTransparentBackColor Or ControlStyles.UserPaint, True)
        SetStyle(ControlStyles.Opaque, False)
    End Sub

    Public Overrides Function GetPreferredSize(ByVal proposedSize As Size) As Size
        proposedSize = MyBase.GetPreferredSize(proposedSize)
        If proposedSize.Height < 41 Then proposedSize.Height = 41
        proposedSize.Width = proposedSize.Width * 4 \ 3
        'proposedSize.Width -= proposedSize.Width Mod 25
        'proposedSize.Width = [If](proposedSize.Width < 75, 75, proposedSize.Width)
        'proposedSize.Height = [If](proposedSize.Height < 25, 25, proposedSize.Height)
        Return proposedSize
    End Function

#Region " Fields and Properties "

    Private _backColor As Color
    ''' <summary> 
    ''' Gets or sets the background color of the control. 
    ''' </summary> 
    ''' <returns>A <see cref="T:System.Drawing.Color" /> value representing the background color.</returns> 
    <DefaultValue(GetType(Color), "Transparent")> _
    Public Overridable Shadows Property BackColor() As Color
        Get
            Return _backColor
        End Get
        Set(ByVal value As Color)
            If Not _backColor.Equals(value) Then
                _backColor = value
                UseVisualStyleBackColor = False
                If IsHandleCreated Then
                    CreateFrames()
                End If
                OnBackColorChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    ''' <summary> 
    ''' Gets or sets the foreground color of the control. 
    ''' </summary> 
    ''' <returns>The foreground <see cref="T:System.Drawing.Color" /> of the control.</returns> 
    <DefaultValue(GetType(Color), "21, 28, 85")> _
    Public Overridable Shadows Property ForeColor() As Color
        Get
            Return MyBase.ForeColor
        End Get
        Set(ByVal value As Color)
            MyBase.ForeColor = value
        End Set
    End Property

    Private _hotForeColor As Color
    ''' <summary> 
    ''' Gets or sets the hot foreground color of the control. 
    ''' </summary> 
    ''' <returns>The hot foreground <see cref="T:System.Drawing.Color" /> of the control.</returns> 
    <DefaultValue(GetType(Color), "7, 74, 229"), Category("Appearance")> _
    Public Overridable Shadows Property HotForeColor() As Color
        Get
            Return _hotForeColor
        End Get
        Set(ByVal value As Color)
            If Not _hotForeColor.Equals(value) Then
                _hotForeColor = value
                'CreateFrames()
                OnHotForeColorChanged(EventArgs.Empty)
                If IsHandleCreated Then
                    Invalidate()
                End If
            End If
        End Set
    End Property

    'Private _note As String
    '''' <summary>
    '''' Gets or sets the supporting note text of the command link.
    '''' </summary>
    '''' <value>New supporting note text of the command link.</value>
    '''' <returns>The supporting note text of the command link.</returns>
    '<Category("Appearance"), _
    'Description("Specifies the supporting note text."), _
    'BrowsableAttribute(True), _
    'DefaultValue(CType(Nothing, String))> _
    'Public Property Note() As String
    '    Get
    '        Return _note
    '    End Get
    '    Set(ByVal value As String)
    '        If _note <> value Then
    '            _note = value
    '            If IsHandleCreated Then
    '                'CreateFrames()
    '                Invalidate()
    '            End If
    '        End If
    '    End Set
    'End Property

    Protected Overrides Sub OnTextChanged(ByVal e As System.EventArgs)
        MyBase.OnTextChanged(e)
        If IsHandleCreated Then
            'CreateFrames()
            Invalidate()
        End If
    End Sub

    Private _showElevationIcon As Boolean
    ''' <summary>
    ''' Determines whether to show the elevation icon (shield).
    ''' </summary>
    <Category("Appearance"), _
    Description("Indicates whether the button should be decorated with the security shield icon."), _
    BrowsableAttribute(True), _
    DefaultValue(False)> _
    Public Property ShowElevationIcon() As Boolean
        Get
            Return _showElevationIcon
        End Get
        Set(ByVal value As Boolean)
            If _showElevationIcon <> value Then
                _showElevationIcon = value
                If IsHandleCreated Then
                    CreateFrames()
                    Invalidate()
                End If
            End If
        End Set
    End Property

    Private _pressedBorder As Color
    Private _pressedBackground As Color
    Private _activeBorderHot1 As Color
    Private _activeBorderHot2 As Color
    Private _activeBorderCold As Color
    Private _hoveredBorder1 As Color
    Private _hoveredBorder2 As Color
    Private _hoveredBorder3 As Color
    Private _hoveredBackground1 As Color
    Private _hoveredBackground2 As Color
    Private _hoveredBackground3 As Color

    Private isHovered As Boolean
    Private isFocused As Boolean
    Private isFocusedByKey As Boolean
    Private isKeyDown As Boolean
    Private isMouseDown As Boolean
    Private ReadOnly Property isPressed() As Boolean
        Get
            Return isKeyDown OrElse (isMouseDown AndAlso isHovered)
        End Get
    End Property

    ''' <summary> 
    ''' Gets the state of the button control. 
    ''' </summary> 
    ''' <value>The state of the button control.</value> 
    <Browsable(False)> _
    Public ReadOnly Property State() As PushButtonState
        Get
            If Not Enabled Then
                Return PushButtonState.Disabled
            End If
            If isPressed Then
                Return PushButtonState.Pressed
            End If
            If isHovered Then
                Return PushButtonState.Hot
            End If
            If isFocused OrElse IsDefault Then
                Return PushButtonState.[Default]
            End If
            Return PushButtonState.Normal
        End Get
    End Property

#End Region

#Region " Events "

    ''' <summary>Occurs when the value of the <see cref="P:Glass.GlassButton.HotForeColor" /> property changes.</summary> 
    <Description("Event raised when the value of the HotForeColor property is changed."), Category("Property Changed")> _
    Public Event HotForeColorChanged As EventHandler

    ''' <summary> 
    ''' Raises the <see cref="E:Glass.GlassButton.HotForeColorChanged" /> event. 
    ''' </summary> 
    ''' <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param> 
    Protected Overridable Sub OnHotForeColorChanged(ByVal e As EventArgs)
        RaiseEvent HotForeColorChanged(Me, e)
    End Sub

    ''' <summary>Occurs when the value of the <see cref="P:Glass.GlassButton.InnerBorderColor" /> property changes.</summary> 
    <Description("Event raised when the value of the InnerBorderColor property is changed."), Category("Property Changed")> _
    Public Event InnerBorderColorChanged As EventHandler

    ''' <summary> 
    ''' Raises the <see cref="E:Glass.GlassButton.InnerBorderColorChanged" /> event. 
    ''' </summary> 
    ''' <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param> 
    Protected Overridable Sub OnInnerBorderColorChanged(ByVal e As EventArgs)
        RaiseEvent InnerBorderColorChanged(Me, e)
    End Sub

    ''' <summary>Occurs when the value of the <see cref="P:Glass.GlassButton.OuterBorderColor" /> property changes.</summary> 
    <Description("Event raised when the value of the OuterBorderColor property is changed."), Category("Property Changed")> _
    Public Event OuterBorderColorChanged As EventHandler

    ''' <summary> 
    ''' Raises the <see cref="E:Glass.GlassButton.OuterBorderColorChanged" /> event. 
    ''' </summary> 
    ''' <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param> 
    Protected Overridable Sub OnOuterBorderColorChanged(ByVal e As EventArgs)
        RaiseEvent OuterBorderColorChanged(Me, e)
    End Sub

    ''' <summary>Occurs when the value of the <see cref="P:Glass.GlassButton.ShineColor" /> property changes.</summary> 
    <Description("Event raised when the value of the ShineColor property is changed."), Category("Property Changed")> _
    Public Event ShineColorChanged As EventHandler

    ''' <summary> 
    ''' Raises the <see cref="E:Glass.GlassButton.ShineColorChanged" /> event. 
    ''' </summary> 
    ''' <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param> 
    Protected Overridable Sub OnShineColorChanged(ByVal e As EventArgs)
        RaiseEvent ShineColorChanged(Me, e)
    End Sub

    ''' <summary>Occurs when the value of the <see cref="P:Glass.GlassButton.GlowColor" /> property changes.</summary> 
    <Description("Event raised when the value of the GlowColor property is changed."), Category("Property Changed")> _
    Public Event GlowColorChanged As EventHandler

    ''' <summary> 
    ''' Raises the <see cref="E:Glass.GlassButton.GlowColorChanged" /> event. 
    ''' </summary> 
    ''' <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param> 
    Protected Overridable Sub OnGlowColorChanged(ByVal e As EventArgs)
        RaiseEvent GlowColorChanged(Me, e)
    End Sub

#End Region

#Region " Overrided Methods "

    ''' <summary> 
    ''' Raises the <see cref="E:System.Windows.Forms.Control.EnabledChanged" /> event. 
    ''' </summary> 
    ''' <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param> 
    Protected Overrides Sub OnEnabledChanged(ByVal e As System.EventArgs)
        Fade()
        MyBase.OnEnabledChanged(e)
    End Sub

    ''' <summary> 
    ''' Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged" /> event. 
    ''' </summary> 
    ''' <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param> 
    Protected Overloads Overrides Sub OnSizeChanged(ByVal e As EventArgs)
        CreateFrames()
        MyBase.OnSizeChanged(e)
    End Sub

    ''' <summary> 
    ''' Raises the <see cref="E:System.Windows.Forms.Control.Click" /> event. 
    ''' </summary> 
    ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param> 
    Protected Overloads Overrides Sub OnClick(ByVal e As EventArgs)
        isKeyDown = False
        isMouseDown = False
        Fade()
        MyBase.OnClick(e)
    End Sub

    ''' <summary> 
    ''' Raises the <see cref="E:System.Windows.Forms.Control.GotFocus" /> event. 
    ''' </summary> 
    ''' <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    Protected Overrides Sub OnEnter(ByVal e As System.EventArgs)
        isFocused = True
        isFocusedByKey = True
        Fade()
        MyBase.OnEnter(e)
    End Sub

    ''' <summary> 
    ''' Raises the <see cref="E:System.Windows.Forms.Control.Leave" /> event. 
    ''' </summary> 
    ''' <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param> 
    Protected Overrides Sub OnLeave(ByVal e As System.EventArgs)
        MyBase.OnLeave(e)
        isFocused = False
        isFocusedByKey = False
        isKeyDown = False
        isMouseDown = False
        Fade()
        Invalidate()
    End Sub

    ''' <summary> 
    ''' Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnKeyUp(System.Windows.Forms.KeyEventArgs)" /> event. 
    ''' </summary> 
    ''' <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs" /> that contains the event data.</param> 
    Protected Overloads Overrides Sub OnKeyDown(ByVal e As KeyEventArgs)
        If e.KeyCode = Keys.Space Then
            isKeyDown = True
            Fade()
            Invalidate()
        End If
        MyBase.OnKeyDown(e)
    End Sub

    ''' <summary> 
    ''' Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnKeyUp(System.Windows.Forms.KeyEventArgs)" /> event. 
    ''' </summary> 
    ''' <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs" /> that contains the event data.</param> 
    Protected Overloads Overrides Sub OnKeyUp(ByVal e As KeyEventArgs)
        If isKeyDown AndAlso e.KeyCode = Keys.Space Then
            isKeyDown = False
            Fade()
            Invalidate()
        End If
        MyBase.OnKeyUp(e)
    End Sub

    ''' <summary> 
    ''' Raises the <see cref="E:System.Windows.Forms.Control.MouseDown" /> event. 
    ''' </summary> 
    ''' <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param> 
    Protected Overloads Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        If Not isMouseDown AndAlso e.Button = MouseButtons.Left Then
            isMouseDown = True
            isFocusedByKey = False
            Fade()
            Invalidate()
        End If
        MyBase.OnMouseDown(e)
    End Sub

    ''' <summary> 
    ''' Raises the <see cref="E:System.Windows.Forms.Control.MouseUp" /> event. 
    ''' </summary> 
    ''' <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param> 
    Protected Overloads Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        If isMouseDown Then
            isMouseDown = False
            Fade()
            Invalidate()
        End If
        MyBase.OnMouseUp(e)
    End Sub

    ''' <summary> 
    ''' Raises the <see cref="M:System.Windows.Forms.Control.OnMouseMove(System.Windows.Forms.MouseEventArgs)" /> event. 
    ''' </summary> 
    ''' <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param> 
    Protected Overloads Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        If e.Button <> MouseButtons.None Then
            If Not ClientRectangle.Contains(e.X, e.Y) Then
                If isHovered Then
                    isHovered = False
                    Fade()
                    Invalidate()
                End If
            ElseIf Not isHovered Then
                isHovered = True
                Fade()
                Invalidate()
            End If
        End If
    End Sub

    ''' <summary> 
    ''' Raises the <see cref="E:System.Windows.Forms.Control.MouseEnter" /> event. 
    ''' </summary> 
    ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param> 
    Protected Overloads Overrides Sub OnMouseEnter(ByVal e As EventArgs)
        isHovered = True
        Fade()
        Invalidate()
        MyBase.OnMouseEnter(e)
    End Sub

    ''' <summary> 
    ''' Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave" /> event. 
    ''' </summary> 
    ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param> 
    Protected Overloads Overrides Sub OnMouseLeave(ByVal e As EventArgs)
        isHovered = False
        Fade()
        Invalidate()
        MyBase.OnMouseLeave(e)
    End Sub

#End Region

#Region " Painting "

    ''' <summary> 
    ''' Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnPaint(System.Windows.Forms.PaintEventArgs)" /> event. 
    ''' </summary> 
    ''' <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param> 
    Protected Overloads Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        DrawButtonBackgroundFromBuffer(e.Graphics)
        DrawButtonForeground(e.Graphics)
        RaiseEvent Paint(Me, e)
    End Sub

    ''' <summary> 
    ''' Occurs when the control is redrawn. 
    ''' </summary> 
    Public Shadows Event Paint As PaintEventHandler

    Private Sub DrawButtonBackgroundFromBuffer(ByVal graphics As Graphics)
        If frames Is Nothing OrElse frames.Count = 0 Then
            CreateFrames()
        End If
        If isAnimating Then
            Dim alphaImage As New Bitmap(ClientSize.Width, ClientSize.Height)
            DrawAlphaImage(alphaImage, frames(animatingFrom), CType((animationDuration - animationProgress) / animationDuration, Single))
            DrawAlphaImage(alphaImage, frames(animatingTo), CType(animationProgress / animationDuration, Single))
            graphics.DrawImage(alphaImage, Point.Empty)
        Else
            graphics.DrawImage(frames(animatingTo), Point.Empty)
        End If
    End Sub

    Private Sub DrawAlphaImage(ByVal alphaImage As Image, ByVal image As Image, ByVal percent As Single)
        Dim newColorMatrix As Single()() = New Single(4)() {}
        newColorMatrix(0) = New Single() {1, 0, 0, 0, 0}
        newColorMatrix(1) = New Single() {0, 1, 0, 0, 0}
        newColorMatrix(2) = New Single() {0, 0, 1, 0, 0}
        newColorMatrix(3) = New Single() {0, 0, 0, percent, 0}
        newColorMatrix(4) = New Single() {0, 0, 0, 0, 1}
        Dim matrix As New System.Drawing.Imaging.ColorMatrix(newColorMatrix)
        Dim imageAttributes As New System.Drawing.Imaging.ImageAttributes()
        imageAttributes.ClearColorKey()
        imageAttributes.SetColorMatrix(matrix)
        Using gr As Graphics = Graphics.FromImage(alphaImage)
            gr.DrawImage(image, New Rectangle(0, 0, Size.Width, Size.Height), 0, 0, Size.Width, Size.Height, GraphicsUnit.Pixel, imageAttributes)
        End Using
    End Sub

    Private Function CreateBackgroundFrame(ByVal frameType As FrameType) As Image
        Dim rect As Rectangle = ClientRectangle
        If rect.Width <= 0 Then
            rect.Width = 1
        End If
        If rect.Height <= 0 Then
            rect.Height = 1
        End If
        Dim img As Image = New Bitmap(rect.Width, rect.Height)
        Using g As Graphics = Graphics.FromImage(img)
            g.Clear(Color.Transparent)
            DrawButtonBackground(g, rect, frameType)
        End Using
        Return img
    End Function

    Private Sub DrawButtonBackground(ByVal g As Graphics, ByVal rectangle As Rectangle, ByVal frameType As FrameType)
        Dim sm As SmoothingMode = g.SmoothingMode
        g.SmoothingMode = SmoothingMode.AntiAlias

        Select Case frameType
            Case CommandLink.FrameType.Disabled
                ' TODO: gray elevated
                If _showElevationIcon Then g.DrawImage(My.Resources.SmallSecurity, 10, 14) Else g.DrawImage(My.Resources.ArrowDisabled, 8, 12)

            Case CommandLink.FrameType.Pressed
                Dim rect As Rectangle = rectangle
                rect.Width -= 1
                rect.Height -= 1
                Using bw As GraphicsPath = CreateRoundRectangle(rect, 3)
                    Using p As New Pen(_pressedBorder)
                        g.DrawPath(p, bw)
                    End Using
                    Using b As New SolidBrush(_pressedBackground)
                        g.FillPath(b, bw)
                    End Using
                End Using
                If _showElevationIcon Then g.DrawImage(My.Resources.SmallSecurity, 10, 14) Else g.DrawImage(My.Resources.ArrowNormal, 8, 12)

            Case CommandLink.FrameType.Normal
                If _showElevationIcon Then g.DrawImage(My.Resources.SmallSecurity, 10, 14) Else g.DrawImage(My.Resources.ArrowNormal, 8, 12)

            Case CommandLink.FrameType.Hovered
                Dim rect As Rectangle = rectangle
                rect.Width -= 1
                rect.Height -= 1
                Using bw As GraphicsPath = CreateRoundRectangle(rect, 3)
                    Using p As New Pen(_hoveredBorder1)
                        g.DrawPath(p, bw)
                    End Using
                End Using
                rect.Inflate(-1, -1)
                Using bw As GraphicsPath = CreateRoundRectangle(rect, 2)
                    Using b As New LinearGradientBrush(rect, _hoveredBorder2, _hoveredBorder3, LinearGradientMode.Vertical)
                        Using p As New Pen(b)
                            g.DrawPath(p, bw)
                        End Using
                    End Using
                    If rect.Height < 40 Then
                        Using b As New LinearGradientBrush(rect, _hoveredBackground1, _hoveredBackground2, LinearGradientMode.Vertical)
                            g.FillPath(b, bw)
                        End Using
                    Else
                        Dim rect1 As RectangleF = rect
                        rect1.Height *= 0.75!
                        Dim rect2 As RectangleF = rect
                        rect2.Height -= rect1.Height
                        rect2.Y += rect1.Height
                        Using b As New LinearGradientBrush(rect1, _hoveredBackground1, _hoveredBackground2, LinearGradientMode.Vertical)
                            g.SetClip(rect1)
                            g.FillPath(b, bw)
                            g.ResetClip()
                        End Using
                        Using b As New LinearGradientBrush(rect2, _hoveredBackground2, _hoveredBackground3, LinearGradientMode.Vertical)
                            g.SetClip(rect2)
                            g.FillPath(b, bw)
                            g.ResetClip()
                        End Using
                    End If
                End Using
                If _showElevationIcon Then g.DrawImage(My.Resources.SmallSecurity, 10, 14) Else g.DrawImage(My.Resources.ArrowHovered, 8, 12)

            Case CommandLink.FrameType.Active1
                Dim rect As Rectangle = rectangle
                rect.Width -= 3
                rect.X += 1
                rect.Height -= 1
                Using bw As GraphicsPath = CreateRoundRectangle(rect, 3)
                    Using b As New LinearGradientBrush(rect, _activeBorderHot1, _activeBorderHot2, LinearGradientMode.Vertical)
                        Using p As New Pen(b)
                            g.DrawPath(p, bw)
                        End Using
                    End Using
                End Using
                If _showElevationIcon Then g.DrawImage(My.Resources.SmallSecurity, 10, 14) Else g.DrawImage(My.Resources.ArrowNormal, 8, 12)

            Case CommandLink.FrameType.Active2
                Dim rect As Rectangle = rectangle
                rect.Width -= 3
                rect.X += 1
                rect.Height -= 1
                Using bw As GraphicsPath = CreateRoundRectangle(rect, 3)
                    Using p As New Pen(_activeBorderCold)
                        g.DrawPath(p, bw)
                    End Using
                End Using
                If _showElevationIcon Then g.DrawImage(My.Resources.SmallSecurity, 10, 14) Else g.DrawImage(My.Resources.ArrowNormal, 8, 12)

        End Select

        g.SmoothingMode = sm
    End Sub

    Private Sub DrawButtonForeground(ByVal g As Graphics)

        If Focused AndAlso ShowFocusCues AndAlso isFocusedByKey Then
            Dim rect As Rectangle = ClientRectangle
            rect.Inflate(-4, -4)
            ControlPaint.DrawFocusRectangle(g, rect)
        End If

        Using f As New Font(Font.FontFamily, Font.SizeInPoints / 0.75!, Font.Style, GraphicsUnit.Point, Font.GdiCharSet, Font.GdiVerticalFont)
            If Not Enabled Then
                TextRenderer.DrawText(g, Text, f, New Point(28, 12), Color.Gray, Color.Transparent)
            ElseIf State = PushButtonState.Hot Then
                TextRenderer.DrawText(g, Text, f, New Point(28, 12), HotForeColor, Color.Transparent)
            Else
                TextRenderer.DrawText(g, Text, f, New Point(28, 12), ForeColor, Color.Transparent)
            End If
        End Using

    End Sub

    'Private imageButton As Button
    'Private Sub DrawForegroundFromButton(ByVal pevent As PaintEventArgs)
    '    If imageButton Is Nothing Then
    '        imageButton = New Button()
    '        imageButton.Parent = New TransparentControl()
    '        imageButton.SuspendLayout()
    '        imageButton.BackColor = Color.Transparent
    '        imageButton.FlatAppearance.BorderSize = 0
    '        imageButton.FlatStyle = FlatStyle.Flat
    '    Else
    '        imageButton.SuspendLayout()
    '    End If
    '    imageButton.AutoEllipsis = AutoEllipsis
    '    If Enabled Then
    '        imageButton.ForeColor = ForeColor
    '    Else
    '        imageButton.ForeColor = Color.FromArgb((3 * ForeColor.R + _backColor.R) >> 2, (3 * ForeColor.G + _backColor.G) >> 2, (3 * ForeColor.B + _backColor.B) >> 2)
    '    End If
    '    imageButton.Font = Font
    '    imageButton.RightToLeft = RightToLeft
    '    If imageButton.Image IsNot Image AndAlso imageButton.Image IsNot Nothing Then
    '        imageButton.Image.Dispose()
    '    End If
    '    If Image IsNot Nothing Then
    '        imageButton.Image = Image
    '        If Not Enabled Then
    '            Dim size As Size = Image.Size
    '            Dim newColorMatrix As Single()() = New Single(4)() {}
    '            newColorMatrix(0) = New Single() {0.2125F, 0.2125F, 0.2125F, 0.0F, 0.0F}
    '            newColorMatrix(1) = New Single() {0.2577F, 0.2577F, 0.2577F, 0.0F, 0.0F}
    '            newColorMatrix(2) = New Single() {0.0361F, 0.0361F, 0.0361F, 0.0F, 0.0F}
    '            Dim arr As Single() = New Single(4) {}
    '            arr(3) = 1.0F
    '            newColorMatrix(3) = arr
    '            newColorMatrix(4) = New Single() {0.38F, 0.38F, 0.38F, 0.0F, 1.0F}
    '            Dim matrix As New System.Drawing.Imaging.ColorMatrix(newColorMatrix)
    '            Dim disabledImageAttr As New System.Drawing.Imaging.ImageAttributes()
    '            disabledImageAttr.ClearColorKey()
    '            disabledImageAttr.SetColorMatrix(matrix)
    '            imageButton.Image = New Bitmap(Image.Width, Image.Height)
    '            Using gr As Graphics = Graphics.FromImage(imageButton.Image)
    '                gr.DrawImage(Image, New Rectangle(0, 0, size.Width, size.Height), 0, 0, size.Width, size.Height, _
    '                GraphicsUnit.Pixel, disabledImageAttr)
    '            End Using
    '        End If
    '    End If
    '    imageButton.ImageAlign = ImageAlign
    '    imageButton.ImageIndex = ImageIndex
    '    imageButton.ImageKey = ImageKey
    '    imageButton.ImageList = ImageList
    '    imageButton.Padding = Padding
    '    imageButton.Size = Size
    '    imageButton.Text = Text
    '    imageButton.TextAlign = TextAlign
    '    imageButton.TextImageRelation = TextImageRelation
    '    imageButton.UseCompatibleTextRendering = UseCompatibleTextRendering
    '    imageButton.UseMnemonic = UseMnemonic
    '    imageButton.ResumeLayout()
    '    InvokePaint(imageButton, pevent)
    'End Sub

    'Private Class TransparentControl
    '    Inherits Control
    '    Protected Overloads Overrides Sub OnPaintBackground(ByVal pevent As PaintEventArgs)
    '    End Sub
    '    Protected Overloads Overrides Sub OnPaint(ByVal e As PaintEventArgs)
    '    End Sub
    'End Class

    Private Shared Function CreateRoundRectangle(ByVal rectangle As Rectangle, ByVal radius As Integer) As GraphicsPath
        Dim path As New GraphicsPath()
        Dim l As Integer = rectangle.Left
        Dim t As Integer = rectangle.Top
        Dim w As Integer = rectangle.Width
        Dim h As Integer = rectangle.Height
        Dim d As Integer = radius << 1
        ' topleft 
        path.AddArc(l, t, d, d, 180, 90)
        ' top 
        path.AddLine(l + radius, t, l + w - radius, t)
        ' topright 
        path.AddArc(l + w - d, t, d, d, 270, 90)
        ' right 
        path.AddLine(l + w, t + radius, l + w, t + h - radius)
        ' bottomright 
        path.AddArc(l + w - d, t + h - d, d, d, 0, 90)
        ' bottom 
        path.AddLine(l + w - radius, t + h, l + radius, t + h)
        ' bottomleft 
        path.AddArc(l, t + h - d, d, d, 90, 90)
        ' left 
        path.AddLine(l, t + h - radius, l, t + radius)
        path.CloseFigure()
        Return path
    End Function

#End Region

#Region " Unused Properties & Events "

    ''' <summary>This property is not relevant for this class.</summary> 
    ''' <returns>This property is not relevant for this class.</returns> 
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Shadows ReadOnly Property FlatAppearance() As FlatButtonAppearance
        Get
            Return MyBase.FlatAppearance
        End Get
    End Property

    ''' <summary>This property is not relevant for this class.</summary> 
    ''' <returns>This property is not relevant for this class.</returns> 
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Shadows Property FlatStyle() As FlatStyle
        Get
            Return MyBase.FlatStyle
        End Get
        Set(ByVal value As FlatStyle)
            MyBase.FlatStyle = value
        End Set
    End Property

    ''' <summary>This property is not relevant for this class.</summary> 
    ''' <returns>This property is not relevant for this class.</returns> 
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Shadows Property UseVisualStyleBackColor() As Boolean
        Get
            Return MyBase.UseVisualStyleBackColor
        End Get
        Set(ByVal value As Boolean)
            MyBase.UseVisualStyleBackColor = value
        End Set
    End Property

#End Region

#Region " Animation Support "

    Private frames As Dictionary(Of FrameType, Image)

    Private Enum FrameType
        Disabled = 0
        Pressed = 1
        Normal = 2
        Hovered = 3
        Active1 = 4
        Active2 = 5
    End Enum

    Private Sub CreateFrames()
        DestroyFrames()
        If Not IsHandleCreated Then
            Return
        End If
        If frames Is Nothing Then
            frames = New Dictionary(Of FrameType, Image)
        End If
        frames.Add(FrameType.Disabled, CreateBackgroundFrame(FrameType.Disabled))
        frames.Add(FrameType.Pressed, CreateBackgroundFrame(FrameType.Pressed))
        frames.Add(FrameType.Normal, CreateBackgroundFrame(FrameType.Normal))
        frames.Add(FrameType.Hovered, CreateBackgroundFrame(FrameType.Hovered))
        frames.Add(FrameType.Active1, CreateBackgroundFrame(FrameType.Active1))
        frames.Add(FrameType.Active2, CreateBackgroundFrame(FrameType.Active2))
    End Sub

    Private Sub DestroyFrames()
        If frames IsNot Nothing Then
            frames(FrameType.Disabled).Dispose()
            frames(FrameType.Pressed).Dispose()
            frames(FrameType.Normal).Dispose()
            frames(FrameType.Hovered).Dispose()
            frames(FrameType.Active1).Dispose()
            frames(FrameType.Active2).Dispose()
            frames.Clear()
            frames = Nothing
        End If
    End Sub

    Private animationProgress As Integer
    Private animatingFrom As FrameType
    Private animatingTo As FrameType
    Private animationDuration As Integer

    Private ReadOnly Property isAnimating() As Boolean
        Get
            Return Timer.Enabled
        End Get
    End Property

    Private Sub Fade()
        If Not Enabled Then
            FadeTo(FrameType.Disabled, 200)
        ElseIf isPressed Then
            FadeTo(FrameType.Pressed, 200)
        ElseIf isHovered OrElse isFocusedByKey Then
            If animatingTo = FrameType.Pressed Then
                FadeTo(FrameType.Hovered, 200)
            Else
                FadeTo(FrameType.Hovered, 200)
            End If
        ElseIf isFocused Then ' OrElse IsDefault Then
            If animatingTo = FrameType.Hovered Then
                FadeTo(FrameType.Active1, 1000)
            Else
                FadeTo(FrameType.Active1, 200)
            End If
        Else
            If animatingTo = FrameType.Pressed Then
                FadeTo(FrameType.Normal, 200)
            Else
                FadeTo(FrameType.Normal, 1000)
            End If
        End If
    End Sub

    Private Sub FadeTo(ByVal frameType As FrameType, ByVal animationDuration As Integer)
        If animatingTo = frameType Then Return
        If animatingFrom = frameType Then
            animationProgress = (Me.animationDuration - animationProgress) * animationDuration \ Me.animationDuration
            Me.animationDuration = animationDuration
            animatingFrom = animatingTo
            animatingTo = frameType
        Else
            Me.animationDuration = animationDuration
            animationProgress = 0
            animatingFrom = animatingTo
            animatingTo = frameType
        End If
        Timer.Enabled = True
    End Sub

    Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer.Tick
        If Not Timer.Enabled Then
            Return
        End If
        animationProgress += Timer.Interval
        If animationProgress > animationDuration Then
            animationProgress = animationDuration
            If animatingTo = FrameType.Active1 Or animatingTo = FrameType.Active2 Then
                If animatingTo = FrameType.Active1 Then
                    animatingFrom = FrameType.Active1
                    animatingTo = FrameType.Active2
                Else
                    animatingFrom = FrameType.Active2
                    animatingTo = FrameType.Active1
                End If
                animationProgress = 0
                animationDuration = 2000
            Else
                Timer.Enabled = False
            End If
        End If
        Refresh()
    End Sub

#End Region

#Region " Methods "
    Private Shared Function [If](Of T)(ByVal condition As Boolean, ByVal truePart As T, ByVal falsePart As T) As T
        If condition Then
            Return truePart
        Else
            Return falsePart
        End If
    End Function
#End Region

End Class
