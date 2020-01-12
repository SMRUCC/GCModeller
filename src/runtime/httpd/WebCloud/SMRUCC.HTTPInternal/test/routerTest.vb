Imports System.ComponentModel
Imports SMRUCC.WebCloud.HTTPInternal.Attributes

Module routerTest

    Public Enum AppRouter
        <Description("/download.vbs")> download
        <Description("/write.vbs")> upload
        <Description("/read.vbs")> read
        <Description("/proxy/text.vbs")> text
        <Description("/proxy/image.vbs")> image
    End Enum

    <RunApp(AppRouter.text)>
    Public Function text() As Boolean

    End Function
End Module
