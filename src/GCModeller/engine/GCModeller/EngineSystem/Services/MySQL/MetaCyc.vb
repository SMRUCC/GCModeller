#Region "Microsoft.VisualBasic::63330c8a5927ea13f03dd77288b6cbbd, engine\GCModeller\EngineSystem\Services\MySQL\MetaCyc.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'Imports System.Text

'Namespace EngineSystem.Services.MySQL

'    ''' <summary>
'    ''' A module use for write the model data which are comes from a compiled MetaCyc database model.
'    ''' (用于将编译好的MetaCyc数据库模型写入数据库的模块)
'    ''' </summary>
'    ''' <remarks></remarks>
'    Public Class MetaCyc : Inherits Service

'        Friend Shadows MYSQl As Services.MySQL.DataModel

'        ''' <summary>
'        ''' The compiled model of the MetaCyc database.
'        ''' (已经编译好的MetaCyc数据库模型)
'        ''' </summary>
'        ''' <remarks></remarks>
'        Dim MetaCyc As SMRUCC.genomics.GCModeller.ModellingEngine.DataModel.Files.MetaCyc.MetaCyc

'        Public Sub CompileMetaCyc(Dir As String)
'            MetaCyc = SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.Compilers.MetaCyc.Compile(Dir)
'        End Sub

'        ''' <summary>
'        ''' Write the compiled model data to dabase.
'        ''' (将编译好的模型数据写入到数据库之中)
'        ''' </summary>
'        ''' <param name="Db">目标数据库的连接字符串</param>
'        ''' <remarks></remarks>
'        Public Sub Write(Db As String)
'            MYSQL = Db

'            Call Write("compounds", MetaCyc.Compounds)
'            Call Write("genes", MetaCyc.Genes)
'            Call Write("polypeptides", MetaCyc.Polypeptide)
'            Call Write("protein_complexes", MetaCyc.ProteinComplexes)
'            Call Write("protein_features", MetaCyc.ProteinFeature)
'            Call Write("reactions", MetaCyc.Reactions)

'            Call MYSQl.Commit()
'        End Sub

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="Table">
'        ''' The name of the target table to write data.(将要写入模型数据的目标数据表)
'        ''' </param>
'        ''' <param name="Data">
'        ''' The data collection to write to the database.(将要写入数据库的目标数据集)
'        ''' </param>
'        ''' <remarks></remarks>
'        Private Sub Write(Table As String, Data As Generic.IEnumerable(Of SMRUCC.genomics.GCModeller.ModellingEngine.DataModel.Files.MetaCyc.DataModel))
'            Dim p As Long = MYSQl.GetMaxHandle(Table) + 1

'            MYSQl.Table = Table

'            For Each row In Data
'                MYSQl.Insert(New SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL.DataModelRecord With {
'                             .GUID = row.GenerateGuid, .RegistryNumber = p, .DataModel = GetXmlModel(row)
'                    }, PendingTransaction:=True)
'                p += 1
'            Next
'        End Sub

'        Public Shared Shadows Widening Operator CType(Connection As Oracle.LinuxCompatibility.MySQL.Client.ConnectionHelper) As MetaCyc
'            Return New MetaCyc With {.MYSQl = Connection.GetConnectionString}
'        End Operator
'    End Class
'End Namespace
