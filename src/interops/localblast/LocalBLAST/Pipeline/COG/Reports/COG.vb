#Region "Microsoft.VisualBasic::593b303b1e4e0770ae848d8c9c6651ee, localblast\LocalBLAST\Pipeline\COG\Reports\COG.vb"

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

    '   Total Lines: 65
    '    Code Lines: 53 (81.54%)
    ' Comment Lines: 3 (4.62%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (13.85%)
    '     File Size: 2.71 KB


    '     Class MGACOG
    ' 
    '         Properties: [class], classDescrib, description, Evalue, Hit
    '                     HitEnd, HitLen, HitStart, identities, Length
    '                     QueryEnd, QueryName, QueryStart, Score
    ' 
    '         Function: LoadDoc, (+2 Overloads) ToMyvaCOG
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Namespace Pipeline.COG

    ''' <summary>
    ''' COG output data from http://weizhong-lab.ucsd.edu/metagenomic-analysis/server/
    ''' </summary>
    Public Class MGACOG
        Implements INamedValue, IFeatureDigest, IQueryHits

        <Column("#Query")>
        Public Property QueryName As String Implements IBlastHit.queryName, INamedValue.Key
        Public Property Hit As String Implements IBlastHit.hitName, IFeatureDigest.Feature
        <Column("E-value")>
        Public Property Evalue As Double
        Public Property Score As Double
        <Column("Query-start")> Public Property QueryStart As Integer
        <Column("Query-End")> Public Property QueryEnd As String
        <Column("Hit-start")> Public Property HitStart As Integer
        <Column("Hit-End")> Public Property HitEnd As Integer
        <Column("Hit-length")> Public Property HitLen As Integer

        <Column("Identity")>
        Public Property identities As Double Implements IQueryHits.identities

        Public Property description As String Implements IBlastHit.description

        Public Property [class] As String
        <Column("class description")>
        Public Property classDescrib As String

        Public Property Length As Integer

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToMyvaCOG() As MyvaCOG
            Return New MyvaCOG With {
                .Category = [class],
                .MyvaCOG = Hit,
                .COG = Hit,
                .Description = description,
                .Evalue = Evalue,
                .Identities = identities,
                .Length = HitLen,
                .LengthQuery = QueryEnd - QueryStart,
                .QueryLength = Length,
                .QueryName = QueryName
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadDoc(path As String) As MGACOG()
            Return DataImports.Imports(Of MGACOG)(path, vbTab)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ToMyvaCOG(source As IEnumerable(Of MGACOG)) As MyvaCOG()
            Return source.Select(Function(h) h.ToMyvaCOG).ToArray
        End Function
    End Class
End Namespace
