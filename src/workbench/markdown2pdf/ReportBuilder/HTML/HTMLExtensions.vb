#Region "Microsoft.VisualBasic::36a8ec5fc389eb7114bb43e802984976, markdown2pdf\ReportBuilder\HTML\HTMLExtensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    ' Module HTMLExtensions
    ' 
    '     Properties: BackPreviousPage, Error404
    ' 
    '     Function: GetHTML, (+3 Overloads) SaveAsHTML, ToHTML
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.DATA
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' 可能会出现中文字符，所以html文档必须要以Utf8编码进行保存
''' </summary>
''' 
<Package("GCModeller.HTML.ReportBuiulder", Publisher:="amethyst.asuka@gcmodeller.org")>
Public Module HTMLExtensions

    Public Const BR As String = "<br />"

    Const CSS As String = "<!-- <style>{CSS}</style> -->"

    <Extension>
    Public Function ToHTML(Of T As Class)(source As IEnumerable(Of T), Optional title As String = "", Optional describ As String = "") As String
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
    Public Function SaveAsHTML(Of T As Class)(source As IEnumerable(Of T), saveHTML As String, Optional title As String = "", Optional describ As String = "") As Boolean
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
