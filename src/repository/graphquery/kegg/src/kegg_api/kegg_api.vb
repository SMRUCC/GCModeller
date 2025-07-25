﻿#Region "Microsoft.VisualBasic::fed666971e519e5c36c822eeef7c1a5f, graphquery\kegg\src\kegg_api\kegg_api.vb"

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

    '   Total Lines: 110
    '    Code Lines: 93 (84.55%)
    ' Comment Lines: 6 (5.45%)
    '    - Xml Docs: 50.00%
    ' 
    '   Blank Lines: 11 (10.00%)
    '     File Size: 4.62 KB


    ' Module kegg_api
    ' 
    '     Function: [get], convertModule, convertToCompound, convertToPathway, convertToReaction
    '               listing, parseWebForm
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.Data
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' KEGG API is a REST-style Application Programming Interface to the KEGG database resource.
''' </summary>
<Package("kegg_api")>
Public Module kegg_api

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("listing")>
    Public Function listing(database As String,
                            Optional [option] As String = Nothing,
                            Optional cache As IHttpGet = Nothing) As Object

        Return New KEGGApi(proxy:=cache).List(database, [option])
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("get")>
    <RApiReturn(GetType(String))>
    Public Function [get](id As String, Optional cache As Object = Nothing, Optional env As Environment = Nothing) As Object
        If cache Is Nothing Then
            ' do nothing
        ElseIf TypeOf cache Is String Then
            ' is a directory
            Dim fsDir As String = cache
            Dim parse As IObjectBuilder = Function(html, anyType) html
            Dim web As New WebQuery(Of String)(Function(url) url, Function(url) url.MD5, parse, Function(hash) hash.Substring(1, 2), cache:=fsDir)

            cache = web
        ElseIf Not cache.GetType.ImplementInterface(Of IHttpGet) Then
            ' invalid driver
            Return Message.InCompatibleType(GetType(IHttpGet), cache.GetType, env)
        End If

        Return New KEGGApi(proxy:=cache).GetObject(id)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("parseForm")>
    Public Function parseWebForm(text As String, Optional unsafe As Boolean = False) As WebForm
        Return New WebForm(text, unsafe)
    End Function

    <ExportAPI("as.module")>
    Public Function convertModule(form As WebForm) As [Module]
        Return PathwayTextParser.ParsePathwayModule(form)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("as.pathway")>
    Public Function convertToPathway(form As WebForm) As Pathway
        Return PathwayTextParser.ParsePathway(form)
    End Function

    <ExportAPI("as.compound")>
    Public Function convertToCompound(form As WebForm) As Compound
        Return New Compound With {
            .entry = form!ENTRY.Split(" "c).First,
            .pathway = form.GetXmlTuples("PATHWAY").ToArray,
            .commonNames = form.GetValue("NAME") _
                .SafeQuery _
                .Select(Function(str)
                            Return Strings.Trim(str) _
                                .StringSplit(";\s*") _
                                .Where(Function(si) Not si.StringEmpty)
                        End Function
                ).IteratesALL _
                 .Distinct _
                 .ToArray,
            .formula = form!FORMULA,
            .exactMass = Val(form!EXACT_MASS),
            .remarks = {form!REMARK},
            .reactionId = Strings.Trim(form!REACTION).StringSplit("\s+"),
            .[Module] = form.GetXmlTuples("MODULE").ToArray,
            .enzyme = Strings.Trim(form!ENZYME).StringSplit("\s+"),
            .molWeight = Val(form!MOL_WEIGHT),
            .DbLinks = form.GetDBLinks.ToArray
        }
    End Function

    <ExportAPI("as.reaction")>
    Public Function convertToReaction(form As WebForm) As Reaction
        Return New Reaction With {
            .ID = form!ENTRY.Split(" "c).First,
            .Enzyme = form!ENZYME.StringSplit("\s+"),
            .Equation = form!EQUATION,
            .Definition = form!DEFINITION,
            .CommonNames = {form!NAME},
            .Comments = form!COMMENT,
            .[Class] = form.GetXmlTuples("RCLASS").ToArray,
            .Pathway = form.GetXmlTuples("PATHWAY").ToArray,
            .[Module] = form.GetXmlTuples("MODULE").ToArray,
            .Orthology = OrthologyTerms.FromTerms(form.GetXmlTuples("ORTHOLOGY")),
            .DBLink = form.GetDBLinks.ToArray
        }
    End Function
End Module
