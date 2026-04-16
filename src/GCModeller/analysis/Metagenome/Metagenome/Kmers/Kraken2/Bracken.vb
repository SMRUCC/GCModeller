#Region "Microsoft.VisualBasic::d5ab73df3628fb3eb45c74815b799649, analysis\Metagenome\Metagenome\Kmers\Kraken2\Bracken.vb"

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
    '    Code Lines: 27 (72.97%)
    ' Comment Lines: 3 (8.11%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (18.92%)
    '     File Size: 1.33 KB


    '     Class Bracken
    ' 
    '         Properties: added_reads, fraction_total_reads, kraken_assigned_reads, name, new_est_reads
    '                     taxonomy_id, taxonomy_lvl, uniqueId
    ' 
    '         Function: LoadTable, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework
Imports SMRUCC.genomics.ComponentModel

Namespace Kmers.Kraken2

    ''' <summary>
    ''' the bracken abundance table
    ''' </summary>
    Public Class Bracken : Implements IExpressionValue

        Public Property name As String
        Public Property taxonomy_id As Integer
        Public Property taxonomy_lvl As String
        Public Property kraken_assigned_reads As Double
        Public Property added_reads As Double
        Public Property new_est_reads As Double
        Public Property fraction_total_reads As Double Implements IExpressionValue.ExpressionValue

        Private ReadOnly Property uniqueId As String Implements IExpressionValue.Identity
            Get
                Return $"{taxonomy_id}.{name}"
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"{name} [{taxonomy_lvl}]"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadTable(tsvfile As String) As Bracken()
            Return tsvfile.LoadCsv(Of Bracken)(mute:=True, tsv:=True)
        End Function

    End Class
End Namespace
