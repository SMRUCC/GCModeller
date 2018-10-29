Imports System.Runtime.CompilerServices

Namespace HTML

    Public Module BuilderHelpers

        <Extension>
        Public Function Show(report As HTMLReport, sectionBegin$, sectionEnd$) As HTMLReport
            Call report.Replace(sectionBegin, "")
            Call report.Replace(sectionEnd, "")

            Return report
        End Function

        ''' <summary>
        ''' 将目标区域注释掉，注意如果这个区域内还存在其他的html注释标签，则当前的html注释操作将会失败
        ''' </summary>
        ''' <param name="report"></param>
        ''' <param name="sectionBegin$"></param>
        ''' <param name="sectionEnd$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Hide(report As HTMLReport, sectionBegin$, sectionEnd$) As HTMLReport
            Call report.Replace(sectionBegin, "<!--")
            Call report.Replace(sectionEnd, "-->")

            Return report
        End Function
    End Module
End Namespace