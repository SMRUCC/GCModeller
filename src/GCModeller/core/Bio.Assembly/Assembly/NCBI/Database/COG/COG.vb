#Region "Microsoft.VisualBasic::c73fba969a431bf09172a2f4c3994aa1, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\COG\COG.vb"

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

    '   Total Lines: 63
    '    Code Lines: 44
    ' Comment Lines: 11
    '   Blank Lines: 8
    '     File Size: 2.39 KB


    '     Class COGFunction
    ' 
    '         Properties: Catalog, Category
    ' 
    '         Function: __notAssigned, GetClass
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace Assembly.NCBI.COG

    ''' <summary>
    ''' COG function description data.
    ''' </summary>
    Public Class COGFunction : Inherits CatalogList
        Implements INamedValue

        ''' <summary>
        ''' COG catagory class enumeration value.
        ''' </summary>
        ''' <returns></returns>
        Public Property Category As COGCategories
        ''' <summary>
        ''' COG
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Property Catalog As String Implements INamedValue.Key

        Private Shared Function __notAssigned() As COGFunction
            Return New COGFunction With {
                .Category = COGCategories.NotAssigned,
                .Catalog = "-",
                .Description = ""
            }
        End Function

        Public Shared Function GetClass(Of T As IFeatureDigest)(source As IEnumerable(Of T), func As [Function]) As COGFunction()
            Dim hash = func.Catalogs.Select(
                Function(x) x.ToArray).IteratesALL _
                        .ToDictionary(Function(x) x.Catalog.First,
                                      Function(x) New With {
                                        .fun = x,
                                        .count = New List(Of String)})
            Dim locus = source _
                .Select(Function(x)
                            Return New With {
                                x.Key,
                                .COG = Strings.UCase([Function].__trimCOGs(x.Feature))
                            }
                        End Function) _
                .ToArray

            hash.Add("-", New With {.fun = __notAssigned(), .count = New List(Of String)})

            For Each x In locus
                For Each c As Char In x.COG
                    hash(c).count.Add(x.Key)
                Next
            Next

            Dim setValue = New SetValue(Of COGFunction)().GetSet(NameOf(COGFunction.IDs))
            Return hash.Values _
                .Select(Function(x) setValue(x.fun, x.count.ToArray)) _
                .ToArray
        End Function
    End Class
End Namespace
