#Region "Microsoft.VisualBasic::6fd72c4f3e5bd2174414f4261c039dd1, GCModeller\data\RegulonDatabase\Regprecise\BBHH.vb"

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

    '   Total Lines: 48
    '    Code Lines: 28
    ' Comment Lines: 14
    '   Blank Lines: 6
    '     File Size: 1.86 KB


    '     Class bbhMappings
    ' 
    '         Properties: definition, Family, hit_name, Identities, Positive
    '                     query_name, vimssId
    ' 
    '         Function: GetQueryMaps, ToString
    ' 
    '     Class FamilyHit
    ' 
    '         Properties: Family, HitName, QueryName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Namespace Regprecise

    ''' <summary>
    ''' bbh mappings of the regulators between the RegPrecise database and annotated genome.
    ''' </summary>
    Public Class bbhMappings : Implements IQueryHits

        Public Property Identities As Double Implements IQueryHits.identities
        Public Property Positive As Double
        ''' <summary>
        ''' 在Regprecise数据库之中的进行注释的源
        ''' </summary>
        ''' <returns></returns>
        Public Property query_name As String Implements IBlastHit.queryName
        ''' <summary>
        ''' 在所需要进行注释的基因组之中的蛋白质基因号
        ''' </summary>
        ''' <returns></returns>
        Public Property hit_name As String Implements IBlastHit.hitName
        Public Property vimssId As Integer
        Public Property Family As String
        Public Property definition As String

        Public Overrides Function ToString() As String
            Return $"{query_name}  --> {hit_name}"
        End Function

        Public Shared Function GetQueryMaps(source As IEnumerable(Of bbhMappings), hit As String) As bbhMappings
            Dim LQuery = (From x As bbhMappings In source
                          Let hitId As String = x.query_name.Split(":"c).Last
                          Where String.Equals(hit, hitId)
                          Select x).FirstOrDefault
            Return LQuery
        End Function
    End Class

    ''' <summary>
    ''' annotated TF hits on Family
    ''' </summary>
    Public Class FamilyHit
        Public Property QueryName As String
        Public Property Family As String
        Public Property HitName As String
    End Class
End Namespace
