#Region "Microsoft.VisualBasic::05fdfe8095e7d58fd2d604ef5130a473, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\CDD\Pn.vb"

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

    '   Total Lines: 58
    '    Code Lines: 39
    ' Comment Lines: 10
    '   Blank Lines: 9
    '     File Size: 1.91 KB


    '     Class Pn
    ' 
    '         Function: GetEnumerator, GetEnumerator1, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.NCBI.CDD

    Public Class Pn : Implements Generic.IEnumerable(Of String)

        ''' <summary>
        ''' The file list of the sub database in this CDD database.
        ''' (这个数据库子库的文件列表)
        ''' </summary>
        ''' <remarks></remarks>
        Dim FileList As String()
        ''' <summary>
        ''' The file path of this pn file.
        ''' (这个pn文件的文件路径)
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend FilePath As String
        Protected Friend DIR As String

        Default Public Property File(Index As Integer) As String
            Get
                Return FileList(Index)
            End Get
            Set(value As String)
                FileList(Index) = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return FilePath
        End Function

        Public Shared Narrowing Operator CType(pn As Pn) As String()
            Return pn.FileList
        End Operator

        Public Shared Narrowing Operator CType(pn As Pn) As String
            Return pn.FilePath
        End Operator

        Public Shared Widening Operator CType(File As String) As Pn
            Return New Pn With {
                .FileList = IO.File.ReadAllLines(File),
                .FilePath = File,
                .DIR = FileIO.FileSystem.GetParentPath(File)
            }
        End Operator

        Public Iterator Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            For i As Integer = 0 To FileList.Length - 1
                Yield DIR & "/" & FileList(i)
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
