#Region "Microsoft.VisualBasic::0f86987a1a0d6055124f28feea91c9f6, GCModeller\core\Bio.Assembly\ComponentModel\TabularLoader.vb"

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


    ' Code Statistics:

    '   Total Lines: 33
    '    Code Lines: 23
    ' Comment Lines: 3
    '   Blank Lines: 7
    '     File Size: 1.16 KB


    '     Class TabularLazyLoader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) __getFiles, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language.UnixBash

Namespace ComponentModel

    ''' <summary>
    ''' NCBI PTT和MetaCyc数据库所公用的多文件的数据库加载器的基本类型
    ''' </summary>
    Public MustInherit Class TabularLazyLoader

        Protected _DIR As String, _filters As String()

        Sub New(DIR As String, filter As String())
            _filters = filter
            _DIR = DIR
        End Sub

        Protected Function __getFiles(filter As String) As IEnumerable(Of String)
            Return ls - l - wildcards(filter) <= _DIR
        End Function

        Protected Function __getFiles(filters As String()) As IEnumerable(Of String)
            Return ls - l - wildcards(filters) <= _DIR
        End Function

        Public Overrides Function ToString() As String
            If Not FileIO.FileSystem.DirectoryExists(_DIR) Then
                Return MyBase.ToString
            End If
            Dim files As String() = __getFiles(_filters).ToArray
            Return String.Format("Stream://{0} {1}", _DIR, String.Join("; ", files))
        End Function
    End Class
End Namespace
