#Region "Microsoft.VisualBasic::246ae30ed643efdb00395d53279313ae, localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\QuertTextProvider.vb"

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

    '     Class QuertTextProvider
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: QueryDatas
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Text

Namespace LocalBLAST.BLASTOutput.BlastPlus

    Public Class QuertTextProvider : Inherits BufferedStream

        Sub New(file As String, Optional encoding As Encodings = Encodings.Default, Optional bufSize As Integer = 64 * 1024 * 1024)
            Call MyBase.New(file, encoding.CodePage, bufSize)
        End Sub

        ''' <summary>
        ''' 這個函數所返回的是已經解析好的query文本部分
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function QueryDatas() As IEnumerable(Of String)
            Do While Not EndRead
                Dim lines$() = MyBase.BufferProvider
            Loop
        End Function
    End Class
End Namespace
