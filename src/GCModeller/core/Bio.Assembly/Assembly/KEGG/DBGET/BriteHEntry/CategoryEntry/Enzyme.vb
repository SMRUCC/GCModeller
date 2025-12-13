#Region "Microsoft.VisualBasic::1e83d1cbe367ad8feb17cc260dcec7ec, core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\CategoryEntry\Enzyme.vb"

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

    '   Total Lines: 67
    '    Code Lines: 49 (73.13%)
    ' Comment Lines: 8 (11.94%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (14.93%)
    '     File Size: 2.15 KB


    '     Class EnzymeEntry
    ' 
    '         Properties: ECName, fullName, geneNames, KO
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetResource, ParseEntries
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' 在这里的entry是KO编号，而非Reaction编号
    ''' </summary>
    Public Class EnzymeEntry : Inherits EnzymaticReaction

        Public ReadOnly Property KO As String
            Get
                Return Entry.Key
            End Get
        End Property

        Public ReadOnly Property geneNames As String()
            Get
                Return Entry.Value.GetTagValue(";").Name.Trim.StringSplit(",\s+")
            End Get
        End Property

        Public ReadOnly Property fullName As String
            Get
                Return Entry.Value.GetTagValue(";", trim:=True).Value
            End Get
        End Property

        Public Property ECName As String

        Sub New(base As EnzymaticReaction)
            Me.Category = base.Category
            Me.Class = base.Class
            Me.EC = base.EC
            Me.Entry = base.Entry
            Me.SubCategory = base.SubCategory

            With EC.GetTagValue(" ", trim:=True)
                EC = .Name
                ECName = .Value

                If EC.StringEmpty Then
                    EC = ECName
                    ECName = $"[EC:{EC}] n/a"
                End If
            End With
        End Sub

        Public Shared Function ParseEntries() As EnzymeEntry()
            Return EnzymaticReaction _
                .Build(GetResource.Hierarchical) _
                .Select(Function(er) New EnzymeEntry(er)) _
                .ToArray
        End Function

        ''' <summary>
        ''' 从卫星资源程序集之中加载数据库数据
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Load resource using <see cref="ResourcesSatellite"/></remarks>
        Public Shared Function GetResource() As htext
            Dim res$ = My.Resources.ko01000
            Dim htext As htext = htext.StreamParser(res)
            Return htext
        End Function
    End Class
End Namespace
