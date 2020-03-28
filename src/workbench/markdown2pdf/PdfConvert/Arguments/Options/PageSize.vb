#Region "Microsoft.VisualBasic::702098be2346ee5856c8ae9df55400df, markdown2pdf\PdfConvert\Arguments\Options\PageSize.vb"

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

    '     Enum QPrinter
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class PageSize
    ' 
    '         Properties: pageheight, pagesize, pagewidth
    ' 
    '         Function: ParsePageSize, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.Values

Namespace Arguments

    ''' <summary>
    ''' The default page size of the rendered document is A4, but using this
    ''' --page-size optionthis can be changed to almost anything else, such as A3,
    ''' Letter And Legal.  For a full list of supported pages sizes please see
    ''' &lt;http: //qt-project.org/doc/qt-4.8/qprinter.html#PaperSize-enum>.
    '''
    ''' For a more fine grained control over the page size the --page-height And
    ''' --page-width options may be used
    ''' 
    ''' This enum type specifies what paper size QPrinter should use. QPrinter does 
    ''' not check that the paper size is available; it just uses this information, 
    ''' together with QPrinter::Orientation and QPrinter::setFullPage(), to determine 
    ''' the printable area.
    '''
    ''' The defined sizes (With setFullPage(True)) are
    ''' </summary>
    Public Enum QPrinter As Byte
        A0 = 5        ' 841 x 1189 mm
        A1 = 6        ' 594 x 841 mm
        A2 = 7        ' 420 x 594 mm
        A3 = 8        ' 297 x 420 mm
        A4 = 0        ' 210 x 297 mm, 8.26 x 11.69 inches
        A5 = 9        ' 148 x 210 mm
        A6 = 10       ' 105 x 148 mm
        A7 = 11       ' 74 x 105 mm
        A8 = 12       ' 52 x 74 mm
        A9 = 13       ' 37 x 52 mm
        B0 = 14       ' 1000 x 1414 mm
        B1 = 15       ' 707 x 1000 mm
        B2 = 17       ' 500 x 707 mm
        B3 = 18       ' 353 x 500 mm
        B4 = 19       ' 250 x 353 mm
        B5 = 1        ' 176 x 250 mm, 6.93 x 9.84 inches
        B6 = 20       ' 125 x 176 mm
        B7 = 21       ' 88 x 125 mm
        B8 = 22       ' 62 x 88 mm
        B9 = 23       ' 33 x 62 mm
        B10 = 16      ' 31 x 44 mm
        C5E = 24      ' 163 x 229 mm
        Comm10E = 25  ' 105 x 241 mm, U.S. Common 10 Envelope
        DLE = 26      ' 110 x 220 mm
        Executive = 4 ' 7.5 x 10 inches, 190.5 x 254 mm
        Folio = 27    ' 210 x 330 mm
        Ledger = 28   ' 431.8 x 279.4 mm
        Legal = 3     ' 8.5 x 14 inches, 215.9 x 355.6 mm
        Letter = 2    ' 8.5 x 11 inches, 215.9 x 279.4 mm
        Tabloid = 29  ' 279.4 x 431.8 mm
        Custom = 30   ' Unknown, Or a user defined size.
    End Enum

    ''' <summary>
    ''' The default page size of the rendered document is A4, but using this
    ''' --page-size optionthis can be changed to almost anything else, such as A3,
    ''' Letter And Legal.  For a full list of supported pages sizes please see
    ''' &lt;http: //qt-project.org/doc/qt-4.8/qprinter.html#PaperSize-enum>.
    '''
    ''' For a more fine grained control over the page size the --page-height And
    ''' --page-width options may be used
    ''' </summary>
    Public Class PageSize

        ''' <summary>
        ''' 如果这个参数为<see cref="QPrinter.Custom"/>，则还需要指定width和height
        ''' </summary>
        ''' <returns></returns>
        <Argv("--page-size", CLITypes.String)>
        Public Property pagesize As QPrinter = QPrinter.A4

        <Argv("--page-width", CLITypes.Double)>
        Public Property pagewidth As Double

        <Argv("--page-height", CLITypes.Double)>
        Public Property pageheight As Double

        ''' <summary>
        ''' Config page size from commandline
        ''' </summary>
        ''' <param name="pdf_size"></param>
        ''' <returns></returns>
        Public Shared Function ParsePageSize(pdf_size As Value(Of String), Optional defaultSize As QPrinter = QPrinter.A4) As QPrinter
            Static toEnums As Dictionary(Of String, QPrinter) = Enums(Of QPrinter).ToDictionary(Function(size) size.ToString.ToLower)

            If toEnums.ContainsKey(pdf_size = pdf_size.ToLower) Then
                Return toEnums(pdf_size)
            Else
                Return defaultSize
            End If
        End Function

        Public Overrides Function ToString() As String
            If pagesize = QPrinter.Custom Then
                Return $"--page-size ""{pagesize.ToString}"" --page-width {pagewidth} --page-height {pageheight}"
            Else
                Return $"--page-size ""{pagesize.ToString}"""
            End If
        End Function
    End Class
End Namespace
