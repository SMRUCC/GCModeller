#Region "Microsoft.VisualBasic::e9fa7e26c66e639f738c4a43c44e4101, localblast\LocalBLAST\Pipeline\COG\Reports\MyvaCOG.vb"

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

    '   Total Lines: 68
    '    Code Lines: 37 (54.41%)
    ' Comment Lines: 23 (33.82%)
    '    - Xml Docs: 95.65%
    ' 
    '   Blank Lines: 8 (11.76%)
    '     File Size: 2.75 KB


    '     Class MyvaCOG
    ' 
    '         Properties: Category, COG, DataAsset, Description, Evalue
    '                     Identities, Length, LengthQuery, MyvaCOG, QueryLength
    '                     QueryName
    ' 
    '         Function: CreateObject, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace Pipeline.COG

    ''' <summary>
    ''' Protein cog category using myva database.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MyvaCOG : Implements INamedValue, IFeatureDigest, IQueryHits, ICOGCatalog

        <Column("query_name")>
        Public Property QueryName As String Implements INamedValue.Key, IBlastHit.queryName
        Public Property Length As Integer
        <Column("cog_myva")> Public Property MyvaCOG As String

        ''' <summary>
        ''' COG category
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("COG_category")> Public Property Category As String Implements ICOGCatalog.Catalog
        <Column("COG")> Public Property COG As String Implements IFeatureDigest.Feature, IBlastHit.hitName, ICOGCatalog.COG
        <Column("description")> Public Property Description As String Implements IBlastHit.description

        Public Property Evalue As Double
        Public Property Identities As Double Implements IQueryHits.identities
        Public Property QueryLength As Integer
        Public Property LengthQuery As Integer

        ''' <summary>
        ''' 额外的附件数据
        ''' </summary>
        ''' <returns></returns>
        Public Property DataAsset As Dictionary(Of String, String)

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", COG, QueryName)
        End Function

        ''' <summary>
        ''' ```
        ''' query   -> <see cref="BestHit.QueryName"/>
        ''' myvaCOG -> <see cref="BestHit.HitName"/>
        ''' ```
        ''' </summary>
        ''' <param name="besthit"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function CreateObject(besthit As BestHit) As MyvaCOG
            Return New MyvaCOG With {
                .QueryName = besthit.QueryName,
                .MyvaCOG = besthit.HitName,
                .Length = besthit.query_length,
                .Evalue = besthit.evalue,
                .Identities = besthit.identities,
                .LengthQuery = besthit.length_query,
                .QueryLength = besthit.query_length
            }
        End Function
    End Class
End Namespace
