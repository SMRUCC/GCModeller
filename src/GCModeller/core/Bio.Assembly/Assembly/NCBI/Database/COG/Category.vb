#Region "Microsoft.VisualBasic::eabedecf65be76e21c78bea55aed6b4a, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\COG\Category.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Assembly.NCBI.COG

    Public Class Category

        <XmlAttribute> Public Property [Class] As COGCategories
        <XmlAttribute> Public Property Description As String
        <XmlElement> Public Property SubClasses As KeyValuePair()
            Get
                Return __innerArray
            End Get
            Set(value As KeyValuePair())
                __innerArray = value
                If __innerArray Is Nothing Then
                    __innerArray = New KeyValuePair() {}
                End If

                Dim Full = __innerArray.Join(__innerArray.ToArray(
                                             Function(obj) New KeyValuePair With {
                                                 .Key = obj.Key.ToLower,
                                                 .Value = obj.Value}))
                __innerHash = New SortedDictionary(Of Char, String)(
                    Full.ToDictionary(Function(obj) obj.Key(Scan0),
                                      Function(obj) obj.Value))
            End Set
        End Property

        Protected Friend __innerHash As SortedDictionary(Of Char, String)
        Protected Friend __innerArray As KeyValuePair()

        Public Overrides Function ToString() As String
            Return Description
        End Function

        Public Function ToArray() As COGFunc()
            Return SubClasses.ToArray(
                Function(x) New COGFunc With {
                    .Category = [Class],
                    .COG = x.Key,
                    .Func = x.Value
            })
        End Function

        Public Function GetDescription(COG As Char, ByRef description As String) As Boolean
            If __innerHash.ContainsKey(COG) Then
                description = __innerHash(COG)
                Return True
            Else
                description = ""
                Return False
            End If
        End Function
    End Class
End Namespace
