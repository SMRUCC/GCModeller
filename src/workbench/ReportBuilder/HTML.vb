Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' 可能会出现中文字符，所以html文档必须要以Utf8编码进行保存
''' </summary>
''' 
<PackageNamespace("GCModeller.HTML.ReportBuiulder", Publisher:="amethyst.asuka@gcmodeller.org")>
Public Module HTML

    Public Const BR As String = "<br />"

    Const CSS As String = "<!-- <style>{CSS}</style> -->"

    <Extension>
    Public Function ToHTML(Of T As Class)(source As Generic.IEnumerable(Of T), Optional title As String = "", Optional describ As String = "") As String
        Dim table As String = source.ToHTMLTable
        Dim html As New StringBuilder(My.Resources.index)

        If String.IsNullOrEmpty(title) Then title = GetType(T).Name

        Call html.Replace(CSS, $"<style>{My.Resources.foundation}</style>")
        Call html.Replace("{Title}", title)

        Dim innerDoc As New StringBuilder($"<p>{describ}</p>")
        Call innerDoc.AppendLine(table)
        Call html.Replace("{doc}", innerDoc.ToString)

        Return html.ToString
    End Function

    <Extension>
    Public Function SaveAsHTML(Of T As Class)(source As Generic.IEnumerable(Of T), saveHTML As String, Optional title As String = "", Optional describ As String = "") As Boolean
        Dim table As String = source.ToHTMLTable
        Dim innerDoc As New StringBuilder($"<p>{describ}</p>")

        If String.IsNullOrEmpty(title) Then title = GetType(T).Name

        Call innerDoc.AppendLine(table)
        Return innerDoc.ToString.SaveAsHTML(saveHTML, title, describ)
    End Function

    <Extension, ExportAPI("Save.HTML")>
    Public Function SaveAsHTML(innerDoc As String, saveHTML As String, Optional title As String = "", Optional describ As String = "") As Boolean
        Dim outDIR As String = FileIO.FileSystem.GetParentPath(saveHTML)
        Call My.Resources.foundation.SaveTo($"{outDIR}/foundation.css")
        Return GetHTML(innerDoc, title).SaveTo(saveHTML, System.Text.Encoding.UTF8)
    End Function

    Public Function GetHTML(doc As String, Optional title As String = "") As String
        Dim html As New StringBuilder(My.Resources.index)

        Call html.Replace("{Title}", title)
        Call html.Replace("{doc}", doc)

        Return html.ToString
    End Function

    <Extension, ExportAPI("Save.HTML")>
    Public Function SaveAsHTML(innerDoc As StringBuilder, saveHTML As String, Optional title As String = "", Optional describ As String = "") As Boolean
        Return innerDoc.ToString.SaveAsHTML(saveHTML, title, describ)
    End Function

    Public ReadOnly Property BackPreviousPage As String = "<script type=""text/javascript"">
              function goBack()
  {
  window.history.back()
  }
        </script><p>
<h6><a href=""javascript:goBack()"">Back to previous page.</a></h6>
</p>"
    Public ReadOnly Property Error404 As String = GetHTML("<p>Oops! Something bad just happened......  <font size=""4""><strong>:-(</strong></font><br />
 </p><p>%EXCEPTION%</p>" & BackPreviousPage, "GCModeller Server ERROR")
End Module
