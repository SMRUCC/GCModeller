#Region "Microsoft.VisualBasic::084cedd6b07f8b5a29037bb0d2a75280, ..\Bio.Assembly\Assembly\NCBI\Database\COG\COG.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic

Namespace Assembly.NCBI.COG

    Public Class COGFunc : Inherits ClassObject
        Implements sIdEnumerable

        Public Property Category As COGCategories
        Public Property COG As String Implements sIdEnumerable.Identifier
        Public Property Func As String
        Public Property locus As String()
        Public ReadOnly Property NumOfLocus As Integer
            Get
                Return locus.Length
            End Get
        End Property

        Private Shared Function __notAssigned() As COGFunc
            Return New COGFunc With {
                .Category = COGCategories.NotAssigned,
                .COG = "-",
                .Func = ""
            }
        End Function

        Public Shared Function GetClass(Of T As ICOGDigest)(source As IEnumerable(Of T), func As [Function]) As COGFunc()
            Dim hash = func.Categories.ToArray(
                Function(x) x.ToArray).MatrixAsIterator _
                        .ToDictionary(Function(x) x.COG.First,
                                      Function(x) New With {
                                        .fun = x,
                                        .count = New List(Of String)})
            Dim locus = source.ToArray(
                Function(x) New With {
                    x.Identifier,
                     .COG = Strings.UCase([Function].__trimCOGs(x.COG))})

            hash.Add("-", New With {.fun = __notAssigned(), .count = New List(Of String)})

            For Each x In locus
                For Each c As Char In x.COG
                    hash(c).count.Add(x.Identifier)
                Next
            Next

            Dim setValue = New SetValue(Of COGFunc)().GetSet(NameOf(COGFunc.locus))
            Return hash.Values.ToArray(Function(x) setValue(x.fun, x.count.ToArray))
        End Function
    End Class
End Namespace
