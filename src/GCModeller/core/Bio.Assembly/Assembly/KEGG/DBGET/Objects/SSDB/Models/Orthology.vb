#Region "Microsoft.VisualBasic::47694951b4da9a20c6d549f864ec645e, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\SSDB\Models\Orthology.vb"

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

    '   Total Lines: 64
    '    Code Lines: 51
    ' Comment Lines: 3
    '   Blank Lines: 10
    '     File Size: 2.06 KB


    '     Class Orthology
    ' 
    '         Properties: [modules], Code, disease, EC, genes
    '                     pathway, references, xref
    ' 
    '         Function: GetXRef, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace Assembly.KEGG.DBGET.bGetObject.SSDB

    ''' <summary>
    ''' KEGG KO分类
    ''' </summary>
    Public Class Orthology : Inherits bGetObject

        Public Overrides ReadOnly Property Code As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return "ko"
            End Get
        End Property

        Public Property [modules] As NamedValue()
        Public Property pathway As NamedValue()
        Public Property disease As NamedValue()
        Public Property genes As QueryEntry()
        Public Property references As Reference()

        Public Property EC As String

#Region "xref"
        Dim _xRef As OrthologyTerms
        Dim _xRefDict As Dictionary(Of String, [Property]())

        Public Property xref As OrthologyTerms
            Get
                Return _xRef
            End Get
            Set(value As OrthologyTerms)
                _xRef = value

                If value.Terms.IsNullOrEmpty Then
                    _xRefDict = New Dictionary(Of String, [Property]())
                Else
                    _xRefDict = (From x As [Property]
                                 In value.Terms
                                 Select x
                                 Group x By x.name Into Group) _
                                      .ToDictionary(Function(x) x.name,
                                                    Function(x) x.Group.ToArray)
                End If
            End Set
        End Property
#End Region

        Public Function GetXRef(Db As String) As [Property]()
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
