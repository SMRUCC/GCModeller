Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.MIME.Markup
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Text
Imports WkHtmlToPdf
Imports WkHtmlToPdf.Arguments

Module Program

    ''' <summary>
    ''' HTML模板
    ''' </summary>
    ReadOnly HTMLtemplate As XElement =
        <html lang="zh">
            <head>
                <meta charset="utf-8"/>
                <meta name="viewport" content="width=device-width"/>

                <style type="text/css">
                    {$style}
                </style>
            </head>
            <body class="markdown haroopad">
                {$content}
            </body>
        </html>

    Public Function Main() As Integer
        Dim args As CommandLine = App.CommandLine
        Dim in$ = If(args Is Nothing OrElse args.Tokens.IsNullOrEmpty, "", args.Tokens(Scan0))
        Dim css$ = If(args Is Nothing OrElse args.Tokens.IsNullOrEmpty, "", args.Tokens.Get(1, ""))

        If Not [in].FileExists Then
            Call Console.WriteLine("markdown2PDF <input.md> [custom.css]")
            Call Console.WriteLine()
        Else
            Dim md As String = [in].ReadAllText
            Dim html$ = New MarkDown.MarkdownHTML().Transform(md)
            ' 2018-10-24
            ' 转换好的html文本不可以保存在临时文件夹
            ' 应该保存在和原始的markdown文档相同的位置，否则markdown文档之中的图片之类的使用相对路径的
            ' 文件会无法找到
            Dim file$ = $"{[in].TrimSuffix}.html"

            If css.FileExists Then
                css = css.ReadAllText
            Else
                css = My.Resources.haroopad
            End If

            With New ScriptBuilder(HTMLtemplate)
                !style = css
                !content = html

                Call .Save(file, UTF8)
            End With

            PdfConvert.ConvertHtmlToPdf(
                document:=PDFContent.DefaultPDFStyle(file, True),
                output:=New PdfOutput With {
                    .OutputFilePath = [in].TrimSuffix & ".pdf"
                }
            )
            Call file.DeleteFile()
        End If

        Return 0
    End Function

    ReadOnly demo As XElement =
        <html>
            <head>
                <meta charset="utf-8"/>
                <meta name="viewport" content="width=device-width"/>

                <title>Hello World!</title>

                <style type="text/css">
                    {$style}
                </style>
            </head>
            <body class="markdown haroopad">
                <h1>Hello World!!!</h1>
                <hr/>
                <h2>Example code</h2>
                <pre class="pre">
                    <code class="code">
Public Function Main() As Integer
    Call println("Hello world!")
    Return 0
End Function</code></pre>

                <h4>Another header</h4>
                <table>
                    <thead>
                        <tr>
                            <th>1</th>
                            <th>2</th>
                            <th>3</th>
                        </tr>
                    </thead>
                    <tr>
                        <td>a</td>
                        <td>b</td>
                        <td>c</td>
                    </tr>
                </table>
                <footer style="position:fixed; font-size:.8em; text-align:right; bottom:0px; margin-left:-25px; height:20px; width:100%;">
            Here is the PDF document footer.</footer>
            </body>
        </html>

    Public Sub HelloWorld()
        Dim demo = New ScriptBuilder(Program.demo).Replace("{$style}", My.Resources.haroopad)
        Dim html As New PdfDocument With {
            .Html = demo.ToString
        }

        Call println(html.GetDocument)
        Call PdfConvert.ConvertHtmlToPdf(
            html,
            App.HOME & "/hello-world.pdf")
    End Sub
End Module
