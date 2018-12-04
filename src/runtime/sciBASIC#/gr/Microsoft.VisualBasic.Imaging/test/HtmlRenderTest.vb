Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.Markup.HTML

Public Module HtmlRenderTest

    ReadOnly testHTML$ =
        (<div style='font-style: normal; font-size: 14; font-family: Microsoft YaHei;' attr2="99999999 + dd">
             <span style="color:red;">Hello</span><span style="color:blue;">world!</span> 
            2<sup>333333</sup> + X<sub>i</sub> = <span style="font-size: 36;">6666666</span>
         </div>).ToString

    Sub Main()

        Dim content = TextAPI.TryParse(testHTML).ToArray

        Using g As IGraphics = New Size(500, 500).CreateGDIDevice

        End Using

        Pause()
    End Sub
End Module
