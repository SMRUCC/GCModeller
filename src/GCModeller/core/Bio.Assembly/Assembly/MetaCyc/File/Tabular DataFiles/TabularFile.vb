#Region "Microsoft.VisualBasic::d59aee2e737e7c1bd0906e30317af744, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\Tabular DataFiles\TabularFile.vb"

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

    '   Total Lines: 77
    '    Code Lines: 50
    ' Comment Lines: 14
    '   Blank Lines: 13
    '     File Size: 2.77 KB


    '     Class TabularFile
    ' 
    '         Properties: Columns, DbProperty, Objects, Size
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging

Namespace Assembly.MetaCyc.File

    ''' <summary>
    ''' Each tabular file contains data for one class of objects, such as reactions or pathways.
    ''' This type of file contains a single table of tab-delimited columns and newline-delimited
    ''' rows. The first row contains headers which describe the data beneath them. Each of the
    ''' remaining rows represents an object, and each column is an attribute of the object.
    '''
    ''' Column names that would otherwise be the same contain a number x having values 1, 2, 3,
    ''' etc. to distinguish them. Comment lines can be anywhere in the file and must begin with
    ''' the following symbol:
    '''
    ''' #
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TabularFile

        Public Property DbProperty As [Property]
        Public Property Columns As String()
        Public Property Objects As RecordLine()

        Public ReadOnly Property Size As Size
            Get
                Return New Size With {
                    .Width = Columns.Length,
                    .Height = Objects.Length
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder(512)

            If String.IsNullOrEmpty(DbProperty.FileName) Then
                sBuilder.Append("Table Data")
            Else
                sBuilder.Append(DbProperty.FileName)
            End If

            For Each e As String In Columns
                sBuilder.Append(e)
                sBuilder.Append(", ")
            Next

            sBuilder.Remove(sBuilder.Length - 2, 2)
            sBuilder.AppendLine()
            For i As Integer = 0 To 2
                sBuilder.AppendLine(Objects(i).ToString)
            Next

            Return sBuilder.ToString
        End Function

        Public Shared Widening Operator CType(Path As String) As TabularFile
            Dim prop As [Property] = Nothing
            Dim File As String() = Nothing
            Dim Columns As String(), TabularFile As RecordLine()
            Dim first As String = ""

            Call FileReader.TabularParser(Path, prop, File, first).Assertion(MSG_TYPES.WRN)

            '第一行为列名行，以TAB作为分隔符
            Columns = Strings.Split(first, vbTab)
            TabularFile = (From line As String In File.AsParallel Select New RecordLine(line)).ToArray

            Return New TabularFile With {
                .Columns = Columns,
                .DbProperty = prop,
                .Objects = TabularFile
            }
        End Operator
    End Class
End Namespace
