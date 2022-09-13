#Region "Microsoft.VisualBasic::60fcdee20670f553e0756fcf162944c7, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\Schemas\Reflection\PathRoute.vb"

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

    '   Total Lines: 64
    '    Code Lines: 39
    ' Comment Lines: 18
    '   Blank Lines: 7
    '     File Size: 3.73 KB


    '     Class PathRoute
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: GetPath, ToString
    ' 
    '         Sub: InitializeSchema
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.MetaCyc.Schema.Reflection

    ''' <summary>
    ''' 寻找MetaCyc数据库之中的任意两个对象之间的连接关系
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PathRoute

        Dim Database As MetaCyc.File.FileSystem.DatabaseLoadder
        Dim TableSchemas As TableSchema()

        ''' <summary>
        ''' MetaCyc数据库的数据文件夹
        ''' </summary>
        ''' <param name="MetaCycDir"></param>
        ''' <remarks></remarks>
        Sub New(MetaCycDir As String)
            Me.Database = MetaCyc.File.FileSystem.DatabaseLoadder.CreateInstance(MetaCycDir)
            Call InitializeSchema()
        End Sub

        Sub New(MetaCyc As MetaCyc.File.FileSystem.DatabaseLoadder)
            Me.Database = MetaCyc
            Call InitializeSchema()
        End Sub

        Private Sub InitializeSchema()
            Me.TableSchemas = New TableSchema() {
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.BindReaction), File.DataFiles.Slots.Object.Tables.bindrxns),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Compound), File.DataFiles.Slots.Object.Tables.compounds),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.DNABindSite), File.DataFiles.Slots.Object.Tables.dnabindsites),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Enzrxn), File.DataFiles.Slots.Object.Tables.enzrxns),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Gene), File.DataFiles.Slots.Object.Tables.genes),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Pathway), File.DataFiles.Slots.Object.Tables.pathways),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Promoter), File.DataFiles.Slots.Object.Tables.promoters),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.ProteinFeature), File.DataFiles.Slots.Object.Tables.proteinfeatures),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Protein), File.DataFiles.Slots.Object.Tables.proteins),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.ProtLigandCplxe), File.DataFiles.Slots.Object.Tables.protligandcplxes),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Reaction), File.DataFiles.Slots.Object.Tables.reactions),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Regulation), File.DataFiles.Slots.Object.Tables.regulation),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Regulon), File.DataFiles.Slots.Object.Tables.regulons),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Terminator), File.DataFiles.Slots.Object.Tables.terminators),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.TransUnit), File.DataFiles.Slots.Object.Tables.transunits),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.tRNA), File.DataFiles.Slots.Object.Tables.trna)}
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="objA">对象A的UniqueId属性</param>
        ''' <param name="objB">对象B的UniqueId属性</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 程序会首先尝试查找A-->B的最短路线，假若没有查找到，则会尝试查找B-->A的最短路线
        ''' </remarks>
        Public Function GetPath(objA As String, objB As String) As Integer
            Throw New NotImplementedException
        End Function

        Public Overrides Function ToString() As String
            Return Database.Database.ToString
        End Function
    End Class
End Namespace
