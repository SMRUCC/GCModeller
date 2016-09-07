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