#Region "Microsoft.VisualBasic::1ec474561aaa50d41dd31795cda6d353, core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Parser\ObjectModel.vb"

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

    '     Class ObjectModel
    ' 
    '         Properties: uid
    ' 
    '         Function: CreateDictionary, ModelParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles

Namespace Assembly.MetaCyc.File

    ''' <summary>
    ''' 每一个数据文件里面的每一个对象的模型
    ''' </summary>
    Public Class ObjectModel : Inherits DynamicPropertyBase(Of String())
        Implements INamedValue

        Public Property uid As String Implements INamedValue.Key

        Const UNIQUE_ID As String = "UNIQUE-ID"

        Public Shared Function ModelParser(buf As String()) As ObjectModel
            Dim lastKey As String = ""
            Dim lastValue As String = ""
            Dim list As New List(Of KeyValuePair(Of String, String))

            For Each line As String In buf
                If line.First = "/"c Then
                    lastValue = lastValue & " " & line
                Else
                    Call list.Add(lastKey, lastValue)

                    Dim pos As Integer = InStr(line, " - ")
                    lastKey = Mid(line, 1, pos).Trim
                    lastValue = Mid(line, pos + 3).Trim
                End If
            Next

            Call list.Add(lastKey, lastValue)

            Dim hash = (From x In list
                        Select x
                        Group x By x.Key Into Group) _
                             .ToDictionary(Function(x) x.Key,
                                           Function(x) x.Group.Select(Function(v) v.Value).ToArray)
            Return New ObjectModel With {
                .uid = hash.TryGetValue(UNIQUE_ID).FirstOrDefault,
                .Properties = hash
            }
        End Function

        Public Shared Function CreateDictionary(om As ObjectModel) As Slots.Object
            Return New Slots.Object(om.Properties)
        End Function
    End Class
End Namespace
