'#Region "Microsoft.VisualBasic::a7eed1c155e0bca688845465caa65348, ..\GCModeller\analysis\annoTools\DataMySql\UniprotSprot\DbTools.vb"

'' Author:
'' 
''       asuka (amethyst.asuka@gcmodeller.org)
''       xieguigang (xie.guigang@live.com)
''       xie (genetics@smrucc.org)
'' 
'' Copyright (c) 2016 GPL3 Licensed
'' 
'' 
'' GNU GENERAL PUBLIC LICENSE (GPL3)
'' 
'' This program is free software: you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation, either version 3 of the License, or
'' (at your option) any later version.
'' 
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
'' 
'' You should have received a copy of the GNU General Public License
'' along with this program. If not, see <http://www.gnu.org/licenses/>.

'#End Region

Imports System.Runtime.CompilerServices
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Namespace kb_UniProtKB

    Public Module MySqlImports

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="uniprot">For imports a ultra large size XML database, using linq method <see cref="UniprotXML.EnumerateEntries(String)"/></param>
        ''' <returns></returns>
        <Extension>
        Public Function ImportsUniProtKB(uniprot As IEnumerable(Of entry)) As Dictionary(Of String, SQLTable())
            Dim hashCodes As New Dictionary(Of String, mysql.hash_table)
            Dim proteinFunctions As New Dictionary(Of String, mysql.protein_functions)

            For Each entry As entry In uniprot


            Next

            Dim mysqlTables As New Dictionary(Of String, SQLTable())

            mysqlTables(NameOf(mysql.hash_table)) = hashCodes.Values.ToArray
            mysqlTables(NameOf(mysql.protein_functions)) = proteinFunctions.Values.ToArray

            Return mysqlTables
        End Function
    End Module
End Namespace