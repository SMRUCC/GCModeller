#Region "Microsoft.VisualBasic::6c0df1d1e66acac3e29ef0dfb4c8dafc, data\GO_gene-ontology\obographs\obographs\test\io\OgProtobufGeneratorTest.vb"

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

    ' 	Class OgProtobufGeneratorTest
    ' 
    ' 	    Sub: test, writeSchema
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports org.junit.Assert

Namespace org.geneontology.obographs.io




	Public Class OgProtobufGeneratorTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			assertTrue(True)
			'System.out.println(s);
	'        makeSchema(GraphDocument.class, "obographs-schema.proto");
	'        makeSchema(Graph.class, "subschemas/obographs-graph-schema.proto");
	'        makeSchema(Meta.class, "subschemas/obographs-meta-schema.proto");
		End Sub
	'    private void makeSchema(Class<?> c, String fn) throws IOException {
	'    }	
	'    private void makeSchemaEXPERIMENTAL(Class<?> c, String fn) throws IOException {
	'        //ProtobufMapper mapper = new ProtobufMapper();
	'        // Protobuf cannot serialize freeform Objects
	'        ObjectMapper mapper = new ProtobufMapper();
	'        //        ObjectMapper mapper = new ProtobufMapper()
	'        //                .setFilterProvider(new SimpleFilterProvider().addFilter(
	'        //                        "GraphDocumentClass",
	'        //                        SimpleBeanPropertyFilter.serializeAllExcept("@context")));
	'        //        // TODO: make these compile and pass!
	'        ProtobufSchemaGenerator gen = new ProtobufSchemaGenerator();
	'        // mapper.acceptJsonFormatVisitor(c, gen);
	'        ProtobufSchema schemaWrapper = gen.getGeneratedSchema();
	'        String s = schemaWrapper.getSource().toString();
	'        writeSchema(fn, s);
	'
	'    }

		Protected Friend Overridable Sub writeSchema(ByVal fn As String, ByVal info As String)
			org.apache.commons.io.FileUtils.writeStringToFile(New File("target/" & fn), info)
		End Sub

	End Class

End Namespace
