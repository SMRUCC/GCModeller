#Region "Microsoft.VisualBasic::f19222889e6aaa064788b6b124e0fc05, localblast\LocalBLAST\LocalBLAST\BlastOutput\Views.vb"

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

    '     Class Overview
    ' 
    '         Properties: Queries
    ' 
    '         Function: ExportAllBestHist, ExportBestHit, ExportParalogs, GetExcelData, LoadExcel
    ' 
    '     Structure Query
    ' 
    '         Properties: Hits, Id
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace LocalBLAST.BLASTOutput.Views

    ''' <summary>
    ''' 方便程序调试的一个对象数据结构
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Overview

        <XmlElement> Public Property Queries As Query()

        Public Function ExportParalogs() As BestHit()
            Dim bh = Me.ExportAllBestHist() '符合最佳条件，但是不是自身的记录都是旁系同源

            bh = LinqAPI.Exec(Of BestHit) <=
 _
                From besthit As BestHit
                In bh
                Where Not String.Equals(
                    besthit.QueryName,
                    besthit.HitName,
                    StringComparison.OrdinalIgnoreCase)
                Select besthit

            Return bh
        End Function

        Public Function GetExcelData() As BestHit()
            Dim LQuery As BestHit() = LinqAPI.Exec(Of BestHit) <=
 _
                From query As Query
                In Queries
                Select query.Hits

            Return LQuery
        End Function

        Public Shared Function LoadExcel(path As String) As Overview
            Dim Excel = path.LoadCsv(Of BestHit)(False)
            Dim LQuery = From besthit As BestHit
                         In Excel
                         Select besthit
                         Group By besthit.QueryName Into Group

            Dim lstQuery As Query() =
                LinqAPI.Exec(Of Query) <=
 _
                From queryEntry
                In LQuery
                Let queryData As Query = New Query With {
                    .Id = queryEntry.QueryName,
                    .Hits = queryEntry.Group.ToArray
                }
                Select queryData

            Return New Overview With {
                .Queries = lstQuery
            }
        End Function

        Public Function ExportAllBestHist(Optional identities As Double = 0.15) As BestHit()
            Dim LQuery = LinqAPI.Exec(Of BestHit) <=
 _
                From besthit As BestHit
                In GetExcelData()
                Where besthit.IsMatchedBesthit(identities)
                Select besthit

            Return LQuery
        End Function

        Public Function ExportBestHit(Optional identities As Double = 0.15) As BestHit()
            Dim LQuery = LinqAPI.Exec(Of BestHit) <=
 _
                From queryEntry As Query
                In Queries.AsParallel
                Let besthit As BestHit = (From hit As BestHit
                                          In queryEntry.Hits
                                          Where hit.IsMatchedBesthit(identities)
                                          Select hit).FirstOrDefault
                Select If(besthit Is Nothing,
                    New BestHit With {
                        .QueryName = queryEntry.Id,
                        .HitName = IBlastOutput.HITS_NOT_FOUND
                    }, besthit)

            Return LQuery
        End Function
    End Class

    Public Structure Query : Implements INamedValue
        <XmlAttribute> Public Property Id As String Implements INamedValue.Key
        <XmlElement> Public Property Hits As BestHit()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
