﻿#Region "Microsoft.VisualBasic::520ab7b2bf7622e9c76fc6c95e08bd4f, models\BioCyc\Models\Model.vb"

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

'   Total Lines: 37
'    Code Lines: 22 (59.46%)
' Comment Lines: 5 (13.51%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 10 (27.03%)
'     File Size: 1.06 KB


' Class Model
' 
'     Properties: citations, comment, commonName, credits, instanceNameTemplate
'                 synonyms, types, uniqueId
' 
'     Function: ToString
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

''' <summary>
''' the abstract biocyc element model
''' </summary>
Public MustInherit Class Model : Implements IReadOnlyId

    ''' <summary>
    ''' the unique reference id of current feature 
    ''' element object.
    ''' </summary>
    ''' <returns></returns>
    <AttributeField("UNIQUE-ID")>
    Public Property uniqueId As String Implements IReadOnlyId.Identity

    <AttributeField("TYPES")>
    Public Property types As String()

    <AttributeField("COMMON-NAME")>
    Public Property commonName As String

    <AttributeField("CITATIONS")>
    Public Property citations As String()

    <AttributeField("COMMENT")>
    Public Property comment As String

    <AttributeField("CREDITS")>
    Public Property credits As String()

    <AttributeField("INSTANCE-NAME-TEMPLATE")>
    Public Property instanceNameTemplate As String
    <AttributeField("SYNONYMS")>
    Public Property synonyms As String()

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return If(commonName, uniqueId)
    End Function

    Public Shared Iterator Function GetDbLinks(db_xrefs As IEnumerable(Of String)) As IEnumerable(Of DBLink)
        For Each id As String In db_xrefs.SafeQuery
            Dim tokens = Tokenizer _
                .CharsParser(id.GetStackValue("(", ")"), delimiter:=" "c) _
                .ToArray

            Yield New DBLink With {
                .DBName = tokens(0),
                .entry = tokens(1)
            }
        Next
    End Function
End Class
