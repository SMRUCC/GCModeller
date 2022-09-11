#Region "Microsoft.VisualBasic::3fac6f9a31dda58a9b160d22a78e6c13, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\CategoryEntry\ReactionClass.vb"

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

    '   Total Lines: 83
    '    Code Lines: 53
    ' Comment Lines: 20
    '   Blank Lines: 10
    '     File Size: 3.02 KB


    '     Class ReactionClass
    ' 
    '         Properties: [class], category, ECNumber, RCNumber, subclass
    ' 
    '         Function: GetPathComponents, LoadFromResource
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Public Class ReactionClass : Implements INamedValue

        ''' <summary>
        ''' A
        ''' </summary>
        ''' <returns></returns>
        Public Property [class] As String
        ''' <summary>
        ''' B
        ''' </summary>
        ''' <returns></returns>
        Public Property subclass As String
        ''' <summary>
        ''' C
        ''' </summary>
        ''' <returns></returns>
        Public Property category As String
        ''' <summary>
        ''' D
        ''' </summary>
        ''' <returns></returns>
        Public Property ECNumber As String

        ''' <summary>
        ''' E: The KEGG RC number
        ''' </summary>
        ''' <returns></returns>
        Public Property RCNumber As String Implements IKeyedEntity(Of String).Key

        Public Shared Iterator Function LoadFromResource() As IEnumerable(Of ReactionClass)
            Dim htext As htext = htext.br08204

            For Each [class] As BriteHText In htext.Hierarchical.categoryItems
                For Each subclass As BriteHText In [class].categoryItems
                    For Each category As BriteHText In subclass.categoryItems
                        For Each ECNumber As BriteHText In category.categoryItems
                            For Each entry As BriteHText In ECNumber.categoryItems.SafeQuery
                                Yield New ReactionClass With {
                                    .category = category.classLabel,
                                    .[class] = [class].classLabel,
                                    .ECNumber = ECNumber.classLabel,
                                    .RCNumber = entry.entryID,
                                    .subclass = subclass.classLabel
                                }
                            Next
                        Next
                    Next
                Next
            Next
        End Function

        Friend Function GetPathComponents() As String
            Dim tokens As New List(Of String)

            If [class].Length > 64 Then
                tokens += Mid([class], 1, 61).NormalizePathString & "~"
            Else
                tokens += [class]
            End If
            If subclass.Length > 64 Then
                tokens += Mid(subclass, 1, 61).NormalizePathString & "~"
            Else
                tokens += subclass
            End If
            If category.Length > 64 Then
                tokens += Mid(category, 1, 61).NormalizePathString & "~"
            Else
                tokens += category
            End If

            tokens += ECNumber

            Return tokens.JoinBy("/")
        End Function
    End Class
End Namespace
