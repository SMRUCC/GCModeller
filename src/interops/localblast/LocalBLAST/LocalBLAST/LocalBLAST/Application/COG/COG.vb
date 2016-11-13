#Region "Microsoft.VisualBasic::b8cd2e5341336ae09e48611c80c8ba38, ..\interops\localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\COG\COG.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace LocalBLAST.Application.RpsBLAST

    ''' <summary>
    ''' COG output data from http://weizhong-lab.ucsd.edu/metagenomic-analysis/server/
    ''' </summary>
    Public Class MGACOG
        Implements sIdEnumerable, ICOGDigest, IQueryHits

        <Column("#Query")> Public Property QueryName As String Implements IBlastHit.locusId, sIdEnumerable.Identifier
        Public Property Hit As String Implements IBlastHit.Address, ICOGDigest.COG
        <Column("E-value")> Public Property Evalue As Double
        Public Property Score As Double
        <Column("Query-start")> Public Property QueryStart As Integer
        <Column("Query-End")> Public Property QueryEnd As String
        <Column("Hit-start")> Public Property HitStart As Integer
        <Column("Hit-End")> Public Property HitEnd As Integer
        <Column("Hit-length")> Public Property HitLen As Integer

        <Column("Identity")> Public Property identities As Double Implements IQueryHits.identities

        Public Property description As String Implements ICOGDigest.Product
        Public Property [class] As String
        <Column("class description")> Public Property classDescrib As String

        Public Property Length As Integer Implements ICOGDigest.Length

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

        Public Shared Function LoadDoc(path As String) As MGACOG()
            Return DataImports.Imports(Of MGACOG)(path, vbTab)
        End Function

        Public Shared Function ToMyvaCOG(source As IEnumerable(Of MGACOG)) As MyvaCOG()
            Return source.ToArray(Function(x) x.ToMyvaCOG)
        End Function
    End Class

    ''' <summary>
    ''' Protein cog category using myva database.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MyvaCOG
        Implements sIdEnumerable, ICOGDigest, IQueryHits

        <Column("query_name")> Public Property QueryName As String Implements sIdEnumerable.Identifier, IBlastHit.locusId
        Public Property Length As Integer Implements ICOGDigest.Length
        <Column("cog_myva")> Public Property MyvaCOG As String

        ''' <summary>
        ''' COG category
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("COG_category")> Public Property Category As String
        <Column("COG")> Public Property COG As String Implements ICOGDigest.COG, IBlastHit.Address
        <Column("description")> Public Property Description As String Implements ICOGDigest.Product

        Public Property Evalue As Double
        Public Property Identities As Double Implements IQueryHits.identities
        Public Property QueryLength As Integer
        Public Property LengthQuery As Integer

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
