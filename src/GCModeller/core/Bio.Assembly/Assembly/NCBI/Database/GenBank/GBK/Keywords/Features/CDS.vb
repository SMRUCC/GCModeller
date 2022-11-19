#Region "Microsoft.VisualBasic::ea6ca4039787bafcfd783b5dbb86e5d5, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\Features\CDS.vb"

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

    '   Total Lines: 88
    '    Code Lines: 58
    ' Comment Lines: 16
    '   Blank Lines: 14
    '     File Size: 3.24 KB


    '     Class CDS
    ' 
    '         Properties: db_xref_GI, db_xref_GO, db_xref_InterPro, db_xref_UniprotKBSwissProt, db_xref_UniprotKBTrEMBL
    '                     DBLinks
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class tRNAAnticodon
    ' 
    '         Properties: aa, pos, seq
    ' 
    '         Function: Parse, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    ''' <summary>
    ''' 为CDS字段记录所特化的对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CDS : Inherits Feature

        Public Property db_xref_GI As String
        Public Property db_xref_GO As String()
        Public Property db_xref_InterPro As String()
        Public Property db_xref_UniprotKBSwissProt As String
        Public Property db_xref_UniprotKBTrEMBL As String

        Public Property DBLinks As MetaCyc.Schema.DBLinkManager

        ''' <summary>
        ''' The cds feature
        ''' </summary>
        ''' <param name="cds"></param>
        Sub New(cds As Feature)

            Call cds.CopyTo(Me.innerList)

            With Me
                .KeyName = cds.KeyName
                .gb = cds.gb
                .Location = cds.Location

                Dim groups = From str As String
                             In Me.QueryDuplicated("db_xref")
                             Let tokens As String() = str.Split(":"c)
                             Select Key = tokens.First, value = tokens.Last
                             Group By Key Into Group
                Dim tmp As String() = Nothing
                Dim db_xref = groups.ToDictionary(
                    Function(k) k.Key,
                    Function(a) (From o In a.Group Select o.value).ToArray)

                Call db_xref.TryGetValue("GI", tmp) : If Not tmp.IsNullOrEmpty Then db_xref_GI = tmp.First
                Call db_xref.TryGetValue("GOA", tmp) : If Not tmp.IsNullOrEmpty Then db_xref_GO = tmp
                Call db_xref.TryGetValue("UniProtKB/Swiss-Prot", tmp) : If Not tmp.IsNullOrEmpty Then db_xref_UniprotKBSwissProt = tmp.First
                Call db_xref.TryGetValue("UniProtKB/TrEMBL", tmp) : If Not tmp.IsNullOrEmpty Then db_xref_UniprotKBTrEMBL = tmp.First
                Call db_xref.TryGetValue("InterPro", db_xref_InterPro)
            End With
        End Sub
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' ```
    ''' /anticodon=(pos:complement(529822..529824),aa:Phe,seq:gaa)
    ''' ```
    ''' </remarks>
    Public Class tRNAAnticodon

        Public Property pos As NucleotideLocation
        Public Property aa As String
        Public Property seq As String

        Public Overrides Function ToString() As String
            Return $"tRNA-{aa}"
        End Function

        Public Shared Function Parse(value As String) As tRNAAnticodon
            Dim tokens As Dictionary(Of String, String) = value _
                .Trim("("c, ")"c, " "c) _
                .Split(","c) _
                .Select(Function(t) t.GetTagValue(":")) _
                .ToDictionary _
                .FlatTable
            Dim location As NucleotideLocation = NucleotideLocation.Parse(tokens.TryGetValue("pos"))
            Dim aa = tokens.TryGetValue("aa")
            Dim seq = tokens.TryGetValue("seq")

            Return New tRNAAnticodon With {
                .aa = aa,
                .seq = seq,
                .pos = location
            }
        End Function
    End Class
End Namespace
