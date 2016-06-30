#Region "Microsoft.VisualBasic::31a1e01c1629a97fc73baa3a8b23db25, ..\Bio.Assembly\Assembly\KEGG\DBGET\Objects\SSDB\Orthology.vb"

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

Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.WebRequest
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Assembly.KEGG.DBGET.bGetObject.SSDB

    ''' <summary>
    ''' KEGG KO分类
    ''' </summary>
    Public Class Orthology : Inherits bGetObject

        Public Overrides ReadOnly Property Code As String
            Get
                Return "ko"
            End Get
        End Property

        Public Property [Module] As KeyValuePair()
        Public Property Pathway As KeyValuePair()
        Public Property Disease As KeyValuePair()
        Public Property Genes As QueryEntry()
        Public Property References As Reference()
        Public Property xRefEntry As TripleKeyValuesPair()
            Get
                Return _xRef
            End Get
            Set(value As TripleKeyValuesPair())
                _xRef = value
                If value.IsNullOrEmpty Then
                    _xRefDict = New Dictionary(Of String, TripleKeyValuesPair())
                Else
                    _xRefDict = (From x As TripleKeyValuesPair
                                 In value
                                 Select x
                                 Group x By x.Key Into Group) _
                                      .ToDictionary(Function(x) x.Key,
                                                    Function(x) x.Group.ToArray)
                End If
            End Set
        End Property
        Public Property EC As String

        Dim _xRef As TripleKeyValuesPair()
        Dim _xRefDict As Dictionary(Of String, TripleKeyValuesPair())

        Public Function GetXRef(Db As String) As TripleKeyValuesPair()
            If _xRefDict.ContainsKey(Db) Then
                Return _xRefDict(Db)
            Else
                Return Nothing
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"[{Me.Entry}]  {Name}: {Me.Definition}"
        End Function
    End Class
End Namespace
