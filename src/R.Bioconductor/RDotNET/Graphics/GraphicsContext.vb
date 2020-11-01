Imports RDotNet.Graphics.Internals
Imports System
Imports System.Runtime.InteropServices

Namespace Graphics
    Public Class GraphicsContext
        Inherits SafeHandle

        Friend Sub New(ByVal pointer As IntPtr)
            MyBase.New(IntPtr.Zero, True)
            SetHandle(pointer)
        End Sub

        Public Overrides ReadOnly Property IsInvalid As Boolean
            Get
                Return handle = IntPtr.Zero
            End Get
        End Property

        Public ReadOnly Property Foreground As Color
            Get
                Return GetContext().col
            End Get
        End Property

        Public ReadOnly Property Background As Color
            Get
                Return GetContext().fill
            End Get
        End Property

        Public ReadOnly Property Gamma As Double
            Get
                Return GetContext().gamma
            End Get
        End Property

        Public ReadOnly Property LineWidth As Double
            Get
                Return GetContext().lwd
            End Get
        End Property

        Public ReadOnly Property LineType As LineType
            Get
                Return GetContext().lty
            End Get
        End Property

        Public ReadOnly Property LineEnd As LineEnd
            Get
                Return GetContext().lend
            End Get
        End Property

        Public ReadOnly Property LineJoin As LineJoin
            Get
                Return GetContext().ljoin
            End Get
        End Property

        Public ReadOnly Property LineMitre As Double
            Get
                Return GetContext().lmitre
            End Get
        End Property

        Public ReadOnly Property CharacterExpansion As Double
            Get
                Return GetContext().cex
            End Get
        End Property

        Public ReadOnly Property FontSizeInPoints As Double
            Get
                Return GetContext().ps
            End Get
        End Property

        Public ReadOnly Property LineHeight As Double
            Get
                Return GetContext().lineheight
            End Get
        End Property

        Public ReadOnly Property FontFace As FontFace
            Get
                Return GetContext().fontface
            End Get
        End Property

        Public ReadOnly Property FontFamily As String
            Get
                Return GetContext().fontfamily
            End Get
        End Property

        Protected Overrides Function ReleaseHandle() As Boolean
            Return True
        End Function

        Private Function GetContext() As GEcontext
            Return Marshal.PtrToStructure(handle, GetType(GEcontext))
        End Function
    End Class
End Namespace
