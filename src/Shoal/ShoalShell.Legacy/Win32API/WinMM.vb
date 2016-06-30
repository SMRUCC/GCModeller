Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports QuartzTypeLib
Imports Microsoft.VisualBasic.Scripting.EntryPointMetaData

<[Namespace]("winmm.dll")>
Public Module WinMM

    ''' <summary>
    ''' DirectShow组件的抽象接口，整个播放器的核心部件
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure DirectShow : Implements IDisposable

        Dim MediaControl As IMediaControl
        Dim MediaPosition As IMediaPosition
        Dim BasicAudio As IBasicAudio

        Public Function RenderFile(spath As String) As Boolean
            MediaControl = New FilgraphManager
            MediaControl.RenderFile(spath)
            BasicAudio = MediaControl
            MediaPosition = MediaControl
            Return True
        End Function

        Public Sub Dispose() Implements System.IDisposable.Dispose
            On Error Resume Next

            MediaControl.Stop()
            MediaPosition = Nothing
            BasicAudio = Nothing
            MediaControl = Nothing
        End Sub

        Public Function IsNull() As Boolean
            Return (BasicAudio Is Nothing OrElse MediaControl Is Nothing OrElse MediaPosition Is Nothing)
        End Function

        Public Sub [Stop]()
            On Error Resume Next
            MediaControl.Stop()
        End Sub

        Public Sub Pause()
            On Error Resume Next
            MediaControl.Pause()
        End Sub

        Public Sub Play()
            On Error Resume Next
            MediaControl.Run()
        End Sub

        Public ReadOnly Property State As Long
            Get
                Dim TimeOut As Long, s As Long = 0
                MediaControl.GetState(TimeOut, s)
                Return s
            End Get
        End Property
    End Structure

    <ImportsConstant> Public Const SND_APPLICATION = &H80 ' look for application specific association
    <ImportsConstant> Public Const SND_ALIAS = &H10000 ' name is a WIN.INI [sounds] entry
    <ImportsConstant> Public Const SND_ALIAS_ID = &H110000 ' name is a WIN.INI [sounds] entry identifier
    <ImportsConstant> Public Const SND_ASYNC = &H1 ' play asynchronously
    <ImportsConstant> Public Const SND_FILENAME = &H20000 ' name is a file name
    <ImportsConstant> Public Const SND_LOOP = &H8 ' loop the sound until next sndPlaySound
    <ImportsConstant> Public Const SND_MEMORY = &H4 ' lpszSoundName points to a memory file
    <ImportsConstant> Public Const SND_NODEFAULT = &H2 ' silence not default, if sound not found
    <ImportsConstant> Public Const SND_NOSTOP = &H10 ' don't stop any currently playing sound
    <ImportsConstant> Public Const SND_NOWAIT = &H2000 ' don't wait if the driver is busy
    <ImportsConstant> Public Const SND_PURGE = &H40 ' purge non-static events for task
    <ImportsConstant> Public Const SND_RESOURCE = &H40004 ' name is a resource name or atom
    <ImportsConstant> Public Const SND_SYNC = &H0 ' play synchronously (default)

    <Command("PlaySoundA")>
    Public Declare Function PlaySound Lib "winmm.dll" Alias "PlaySoundA" (lpszName As String, hModule As Integer, dwFlags As Integer) As Integer

    <Command("Invoke.DirectShow")>
    Public Function InvokeDirectShow(<ParameterAlias("media.url", "The file path of the media file on your file system.")> filename As String) As Double
        Dim Device As WinMM.DirectShow = New DirectShow
        Call Device.RenderFile(filename)
        Call Device.Play()
        Return 0
    End Function
End Module
